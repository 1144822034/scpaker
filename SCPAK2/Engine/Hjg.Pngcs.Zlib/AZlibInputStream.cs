using System;
using System.IO;

namespace Hjg.Pngcs.Zlib
{
	internal abstract class AZlibInputStream : Stream
	{
		public readonly Stream rawStream;

		public readonly bool leaveOpen;

		public override bool CanRead => true;

		public override bool CanWrite => false;

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

		public override bool CanTimeout => false;

		public AZlibInputStream(Stream st, bool leaveOpen)
		{
			rawStream = st;
			this.leaveOpen = leaveOpen;
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public abstract string getImplementationId();
	}
}
