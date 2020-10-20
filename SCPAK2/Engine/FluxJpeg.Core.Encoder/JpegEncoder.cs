using FluxJpeg.Core.IO;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FluxJpeg.Core.Encoder
{
	internal class JpegEncoder
	{
		public JpegEncodeProgressChangedArgs _progress;

		public DecodedJpeg _input;

		public Stream _outStream;

		public HuffmanTable _huf;

		public DCT _dct;

		public int _height;

		public int _width;

		public int _quality;

		public const int Ss = 0;

		public const int Se = 63;

		public const int Ah = 0;

		public const int Al = 0;

		public static readonly int[] CompID = new int[3]
		{
			1,
			2,
			3
		};

		public static readonly int[] QtableNumber = new int[3]
		{
			0,
			1,
			1
		};

		public static readonly int[] DCtableNumber = new int[3]
		{
			0,
			1,
			1
		};

		public static readonly int[] ACtableNumber = new int[3]
		{
			0,
			1,
			1
		};

		public event EventHandler<JpegEncodeProgressChangedArgs> EncodeProgressChanged;

		public JpegEncoder(Image image, int quality, Stream outStream)
			: this(new DecodedJpeg(image), quality, outStream)
		{
		}

		public JpegEncoder(DecodedJpeg decodedJpeg, int quality, Stream outStream)
		{
			_input = decodedJpeg;
			_input.Image.ChangeColorSpace(ColorSpace.YCbCr);
			_quality = quality;
			_height = _input.Image.Height;
			_width = _input.Image.Width;
			_outStream = outStream;
			_dct = new DCT(_quality);
			_huf = new HuffmanTable(null);
		}

		public void Encode()
		{
			_progress = new JpegEncodeProgressChangedArgs();
			WriteHeaders();
			CompressTo(_outStream);
			WriteMarker(new byte[2]
			{
				255,
				217
			});
			_progress.EncodeProgress = 1.0;
			if (this.EncodeProgressChanged != null)
			{
				this.EncodeProgressChanged(this, _progress);
			}
			_outStream.Flush();
		}

		internal void WriteHeaders()
		{
			byte[] data = new byte[2]
			{
				255,
				216
			};
			WriteMarker(data);
			if (!_input.HasJFIF)
			{
				byte[] data2 = new byte[18]
				{
					255,
					224,
					0,
					16,
					74,
					70,
					73,
					70,
					0,
					1,
					0,
					0,
					0,
					1,
					0,
					1,
					0,
					0
				};
				WriteArray(data2);
			}
			FluxJpeg.Core.IO.BinaryWriter binaryWriter = new FluxJpeg.Core.IO.BinaryWriter(_outStream);
			bool flag = false;
			foreach (JpegHeader metaHeader in _input.MetaHeaders)
			{
				binaryWriter.Write(byte.MaxValue);
				binaryWriter.Write(metaHeader.Marker);
				if (metaHeader.Marker == 224)
				{
					flag = true;
					byte[] bytes = BitConverter.GetBytes((short)_input.Image.DensityX);
					byte[] bytes2 = BitConverter.GetBytes((short)_input.Image.DensityY);
					Array.Reverse<byte>(bytes);
					Array.Reverse<byte>(bytes2);
					bytes.CopyTo(metaHeader.Data, 8);
					bytes2.CopyTo(metaHeader.Data, 10);
				}
				binaryWriter.Write((short)(metaHeader.Data.Length + 2));
				binaryWriter.Write(metaHeader.Data);
			}
			if (!flag)
			{
				flag = true;
				binaryWriter.Write(byte.MaxValue);
				binaryWriter.Write(224);
				binaryWriter.Write((short)16);
				binaryWriter.Write(74);
				binaryWriter.Write(70);
				binaryWriter.Write(73);
				binaryWriter.Write(70);
				binaryWriter.Write(0);
				binaryWriter.Write(1);
				binaryWriter.Write(1);
				binaryWriter.Write(1);
				binaryWriter.Write((short)_input.Image.DensityX);
				binaryWriter.Write((short)_input.Image.DensityY);
				binaryWriter.Write(0);
				binaryWriter.Write(0);
			}
			byte[] array = new byte[134];
			array[0] = byte.MaxValue;
			array[1] = 219;
			array[2] = 0;
			array[3] = 132;
			int num = 4;
			for (int i = 0; i < 2; i++)
			{
				array[num++] = (byte)i;
				int[] array2 = _dct.quantum[i];
				for (int j = 0; j < 64; j++)
				{
					array[num++] = (byte)array2[ZigZag.ZigZagMap[j]];
				}
			}
			WriteArray(array);
			byte[] array3 = new byte[19]
			{
				255,
				192,
				0,
				17,
				(byte)_input.Precision,
				(byte)((_input.Image.Height >> 8) & 0xFF),
				(byte)(_input.Image.Height & 0xFF),
				(byte)((_input.Image.Width >> 8) & 0xFF),
				(byte)(_input.Image.Width & 0xFF),
				(byte)_input.Image.ComponentCount,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			};
			int num2 = 10;
			for (int i = 0; i < array3[9]; i++)
			{
				array3[num2++] = (byte)CompID[i];
				array3[num2++] = (byte)((_input.HsampFactor[i] << 4) + _input.VsampFactor[i]);
				array3[num2++] = (byte)QtableNumber[i];
			}
			WriteArray(array3);
			num2 = 4;
			int num3 = 4;
			byte[] array4 = new byte[17];
			byte[] array5 = new byte[4]
			{
				255,
				196,
				0,
				0
			};
			for (int i = 0; i < 4; i++)
			{
				int num4 = 0;
				int num5;
				switch (i)
				{
				default:
					num5 = 17;
					break;
				case 2:
					num5 = 1;
					break;
				case 1:
					num5 = 16;
					break;
				case 0:
					num5 = 0;
					break;
				}
				byte b = (byte)num5;
				array4[num2++ - num3] = b;
				for (int j = 0; j < 16; j++)
				{
					int num6 = _huf.bitsList[i][j];
					array4[num2++ - num3] = (byte)num6;
					num4 += num6;
				}
				int num7 = num2;
				byte[] array6 = new byte[num4];
				for (int j = 0; j < num4; j++)
				{
					array6[num2++ - num7] = (byte)_huf.val[i][j];
				}
				byte[] array7 = new byte[num2];
				Array.Copy(array5, 0, array7, 0, num3);
				Array.Copy(array4, 0, array7, num3, 17);
				Array.Copy(array6, 0, array7, num3 + 17, num4);
				array5 = array7;
				num3 = num2;
			}
			array5[2] = (byte)((num2 - 2 >> 8) & 0xFF);
			array5[3] = (byte)((num2 - 2) & 0xFF);
			WriteArray(array5);
			byte[] array8 = new byte[14]
			{
				255,
				218,
				0,
				12,
				(byte)_input.Image.ComponentCount,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			};
			num2 = 5;
			for (int i = 0; i < array8[4]; i++)
			{
				array8[num2++] = (byte)CompID[i];
				array8[num2++] = (byte)((DCtableNumber[i] << 4) + ACtableNumber[i]);
			}
			array8[num2++] = 0;
			array8[num2++] = 63;
			array8[num2++] = 0;
			WriteArray(array8);
		}

		internal void CompressTo(Stream outStream)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			byte[,] array = null;
			float[,] array2 = new float[8, 8];
			float[,] array3 = new float[8, 8];
			int[] array4 = new int[64];
			int[] array5 = new int[_input.Image.ComponentCount];
			int num7 = (_width % 8 != 0) ? ((int)(Math.Floor((double)_width / 8.0) + 1.0) * 8) : _width;
			int num8 = (_height % 8 != 0) ? ((int)(Math.Floor((double)_height / 8.0) + 1.0) * 8) : _height;
			for (int i = 0; i < _input.Image.ComponentCount; i++)
			{
				num7 = Math.Min(num7, _input.BlockWidth[i]);
				num8 = Math.Min(num8, _input.BlockHeight[i]);
			}
			int num9 = 0;
			for (num3 = 0; num3 < num8; num3++)
			{
				_progress.EncodeProgress = (double)num3 / (double)num8;
				if (this.EncodeProgressChanged != null)
				{
					this.EncodeProgressChanged(this, _progress);
				}
				for (num4 = 0; num4 < num7; num4++)
				{
					num9 = num4 * 8;
					int num10 = num3 * 8;
					for (int i = 0; i < _input.Image.ComponentCount; i++)
					{
						array = _input.Image.Raster[i];
						for (num = 0; num < _input.VsampFactor[i]; num++)
						{
							for (num2 = 0; num2 < _input.HsampFactor[i]; num2++)
							{
								int num11 = num2 * 8;
								int num12 = num * 8;
								for (num5 = 0; num5 < 8; num5++)
								{
									int num13 = num10 + num12 + num5;
									if (num13 >= _height)
									{
										break;
									}
									for (num6 = 0; num6 < 8; num6++)
									{
										int num14 = num9 + num11 + num6;
										if (num14 >= _width)
										{
											break;
										}
										array2[num5, num6] = (int)array[num14, num13];
									}
								}
								array3 = _dct.FastFDCT(array2);
								array4 = _dct.QuantizeBlock(array3, QtableNumber[i]);
								_huf.HuffmanBlockEncoder(outStream, array4, array5[i], DCtableNumber[i], ACtableNumber[i]);
								array5[i] = array4[0];
							}
						}
					}
				}
			}
			_huf.FlushBuffer(outStream);
		}

		public void WriteMarker(byte[] data)
		{
			_outStream.Write(data, 0, 2);
		}

		public void WriteArray(byte[] data)
		{
			int count = ((data[2] & 0xFF) << 8) + (data[3] & 0xFF) + 2;
			_outStream.Write(data, 0, count);
		}
	}
}
