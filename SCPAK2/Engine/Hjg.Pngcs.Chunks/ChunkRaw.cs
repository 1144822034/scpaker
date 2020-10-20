using Hjg.Pngcs.Zlib;
using System;
using System.IO;

namespace Hjg.Pngcs.Chunks
{
	internal class ChunkRaw
	{
		public readonly int Length;

		public readonly byte[] IdBytes;

		public byte[] Data;

		public int crcval;

		internal ChunkRaw(int length, byte[] idbytes, bool alloc)
		{
			IdBytes = new byte[4];
			Data = null;
			crcval = 0;
			Length = length;
			Array.Copy(idbytes, 0, IdBytes, 0, 4);
			if (alloc)
			{
				AllocData();
			}
		}

		public int ComputeCrc()
		{
			CRC32 cRC = PngHelperInternal.GetCRC();
			cRC.Reset();
			cRC.Update(IdBytes, 0, 4);
			if (Length > 0)
			{
				cRC.Update(Data, 0, Length);
			}
			return (int)cRC.GetValue();
		}

		internal void WriteChunk(Stream os)
		{
			if (IdBytes.Length != 4)
			{
				throw new PngjOutputException("bad chunkid [" + ChunkHelper.ToString(IdBytes) + "]");
			}
			crcval = ComputeCrc();
			PngHelperInternal.WriteInt4(os, Length);
			PngHelperInternal.WriteBytes(os, IdBytes);
			if (Length > 0)
			{
				PngHelperInternal.WriteBytes(os, Data, 0, Length);
			}
			PngHelperInternal.WriteInt4(os, crcval);
		}

		internal int ReadChunkData(Stream stream, bool checkCrc)
		{
			PngHelperInternal.ReadBytes(stream, Data, 0, Length);
			crcval = PngHelperInternal.ReadInt4(stream);
			if (checkCrc)
			{
				int num = ComputeCrc();
				if (num != crcval)
				{
					throw new PngjBadCrcException("crc invalid for chunk " + ToString() + " calc=" + num.ToString() + " read=" + crcval.ToString());
				}
			}
			return Length + 4;
		}

		internal MemoryStream GetAsByteStream()
		{
			return new MemoryStream(Data);
		}

		public void AllocData()
		{
			if (Data == null || Data.Length < Length)
			{
				Data = new byte[Length];
			}
		}

		public override string ToString()
		{
			return "chunkid=" + ChunkHelper.ToString(IdBytes) + " len=" + Length.ToString();
		}
	}
}
