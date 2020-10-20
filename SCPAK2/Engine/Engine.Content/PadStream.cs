using System;
using System.IO;

namespace Engine.Content
{
	public class PadStream : Stream
	{
		public Stream m_stream;

		public bool m_leaveOpen;

		public byte[] Pad
		{
			get;
			set;
		}

		public override bool CanRead => m_stream.CanRead;

		public override bool CanSeek => m_stream.CanSeek;

		public override bool CanWrite => m_stream.CanWrite;

		public override long Length => m_stream.Length;

		public override long Position
		{
			get
			{
				return m_stream.Position;
			}
			set
			{
				m_stream.Position = value;
			}
		}

		public PadStream(Stream stream, bool leaveOpen = false)
		{
			m_stream = stream;
			m_leaveOpen = leaveOpen;
		}

		public override void Flush()
		{
			m_stream.Flush();
		}

		public override int ReadByte()
		{
			if (Pad != null)
			{
				int num = m_stream.ReadByte();
				if (num < 0)
				{
					return -1;
				}
				return AddPad((byte)num, Position - 1, Pad);
			}
			return m_stream.ReadByte();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = m_stream.Read(buffer, offset, count);
			if (Pad != null)
			{
				AddPad(buffer, offset, count, Position - num, Pad);
			}
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (Pad != null)
			{
				byte[] array = new byte[count];
				Array.Copy(buffer, offset, array, 0, count);
				AddPad(array, 0, count, Position, Pad);
				m_stream.Write(array, offset, count);
			}
			else
			{
				m_stream.Write(buffer, offset, count);
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return m_stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			m_stream.SetLength(value);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!m_leaveOpen)
			{
				m_stream.Dispose();
			}
		}

		public static void AddPad(byte[] buffer, int offset, int count, long position, byte[] pad)
		{
			for (int i = 0; i < count; i++)
			{
				buffer[offset + i] = AddPad(buffer[offset + i], position + i, pad);
			}
		}

		public static byte AddPad(byte b, long position, byte[] pad)
		{
			return (byte)(b ^ pad[position % pad.Length]);
		}
	}
}
