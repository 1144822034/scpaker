using System;
using System.IO;
using System.IO.Compression;

namespace Hjg.Pngcs.Zlib
{
	internal class ZlibInputStreamMs : AZlibInputStream
	{
		public DeflateStream deflateStream;

		public bool initdone;

		public bool closed;

		public bool fdict;

		public byte[] dictid;

		public byte[] crcread;

		public ZlibInputStreamMs(Stream st, bool leaveOpen)
			: base(st, leaveOpen)
		{
		}

		public override int Read(byte[] array, int offset, int count)
		{
			if (!initdone)
			{
				doInit();
			}
			if (deflateStream == null && count > 0)
			{
				initStream();
			}
			int num = deflateStream.Read(array, offset, count);
			if (num < 1 && crcread == null)
			{
				crcread = new byte[4];
				for (int i = 0; i < 4; i++)
				{
					crcread[i] = (byte)rawStream.ReadByte();
				}
			}
			return num;
		}

		protected override void Dispose(bool disposing)
		{
			if (!initdone)
			{
				doInit();
			}
			if (closed)
			{
				return;
			}
			closed = true;
			if (deflateStream != null)
			{
				deflateStream.Dispose();
			}
			if (crcread == null)
			{
				crcread = new byte[4];
				for (int i = 0; i < 4; i++)
				{
					crcread[i] = (byte)rawStream.ReadByte();
				}
			}
			if (!leaveOpen)
			{
				rawStream.Dispose();
			}
		}

		public void initStream()
		{
			if (deflateStream == null)
			{
				deflateStream = new DeflateStream(rawStream, CompressionMode.Decompress, leaveOpen: true);
			}
		}

		public void doInit()
		{
			if (initdone)
			{
				return;
			}
			initdone = true;
			int num = rawStream.ReadByte();
			int num2 = rawStream.ReadByte();
			if (num == -1 || num2 == -1)
			{
				return;
			}
			if ((num & 0xF) != 8)
			{
				throw new Exception("Bad compression method for ZLIB header: cmf=" + num.ToString());
			}
			fdict = ((num2 & 0x20) != 0);
			if (fdict)
			{
				dictid = new byte[4];
				for (int i = 0; i < 4; i++)
				{
					dictid[i] = (byte)rawStream.ReadByte();
				}
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
			return "Zlib inflater: .Net CLR 4.5";
		}
	}
}
