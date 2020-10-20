using FluxJpeg.Core.IO;
using System;
using System.Collections.Generic;

namespace FluxJpeg.Core.Decoder
{
	internal class JpegComponent
	{
		internal delegate void DecodeFunction(JPEGBinaryReader jpegReader, float[] zigzagMCU);

		public byte factorH;

		public byte factorV;

		public byte component_id;

		public byte quant_id;

		public int width;

		public int height;

		public HuffmanTable ACTable;

		public HuffmanTable DCTable;

		public int[] quantizationTable;

		public float previousDC;

		public JpegScan parent;

		public float[,][] scanMCUs;

		public List<float[,][]> scanData = new List<float[,][]>();

		public List<byte[,]> scanDecoded = new List<byte[,]>();

		public int spectralStart;

		public int spectralEnd;

		public int successiveLow;

		public DCT _dct = new DCT();

		public DecodeFunction Decode;

		public int[] QuantizationTable
		{
			set
			{
				quantizationTable = value;
			}
		}

		public int BlockCount => scanData.Count;

		public int factorUpV => parent.MaxV / (int)factorV;

		public int factorUpH => parent.MaxH / (int)factorH;

		public JpegComponent(JpegScan parentScan, byte id, byte factorHorizontal, byte factorVertical, byte quantizationID, byte colorMode)
		{
			parent = parentScan;
			if (colorMode == JPEGFrame.JPEG_COLOR_YCbCr)
			{
				if (id == 1)
				{
					ACTable = new HuffmanTable(JpegHuffmanTable.StdACLuminance);
					DCTable = new HuffmanTable(JpegHuffmanTable.StdDCLuminance);
				}
				else
				{
					ACTable = new HuffmanTable(JpegHuffmanTable.StdACChrominance);
					DCTable = new HuffmanTable(JpegHuffmanTable.StdACLuminance);
				}
			}
			component_id = id;
			factorH = factorHorizontal;
			factorV = factorVertical;
			quant_id = quantizationID;
		}

		public void padMCU(int index, int length)
		{
			scanMCUs = new float[factorH, factorV][];
			for (int i = 0; i < length; i++)
			{
				if (scanData.Count >= index + length)
				{
					continue;
				}
				for (int j = 0; j < factorH; j++)
				{
					for (int k = 0; k < factorV; k++)
					{
						scanMCUs[j, k] = (float[])scanData[index - 1][j, k].Clone();
					}
				}
				scanData.Add(scanMCUs);
			}
		}

		public void resetInterval()
		{
			previousDC = 0f;
		}

		public void quantizeData()
		{
			for (int i = 0; i < scanData.Count; i++)
			{
				for (int j = 0; j < factorV; j++)
				{
					for (int k = 0; k < factorH; k++)
					{
						float[] array = scanData[i][k, j];
						for (int l = 0; l < quantizationTable.Length; l++)
						{
							array[l] *= quantizationTable[l];
						}
					}
				}
			}
		}

		public void setDCTable(JpegHuffmanTable table)
		{
			DCTable = new HuffmanTable(table);
		}

		public void setACTable(JpegHuffmanTable table)
		{
			ACTable = new HuffmanTable(table);
		}

		public void idctData()
		{
			float[] array = new float[64];
			for (int i = 0; i < scanData.Count; i++)
			{
				for (int j = 0; j < factorV; j++)
				{
					for (int k = 0; k < factorH; k++)
					{
						ZigZag.UnZigZag(scanData[i][k, j], array);
						scanDecoded.Add(_dct.FastIDCT(array));
					}
				}
			}
		}

		public void scaleByFactors(BlockUpsamplingMode mode)
		{
			int factorUpV = this.factorUpV;
			int factorUpH = this.factorUpH;
			if (factorUpV == 1 && factorUpH == 1)
			{
				return;
			}
			for (int i = 0; i < scanDecoded.Count; i++)
			{
				byte[,] array = scanDecoded[i];
				int length = array.GetLength(0);
				int length2 = array.GetLength(1);
				int num = length * factorUpV;
				int num2 = length2 * factorUpH;
				byte[,] array2 = new byte[num, num2];
				switch (mode)
				{
				case BlockUpsamplingMode.BoxFilter:
				{
					for (int n = 0; n < num2; n++)
					{
						int num6 = n / factorUpH;
						for (int num7 = 0; num7 < num; num7++)
						{
							int num8 = num7 / factorUpV;
							array2[num7, n] = array[num8, num6];
						}
					}
					break;
				}
				case BlockUpsamplingMode.Interpolate:
				{
					for (int j = 0; j < num2; j++)
					{
						for (int k = 0; k < num; k++)
						{
							int num3 = 0;
							for (int l = 0; l < factorUpH; l++)
							{
								int num4 = (j + l) / factorUpH;
								if (num4 >= length2)
								{
									num4 = length2 - 1;
								}
								for (int m = 0; m < factorUpV; m++)
								{
									int num5 = (k + m) / factorUpV;
									if (num5 >= length)
									{
										num5 = length - 1;
									}
									num3 += array[num5, num4];
								}
							}
							array2[k, j] = (byte)(num3 / (factorUpH * factorUpV));
						}
					}
					break;
				}
				default:
					throw new ArgumentException("Upsampling mode not supported.");
				}
				scanDecoded[i] = array2;
			}
		}

