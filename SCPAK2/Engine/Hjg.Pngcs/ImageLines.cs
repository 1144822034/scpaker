namespace Hjg.Pngcs
{
	internal class ImageLines
	{
		internal readonly int channels;

		internal readonly int bitDepth;

		internal readonly int elementsPerRow;

		public ImageInfo ImgInfo
		{
			get;
			set;
		}

		public ImageLine.ESampleType sampleType
		{
			get;
			set;
		}

		public bool SamplesUnpacked
		{
			get;
			set;
		}

		public int RowOffset
		{
			get;
			set;
		}

		public int Nrows
		{
			get;
			set;
		}

		public int RowStep
		{
			get;
			set;
		}

		public int[][] Scanlines
		{
			get;
			set;
		}

		public byte[][] ScanlinesB
		{
			get;
			set;
		}

		public ImageLines(ImageInfo ImgInfo, ImageLine.ESampleType sampleType, bool unpackedMode, int rowOffset, int nRows, int rowStep)
		{
			this.ImgInfo = ImgInfo;
			channels = ImgInfo.Channels;
			bitDepth = ImgInfo.BitDepth;
			this.sampleType = sampleType;
			SamplesUnpacked = (unpackedMode || !ImgInfo.Packed);
			RowOffset = rowOffset;
			Nrows = nRows;
			RowStep = rowStep;
			elementsPerRow = (unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked);
			switch (sampleType)
			{
			case ImageLine.ESampleType.INT:
			{
				Scanlines = new int[nRows][];
				for (int j = 0; j < nRows; j++)
				{
					Scanlines[j] = new int[elementsPerRow];
				}
				ScanlinesB = null;
				break;
			}
			case ImageLine.ESampleType.BYTE:
			{
				ScanlinesB = new byte[nRows][];
				for (int i = 0; i < nRows; i++)
				{
					ScanlinesB[i] = new byte[elementsPerRow];
				}
				Scanlines = null;
				break;
			}
			default:
				throw new PngjExceptionInternal("bad ImageLine initialization");
			}
		}

		public int ImageRowToMatrixRow(int imrow)
		{
			int num = (imrow - RowOffset) / RowStep;
			if (num >= 0)
			{
				if (num >= Nrows)
				{
					return Nrows - 1;
				}
				return num;
			}
			return 0;
		}

		public int ImageRowToMatrixRowStrict(int imrow)
		{
			imrow -= RowOffset;
			int num = (imrow >= 0 && imrow % RowStep == 0) ? (imrow / RowStep) : (-1);
			if (num >= Nrows)
			{
				return -1;
			}
			return num;
		}

		public int MatrixRowToImageRow(int mrow)
		{
			return mrow * RowStep + RowOffset;
		}

		public ImageLine GetImageLineAtMatrixRow(int mrow)
		{
			if (mrow < 0 || mrow > Nrows)
			{
				throw new PngjException("Bad row " + mrow.ToString() + ". Should be positive and less than " + Nrows.ToString());
			}
			ImageLine obj = (sampleType == ImageLine.ESampleType.INT) ? new ImageLine(ImgInfo, sampleType, SamplesUnpacked, Scanlines[mrow], null) : new ImageLine(ImgInfo, sampleType, SamplesUnpacked, null, ScanlinesB[mrow]);
			obj.Rown = MatrixRowToImageRow(mrow);
			return obj;
		}
	}
}
