using Hjg.Pngcs.Zlib;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hjg.Pngcs.Chunks
{
	internal class ChunkHelper
	{
		internal const string IHDR = "IHDR";

		internal const string PLTE = "PLTE";

		internal const string IDAT = "IDAT";

		internal const string IEND = "IEND";

		internal const string cHRM = "cHRM";

		internal const string gAMA = "gAMA";

		internal const string iCCP = "iCCP";

		internal const string sBIT = "sBIT";

		internal const string sRGB = "sRGB";

		internal const string bKGD = "bKGD";

		internal const string hIST = "hIST";

		internal const string tRNS = "tRNS";

		internal const string pHYs = "pHYs";

		internal const string sPLT = "sPLT";

		internal const string tIME = "tIME";

		internal const string iTXt = "iTXt";

		internal const string tEXt = "tEXt";

		internal const string zTXt = "zTXt";

		internal static readonly byte[] b_IHDR = ToBytes("IHDR");

		internal static readonly byte[] b_PLTE = ToBytes("PLTE");

		internal static readonly byte[] b_IDAT = ToBytes("IDAT");

		internal static readonly byte[] b_IEND = ToBytes("IEND");

		public static byte[] ToBytes(string x)
		{
			return PngHelperInternal.charsetLatin1.GetBytes(x);
		}

		public static string ToString(byte[] x)
		{
			return PngHelperInternal.charsetLatin1.GetString(x, 0, x.Length);
		}

		public static string ToString(byte[] x, int offset, int len)
		{
			return PngHelperInternal.charsetLatin1.GetString(x, offset, len);
		}

		public static byte[] ToBytesUTF8(string x)
		{
			return PngHelperInternal.charsetUtf8.GetBytes(x);
		}

		public static string ToStringUTF8(byte[] x)
		{
			return PngHelperInternal.charsetUtf8.GetString(x, 0, x.Length);
		}

		public static string ToStringUTF8(byte[] x, int offset, int len)
		{
			return PngHelperInternal.charsetUtf8.GetString(x, offset, len);
		}

		public static void WriteBytesToStream(Stream stream, byte[] bytes)
		{
			stream.Write(bytes, 0, bytes.Length);
		}

		public static bool IsCritical(string id)
		{
			return char.IsUpper(id[0]);
		}

		public static bool IsPublic(string id)
		{
			return char.IsUpper(id[1]);
		}

		public static bool IsSafeToCopy(string id)
		{
			return !char.IsUpper(id[3]);
		}

		public static bool IsUnknown(PngChunk chunk)
		{
			return chunk is PngChunkUNKNOWN;
		}

		public static int PosNullByte(byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				if (bytes[i] == 0)
				{
					return i;
				}
			}
			return -1;
		}

		public static bool ShouldLoad(string id, ChunkLoadBehaviour behav)
		{
			if (IsCritical(id))
			{
				return true;
			}
			bool flag = PngChunk.isKnown(id);
			switch (behav)
			{
			case ChunkLoadBehaviour.LOAD_CHUNK_ALWAYS:
				return true;
			case ChunkLoadBehaviour.LOAD_CHUNK_IF_SAFE:
				if (!flag)
				{
					return IsSafeToCopy(id);
				}
				return true;
			case ChunkLoadBehaviour.LOAD_CHUNK_KNOWN:
				return flag;
			case ChunkLoadBehaviour.LOAD_CHUNK_NEVER:
				return false;
			default:
				return false;
			}
		}

		internal static byte[] compressBytes(byte[] ori, bool compress)
		{
			return compressBytes(ori, 0, ori.Length, compress);
		}

		internal static byte[] compressBytes(byte[] ori, int offset, int len, bool compress)
		{
			try
			{
				MemoryStream memoryStream = new MemoryStream(ori, offset, len);
				Stream stream = memoryStream;
				if (!compress)
				{
					stream = ZlibStreamFactory.createZlibInputStream(memoryStream);
				}
				MemoryStream memoryStream2 = new MemoryStream();
				Stream stream2 = memoryStream2;
				if (compress)
				{
					stream2 = ZlibStreamFactory.createZlibOutputStream(memoryStream2);
				}
				shovelInToOut(stream, stream2);
				stream.Dispose();
				stream2.Dispose();
				return memoryStream2.ToArray();
			}
			catch (Exception cause)
			{
				throw new PngjException(cause);
			}
		}

		public static void shovelInToOut(Stream inx, Stream outx)
		{
			byte[] buffer = new byte[1024];
			int count;
			while ((count = inx.Read(buffer, 0, 1024)) > 0)
			{
				outx.Write(buffer, 0, count);
			}
		}

		internal static bool maskMatch(int v, int mask)
		{
			return (v & mask) != 0;
		}

		public static List<PngChunk> FilterList(List<PngChunk> list, ChunkPredicate predicateKeep)
		{
			List<PngChunk> list2 = new List<PngChunk>();
			foreach (PngChunk item in list)
			{
				if (predicateKeep.Matches(item))
				{
					list2.Add(item);
				}
			}
			return list2;
		}

		public static int TrimList(List<PngChunk> list, ChunkPredicate predicateRemove)
		{
			int num = 0;
			for (int num2 = list.Count - 1; num2 >= 0; num2--)
			{
				if (predicateRemove.Matches(list[num2]))
				{
					list.RemoveAt(num2);
					num++;
				}
			}
			return num;
		}

		public static bool Equivalent(PngChunk c1, PngChunk c2)
		{
			if (c1 == c2)
			{
				return true;
			}
			if (c1 == null || c2 == null || !c1.Id.Equals(c2.Id))
			{
				return false;
			}
			if (c1.GetType() != c2.GetType())
			{
				return false;
			}
			if (!c2.AllowsMultiple())
			{
				return true;
			}
			if (c1 is PngChunkTextVar)
			{
				return ((PngChunkTextVar)c1).GetKey().Equals(((PngChunkTextVar)c2).GetKey());
			}
			if (c1 is PngChunkSPLT)
			{
				return ((PngChunkSPLT)c1).PalName.Equals(((PngChunkSPLT)c2).PalName);
			}
			return false;
		}

		public static bool IsText(PngChunk c)
		{
			return c is PngChunkTextVar;
		}
	}
}
