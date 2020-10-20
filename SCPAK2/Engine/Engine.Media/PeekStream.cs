using System;
using System.IO;

namespace Engine.Media
{
	internal class PeekStream : Stream
	{
		public Stream m_stream;

		public byte[] m_buffer;

		public long m_position;

		public int m_end;

		public override bool CanRead => true;

		public override bool CanWrite => false;

		public override bool CanSeek => m_stream.CanSeek;

		public override long Length => m_stream.Length;

		public override long Position
		{
			get
			{
				if (CanSeek)
				{
					return m_position;
				}
				throw new NotSupportedException();
			}
			set
			{
				if (CanSeek)
				{
					m_position = value;
					m_stream.Position = MathUtils.Max(m_position, m_end);
					return;
				}
				throw new NotSupportedException();
			}
		}

		public PeekStream(Stream stream, int peekSize)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("Stream is not readable.");
			}
			if (peekSize < 0)
			{
				throw new ArgumentOutOfRangeException("peekSize");
			}
			m_stream = stream;
			m_buffer = new byte[peekSize];
			m_end = stream.Read(m_buffer, 0, peekSize);
		}

		public MemoryStream GetInitialBytesStream()
		{
			return new MemoryStream(m_buffer, 0, m_end, writable: false);
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
			m_stream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (offset + count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			int num = 0;
			if (m_position < m_end)
			{
				int num2 = MathUtils.Min(m_end - (int)m_position, count);
				Array.Copy(m_buffer, (int)m_position, buffer, offset, num2);
				m_position += num2;
				offset += num2;
				count -= num2;
				num += num2;
			}
			if (count > 0)
			{
				num += m_stream.Read(buffer, offset, count);
				m_position += num;
			}
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override int ReadByte()
		{
			if (m_position < m_end)
			{
				return m_buffer[m_position++];
			}
			int num = m_stream.ReadByte();
			if (num >= 0)
			{
				m_position++;
			}
			return num;
		}

		public override void Flush()
		{
			m_stream.Flush();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				m_stream.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
