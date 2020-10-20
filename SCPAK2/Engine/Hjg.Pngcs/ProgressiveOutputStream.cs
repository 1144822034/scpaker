namespace Hjg.Pngcs
{
	internal abstract class ProgressiveOutputStream : CustomMemoryStream
	{
		public readonly int size;

		public long countFlushed;

		public ProgressiveOutputStream(int size_0)
		{
			size = size_0;
			if (size < 8)
			{
				throw new PngjException("bad size for ProgressiveOutputStream: " + size.ToString());
			}
		}

		public override void Close()
		{
			Flush();
			base.Close();
		}

		public override void Flush()
		{
			base.Flush();
			CheckFlushBuffer(forced: true);
		}

		public override void Write(byte[] b, int off, int len)
		{
			base.Write(b, off, len);
			CheckFlushBuffer(forced: false);
		}

		public void Write(byte[] b)
		{
			Write(b, 0, b.Length);
			CheckFlushBuffer(forced: false);
		}

		public void CheckFlushBuffer(bool forced)
		{
			int num = (int)Position;
			byte[] array = ToArray();
			while (forced || num >= size)
			{
				int num2 = size;
				if (num2 > num)
				{
					num2 = num;
				}
				if (num2 == 0)
				{
					break;
				}
				FlushBuffer(array, num2);
				countFlushed += num2;
				int num3 = num - num2;
				num = num3;
				Position = 0L;
				if (num3 > 0)
				{
					Write(array, num2, num3);
				}
			}
		}

		public abstract void FlushBuffer(byte[] b, int n);

		public long GetCountFlushed()
		{
			return countFlushed;
		}
	}
}
