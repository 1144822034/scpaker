using FluxJpeg.Core.IO;
using System.Collections.Generic;
using System.IO;

namespace FluxJpeg.Core
{
	internal class HuffmanTable
	{
		public static int HUFFMAN_MAX_TABLES = 4;

		public short[] huffcode = new short[256];

		public short[] huffsize = new short[256];

		public short[] valptr = new short[16];

		public short[] mincode = new short[16]
		{
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1
		};

		public short[] maxcode = new short[16]
		{
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1
		};

		public short[] huffval;

		public short[] bits;

		public int bufferPutBits;

		public int bufferPutBuffer;

		internal int[,] DC_matrix0;

		internal int[,] AC_matrix0;

		internal int[,] DC_matrix1;

		internal int[,] AC_matrix1;

		internal int[][,] DC_matrix;

		internal int[][,] AC_matrix;

		internal int NumOfDCTables;

		internal int NumOfACTables;

		public List<short[]> bitsList;

		public List<short[]> val;

		public static byte JPEG_DC_TABLE = 0;

		public static byte JPEG_AC_TABLE = 1;

		internal HuffmanTable(JpegHuffmanTable table)
		{
			if (table != null)
			{
				huffval = table.Values;
				bits = table.Lengths;
				GenerateSizeTable();
				GenerateCodeTable();
				GenerateDecoderTables();
				return;
			}
			bitsList = new List<short[]>();
			bitsList.Add(JpegHuffmanTable.StdDCLuminance.Lengths);
			bitsList.Add(JpegHuffmanTable.StdACLuminance.Lengths);
			bitsList.Add(JpegHuffmanTable.StdDCChrominance.Lengths);
			bitsList.Add(JpegHuffmanTable.StdACChrominance.Lengths);
			val = new List<short[]>();
			val.Add(JpegHuffmanTable.StdDCLuminance.Values);
			val.Add(JpegHuffmanTable.StdACLuminance.Values);
			val.Add(JpegHuffmanTable.StdDCChrominance.Values);
			val.Add(JpegHuffmanTable.StdACChrominance.Values);
			initHuf();
		}

		public void GenerateSizeTable()
		{
			short num = 0;
			for (short num2 = 0; num2 < bits.Length; num2 = (short)(num2 + 1))
			{
				for (short num3 = 0; num3 < bits[num2]; num3 = (short)(num3 + 1))
				{
					huffsize[num] = (short)(num2 + 1);
					num = (short)(num + 1);
				}
			}
		}

		public void GenerateCodeTable()
		{
			short num = 0;
			short num2 = huffsize[0];
			short num3 = 0;
			for (short num4 = 0; num4 < huffsize.Length; num4 = (short)(num4 + 1))
			{
				while (huffsize[num] == num2)
				{
					huffcode[num] = num3;
					num3 = (short)(num3 + 1);
					num = (short)(num + 1);
				}
				num3 = (short)(num3 << 1);
				num2 = (short)(num2 + 1);
			}
		}

		public void GenerateDecoderTables()
		{
			short num = 0;
			for (int i = 0; i < 16; i++)
			{
				if (bits[i] != 0)
				{
					valptr[i] = num;
				}
				for (int j = 0; j < bits[i]; j++)
				{
					if (huffcode[j + num] < mincode[i] || mincode[i] == -1)
					{
						mincode[i] = huffcode[j + num];
					}
					if (huffcode[j + num] > maxcode[i])
					{
						maxcode[i] = huffcode[j + num];
					}
				}
				if (mincode[i] != -1)
				{
					valptr[i] = (short)(valptr[i] - mincode[i]);
				}
				num = (short)(num + bits[i]);
			}
		}

		public static int Extend(int diff, int t)
		{
			int num = 1 << t - 1;
			if (diff < num)
			{
				num = (-1 << t) + 1;
				diff += num;
			}
			return diff;
		}

		public int Decode(JPEGBinaryReader JPEGStream)
		{
			int num = 0;
			short num2;
			for (num2 = (short)JPEGStream.ReadBits(1); num2 > maxcode[num]; num2 = (short)(num2 | (short)JPEGStream.ReadBits(1)))
			{
				num++;
				num2 = (short)(num2 << 1);
			}
			int num3 = huffval[num2 + valptr[num]];
			if (num3 < 0)
			{
				num3 = 256 + num3;
			}
			return num3;
		}

		internal void HuffmanBlockEncoder(Stream outStream, int[] zigzag, int prec, int DCcode, int ACcode)
		{
			NumOfDCTables = 2;
			NumOfACTables = 2;
			int num;
			int num2 = num = zigzag[0] - prec;
			if (num2 < 0)
			{
				num2 = -num2;
				num--;
			}
			int num3 = 0;
			while (num2 != 0)
			{
				num3++;
				num2 >>= 1;
			}
			bufferIt(outStream, DC_matrix[DCcode][num3, 0], DC_matrix[DCcode][num3, 1]);
			if (num3 != 0)
			{
				bufferIt(outStream, num, num3);
			}
			int num4 = 0;
			for (int i = 1; i < 64; i++)
			{
				if ((num2 = zigzag[ZigZag.ZigZagMap[i]]) == 0)
				{
					num4++;
					continue;
				}
				while (num4 > 15)
				{
					bufferIt(outStream, AC_matrix[ACcode][240, 0], AC_matrix[ACcode][240, 1]);
					num4 -= 16;
				}
				num = num2;
				if (num2 < 0)
				{
					num2 = -num2;
					num--;
				}
				num3 = 1;
				while ((num2 >>= 1) != 0)
				{
					num3++;
				}
				int num5 = (num4 << 4) + num3;
				bufferIt(outStream, AC_matrix[ACcode][num5, 0], AC_matrix[ACcode][num5, 1]);
				bufferIt(outStream, num, num3);
				num4 = 0;
			}
			if (num4 > 0)
			{
				bufferIt(outStream, AC_matrix[ACcode][0, 0], AC_matrix[ACcode][0, 1]);
			}
		}

