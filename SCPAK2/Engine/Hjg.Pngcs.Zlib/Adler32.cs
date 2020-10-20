namespace Hjg.Pngcs.Zlib
{
	internal class Adler32
	{
		public uint a = 1u;

		public uint b;

		public const int _base = 65521;

		public const int _nmax = 5550;

		public int pend;

		public void Update(byte data)
		{
			if (pend >= 5550)
			{
				updateModulus();
			}
			a += data;
			b += a;
			pend++;
		}

		public void Update(byte[] data)
		{
			Update(data, 0, data.Length);
		}

		public void Update(byte[] data, int offset, int length)
		{
			int num = 5550 - pend;
			for (int i = 0; i < length; i++)
			{
				if (i == num)
				{
					updateModulus();
					num = i + 5550;
				}
				a += data[i + offset];
				b += a;
				pend++;
			}
		}

		public void Reset()
		{
			a = 1u;
			b = 0u;
			pend = 0;
		}

		public void updateModulus()
		{
			a %= 65521u;
			b %= 65521u;
			pend = 0;
		}

		public uint GetValue()
		{
			if (pend > 0)
			{
				updateModulus();
			}
			return (b << 16) | a;
		}
	}
}
