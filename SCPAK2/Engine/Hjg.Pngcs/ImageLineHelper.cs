using Hjg.Pngcs.Chunks;
using System;

namespace Hjg.Pngcs
{
	internal class ImageLineHelper
	{
		public static int[] Palette2rgb(ImageLine line, PngChunkPLTE pal, PngChunkTRNS trns, int[] buf)
		{
			bool flag = trns != null;
			int num = flag ? 4 : 3;
			int num2 = line.ImgInfo.Cols * num;
			if (buf == null || buf.Length < num2)
			{
				buf = new int[num2];
			}
			if (!line.SamplesUnpacked)
			{
				line = line.unpackToNewImageLine();
			}
			bool flag2 = line.SampleType == ImageLine.ESampleType.BYTE;
			int num3 = (trns != null) ? trns.GetPalletteAlpha().Length : 0;
			for (int i = 0; i < line.ImgInfo.Cols; i++)
			{
				int num4 = flag2 ? (line.ScanlineB[i] & 0xFF) : line.Scanline[i];
				pal.GetEntryRgb(num4, buf, i * num);
				if (flag)
				{
					int num5 = buf[i * num + 3] = ((num4 < num3) ? trns.GetPalletteAlpha()[num4] : 255);
				}
			}
			return buf;
		}

		public static int[] Palette2rgb(ImageLine line, PngChunkPLTE pal, int[] buf)
		{
			return Palette2rgb(line, pal, null, buf);
		}

		public static int ToARGB8(int r, int g, int b)
		{
			return -16777216 | (r << 16) | (g << 8) | b;
		}

		public static int ToARGB8(int r, int g, int b, int a)
		{
			return (a << 24) | (r << 16) | (g << 8) | b;
		}

		public static int ToARGB8(int[] buff, int offset, bool alpha)
		{
			if (!alpha)
			{
				return ToARGB8(buff[offset++], buff[offset++], buff[offset++], buff[offset]);
			}
			return ToARGB8(buff[offset++], buff[offset++], buff[offset]);
		}

		public static int ToARGB8(byte[] buff, int offset, bool alpha)
		{
			if (!alpha)
			{
				return ToARGB8(buff[offset++], buff[offset++], buff[offset++], buff[offset]);
			}
			return ToARGB8(buff[offset++], buff[offset++], buff[offset]);
		}

		public static void FromARGB8(int val, int[] buff, int offset, bool alpha)
		{
			buff[offset++] = ((val >> 16) & 0xFF);
			buff[offset++] = ((val >> 8) & 0xFF);
			buff[offset] = (val & 0xFF);
			if (alpha)
			{
				buff[offset + 1] = ((val >> 24) & 0xFF);
			}
		}

		public static void FromARGB8(int val, byte[] buff, int offset, bool alpha)
		{
			buff[offset++] = (byte)((val >> 16) & 0xFF);
			buff[offset++] = (byte)((val >> 8) & 0xFF);
			buff[offset] = (byte)(val & 0xFF);
			if (alpha)
			{
				buff[offset + 1] = (byte)((val >> 24) & 0xFF);
			}
		}

		public static int GetPixelToARGB8(ImageLine line, int column)
		{
			if (line.IsInt())
			{
				return ToARGB8(line.Scanline, column * line.channels, line.ImgInfo.Alpha);
			}
			return ToARGB8(line.ScanlineB, column * line.channels, line.ImgInfo.Alpha);
		}

		public static void SetPixelFromARGB8(ImageLine line, int column, int argb)
		{
			if (line.IsInt())
			{
				FromARGB8(argb, line.Scanline, column * line.channels, line.ImgInfo.Alpha);
			}
			else
			{
				FromARGB8(argb, line.ScanlineB, column * line.channels, line.ImgInfo.Alpha);
			}
		}

		public static void SetPixel(ImageLine line, int col, int r, int g, int b, int a)
		{
			int num = col * line.channels;
			if (line.IsInt())
			{
				line.Scanline[num++] = r;
				line.Scanline[num++] = g;
				line.Scanline[num] = b;
				if (line.ImgInfo.Alpha)
				{
					line.Scanline[num + 1] = a;
				}
			}
			else
			{
				line.ScanlineB[num++] = (byte)r;
				line.ScanlineB[num++] = (byte)g;
				line.ScanlineB[num] = (byte)b;
				if (line.ImgInfo.Alpha)
				{
					line.ScanlineB[num + 1] = (byte)a;
				}
			}
		}

