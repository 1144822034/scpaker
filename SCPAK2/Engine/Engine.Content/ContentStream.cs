using System;
using System.IO;

namespace Engine.Content
{
	public class ContentStream : Stream
	{
		public Func<Stream> m_streamFactory;

		public Stream m_stream;

		public byte[] m_data;

		public long m_start;

		public long m_length;

		public bool m_isSubstream;

		public byte[] Pad
		{
			get
			{
				return ((PadStream)m_stream).Pad;
			}
			set
			{
				((PadStream)m_stream).Pad = value;
			}
		}

		public override bool CanRead => true;

		public override bool CanWrite => false;

		public override bool CanSeek => true;

		public override long Length => m_length;

		public override long Position
		{
			get
			{
				return m_stream.Position - m_start;
			}
			set
			{
				if (value < 0 || value > Length)
				{
					throw new InvalidOperationException("Cannot seek outside of the stream.");
				}
				m_stream.Position = value + m_start;
			}
		}

		public ContentStream(Func<Stream> streamFactory)
		{
			m_streamFactory = streamFactory;
			m_stream = new PadStream(m_streamFactory());
			if (!m_stream.CanSeek)
			{
				MemoryStream memoryStream = new MemoryStream();
				m_stream.CopyTo(memoryStream);
				m_stream.Dispose();
				m_data = memoryStream.ToArray();
				m_stream = new PadStream(new MemoryStream(m_data, writable: false));
			}
			m_start = 0L;
			m_length = m_stream.Length;
		}

		public ContentStream()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				Position = offset;
				break;
			case SeekOrigin.End:
				Position = Length + offset;
				break;
			case SeekOrigin.Current:
				Position += offset;
				break;
			default:
				throw new ArgumentException("Invalid origin.", "origin");
			}
			return Position;
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			count = (int)MathUtils.Min(count, Length - Position);
			return m_stream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override int ReadByte()
		{
			if (Position < Length)
			{
				return m_stream.ReadByte();
			}
			return -1;
		}

		public override void Flush()
		{
			m_stream.Flush();
		}

		public ContentStream Duplicate()
		{
			ContentStream contentStream = new ContentStream();
			contentStream.m_streamFactory = m_streamFactory;
			contentStream.m_data = m_data;
			if (m_data != null)
			{
				contentStream.m_stream = new PadStream(new MemoryStream(m_data, writable: false));
			}
			else
			{
				contentStream.m_stream = new PadStream(m_streamFactory());
			}
			contentStream.m_stream.Position = m_stream.Position;
			contentStream.m_start = m_start;
			contentStream.m_length = m_length;
			contentStream.Pad = Pad;
			return contentStream;
		}

		public ContentStream CreateSubstream(long length)
		{
			if (Position + length > Length)
			{
				throw new InvalidOperationException("Substream extends beyond stream.");
			}
			return new ContentStream
			{
				m_streamFactory = m_streamFactory,
				m_data = m_data,
				m_stream = m_stream,
				m_start = (int)Position,
				m_length = length,
				m_isSubstream = true
			};
		}

		protected override void Dispose(bool disposing)
		{
			if (!m_isSubstream)
			{
				if (disposing)
				{
					m_stream.Dispose();
				}
				base.Dispose(disposing);
			}
		}
	}
}
