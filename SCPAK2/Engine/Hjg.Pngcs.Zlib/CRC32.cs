namespace Hjg.Pngcs.Zlib
{
	internal class CRC32
	{
		public const uint defaultPolynomial = 3988292384u;

		public const uint defaultSeed = uint.MaxValue;

		public static uint[] defaultTable;

		public uint hash;

		public uint seed;

		public uint[] table;

		public CRC32()
			: this(3988292384u, uint.MaxValue)
		{
		}

		public CRC32(uint polynomial, uint seed)
		{
			table = InitializeTable(polynomial);
			this.seed = seed;
			hash = seed;
		}

		public void Update(byte[] buffer)
		{
			Update(buffer, 0, buffer.Length);
		}

		public void Update(byte[] buffer, int start, int length)
		{
			int num = 0;
			int num2 = start;
			while (num < length)
			{
				hash = ((hash >> 8) ^ table[buffer[num2] ^ (hash & 0xFF)]);
				num++;
				num2++;
			}
		}

		public uint GetValue()
		{
			return ~hash;
		}

		public void Reset()
		{
			hash = seed;
		}

		public static uint[] InitializeTable(uint polynomial)
		{
			if (polynomial == 3988292384u && defaultTable != null)
			{
				return defaultTable;
			}
			uint[] array = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint num = (uint)i;
				for (int j = 0; j < 8; j++)
				{
					num = (((num & 1) != 1) ? (num >> 1) : ((num >> 1) ^ polynomial));
				}
				array[i] = num;
			}
			if (polynomial == 3988292384u)
			{
				defaultTable = array;
			}
			return array;
		}
	}
}
