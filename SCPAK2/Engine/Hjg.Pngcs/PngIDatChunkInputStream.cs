using Hjg.Pngcs.Chunks;
using Hjg.Pngcs.Zlib;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hjg.Pngcs
{
	internal class PngIDatChunkInputStream : Stream
	{
		public class IdatChunkInfo
		{
			public readonly int len;

			public readonly long offset;

			public IdatChunkInfo(int len_0, long offset_1)
			{
				len = len_0;
				offset = offset_1;
			}
		}

		public readonly Stream inputStream;

		public readonly CRC32 crcEngine;

		public bool checkCrc;

		public int lenLastChunk;

		public byte[] idLastChunk;

		public int toReadThisChunk;

		public bool ended;

		public long offset;

		public IList<IdatChunkInfo> foundChunksInfo;

		public override long Position
		{
			get;
			set;
		}

		public override long Length => 0L;

		public override bool CanWrite => false;

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override void Write(byte[] buffer, int offset, int count)
		{
		}

		public override void SetLength(long value)
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return -1L;
		}

		public override void Flush()
		{
		}

		public PngIDatChunkInputStream(Stream iStream, int lenFirstChunk, long offset_0)
		{
			idLastChunk = new byte[4];
			toReadThisChunk = 0;
			ended = false;
			foundChunksInfo = new List<IdatChunkInfo>();
			offset = offset_0;
			checkCrc = true;
			inputStream = iStream;
			crcEngine = new CRC32();
			lenLastChunk = lenFirstChunk;
			toReadThisChunk = lenFirstChunk;
			Array.Copy(ChunkHelper.b_IDAT, 0, idLastChunk, 0, 4);
			crcEngine.Update(idLastChunk, 0, 4);
			foundChunksInfo.Add(new IdatChunkInfo(lenLastChunk, offset_0 - 8));
			if (lenLastChunk == 0)
			{
				EndChunkGoForNext();
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		public void EndChunkGoForNext()
		{
			while (true)
			{
				int num = PngHelperInternal.ReadInt4(inputStream);
				offset += 4L;
				if (checkCrc)
				{
					int value = (int)crcEngine.GetValue();
					if (lenLastChunk > 0 && num != value)
					{
						throw new PngjBadCrcException("error reading idat; offset: " + offset.ToString());
					}
					crcEngine.Reset();
				}
				lenLastChunk = PngHelperInternal.ReadInt4(inputStream);
				if (lenLastChunk < 0)
				{
					break;
				}
				toReadThisChunk = lenLastChunk;
				PngHelperInternal.ReadBytes(inputStream, idLastChunk, 0, 4);
				offset += 8L;
				ended = !PngCsUtils.arraysEqual4(idLastChunk, ChunkHelper.b_IDAT);
				if (!ended)
				{
					foundChunksInfo.Add(new IdatChunkInfo(lenLastChunk, offset - 8));
					if (checkCrc)
					{
						crcEngine.Update(idLastChunk, 0, 4);
					}
				}
				if (lenLastChunk != 0 || ended)
				{
					return;
				}
			}
			throw new PngjInputException("invalid len for chunk: " + lenLastChunk.ToString());
		}

		public void ForceChunkEnd()
		{
			if (!ended)
			{
				byte[] array = new byte[toReadThisChunk];
				PngHelperInternal.ReadBytes(inputStream, array, 0, toReadThisChunk);
				if (checkCrc)
				{
					crcEngine.Update(array, 0, toReadThisChunk);
				}
				EndChunkGoForNext();
			}
		}

		public override int Read(byte[] b, int off, int len_0)
		{
			if (ended)
			{
				return -1;
			}
			if (toReadThisChunk == 0)
			{
				throw new Exception("this should not happen");
			}
			int num = inputStream.Read(b, off, (len_0 >= toReadThisChunk) ? toReadThisChunk : len_0);
			if (num == -1)
			{
				num = -2;
			}
			if (num > 0)
			{
				if (checkCrc)
				{
					crcEngine.Update(b, off, num);
				}
				offset += num;
				toReadThisChunk -= num;
			}
			if (num >= 0 && toReadThisChunk == 0)
			{
				EndChunkGoForNext();
			}
			return num;
		}

		public int Read(byte[] b)
		{
			return Read(b, 0, b.Length);
		}

		public override int ReadByte()
		{
			byte[] array = new byte[1];
			if (Read(array, 0, 1) >= 0)
			{
				return array[0];
			}
			return -1;
		}

		public int GetLenLastChunk()
		{
			return lenLastChunk;
		}

		public byte[] GetIdLastChunk()
		{
			return idLastChunk;
		}

		public long GetOffset()
		{
			return offset;
		}

		public bool IsEnded()
		{
			return ended;
		}

		internal void DisableCrcCheck()
		{
			checkCrc = false;
		}
	}
}
