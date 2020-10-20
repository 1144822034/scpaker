using System;
using System.IO;
using System.Threading;

namespace NVorbis
{
	internal class BufferedReadStream : Stream
	{
		public const int DEFAULT_INITIAL_SIZE = 32768;

		public const int DEFAULT_MAX_SIZE = 262144;

		public Stream _baseStream;

		public StreamReadBuffer _buffer;

		public long _readPosition;

		public object _localLock = new object();

		public int _owningThreadId;

		public int _lockCount;

		public bool CloseBaseStream
		{
			get;
			set;
		}

		public bool MinimalRead
		{
			get
			{
				return _buffer.MinimalRead;
			}
			set
			{
				_buffer.MinimalRead = value;
			}
		}

		public int MaxBufferSize
		{
			get
			{
				return _buffer.MaxSize;
			}
			set
			{
				CheckLock();
				_buffer.MaxSize = value;
			}
		}

		public long BufferBaseOffset => _buffer.BaseOffset;

		public int BufferBytesFilled => _buffer.BytesFilled;

		public override bool CanRead => true;

		public override bool CanSeek => true;

		public override bool CanWrite => false;

		public override long Length => _baseStream.Length;

		public override long Position
		{
			get
			{
				return _readPosition;
			}
			set
			{
				Seek(value, SeekOrigin.Begin);
			}
		}

		public BufferedReadStream(Stream baseStream)
			: this(baseStream, 32768, 262144, minimalRead: false)
		{
		}

		public BufferedReadStream(Stream baseStream, bool minimalRead)
			: this(baseStream, 32768, 262144, minimalRead)
		{
		}

		public BufferedReadStream(Stream baseStream, int initialSize, int maxSize)
			: this(baseStream, initialSize, maxSize, minimalRead: false)
		{
		}

		public BufferedReadStream(Stream baseStream, int initialSize, int maxBufferSize, bool minimalRead)
		{
			if (baseStream == null)
			{
				throw new ArgumentNullException("baseStream");
			}
			if (!baseStream.CanRead)
			{
				throw new ArgumentException("baseStream");
			}
			if (maxBufferSize < 1)
			{
				maxBufferSize = 1;
			}
			if (initialSize < 1)
			{
				initialSize = 1;
			}
			if (initialSize > maxBufferSize)
			{
				initialSize = maxBufferSize;
			}
			_baseStream = baseStream;
			_buffer = new StreamReadBuffer(baseStream, initialSize, maxBufferSize, minimalRead);
			_buffer.MaxSize = maxBufferSize;
			_buffer.MinimalRead = minimalRead;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				if (_buffer != null)
				{
					_buffer.Dispose();
					_buffer = null;
				}
				if (CloseBaseStream)
				{
					_baseStream.Flush();
					_baseStream.Dispose();
				}
			}
		}

		public void TakeLock()
		{
			Monitor.Enter(_localLock);
			if (++_lockCount == 1)
			{
				_owningThreadId = Environment.CurrentManagedThreadId;
			}
		}

		public void CheckLock()
		{
			if (_owningThreadId != Environment.CurrentManagedThreadId)
			{
				throw new SynchronizationLockException();
			}
		}

		public void ReleaseLock()
		{
			CheckLock();
			if (--_lockCount == 0)
			{
				_owningThreadId = int.MaxValue;
			}
			Monitor.Exit(_localLock);
		}

		public void Discard(int bytes)
		{
			CheckLock();
			_buffer.DiscardThrough(_buffer.BaseOffset + bytes);
		}

		public void DiscardThrough(long offset)
		{
			CheckLock();
			_buffer.DiscardThrough(offset);
		}

		public override void Flush()
		{
		}

		public override int ReadByte()
		{
			CheckLock();
			int num = _buffer.ReadByte(Position);
			if (num > -1)
			{
				Seek(1L, SeekOrigin.Current);
			}
			return num;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			CheckLock();
			int num = _buffer.Read(Position, buffer, offset, count);
			Seek(num, SeekOrigin.Current);
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			CheckLock();
			switch (origin)
			{
			case SeekOrigin.Current:
				offset += Position;
				break;
			case SeekOrigin.End:
				offset += _baseStream.Length;
				break;
			}
			if (!_baseStream.CanSeek)
			{
				if (offset < _buffer.BaseOffset)
				{
					throw new InvalidOperationException("Cannot seek to before the start of the buffer!");
				}
				if (offset >= _buffer.BufferEndOffset)
				{
					throw new InvalidOperationException("Cannot seek to beyond the end of the buffer!  Discard some bytes.");
				}
			}
			return _readPosition = offset;
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
	}
}
