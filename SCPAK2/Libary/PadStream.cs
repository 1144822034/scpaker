using System;
using System.IO;

namespace SCPAK
{
	public class PadStream : Stream
	{
		public byte[] keys;

		private readonly Stream stream;

		private readonly bool leaveOpen;

		public override bool CanRead => stream.CanRead;

		public override bool CanSeek => stream.CanSeek;

		public override bool CanWrite => stream.CanWrite;

		public override long Length => stream.Length;

		public override long Position
		{
			get
			{
				return stream.Position;
			}
			set
			{
				stream.Position = value;
			}
		}

		public PadStream(Stream stream, byte[] keys = null, bool leaveOpen = false)
		{
			this.stream = stream;
			this.leaveOpen = leaveOpen;
			this.keys = keys;
		}

		public override void Flush()
		{
			stream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = stream.Read(buffer, offset, count);
			if (keys != null)
			{
				OnPad(buffer, offset, count, Position - num, keys);
			}
			return num;
		}

		public override int ReadByte()
		{
			if (keys == null)
			{
				return stream.ReadByte();
			}
			int num = stream.ReadByte();
			if (num < 0)
			{
				return -1;
			}
			return OnPad((byte)num, Position - 1, keys);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			stream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (keys != null)
			{
				byte[] array = new byte[count];
				Array.Copy(buffer, offset, array, 0, count);
				OnPad(array, 0, count, Position, keys);
				stream.Write(array, offset, count);
			}
			else
			{
				stream.Write(buffer, offset, count);
			}
		}

		private static void OnPad(byte[] buffer, int offset, int count, long position, byte[] pad)
		{
			for (int i = 0; i < count; i++)
			{
				buffer[offset + i] = OnPad(buffer[offset + i], position + i, pad);
			}
		}

		private static byte OnPad(byte b, long position, byte[] pad)
		{
			return (byte)(b ^ pad[(int)(position % pad.Length)]);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!leaveOpen)
			{
				stream.Dispose();
			}
		}
	}
}
