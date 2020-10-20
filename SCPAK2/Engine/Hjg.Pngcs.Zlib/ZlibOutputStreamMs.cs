using System.IO;
using System.IO.Compression;

namespace Hjg.Pngcs.Zlib
{
	internal class ZlibOutputStreamMs : AZlibOutputStream
	{
		public DeflateStream deflateStream;

		public Adler32 adler32 = new Adler32();

		public bool initdone;

		public bool closed;

		public ZlibOutputStreamMs(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen)
			: base(st, compressLevel, strat, leaveOpen)
		{
		}

		public override void WriteByte(byte value)
		{
			if (!initdone)
			{
				doInit();
			}
			if (deflateStream == null)
			{
				initStream();
			}
			base.WriteByte(value);
			adler32.Update(value);
		}

		public override void Write(byte[] array, int offset, int count)
		{
			if (count != 0)
			{
				if (!initdone)
				{
					doInit();
				}
				if (deflateStream == null)
				{
					initStream();
				}
				deflateStream.Write(array, offset, count);
				adler32.Update(array, offset, count);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!initdone)
			{
				doInit();
			}
			if (!closed)
			{
				closed = true;
				if (deflateStream != null)
				{
					deflateStream.Dispose();
				}
				else
				{
					rawStream.WriteByte(3);
					rawStream.WriteByte(0);
				}
				uint value = adler32.GetValue();
				rawStream.WriteByte((byte)((value >> 24) & 0xFF));
				rawStream.WriteByte((byte)((value >> 16) & 0xFF));
				rawStream.WriteByte((byte)((value >> 8) & 0xFF));
				rawStream.WriteByte((byte)(value & 0xFF));
				if (!leaveOpen)
				{
					rawStream.Dispose();
				}
			}
		}

		public void initStream()
		{
			if (deflateStream == null)
			{
				CompressionLevel compressionLevel = CompressionLevel.Optimal;
				if (compressLevel >= 1 && compressLevel <= 5)
				{
					compressionLevel = CompressionLevel.Fastest;
				}
				else if (compressLevel == 0)
				{
					compressionLevel = CompressionLevel.NoCompression;
				}
				deflateStream = new DeflateStream(rawStream, compressionLevel, leaveOpen: true);
			}
		}

		public void doInit()
		{
			if (!initdone)
			{
				initdone = true;
				int num = 120;
				int num2 = 218;
				if (compressLevel >= 5 && compressLevel <= 6)
				{
					num2 = 156;
				}
				else if (compressLevel >= 3 && compressLevel <= 4)
				{
					num2 = 94;
				}
				else if (compressLevel <= 2)
				{
					num2 = 1;
				}
				num2 -= (num * 256 + num2) % 31;
				if (num2 < 0)
				{
					num2 += 31;
				}
				rawStream.WriteByte((byte)num);
				rawStream.WriteByte((byte)num2);
			}
		}

		public override void Flush()
		{
			if (deflateStream != null)
			{
				deflateStream.Flush();
			}
		}

		public override string getImplementationId()
		{
			return "Zlib deflater: .Net CLR 4.5";
		}
	}
}
