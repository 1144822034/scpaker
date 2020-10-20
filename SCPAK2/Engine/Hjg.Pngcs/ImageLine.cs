using System;

namespace Hjg.Pngcs
{
	internal class ImageLine
	{
		public enum ESampleType
		{
			INT,
			BYTE
		}

		internal readonly int channels;

		internal readonly int bitDepth;

		public ImageInfo ImgInfo
		{
			get;
			set;
		}

		public int[] Scanline
		{
			get;
			set;
		}

		public byte[] ScanlineB
		{
			get;
			set;
		}

		public int Rown
		{
			get;
			set;
		}

		public int ElementsPerRow
		{
			get;
			set;
		}

		public int maxSampleVal
		{
			get;
			set;
		}

		public ESampleType SampleType
		{
			get;
			set;
		}

		public bool SamplesUnpacked
		{
			get;
			set;
		}

		public FilterType FilterUsed
		{
			get;
			set;
		}

		public ImageLine(ImageInfo imgInfo)
			: this(imgInfo, ESampleType.INT, unpackedMode: false)
		{
		}

		public ImageLine(ImageInfo imgInfo, ESampleType stype)
			: this(imgInfo, stype, unpackedMode: false)
		{
		}

		public ImageLine(ImageInfo imgInfo, ESampleType stype, bool unpackedMode)
			: this(imgInfo, stype, unpackedMode, null, null)
		{
		}

		internal ImageLine(ImageInfo imgInfo, ESampleType stype, bool unpackedMode, int[] sci, byte[] scb)
		{
			ImgInfo = imgInfo;
			channels = imgInfo.Channels;
			bitDepth = imgInfo.BitDepth;
			FilterUsed = FilterType.FILTER_UNKNOWN;
			SampleType = stype;
			SamplesUnpacked = (unpackedMode || !imgInfo.Packed);
			ElementsPerRow = (SamplesUnpacked ? imgInfo.SamplesPerRow : imgInfo.SamplesPerRowPacked);
			switch (stype)
			{
			case ESampleType.INT:
				Scanline = ((sci != null) ? sci : new int[ElementsPerRow]);
				ScanlineB = null;
				maxSampleVal = ((bitDepth == 16) ? 65535 : GetMaskForPackedFormatsLs(bitDepth));
				break;
			case ESampleType.BYTE:
				ScanlineB = ((scb != null) ? scb : new byte[ElementsPerRow]);
				Scanline = null;
				maxSampleVal = ((bitDepth == 16) ? 255 : GetMaskForPackedFormatsLs(bitDepth));
				break;
			default:
				throw new PngjExceptionInternal("bad ImageLine initialization");
			}
			Rown = -1;
		}

		internal static void unpackInplaceInt(ImageInfo iminfo, int[] src, int[] dst, bool Scale)
		{
			int num = iminfo.BitDepth;
			if (num >= 8)
			{
				return;
			}
			int maskForPackedFormatsLs = GetMaskForPackedFormatsLs(num);
			int num2 = 8 - num;
			int num3 = 8 * iminfo.SamplesPerRowPacked - num * iminfo.SamplesPerRow;
			int num4;
			int num5;
			if (num3 != 8)
			{
				num4 = maskForPackedFormatsLs << num3;
				num5 = num3;
			}
			else
			{
				num4 = maskForPackedFormatsLs;
				num5 = 0;
			}
			int num6 = iminfo.SamplesPerRow - 1;
			int num7 = iminfo.SamplesPerRowPacked - 1;
			while (num6 >= 0)
			{
				int num8 = (src[num7] & num4) >> num5;
				if (Scale)
				{
					num8 <<= num2;
				}
				dst[num6] = num8;
				num4 <<= num;
				num5 += num;
				if (num5 == 8)
				{
					num4 = maskForPackedFormatsLs;
					num5 = 0;
					num7--;
				}
				num6--;
			}
		}

		internal static void packInplaceInt(ImageInfo iminfo, int[] src, int[] dst, bool scaled)
		{
			int num = iminfo.BitDepth;
			if (num >= 8)
			{
				return;
			}
			int maskForPackedFormatsLs = GetMaskForPackedFormatsLs(num);
			int num2 = 8 - num;
			int num3 = 8 - num;
			int num4 = 8 - num;
			int num5 = src[0];
			dst[0] = 0;
			if (scaled)
			{
				num5 >>= num2;
			}
			num5 = (num5 & maskForPackedFormatsLs) << num4;
			int num6 = 0;
			for (int i = 0; i < iminfo.SamplesPerRow; i++)
			{
				int num7 = src[i];
				if (scaled)
				{
					num7 >>= num2;
				}
				dst[num6] |= (num7 & maskForPackedFormatsLs) << num4;
				num4 -= num;
				if (num4 < 0)
				{
					num4 = num3;
					num6++;
					dst[num6] = 0;
				}
			}
			dst[0] |= num5;
		}

