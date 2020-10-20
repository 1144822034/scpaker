using FluxJpeg.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace FluxJpeg.Core.Decoder
{
	internal class JpegDecoder
	{
		public enum UnitType
		{
			None,
			Inches,
			Centimeters
		}

		public static long ProgressUpdateByteInterval = 100L;

		public JpegDecodeProgressChangedArgs DecodeProgress = new JpegDecodeProgressChangedArgs();

		public byte majorVersion;

		public byte minorVersion;

		public UnitType Units;

		public ushort XDensity;

		public ushort YDensity;

		public byte Xthumbnail;

		public byte Ythumbnail;

		public byte[] thumbnail;

		public Image image;

		public bool progressive;

		public byte marker;

		internal const byte MAJOR_VERSION = 1;

		internal const byte MINOR_VERSION = 2;

		internal static short JFIF_FIXED_LENGTH = 16;

		internal static short JFXX_FIXED_LENGTH = 8;

		public JPEGBinaryReader jpegReader;

		public List<JPEGFrame> jpegFrames = new List<JPEGFrame>();

		public JpegHuffmanTable[] dcTables = new JpegHuffmanTable[4];

		public JpegHuffmanTable[] acTables = new JpegHuffmanTable[4];

		public JpegQuantizationTable[] qTables = new JpegQuantizationTable[4];

		public BlockUpsamplingMode BlockUpsamplingMode
		{
			get;
			set;
		}

		public event EventHandler<JpegDecodeProgressChangedArgs> DecodeProgressChanged;

		public JpegDecoder(Stream input)
		{
			jpegReader = new JPEGBinaryReader(input);
			if (jpegReader.GetNextMarker() != 216)
			{
				throw new Exception("Failed to find SOI marker.");
			}
		}

		public bool TryParseJFIF(byte[] data)
		{
			FluxJpeg.Core.IO.BinaryReader binaryReader = new FluxJpeg.Core.IO.BinaryReader(new MemoryStream(data));
			int num = data.Length + 2;
			if (num < JFIF_FIXED_LENGTH)
			{
				return false;
			}
			byte[] array = new byte[5];
			binaryReader.Read(array, 0, array.Length);
			if (array[0] != 74 || array[1] != 70 || array[2] != 73 || array[3] != 70 || array[4] != 0)
			{
				return false;
			}
			majorVersion = binaryReader.ReadByte();
			minorVersion = binaryReader.ReadByte();
			if (majorVersion != 1 || (majorVersion == 1 && minorVersion > 2))
			{
				return false;
			}
			Units = (UnitType)binaryReader.ReadByte();
			if (Units != 0 && Units != UnitType.Inches && Units != UnitType.Centimeters)
			{
				return false;
			}
			XDensity = binaryReader.ReadShort();
			YDensity = binaryReader.ReadShort();
			Xthumbnail = binaryReader.ReadByte();
			Ythumbnail = binaryReader.ReadByte();
			int num2 = 3 * Xthumbnail * Ythumbnail;
			if (num > JFIF_FIXED_LENGTH && num2 != num - JFIF_FIXED_LENGTH)
			{
				return false;
			}
			if (num2 > 0)
			{
				thumbnail = new byte[num2];
				if (binaryReader.Read(thumbnail, 0, num2) != num2)
				{
					return false;
				}
			}
			return true;
		}

		public DecodedJpeg Decode()
		{
			JPEGFrame jPEGFrame = null;
			int resetInterval = 0;
			bool flag = false;
			bool flag2 = false;
			List<JpegHeader> list = new List<JpegHeader>();
			while (true)
			{
				if (DecodeProgress.Abort)
				{
					return null;
				}
				switch (marker)
				{
				case 224:
				case 225:
				case 226:
				case 227:
				case 228:
				case 229:
				case 230:
				case 231:
				case 232:
				case 233:
				case 234:
				case 235:
				case 236:
				case 237:
				case 238:
				case 239:
				case 254:
				{
					JpegHeader jpegHeader = ExtractHeader();
					if (jpegHeader.Marker == 225 && jpegHeader.Data.Length >= 6)
					{
						byte[] data = jpegHeader.Data;
						if (data[0] == 69 && data[1] == 120 && data[2] == 105 && data[3] == 102 && data[4] == 0)
						{
							_ = data[5];
						}
					}
					if (jpegHeader.Data.Length >= 5 && jpegHeader.Marker == 238)
					{
						_ = (Encoding.UTF8.GetString(jpegHeader.Data, 0, 5) == "Adobe");
					}
					list.Add(jpegHeader);
					if (flag2 || marker != 224)
					{
						break;
					}
					flag2 = TryParseJFIF(jpegHeader.Data);
					if (flag2)
					{
						jpegHeader.IsJFIF = true;
						marker = jpegReader.GetNextMarker();
						if (marker == 224)
						{
							jpegHeader = ExtractHeader();
							list.Add(jpegHeader);
						}
						else
						{
							flag = true;
						}
					}
					break;
				}
				case 192:
				case 194:
				{
					progressive = (marker == 194);
					jpegFrames.Add(new JPEGFrame());
					jPEGFrame = jpegFrames[jpegFrames.Count - 1];
					jPEGFrame.ProgressUpdateMethod = UpdateStreamProgress;
					jpegReader.ReadShort();
					jPEGFrame.setPrecision(jpegReader.ReadByte());
					jPEGFrame.ScanLines = jpegReader.ReadShort();
					jPEGFrame.SamplesPerLine = jpegReader.ReadShort();
					jPEGFrame.ComponentCount = jpegReader.ReadByte();
					DecodeProgress.Height = jPEGFrame.Height;
					DecodeProgress.Width = jPEGFrame.Width;
					DecodeProgress.SizeReady = true;
					if (this.DecodeProgressChanged != null)
					{
						this.DecodeProgressChanged(this, DecodeProgress);
						if (DecodeProgress.Abort)
						{
							return null;
						}
					}
					for (int m = 0; m < jPEGFrame.ComponentCount; m++)
					{
						byte componentID = jpegReader.ReadByte();
						byte num5 = jpegReader.ReadByte();
						byte quantizationTableID = jpegReader.ReadByte();
						byte sampleHFactor = (byte)(num5 >> 4);
						byte sampleVFactor = (byte)(num5 & 0xF);
						jPEGFrame.AddComponent(componentID, sampleHFactor, sampleVFactor, quantizationTableID);
					}
					break;
				}
				case 196:
				{
					int num2 = jpegReader.ReadShort() - 2;
					while (num2 > 0)
					{
						byte num3 = jpegReader.ReadByte();
						byte b = (byte)(num3 >> 4);
						byte b2 = (byte)(num3 & 0xF);
						short[] array = new short[16];
						for (int j = 0; j < array.Length; j++)
						{
							array[j] = jpegReader.ReadByte();
						}
						int num4 = 0;
						for (int k = 0; k < 16; k++)
						{
							num4 += array[k];
						}
						num2 -= num4 + 17;
						short[] array2 = new short[num4];
						for (int l = 0; l < array2.Length; l++)
						{
							array2[l] = jpegReader.ReadByte();
						}
						if (b == HuffmanTable.JPEG_DC_TABLE)
						{
							dcTables[b2] = new JpegHuffmanTable(array, array2);
						}
						else if (b == HuffmanTable.JPEG_AC_TABLE)
						{
							acTables[b2] = new JpegHuffmanTable(array, array2);
						}
					}
					break;
				}
				case 219:
				{
					short num9 = (short)(jpegReader.ReadShort() - 2);
					for (int num10 = 0; num10 < num9 / 65; num10++)
					{
						byte b5 = jpegReader.ReadByte();
						int[] array4 = new int[64];
						if ((byte)(b5 >> 4) == 0)
						{
							for (int num11 = 0; num11 < 64; num11++)
							{
								array4[num11] = jpegReader.ReadByte();
							}
						}
						else if ((byte)(b5 >> 4) == 1)
						{
							for (int num12 = 0; num12 < 64; num12++)
							{
								array4[num12] = jpegReader.ReadShort();
							}
						}
						qTables[b5 & 0xF] = new JpegQuantizationTable(array4);
					}
					break;
				}
				case 218:
				{
					jpegReader.ReadShort();
					byte b3 = jpegReader.ReadByte();
					byte[] array3 = new byte[b3];
					for (int n = 0; n < b3; n++)
					{
						byte b4 = jpegReader.ReadByte();
						byte num6 = jpegReader.ReadByte();
						int num7 = (num6 >> 4) & 0xF;
						int num8 = num6 & 0xF;
						jPEGFrame.setHuffmanTables(b4, acTables[(byte)num8], dcTables[(byte)num7]);
						array3[n] = b4;
					}
					byte startSpectralSelection = jpegReader.ReadByte();
					byte endSpectralSelection = jpegReader.ReadByte();
					byte successiveApproximation = jpegReader.ReadByte();
					if (!progressive)
					{
						jPEGFrame.DecodeScanBaseline(b3, array3, resetInterval, jpegReader, ref marker);
						flag = true;
					}
					if (progressive)
					{
						jPEGFrame.DecodeScanProgressive(successiveApproximation, startSpectralSelection, endSpectralSelection, b3, array3, resetInterval, jpegReader, ref marker);
						flag = true;
					}
					break;
				}
				case 221:
					jpegReader.BaseStream.Seek(2L, SeekOrigin.Current);
					resetInterval = jpegReader.ReadShort();
					break;
				case 220:
					jPEGFrame.ScanLines = jpegReader.ReadShort();
					break;
				case 217:
				{
					if (jpegFrames.Count == 0)
					{
						throw new NotSupportedException("No JPEG frames could be located.");
					}
					if (jpegFrames.Count > 1)
					{
						jPEGFrame = jpegFrames.OrderByDescending((JPEGFrame f) => f.Width * f.Height).FirstOrDefault();
					}
					byte[][,] raster = Image.CreateRaster(jPEGFrame.Width, jPEGFrame.Height, jPEGFrame.ComponentCount);
					IList<JpegComponent> components = jPEGFrame.Scan.Components;
					int stepsTotal = components.Count * 3;
					int num = 0;
					for (int i = 0; i < components.Count; i++)
					{
						JpegComponent jpegComponent = components[i];
						jpegComponent.QuantizationTable = qTables[jpegComponent.quant_id].Table;
						jpegComponent.quantizeData();
						UpdateProgress(++num, stepsTotal);
						jpegComponent.idctData();
						UpdateProgress(++num, stepsTotal);
						jpegComponent.writeDataScaled(raster, i, BlockUpsamplingMode);
						UpdateProgress(++num, stepsTotal);
						jpegComponent = null;
						GC.Collect();
					}
					ColorModel colorModel;
					if (jPEGFrame.ComponentCount == 1)
					{
						colorModel = default(ColorModel);
						colorModel.colorspace = ColorSpace.Gray;
						colorModel.Opaque = true;
						ColorModel cm = colorModel;
						image = new Image(cm, raster);
					}
					else
					{
						if (jPEGFrame.ComponentCount != 3)
						{
							throw new NotSupportedException("Unsupported Color Mode: 4 Component Color Mode found.");
						}
						colorModel = default(ColorModel);
						colorModel.colorspace = ColorSpace.YCbCr;
						colorModel.Opaque = true;
						ColorModel cm2 = colorModel;
						image = new Image(cm2, raster);
					}
					Func<double, double> func = (double x) => (Units != UnitType.Inches) ? (x / 2.54) : x;
					image.DensityX = func((int)XDensity);
					image.DensityY = func((int)YDensity);
					break;
				}
				case 193:
				case 195:
				case 197:
				case 198:
				case 199:
				case 201:
				case 202:
				case 203:
				case 205:
				case 206:
				case 207:
					throw new NotSupportedException("Unsupported codec type.");
				}
				if (flag)
				{
					flag = false;
				}
				else
				{
					try
					{
						marker = jpegReader.GetNextMarker();
					}
					catch (EndOfStreamException)
					{
						break;
					}
				}
			}
			return new DecodedJpeg(image, list);
		}

		public JpegHeader ExtractHeader()
		{
			int num = jpegReader.ReadShort() - 2;
			byte[] array = new byte[num];
			jpegReader.Read(array, 0, num);
			return new JpegHeader
			{
				Marker = marker,
				Data = array
			};
		}

		public void UpdateStreamProgress(long StreamPosition)
		{
			if (this.DecodeProgressChanged != null)
			{
				DecodeProgress.ReadPosition = StreamPosition;
				this.DecodeProgressChanged(this, DecodeProgress);
			}
		}

		public void UpdateProgress(int stepsFinished, int stepsTotal)
		{
			if (this.DecodeProgressChanged != null)
			{
				DecodeProgress.DecodeProgress = (double)stepsFinished / (double)stepsTotal;
				this.DecodeProgressChanged(this, DecodeProgress);
			}
		}
	}
}
