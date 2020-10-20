namespace NVorbis.Ogg
{
	internal class Crc
	{
		public const uint CRC32_POLY = 79764919u;

		public static uint[] crcTable;

		public uint _crc;

		static Crc()
		{
			crcTable = new uint[256];
			for (uint num = 0u; num < 256; num++)
			{
				uint num2 = num << 24;
				for (int i = 0; i < 8; i++)
				{
					num2 = (uint)((int)(num2 << 1) ^ ((num2 >= 2147483648u) ? 79764919 : 0));
				}
				crcTable[num] = num2;
			}
		}

		public Crc()
		{
			Reset();
		}

		public void Reset()
		{
			_crc = 0u;
		}

		public void Update(int nextVal)
		{
			_crc = ((_crc << 8) ^ crcTable[nextVal ^ (_crc >> 24)]);
		}

		public bool Test(uint checkCrc)
		{
			return _crc == checkCrc;
		}
	}
}