		public void writeBlock(byte[][,] raster, byte[,] data, int compIndex, int x, int y)
		{
			int length = raster[0].GetLength(0);
			int length2 = raster[0].GetLength(1);
			byte[,] array = raster[compIndex];
			int num = data.GetLength(0);
			if (y + num > length2)
			{
				num = length2 - y;
			}
			int num2 = data.GetLength(1);
			if (x + num2 > length)
			{
				num2 = length - x;
			}
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					array[x + j, y + i] = data[i, j];
				}
			}
		}

		public void writeDataScaled(byte[][,] raster, int componentIndex, BlockUpsamplingMode mode)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int length = raster[0].GetLength(0);
			while (num5 < scanDecoded.Count)
			{
				int num6 = 0;
				int num7 = 0;
				if (num >= length)
				{
					num = 0;
					num2 += num4;
				}
				for (int i = 0; i < factorV; i++)
				{
					num6 = 0;
					for (int j = 0; j < factorH; j++)
					{
						byte[,] array = scanDecoded[num5++];
						writeBlockScaled(raster, array, componentIndex, num, num2, mode);
						num6 += array.GetLength(1) * factorUpH;
						num += array.GetLength(1) * factorUpH;
						num7 = array.GetLength(0) * factorUpV;
					}
					num2 += num7;
					num -= num6;
					num3 += num7;
				}
				num2 -= num3;
				num4 = num3;
				num3 = 0;
				num += num6;
			}
		}

		public void writeBlockScaled(byte[][,] raster, byte[,] blockdata, int compIndex, int x, int y, BlockUpsamplingMode mode)
		{
			int length = raster[0].GetLength(0);
			int length2 = raster[0].GetLength(1);
			int factorUpV = this.factorUpV;
			int factorUpH = this.factorUpH;
			int length3 = blockdata.GetLength(0);
			int length4 = blockdata.GetLength(1);
			int num = length3 * factorUpV;
			int num2 = length4 * factorUpH;
			byte[,] array = raster[compIndex];
			int num3 = num;
			if (y + num3 > length2)
			{
				num3 = length2 - y;
			}
			int num4 = num2;
			if (x + num4 > length)
			{
				num4 = length - x;
			}
			if (mode == BlockUpsamplingMode.BoxFilter)
			{
				if (factorUpV == 1 && factorUpH == 1)
				{
					for (int i = 0; i < num4; i++)
					{
						for (int j = 0; j < num3; j++)
						{
							array[i + x, y + j] = blockdata[j, i];
						}
					}
					return;
				}
				if (factorUpH == 2 && factorUpV == 2 && num4 == num2 && num3 == num)
				{
					for (int k = 0; k < length4; k++)
					{
						int num5 = k * 2 + x;
						for (int l = 0; l < length3; l++)
						{
							byte b = blockdata[l, k];
							int num6 = l * 2 + y;
							array[num5, num6] = b;
							array[num5, num6 + 1] = b;
							array[num5 + 1, num6] = b;
							array[num5 + 1, num6 + 1] = b;
						}
					}
					return;
				}
				for (int m = 0; m < num4; m++)
				{
					int num7 = m / factorUpH;
					for (int n = 0; n < num3; n++)
					{
						int num8 = n / factorUpV;
						array[m + x, y + n] = blockdata[num8, num7];
					}
				}
				return;
			}
			throw new ArgumentException("Upsampling mode not supported.");
		}

		public void DecodeBaseline(JPEGBinaryReader stream, float[] dest)
		{
			float num = decode_dc_coefficient(stream);
			decode_ac_coefficients(stream, dest);
			dest[0] = num;
		}

		public void DecodeDCFirst(JPEGBinaryReader stream, float[] dest)
		{
			int num = DCTable.Decode(stream);
			num = HuffmanTable.Extend(stream.ReadBits(num), num);
			num = (int)previousDC + num;
			previousDC = num;
			dest[0] = num << successiveLow;
		}

		public void DecodeACFirst(JPEGBinaryReader stream, float[] zz)
		{
			if (stream.eob_run > 0)
			{
				stream.eob_run--;
				return;
			}
			int num = spectralStart;
			int num3;
			while (true)
			{
				if (num > spectralEnd)
				{
					return;
				}
				int num2 = ACTable.Decode(stream);
				num3 = num2 >> 4;
				num2 &= 0xF;
				if (num2 != 0)
				{
					num += num3;
					num3 = stream.ReadBits(num2);
					num2 = HuffmanTable.Extend(num3, num2);
					zz[num] = num2 << successiveLow;
				}
				else
				{
					if (num3 != 15)
					{
						break;
					}
					num += 15;
				}
				num++;
			}
			stream.eob_run = 1 << num3;
			if (num3 != 0)
			{
				stream.eob_run += stream.ReadBits(num3);
			}
			stream.eob_run--;
		}

		public void DecodeDCRefine(JPEGBinaryReader stream, float[] dest)
		{
			if (stream.ReadBits(1) == 1)
			{
				dest[0] = ((int)dest[0] | (1 << successiveLow));
			}
		}

		public void DecodeACRefine(JPEGBinaryReader stream, float[] dest)
		{
			int num = 1 << successiveLow;
			int num2 = -1 << successiveLow;
			int i = spectralStart;
			if (stream.eob_run == 0)
			{
				for (; i <= spectralEnd; i++)
				{
					int num3 = ACTable.Decode(stream);
					int num4 = num3 >> 4;
					num3 &= 0xF;
					if (num3 != 0)
					{
						if (num3 != 1)
						{
							throw new Exception("Decode Error");
						}
						num3 = ((stream.ReadBits(1) != 1) ? num2 : num);
					}
					else if (num4 != 15)
					{
						stream.eob_run = 1 << num4;
						if (num4 > 0)
						{
							stream.eob_run += stream.ReadBits(num4);
						}
						break;
					}
					do
					{
						if (dest[i] != 0f)
						{
							if (stream.ReadBits(1) == 1 && ((int)dest[i] & num) == 0)
							{
								if (dest[i] >= 0f)
								{
									dest[i] += num;
								}
								else
								{
									dest[i] += num2;
								}
							}
						}
						else if (--num4 < 0)
						{
							break;
						}
						i++;
					}
					while (i <= spectralEnd);
					if (num3 != 0 && i < 64)
					{
						dest[i] = num3;
					}
				}
			}
			if (stream.eob_run <= 0)
			{
				return;
			}
			for (; i <= spectralEnd; i++)
			{
				if (dest[i] != 0f && stream.ReadBits(1) == 1 && ((int)dest[i] & num) == 0)
				{
					if (dest[i] >= 0f)
					{
						dest[i] += num;
					}
					else
					{
						dest[i] += num2;
					}
				}
			}
			stream.eob_run--;
		}

		public void SetBlock(int idx)
		{
			if (scanData.Count < idx)
			{
				throw new Exception("Invalid block ID.");
			}
			if (scanData.Count == idx)
			{
				scanMCUs = new float[factorH, factorV][];
				for (int i = 0; i < factorH; i++)
				{
					for (int j = 0; j < factorV; j++)
					{
						scanMCUs[i, j] = new float[64];
					}
				}
				scanData.Add(scanMCUs);
			}
			else
			{
				scanMCUs = scanData[idx];
			}
		}

		public void DecodeMCU(JPEGBinaryReader jpegReader, int i, int j)
		{
			Decode(jpegReader, scanMCUs[i, j]);
		}

		public float decode_dc_coefficient(JPEGBinaryReader JPEGStream)
		{
			int num = DCTable.Decode(JPEGStream);
			float num2 = JPEGStream.ReadBits(num);
			num2 = HuffmanTable.Extend((int)num2, num);
			return previousDC += num2;
		}

		internal void decode_ac_coefficients(JPEGBinaryReader JPEGStream, float[] zz)
		{
			int num;
			for (num = 1; num < 64; num++)
			{
				int num2 = ACTable.Decode(JPEGStream);
				int num3 = num2 >> 4;
				num2 &= 0xF;
				if (num2 != 0)
				{
					num += num3;
					num3 = JPEGStream.ReadBits(num2);
					num2 = HuffmanTable.Extend(num3, num2);
					zz[num] = num2;
				}
				else
				{
					if (num3 != 15)
					{
						break;
					}
					num += 15;
				}
			}
		}
	}
}
