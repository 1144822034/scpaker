using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NVorbis
{
	internal static class Utils
	{
		[StructLayout(LayoutKind.Explicit)]
		public struct FloatBits
		{
			[FieldOffset(0)]
			public float Float;

			[FieldOffset(0)]
			public uint Bits;
		}

		internal static int ilog(int x)
		{
			int num = 0;
			while (x > 0)
			{
				num++;
				x >>= 1;
			}
			return num;
		}

		internal static uint BitReverse(uint n)
		{
			return BitReverse(n, 32);
		}

		internal static uint BitReverse(uint n, int bits)
		{
			n = (((uint)((int)n & -1431655766) >> 1) | ((n & 0x55555555) << 1));
			n = (((uint)((int)n & -858993460) >> 2) | ((n & 0x33333333) << 2));
			n = (((uint)((int)n & -252645136) >> 4) | ((n & 0xF0F0F0F) << 4));
			n = (((uint)((int)n & -16711936) >> 8) | ((n & 0xFF00FF) << 8));
			return ((n >> 16) | (n << 16)) >> 32 - bits;
		}

		internal static float ClipValue(float value, ref bool clipped)
		{
			FloatBits floatBits = default(FloatBits);
			floatBits.Bits = 0u;
			floatBits.Float = value;
			if ((floatBits.Bits & int.MaxValue) > 1065353215)
			{
				clipped = true;
				floatBits.Bits = (uint)(0x3F7FFFFF | ((int)floatBits.Bits & int.MinValue));
			}
			return floatBits.Float;
		}

		internal static float ConvertFromVorbisFloat32(uint bits)
		{
			int num = (int)bits >> 31;
			double y = (int)(((bits & 0x7FE00000) >> 21) - 788);
			return (float)(((bits & 0x1FFFFF) ^ num) + (num & 1)) * (float)Math.Pow(2.0, y);
		}

		internal static int Sum(Queue<int> queue)
		{
			int num = 0;
			for (int i = 0; i < queue.Count; i++)
			{
				int num2 = queue.Dequeue();
				num += num2;
				queue.Enqueue(num2);
			}
			return num;
		}
	}
}
