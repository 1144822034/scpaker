using System;
using System.IO;

namespace Hjg.Pngcs.Zlib
{
	internal abstract class AZlibOutputStream : Stream
	{
		public readonly Stream rawStream;

		public readonly bool leaveOpen;

		public int compressLevel;

		public EDeflateCompressStrategy strategy;

		public override bool CanSeek => false;

		public override long Position
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override long Length
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool CanRead => false;

		public override bool CanWrite => true;

		public override bool CanTimeout => false;

		public AZlibOutputStream(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen)
		{
			rawStream = st;
			this.leaveOpen = leaveOpen;
			strategy = strat;
			this.compressLevel = compressLevel;
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public abstract string getImplementationId();
	}
}