		public static void SetPixel(ImageLine line, int col, int r, int g, int b)
		{
			SetPixel(line, col, r, g, b, line.maxSampleVal);
		}

		public static double ReadDouble(ImageLine line, int pos)
		{
			if (line.IsInt())
			{
				return (double)line.Scanline[pos] / ((double)line.maxSampleVal + 0.9);
			}
			return (double)(int)line.ScanlineB[pos] / ((double)line.maxSampleVal + 0.9);
		}

		public static void WriteDouble(ImageLine line, double d, int pos)
		{
			if (line.IsInt())
			{
				line.Scanline[pos] = (int)(d * ((double)line.maxSampleVal + 0.99));
			}
			else
			{
				line.ScanlineB[pos] = (byte)(d * ((double)line.maxSampleVal + 0.99));
			}
		}

		public static int Interpol(int a, int b, int c, int d, double dx, double dy)
		{
			double num = (double)a * (1.0 - dx) + (double)b * dx;
			double num2 = (double)c * (1.0 - dx) + (double)d * dx;
			return (int)(num * (1.0 - dy) + num2 * dy + 0.5);
		}

		public static int ClampTo_0_255(int i)
		{
			if (i <= 255)
			{
				if (i >= 0)
				{
					return i;
				}
				return 0;
			}
			return 255;
		}

		public static double ClampDouble(double i)
		{
			if (!(i < 0.0))
			{
				if (!(i >= 1.0))
				{
					return i;
				}
				return 0.999999;
			}
			return 0.0;
		}

		public static int ClampTo_0_65535(int i)
		{
			if (i <= 65535)
			{
				if (i >= 0)
				{
					return i;
				}
				return 0;
			}
			return 65535;
		}

		public static int ClampTo_128_127(int x)
		{
			if (x <= 127)
			{
				if (x >= -128)
				{
					return x;
				}
				return -128;
			}
			return 127;
		}

		public static int[] Unpack(ImageInfo imgInfo, int[] src, int[] dst, bool scale)
		{
			int samplesPerRow = imgInfo.SamplesPerRow;
			int samplesPerRowPacked = imgInfo.SamplesPerRowPacked;
			if (dst == null || dst.Length < samplesPerRow)
			{
				dst = new int[samplesPerRow];
			}
			if (imgInfo.Packed)
			{
				ImageLine.unpackInplaceInt(imgInfo, src, dst, scale);
			}
			else
			{
				Array.Copy(src, 0, dst, 0, samplesPerRowPacked);
			}
			return dst;
		}

		public static byte[] Unpack(ImageInfo imgInfo, byte[] src, byte[] dst, bool scale)
		{
			int samplesPerRow = imgInfo.SamplesPerRow;
			int samplesPerRowPacked = imgInfo.SamplesPerRowPacked;
			if (dst == null || dst.Length < samplesPerRow)
			{
				dst = new byte[samplesPerRow];
			}
			if (imgInfo.Packed)
			{
				ImageLine.unpackInplaceByte(imgInfo, src, dst, scale);
			}
			else
			{
				Array.Copy(src, 0, dst, 0, samplesPerRowPacked);
			}
			return dst;
		}

		public static int[] Pack(ImageInfo imgInfo, int[] src, int[] dst, bool scale)
		{
			int samplesPerRowPacked = imgInfo.SamplesPerRowPacked;
			if (dst == null || dst.Length < samplesPerRowPacked)
			{
				dst = new int[samplesPerRowPacked];
			}
			if (imgInfo.Packed)
			{
				ImageLine.packInplaceInt(imgInfo, src, dst, scale);
			}
			else
			{
				Array.Copy(src, 0, dst, 0, samplesPerRowPacked);
			}
			return dst;
		}

		public static byte[] Pack(ImageInfo imgInfo, byte[] src, byte[] dst, bool scale)
		{
			int samplesPerRowPacked = imgInfo.SamplesPerRowPacked;
			if (dst == null || dst.Length < samplesPerRowPacked)
			{
				dst = new byte[samplesPerRowPacked];
			}
			if (imgInfo.Packed)
			{
				ImageLine.packInplaceByte(imgInfo, src, dst, scale);
			}
			else
			{
				Array.Copy(src, 0, dst, 0, samplesPerRowPacked);
			}
			return dst;
		}
	}
}
