namespace Hjg.Pngcs
{
	internal class PngDeinterlacer
	{
		public readonly ImageInfo imi;

		public int pass;

		public int rows;

		public int cols;

		public int dY;

		public int dX;

		public int oY;

		public int oX;

		public int oXsamples;

		public int dXsamples;

		public int currRowSubimg = -1;

		public int currRowReal = -1;

		public readonly int packedValsPerPixel;

		public readonly int packedMask;

		public readonly int packedShift;

		public int[][] imageInt;

		public byte[][] imageByte;

		internal PngDeinterlacer(ImageInfo iminfo)
		{
			imi = iminfo;
			pass = 0;
			if (imi.Packed)
			{
				packedValsPerPixel = 8 / imi.BitDepth;
				packedShift = imi.BitDepth;
				if (imi.BitDepth == 1)
				{
					packedMask = 128;
				}
				else if (imi.BitDepth == 2)
				{
					packedMask = 192;
				}
				else
				{
					packedMask = 240;
				}
			}
			else
			{
				packedMask = (packedShift = (packedValsPerPixel = 1));
			}
			setPass(1);
			setRow(0);
		}

		internal void setRow(int n)
		{
			currRowSubimg = n;
			currRowReal = n * dY + oY;
			if (currRowReal < 0 || currRowReal >= imi.Rows)
			{
				throw new PngjExceptionInternal("bad row - this should not happen");
			}
		}

		internal void setPass(int p)
		{
			if (pass != p)
			{
				pass = p;
				switch (pass)
				{
				case 1:
					dY = (dX = 8);
					oX = (oY = 0);
					break;
				case 2:
					dY = (dX = 8);
					oX = 4;
					oY = 0;
					break;
				case 3:
					dX = 4;
					dY = 8;
					oX = 0;
					oY = 4;
					break;
				case 4:
					dX = (dY = 4);
					oX = 2;
					oY = 0;
					break;
				case 5:
					dX = 2;
					dY = 4;
					oX = 0;
					oY = 2;
					break;
				case 6:
					dX = (dY = 2);
					oX = 1;
					oY = 0;
					break;
				case 7:
					dX = 1;
					dY = 2;
					oX = 0;
					oY = 1;
					break;
				default:
					throw new PngjExceptionInternal("bad interlace pass" + pass.ToString());
				}
				rows = (imi.Rows - oY) / dY + 1;
				if ((rows - 1) * dY + oY >= imi.Rows)
				{
					rows--;
				}
				cols = (imi.Cols - oX) / dX + 1;
				if ((cols - 1) * dX + oX >= imi.Cols)
				{
					cols--;
				}
				if (cols == 0)
				{
					rows = 0;
				}
				dXsamples = dX * imi.Channels;
				oXsamples = oX * imi.Channels;
			}
		}

		internal void deinterlaceInt(int[] src, int[] dst, bool readInPackedFormat)
		{
			if (!(imi.Packed && readInPackedFormat))
			{
				int num = 0;
				int num2 = oXsamples;
				while (num < cols * imi.Channels)
				{
					for (int i = 0; i < imi.Channels; i++)
					{
						dst[num2 + i] = src[num + i];
					}
					num += imi.Channels;
					num2 += dXsamples;
				}
			}
			else
			{
				deinterlaceIntPacked(src, dst);
			}
		}

		public void deinterlaceIntPacked(int[] src, int[] dst)
		{
			int num = 0;
			int num2 = packedMask;
			int num3 = -1;
			int num4 = 0;
			int num5 = oX;
			while (num4 < cols)
			{
				num = num4 / packedValsPerPixel;
				num3++;
				if (num3 >= packedValsPerPixel)
				{
					num3 = 0;
				}
				num2 >>= packedShift;
				if (num3 == 0)
				{
					num2 = packedMask;
				}
				int num6 = num5 / packedValsPerPixel;
				int num7 = num5 % packedValsPerPixel;
				int num8 = src[num] & num2;
				int num9 = num7 - num3;
				if (num9 > 0)
				{
					num8 >>= num9 * packedShift;
				}
				else if (num9 < 0)
				{
					num8 <<= -num9 * packedShift;
				}
				dst[num6] |= num8;
				num4++;
				num5 += dX;
			}
		}

		internal void deinterlaceByte(byte[] src, byte[] dst, bool readInPackedFormat)
		{
			if (!(imi.Packed && readInPackedFormat))
			{
				int num = 0;
				int num2 = oXsamples;
				while (num < cols * imi.Channels)
				{
					for (int i = 0; i < imi.Channels; i++)
					{
						dst[num2 + i] = src[num + i];
					}
					num += imi.Channels;
					num2 += dXsamples;
				}
			}
			else
			{
				deinterlacePackedByte(src, dst);
			}
		}

		public void deinterlacePackedByte(byte[] src, byte[] dst)
		{
			int num = 0;
			int num2 = packedMask;
			int num3 = -1;
			int num4 = 0;
			int num5 = oX;
			while (num4 < cols)
			{
				num = num4 / packedValsPerPixel;
				num3++;
				if (num3 >= packedValsPerPixel)
				{
					num3 = 0;
				}
				num2 >>= packedShift;
				if (num3 == 0)
				{
					num2 = packedMask;
				}
				int num6 = num5 / packedValsPerPixel;
				int num7 = num5 % packedValsPerPixel;
				int num8 = src[num] & num2;
				int num9 = num7 - num3;
				if (num9 > 0)
				{
					num8 >>= num9 * packedShift;
				}
				else if (num9 < 0)
				{
					num8 <<= -num9 * packedShift;
				}
				dst[num6] |= (byte)num8;
				num4++;
				num5 += dX;
			}
		}

		internal bool isAtLastRow()
		{
			if (pass == 7)
			{
				return currRowSubimg == rows - 1;
			}
			return false;
		}

		internal int getCurrRowSubimg()
		{
			return currRowSubimg;
		}

		internal int getCurrRowReal()
		{
			return currRowReal;
		}

		internal int getPass()
		{
			return pass;
		}

		internal int getRows()
		{
			return rows;
		}

		internal int getCols()
		{
			return cols;
		}

		internal int getPixelsToRead()
		{
			return getCols();
		}

		internal int[][] getImageInt()
		{
			return imageInt;
		}

		internal void setImageInt(int[][] imageInt)
		{
			this.imageInt = imageInt;
		}

		internal byte[][] getImageByte()
		{
			return imageByte;
		}

		internal void setImageByte(byte[][] imageByte)
		{
			this.imageByte = imageByte;
		}
	}
}