		internal static void unpackInplaceByte(ImageInfo iminfo, byte[] src, byte[] dst, bool scale)
		{
			int num = iminfo.BitDepth;
			if (num >= 8)
			{
				return;
			}
			int maskForPackedFormatsLs = GetMaskForPackedFormatsLs(num);
			int num2 = 8 - num;
			int num3 = 8 * iminfo.SamplesPerRowPacked - num * iminfo.SamplesPerRow;
			int num4;
			int num5;
			if (num3 != 8)
			{
				num4 = maskForPackedFormatsLs << num3;
				num5 = num3;
			}
			else
			{
				num4 = maskForPackedFormatsLs;
				num5 = 0;
			}
			int num6 = iminfo.SamplesPerRow - 1;
			int num7 = iminfo.SamplesPerRowPacked - 1;
			while (num6 >= 0)
			{
				int num8 = (src[num7] & num4) >> num5;
				if (scale)
				{
					num8 <<= num2;
				}
				dst[num6] = (byte)num8;
				num4 <<= num;
				num5 += num;
				if (num5 == 8)
				{
					num4 = maskForPackedFormatsLs;
					num5 = 0;
					num7--;
				}
				num6--;
			}
		}

		internal static void packInplaceByte(ImageInfo iminfo, byte[] src, byte[] dst, bool scaled)
		{
			int num = iminfo.BitDepth;
			if (num >= 8)
			{
				return;
			}
			byte b = (byte)GetMaskForPackedFormatsLs(num);
			byte b2 = (byte)(8 - num);
			byte b3 = (byte)(8 - num);
			int num2 = 8 - num;
			byte b4 = src[0];
			dst[0] = 0;
			if (scaled)
			{
				b4 = (byte)(b4 >> (int)b2);
			}
			b4 = (byte)((b4 & b) << num2);
			int num3 = 0;
			for (int i = 0; i < iminfo.SamplesPerRow; i++)
			{
				byte b5 = src[i];
				if (scaled)
				{
					b5 = (byte)(b5 >> (int)b2);
				}
				dst[num3] |= (byte)((b5 & b) << num2);
				num2 -= num;
				if (num2 < 0)
				{
					num2 = b3;
					num3++;
					dst[num3] = 0;
				}
			}
			dst[0] |= b4;
		}

		internal void SetScanLine(int[] b)
		{
			Array.Copy(b, 0, Scanline, 0, Scanline.Length);
		}

		internal int[] GetScanLineCopy(int[] b)
		{
			if (b == null || b.Length < Scanline.Length)
			{
				b = new int[Scanline.Length];
			}
			Array.Copy(Scanline, 0, b, 0, Scanline.Length);
			return b;
		}

		public ImageLine unpackToNewImageLine()
		{
			ImageLine imageLine = new ImageLine(ImgInfo, SampleType, unpackedMode: true);
			if (SampleType == ESampleType.INT)
			{
				unpackInplaceInt(ImgInfo, Scanline, imageLine.Scanline, Scale: false);
			}
			else
			{
				unpackInplaceByte(ImgInfo, ScanlineB, imageLine.ScanlineB, scale: false);
			}
			return imageLine;
		}

		public ImageLine packToNewImageLine()
		{
			ImageLine imageLine = new ImageLine(ImgInfo, SampleType, unpackedMode: false);
			if (SampleType == ESampleType.INT)
			{
				packInplaceInt(ImgInfo, Scanline, imageLine.Scanline, scaled: false);
			}
			else
			{
				packInplaceByte(ImgInfo, ScanlineB, imageLine.ScanlineB, scaled: false);
			}
			return imageLine;
		}

		public int[] GetScanlineInt()
		{
			return Scanline;
		}

		public byte[] GetScanlineByte()
		{
			return ScanlineB;
		}

		public bool IsInt()
		{
			return SampleType == ESampleType.INT;
		}

		public bool IsByte()
		{
			return SampleType == ESampleType.BYTE;
		}

		public override string ToString()
		{
			return "row=" + Rown.ToString() + " cols=" + ImgInfo.Cols.ToString() + " bpc=" + ImgInfo.BitDepth.ToString() + " size=" + Scanline.Length.ToString();
		}

		internal static int GetMaskForPackedFormats(int bitDepth)
		{
			switch (bitDepth)
			{
			case 4:
				return 240;
			case 2:
				return 192;
			case 1:
				return 128;
			default:
				return 255;
			}
		}

		internal static int GetMaskForPackedFormatsLs(int bitDepth)
		{
			switch (bitDepth)
			{
			case 4:
				return 15;
			case 2:
				return 3;
			case 1:
				return 1;
			default:
				return 255;
			}
		}
	}
}
