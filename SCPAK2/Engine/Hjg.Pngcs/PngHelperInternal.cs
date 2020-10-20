using Hjg.Pngcs.Zlib;
using System;
using System.IO;
using System.Text;

namespace Hjg.Pngcs
{
	internal class PngHelperInternal
	{
		[ThreadStatic]
		public static CRC32 crc32Engine = null;

		public static readonly byte[] PNG_ID_SIGNATURE = new byte[8]
		{
			137,
			80,
			78,
			71,
			13,
			10,
			26,
			10
		};

		public static Encoding charsetLatin1 = Encoding.GetEncoding("ISO-8859-1");

		public static Encoding charsetUtf8 = Encoding.GetEncoding("UTF-8");

		public static bool DEBUG = false;

		public static CRC32 GetCRC()
		{
			if (crc32Engine == null)
			{
				crc32Engine = new CRC32();
			}
			return crc32Engine;
		}

		public static int DoubleToInt100000(double d)
		{
			return (int)(d * 100000.0 + 0.5);
		}

		public static double IntToDouble100000(int i)
		{
			return (double)i / 100000.0;
		}

		public static void WriteInt2(Stream os, int n)
		{
			byte[] b = new byte[2]
			{
				(byte)((n >> 8) & 0xFF),
				(byte)(n & 0xFF)
			};
			WriteBytes(os, b);
		}

		public static int ReadInt2(Stream mask0)
		{
			try
			{
				int num = mask0.ReadByte();
				int num2 = mask0.ReadByte();
				if (num == -1 || num2 == -1)
				{
					return -1;
				}
				return (num << 8) + num2;
			}
			catch (IOException cause)
			{
				throw new PngjInputException("error reading readInt2", cause);
			}
		}

		public static int ReadInt4(Stream mask0)
		{
			try
			{
				int num = mask0.ReadByte();
				int num2 = mask0.ReadByte();
				int num3 = mask0.ReadByte();
				int num4 = mask0.ReadByte();
				if (num == -1 || num2 == -1 || num3 == -1 || num4 == -1)
				{
					return -1;
				}
				return (num << 24) + (num2 << 16) + (num3 << 8) + num4;
			}
			catch (IOException cause)
			{
				throw new PngjInputException("error reading readInt4", cause);
			}
		}

		public static int ReadInt1fromByte(byte[] b, int offset)
		{
			return b[offset] & 0xFF;
		}

		public static int ReadInt2fromBytes(byte[] b, int offset)
		{
			return ((b[offset] & 0xFF) << 16) | (b[offset + 1] & 0xFF);
		}

		public static int ReadInt4fromBytes(byte[] b, int offset)
		{
			return ((b[offset] & 0xFF) << 24) | ((b[offset + 1] & 0xFF) << 16) | ((b[offset + 2] & 0xFF) << 8) | (b[offset + 3] & 0xFF);
		}

		public static void WriteInt2tobytes(int n, byte[] b, int offset)
		{
			b[offset] = (byte)((n >> 8) & 0xFF);
			b[offset + 1] = (byte)(n & 0xFF);
		}

		public static void WriteInt4tobytes(int n, byte[] b, int offset)
		{
			b[offset] = (byte)((n >> 24) & 0xFF);
			b[offset + 1] = (byte)((n >> 16) & 0xFF);
			b[offset + 2] = (byte)((n >> 8) & 0xFF);
			b[offset + 3] = (byte)(n & 0xFF);
		}

		public static void WriteInt4(Stream os, int n)
		{
			byte[] b = new byte[4];
			WriteInt4tobytes(n, b, 0);
			WriteBytes(os, b);
		}

		public static void ReadBytes(Stream mask0, byte[] b, int offset, int len)
		{
			if (len == 0)
			{
				return;
			}
			try
			{
				int num = 0;
				int num2;
				while (true)
				{
					if (num >= len)
					{
						return;
					}
					num2 = mask0.Read(b, offset + num, len - num);
					if (num2 < 1)
					{
						break;
					}
					num += num2;
				}
				throw new Exception("error reading, " + num2.ToString() + " !=" + len.ToString());
			}
			catch (IOException cause)
			{
				throw new PngjInputException("error reading", cause);
			}
		}

		public static void SkipBytes(Stream ist, int len)
		{
			byte[] array = new byte[32768];
			int num = len;
			try
			{
				while (true)
				{
					if (num <= 0)
					{
						return;
					}
					int num2 = ist.Read(array, 0, (num > array.Length) ? array.Length : num);
					if (num2 < 0)
					{
						break;
					}
					num -= num2;
				}
				throw new PngjInputException("error reading (skipping) : EOF");
			}
			catch (IOException cause)
			{
				throw new PngjInputException("error reading (skipping)", cause);
			}
		}

		public static void WriteBytes(Stream os, byte[] b)
		{
			try
			{
				os.Write(b, 0, b.Length);
			}
			catch (IOException cause)
			{
				throw new PngjOutputException(cause);
			}
		}

		public static void WriteBytes(Stream os, byte[] b, int offset, int n)
		{
			try
			{
				os.Write(b, offset, n);
			}
			catch (IOException cause)
			{
				throw new PngjOutputException(cause);
			}
		}

		public static int ReadByte(Stream mask0)
		{
			try
			{
				return mask0.ReadByte();
			}
			catch (IOException cause)
			{
				throw new PngjOutputException(cause);
			}
		}

		public static void WriteByte(Stream os, byte b)
		{
			try
			{
				os.WriteByte(b);
			}
			catch (IOException cause)
			{
				throw new PngjOutputException(cause);
			}
		}

		public static int UnfilterRowPaeth(int r, int a, int b, int c)
		{
			return (r + FilterPaethPredictor(a, b, c)) & 0xFF;
		}

		public static int FilterPaethPredictor(int a, int b, int c)
		{
			int num = a + b - c;
			int num2 = (num >= a) ? (num - a) : (a - num);
			int num3 = (num >= b) ? (num - b) : (b - num);
			int num4 = (num >= c) ? (num - c) : (c - num);
			if (num2 <= num3 && num2 <= num4)
			{
				return a;
			}
			if (num3 <= num4)
			{
				return b;
			}
			return c;
		}

		public static void Logdebug(string msg)
		{
		}

		public static void InitCrcForTests(PngReader pngr)
		{
			pngr.InitCrctest();
		}

		public static long GetCrctestVal(PngReader pngr)
		{
			return pngr.GetCrctestVal();
		}
	}
}
