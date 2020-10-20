using System;
using System.Collections.Generic;

namespace NVorbis
{
	internal abstract class DataPacket
	{
		[Flags]
		public enum PacketFlags : byte
		{
			IsResync = 0x1,
			IsEndOfStream = 0x2,
			IsShort = 0x4,
			HasGranuleCount = 0x8,
			User1 = 0x10,
			User2 = 0x20,
			User3 = 0x40,
			User4 = 0x80
		}

		public ulong _bitBucket;

		public int _bitCount;

		public int _readBits;

		public byte _overflowBits;

		public PacketFlags _packetFlags;

		public long _granulePosition;

		public long _pageGranulePosition;

		public int _length;

		public int _granuleCount;

		public int _pageSequenceNumber;

		public bool IsResync
		{
			get
			{
				return GetFlag(PacketFlags.IsResync);
			}
			internal set
			{
				SetFlag(PacketFlags.IsResync, value);
			}
		}

		public long GranulePosition
		{
			get
			{
				return _granulePosition;
			}
			set
			{
				_granulePosition = value;
			}
		}

		public long PageGranulePosition
		{
			get
			{
				return _pageGranulePosition;
			}
			internal set
			{
				_pageGranulePosition = value;
			}
		}

		public int Length
		{
			get
			{
				return _length;
			}
			set
			{
				_length = value;
			}
		}

		public bool IsEndOfStream
		{
			get
			{
				return GetFlag(PacketFlags.IsEndOfStream);
			}
			internal set
			{
				SetFlag(PacketFlags.IsEndOfStream, value);
			}
		}

		public long BitsRead => _readBits;

		public int? GranuleCount
		{
			get
			{
				if (GetFlag(PacketFlags.HasGranuleCount))
				{
					return _granuleCount;
				}
				return null;
			}
			set
			{
				if (value.HasValue)
				{
					_granuleCount = value.Value;
					SetFlag(PacketFlags.HasGranuleCount, value: true);
				}
				else
				{
					SetFlag(PacketFlags.HasGranuleCount, value: false);
				}
			}
		}

		internal int PageSequenceNumber
		{
			get
			{
				return _pageSequenceNumber;
			}
			set
			{
				_pageSequenceNumber = value;
			}
		}

		internal bool IsShort
		{
			get
			{
				return GetFlag(PacketFlags.IsShort);
			}
			set
			{
				SetFlag(PacketFlags.IsShort, value);
			}
		}

		public bool GetFlag(PacketFlags flag)
		{
			return (_packetFlags & flag) == flag;
		}

		public void SetFlag(PacketFlags flag, bool value)
		{
			if (value)
			{
				_packetFlags |= flag;
			}
			else
			{
				_packetFlags &= (PacketFlags)(byte)(~(uint)flag);
			}
		}

		public DataPacket(int length)
		{
			Length = length;
		}

		public abstract int ReadNextByte();

		public virtual void Done()
		{
		}

		public ulong TryPeekBits(int count, out int bitsRead)
		{
			ulong num = 0uL;
			switch (count)
			{
			default:
				throw new ArgumentOutOfRangeException("count");
			case 0:
				bitsRead = 0;
				return 0uL;
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 26:
			case 27:
			case 28:
			case 29:
			case 30:
			case 31:
			case 32:
			case 33:
			case 34:
			case 35:
			case 36:
			case 37:
			case 38:
			case 39:
			case 40:
			case 41:
			case 42:
			case 43:
			case 44:
			case 45:
			case 46:
			case 47:
			case 48:
			case 49:
			case 50:
			case 51:
			case 52:
			case 53:
			case 54:
			case 55:
			case 56:
			case 57:
			case 58:
			case 59:
			case 60:
			case 61:
			case 62:
			case 63:
			case 64:
				break;
			}
			while (_bitCount < count)
			{
				int num2 = ReadNextByte();
				if (num2 == -1)
				{
					bitsRead = _bitCount;
					num = _bitBucket;
					_bitBucket = 0uL;
					_bitCount = 0;
					IsShort = true;
					return num;
				}
				_bitBucket = (ulong)(((long)(num2 & 0xFF) << _bitCount) | (long)_bitBucket);
				_bitCount += 8;
				if (_bitCount > 64)
				{
					_overflowBits = (byte)(num2 >> 72 - _bitCount);
				}
			}
			num = _bitBucket;
			if (count < 64)
			{
				num = (ulong)((long)num & ((1L << count) - 1));
			}
			bitsRead = count;
			return num;
		}

		public void SkipBits(int count)
		{
			if (count == 0)
			{
				return;
			}
			if (_bitCount > count)
			{
				if (count > 63)
				{
					_bitBucket = 0uL;
				}
				else
				{
					_bitBucket >>= count;
				}
				if (_bitCount > 64)
				{
					int num = _bitCount - 64;
					_bitBucket |= (ulong)_overflowBits << _bitCount - count - num;
					if (num > count)
					{
						_overflowBits = (byte)(_overflowBits >> count);
					}
				}
				_bitCount -= count;
				_readBits += count;
				return;
			}
			if (_bitCount == count)
			{
				_bitBucket = 0uL;
				_bitCount = 0;
				_readBits += count;
				return;
			}
			count -= _bitCount;
			_readBits += _bitCount;
			_bitCount = 0;
			_bitBucket = 0uL;
			while (count > 8)
			{
				if (ReadNextByte() == -1)
				{
					count = 0;
					IsShort = true;
					break;
				}
				count -= 8;
				_readBits += 8;
			}
			if (count > 0)
			{
				int num2 = ReadNextByte();
				if (num2 == -1)
				{
					IsShort = true;
					return;
				}
				_bitBucket = (ulong)(num2 >> count);
				_bitCount = 8 - count;
				_readBits += count;
			}
		}

		public void ResetBitReader()
		{
			_bitBucket = 0uL;
			_bitCount = 0;
			_readBits = 0;
			IsShort = false;
		}

		public ulong ReadBits(int count)
		{
			if (count == 0)
			{
				return 0uL;
			}
			int bitsRead;
			ulong result = TryPeekBits(count, out bitsRead);
			SkipBits(count);
			return result;
		}

		public byte PeekByte()
		{
			int bitsRead;
			return (byte)TryPeekBits(8, out bitsRead);
		}

		public byte ReadByte()
		{
			return (byte)ReadBits(8);
		}

		public byte[] ReadBytes(int count)
		{
			List<byte> list = new List<byte>(count);
			while (list.Count < count)
			{
				list.Add(ReadByte());
			}
			return list.ToArray();
		}

		public int Read(byte[] buffer, int index, int count)
		{
			if (index < 0 || index + count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			for (int i = 0; i < count; i++)
			{
				int bitsRead;
				byte b = (byte)TryPeekBits(8, out bitsRead);
				if (bitsRead == 0)
				{
					return i;
				}
				buffer[index++] = b;
				SkipBits(8);
			}
			return count;
		}

		public bool ReadBit()
		{
			return ReadBits(1) == 1;
		}

		public short ReadInt16()
		{
			return (short)ReadBits(16);
		}

		public int ReadInt32()
		{
			return (int)ReadBits(32);
		}

		public long ReadInt64()
		{
			return (long)ReadBits(64);
		}

		public ushort ReadUInt16()
		{
			return (ushort)ReadBits(16);
		}

		public uint ReadUInt32()
		{
			return (uint)ReadBits(32);
		}

		public ulong ReadUInt64()
		{
			return ReadBits(64);
		}

		public void SkipBytes(int count)
		{
			SkipBits(count * 8);
		}
	}
}
