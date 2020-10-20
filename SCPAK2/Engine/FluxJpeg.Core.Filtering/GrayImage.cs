namespace FluxJpeg.Core.Filtering
{
	internal class GrayImage
	{
		public float[] Scan0;

		public int _width;

		public int _height;

		public int Width => _width;

		public int Height => _height;

		public float this[int x, int y]
		{
			get
			{
				return Scan0[y * _width + x];
			}
			set
			{
				Scan0[y * _width + x] = value;
			}
		}

		public GrayImage(int width, int height)
		{
			_width = width;
			_height = height;
			Scan0 = new float[width * height];
		}

		public GrayImage(byte[,] channel)
		{
			Convert(channel);
		}

		public void Convert(byte[,] channel)
		{
			_width = channel.GetLength(0);
			_height = channel.GetLength(1);
			Scan0 = new float[_width * _height];
			int num = 0;
			for (int i = 0; i < _height; i++)
			{
				for (int j = 0; j < _width; j++)
				{
					Scan0[num++] = (float)(int)channel[j, i] / 255f;
				}
			}
		}

		public byte[,] ToByteArray2D()
		{
			byte[,] array = new byte[_width, _height];
			int num = 0;
			for (int i = 0; i < _height; i++)
			{
				for (int j = 0; j < _width; j++)
				{
					array[j, i] = (byte)(Scan0[num++] * 255f);
				}
			}
			return array;
		}
	}
}
