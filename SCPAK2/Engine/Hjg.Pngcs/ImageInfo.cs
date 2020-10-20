namespace Hjg.Pngcs
{
	internal class ImageInfo
	{
		public const int MAX_COLS_ROWS_VAL = 400000;

		public readonly int Cols;

		public readonly int Rows;

		public readonly int BitDepth;

		public readonly int Channels;

		public readonly int BitspPixel;

		public readonly int BytesPixel;

		public readonly int BytesPerRow;

		public readonly int SamplesPerRow;

		public readonly int SamplesPerRowPacked;

		public readonly bool Alpha;

		public readonly bool Greyscale;

		public readonly bool Indexed;

		public readonly bool Packed;

		public ImageInfo(int cols, int rows, int bitdepth, bool alpha)
			: this(cols, rows, bitdepth, alpha, grayscale: false, palette: false)
		{
		}

		public ImageInfo(int cols, int rows, int bitdepth, bool alpha, bool grayscale, bool palette)
		{
			Cols = cols;
			Rows = rows;
			Alpha = alpha;
			Indexed = palette;
			Greyscale = grayscale;
			if (Greyscale && palette)
			{
				throw new PngjException("palette and greyscale are exclusive");
			}
			Channels = ((!(grayscale | palette)) ? (alpha ? 4 : 3) : ((!alpha) ? 1 : 2));
			BitDepth = bitdepth;
			Packed = (bitdepth < 8);
			BitspPixel = Channels * BitDepth;
			BytesPixel = (BitspPixel + 7) / 8;
			BytesPerRow = (BitspPixel * cols + 7) / 8;
			SamplesPerRow = Channels * Cols;
			SamplesPerRowPacked = (Packed ? BytesPerRow : SamplesPerRow);
			switch (BitDepth)
			{
			case 1:
			case 2:
			case 4:
				if (!Indexed && !Greyscale)
				{
					throw new PngjException("only indexed or grayscale can have bitdepth=" + BitDepth.ToString());
				}
				break;
			case 16:
				if (Indexed)
				{
					throw new PngjException("indexed can't have bitdepth=" + BitDepth.ToString());
				}
				break;
			default:
				throw new PngjException("invalid bitdepth=" + BitDepth.ToString());
			case 8:
				break;
			}
			if (cols < 1 || cols > 400000)
			{
				throw new PngjException("invalid cols=" + cols.ToString() + " ???");
			}
			if (rows < 1 || rows > 400000)
			{
				throw new PngjException("invalid rows=" + rows.ToString() + " ???");
			}
		}

		public override string ToString()
		{
			return "ImageInfo [cols=" + Cols.ToString() + ", rows=" + Rows.ToString() + ", bitDepth=" + BitDepth.ToString() + ", channels=" + Channels.ToString() + ", bitspPixel=" + BitspPixel.ToString() + ", bytesPixel=" + BytesPixel.ToString() + ", bytesPerRow=" + BytesPerRow.ToString() + ", samplesPerRow=" + SamplesPerRow.ToString() + ", samplesPerRowP=" + SamplesPerRowPacked.ToString() + ", alpha=" + Alpha.ToString() + ", greyscale=" + Greyscale.ToString() + ", indexed=" + Indexed.ToString() + ", packed=" + Packed.ToString() + "]";
		}

		public override int GetHashCode()
		{
			int num = 1;
			num = 31 * num + (Alpha ? 1231 : 1237);
			num = 31 * num + BitDepth;
			num = 31 * num + Channels;
			num = 31 * num + Cols;
			num = 31 * num + (Greyscale ? 1231 : 1237);
			num = 31 * num + (Indexed ? 1231 : 1237);
			return 31 * num + Rows;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if ((object)GetType() != obj.GetType())
			{
				return false;
			}
			ImageInfo imageInfo = (ImageInfo)obj;
			if (Alpha != imageInfo.Alpha)
			{
				return false;
			}
			if (BitDepth != imageInfo.BitDepth)
			{
				return false;
			}
			if (Channels != imageInfo.Channels)
			{
				return false;
			}
			if (Cols != imageInfo.Cols)
			{
				return false;
			}
			if (Greyscale != imageInfo.Greyscale)
			{
				return false;
			}
			if (Indexed != imageInfo.Indexed)
			{
				return false;
			}
			if (Rows != imageInfo.Rows)
			{
				return false;
			}
			return true;
		}
	}
}