		public void bufferIt(Stream outStream, int code, int size)
		{
			int num = bufferPutBits;
			int num2 = code & ((1 << size) - 1);
			num += size;
			num2 <<= 24 - num;
			num2 |= bufferPutBuffer;
			while (num >= 8)
			{
				int num3 = (num2 >> 16) & 0xFF;
				outStream.WriteByte((byte)num3);
				if (num3 == 255)
				{
					outStream.WriteByte(0);
				}
				num2 <<= 8;
				num -= 8;
			}
			bufferPutBuffer = num2;
			bufferPutBits = num;
		}

		public void FlushBuffer(Stream outStream)
		{
			int num = bufferPutBuffer;
			int num2;
			for (num2 = bufferPutBits; num2 >= 8; num2 -= 8)
			{
				int num3 = (num >> 16) & 0xFF;
				outStream.WriteByte((byte)num3);
				if (num3 == 255)
				{
					outStream.WriteByte(0);
				}
				num <<= 8;
			}
			if (num2 > 0)
			{
				int num4 = (num >> 16) & 0xFF;
				outStream.WriteByte((byte)num4);
			}
		}

		public void initHuf()
		{
			DC_matrix0 = new int[12, 2];
			DC_matrix1 = new int[12, 2];
			AC_matrix0 = new int[255, 2];
			AC_matrix1 = new int[255, 2];
			DC_matrix = new int[2][,];
			AC_matrix = new int[2][,];
			int[] array = new int[257];
			int[] array2 = new int[257];
			short[] lengths = JpegHuffmanTable.StdDCChrominance.Lengths;
			short[] lengths2 = JpegHuffmanTable.StdACChrominance.Lengths;
			short[] lengths3 = JpegHuffmanTable.StdDCLuminance.Lengths;
			short[] lengths4 = JpegHuffmanTable.StdACLuminance.Lengths;
			short[] values = JpegHuffmanTable.StdDCChrominance.Values;
			short[] values2 = JpegHuffmanTable.StdACChrominance.Values;
			short[] values3 = JpegHuffmanTable.StdDCLuminance.Values;
			short[] values4 = JpegHuffmanTable.StdACLuminance.Values;
			int num = 0;
			for (int i = 0; i < 16; i++)
			{
				for (int j = 1; j <= lengths[i]; j++)
				{
					array[num++] = i + 1;
				}
			}
			array[num] = 0;
			int num2 = num;
			int num3 = 0;
			int num4 = array[0];
			num = 0;
			while (array[num] != 0)
			{
				while (array[num] == num4)
				{
					array2[num++] = num3;
					num3++;
				}
				num3 <<= 1;
				num4++;
			}
			for (num = 0; num < num2; num++)
			{
				DC_matrix1[values[num], 0] = array2[num];
				DC_matrix1[values[num], 1] = array[num];
			}
			num = 0;
			for (int i = 0; i < 16; i++)
			{
				for (int j = 1; j <= lengths2[i]; j++)
				{
					array[num++] = i + 1;
				}
			}
			array[num] = 0;
			num2 = num;
			num3 = 0;
			num4 = array[0];
			num = 0;
			while (array[num] != 0)
			{
				while (array[num] == num4)
				{
					array2[num++] = num3;
					num3++;
				}
				num3 <<= 1;
				num4++;
			}
			for (num = 0; num < num2; num++)
			{
				AC_matrix1[values2[num], 0] = array2[num];
				AC_matrix1[values2[num], 1] = array[num];
			}
			num = 0;
			for (int i = 0; i < 16; i++)
			{
				for (int j = 1; j <= lengths3[i]; j++)
				{
					array[num++] = i + 1;
				}
			}
			array[num] = 0;
			num2 = num;
			num3 = 0;
			num4 = array[0];
			num = 0;
			while (array[num] != 0)
			{
				while (array[num] == num4)
				{
					array2[num++] = num3;
					num3++;
				}
				num3 <<= 1;
				num4++;
			}
			for (num = 0; num < num2; num++)
			{
				DC_matrix0[values3[num], 0] = array2[num];
				DC_matrix0[values3[num], 1] = array[num];
			}
			num = 0;
			for (int i = 0; i < 16; i++)
			{
				for (int j = 1; j <= lengths4[i]; j++)
				{
					array[num++] = i + 1;
				}
			}
			array[num] = 0;
			num2 = num;
			num3 = 0;
			num4 = array[0];
			num = 0;
			while (array[num] != 0)
			{
				while (array[num] == num4)
				{
					array2[num++] = num3;
					num3++;
				}
				num3 <<= 1;
				num4++;
			}
			for (int k = 0; k < num2; k++)
			{
				AC_matrix0[values4[k], 0] = array2[k];
				AC_matrix0[values4[k], 1] = array[k];
			}
			DC_matrix[0] = DC_matrix0;
			DC_matrix[1] = DC_matrix1;
			AC_matrix[0] = AC_matrix0;
			AC_matrix[1] = AC_matrix1;
		}
	}
}
