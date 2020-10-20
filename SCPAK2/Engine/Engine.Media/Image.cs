using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Media
{
	public class Image
	{
		public readonly int Width;

		public readonly int Height;

		public readonly Color[] Pixels;

		public Image(Image image)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			Width = image.Width;
			Height = image.Height;
			Pixels = (Color[])image.Pixels.Clone();
		}

		public Image(int width, int height)
		{
			if (width < 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height < 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			Width = width;
			Height = height;
			Pixels = new Color[width * height];
		}

		public Color GetPixel(int x, int y)
		{
			if (x < 0 || x >= Width)
			{
				throw new ArgumentOutOfRangeException("x");
			}
			if (y < 0 || y >= Height)
			{
				throw new ArgumentOutOfRangeException("y");
			}
			return Pixels[x + y * Width];
		}

		public void SetPixel(int x, int y, Color color)
		{
			if (x < 0 || x >= Width)
			{
				throw new ArgumentOutOfRangeException("x");
			}
			if (y < 0 || y >= Height)
			{
				throw new ArgumentOutOfRangeException("y");
			}
			Pixels[x + y * Width] = color;
		}

		public static void PremultiplyAlpha(Image image)
		{
			for (int i = 0; i < image.Pixels.Length; i++)
			{
				image.Pixels[i] = Color.PremultiplyAlpha(image.Pixels[i]);
			}
		}

		public static IEnumerable<Image> GenerateMipmaps(Image image, int maxLevelsCount = int.MaxValue)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (maxLevelsCount < 0)
			{
				throw new ArgumentOutOfRangeException("maxLevelsCount");
			}
			if (maxLevelsCount == 0)
			{
				yield break;
			}
			int mipWidth = image.Width;
			int mipHeight = image.Height;
			yield return image;
			int level = 1;
			while (true)
			{
				if (level >= maxLevelsCount)
				{
					yield break;
				}
				if ((mipWidth > 1 && mipWidth % 2 != 0) || (mipHeight > 1 && mipHeight % 2 != 0))
				{
					break;
				}
				int num = mipWidth;
				int num2 = mipHeight;
				mipWidth = MathUtils.Max(num / 2, 1);
				mipHeight = MathUtils.Max(num2 / 2, 1);
				Image mipImage = new Image(mipWidth, mipHeight);
				int num3 = num / mipWidth;
				int num4 = num2 / mipHeight;
				if (num3 == 2 && num4 == 2)
				{
					int i = 0;
					int num5 = 0;
					for (; i < mipHeight; i++)
					{
						int num6 = i * 2 * num;
						int num7 = 0;
						while (num7 < mipWidth)
						{
							Color color = image.Pixels[num6];
							Color color2 = image.Pixels[num6 + 1];
							Color color3 = image.Pixels[num6 + num];
							Color color4 = image.Pixels[num6 + num + 1];
							byte r = (byte)((color.R + color2.R + color3.R + color4.R + 2) / 4);
							byte g = (byte)((color.G + color2.G + color3.G + color4.G + 2) / 4);
							byte b = (byte)((color.B + color2.B + color3.B + color4.B + 2) / 4);
							byte a = (byte)((color.A + color2.A + color3.A + color4.A + 2) / 4);
							mipImage.Pixels[num5] = new Color(r, g, b, a);
							num7++;
							num6 += 2;
							num5++;
						}
					}
				}
				else if (num3 == 2 && num4 == 1)
				{
					int j = 0;
					int num8 = 0;
					for (; j < mipHeight; j++)
					{
						int num9 = j * num;
						int num10 = 0;
						while (num10 < mipWidth)
						{
							Color color5 = image.Pixels[num9];
							Color color6 = image.Pixels[num9 + 1];
							byte r2 = (byte)((color5.R + color6.R + 1) / 2);
							byte g2 = (byte)((color5.G + color6.G + 1) / 2);
							byte b2 = (byte)((color5.B + color6.B + 1) / 2);
							byte a2 = (byte)((color5.A + color6.A + 1) / 2);
							mipImage.Pixels[num8] = new Color(r2, g2, b2, a2);
							num10++;
							num9 += 2;
							num8++;
						}
					}
				}
				else
				{
					if (num3 != 1 || num4 != 2)
					{
						yield break;
					}
					int k = 0;
					int num11 = 0;
					for (; k < mipHeight; k++)
					{
						int num12 = k * 2 * num;
						int num13 = 0;
						while (num13 < mipWidth)
						{
							Color color7 = image.Pixels[num12];
							Color color8 = image.Pixels[num12 + num];
							byte r3 = (byte)((color7.R + color8.R + 1) / 2);
							byte g3 = (byte)((color7.G + color8.G + 1) / 2);
							byte b3 = (byte)((color7.B + color8.B + 1) / 2);
							byte a3 = (byte)((color7.A + color8.A + 1) / 2);
							mipImage.Pixels[num11] = new Color(r3, g3, b3, a3);
							num13++;
							num12++;
							num11++;
						}
					}
				}
				yield return mipImage;
				image = mipImage;
				int num14 = level + 1;
				level = num14;
			}
			throw new InvalidOperationException("Generating mipmaps with not 2:1 scaling is not supported. Limit mipmap levels count using maxLevelsCount parameter.");
		}

		public static ImageFileFormat DetermineFileFormat(string extension)
		{
			if (extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
			{
				return ImageFileFormat.Bmp;
			}
			if (extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
			{
				return ImageFileFormat.Png;
			}
			if (extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
			{
				return ImageFileFormat.Jpg;
			}
			throw new InvalidOperationException("Unsupported image file format.");
		}

		public static ImageFileFormat DetermineFileFormat(Stream stream)
		{
			if (Bmp.IsBmpStream(stream))
			{
				return ImageFileFormat.Bmp;
			}
			if (Png.IsPngStream(stream))
			{
				return ImageFileFormat.Png;
			}
			if (Jpg.IsJpgStream(stream))
			{
				return ImageFileFormat.Jpg;
			}
			throw new InvalidOperationException("Unsupported image file format.");
		}

		public static Image Load(Stream stream, ImageFileFormat format)
		{
			switch (format)
			{
			case ImageFileFormat.Bmp:
				return Bmp.Load(stream);
			case ImageFileFormat.Png:
				return Png.Load(stream);
			case ImageFileFormat.Jpg:
				return Jpg.Load(stream);
			default:
				throw new InvalidOperationException("Unsupported image file format.");
			}
		}

		public static Image Load(string fileName, ImageFileFormat format)
		{
			using (Stream stream = Storage.OpenFile(fileName, OpenFileMode.Read))
			{
				return Load(stream, format);
			}
		}

		public static Image Load(Stream stream)
		{
			PeekStream peekStream = new PeekStream(stream, 64);
			ImageFileFormat format = DetermineFileFormat(peekStream.GetInitialBytesStream());
			return Load(peekStream, format);
		}

		public static Image Load(string fileName)
		{
			using (Stream stream = Storage.OpenFile(fileName, OpenFileMode.Read))
			{
				return Load(stream);
			}
		}

		public static void Save(Image image, Stream stream, ImageFileFormat format, bool saveAlpha)
		{
			switch (format)
			{
			case ImageFileFormat.Bmp:
				Bmp.Save(image, stream, (!saveAlpha) ? Bmp.Format.RGB8 : Bmp.Format.RGBA8);
				break;
			case ImageFileFormat.Png:
				Png.Save(image, stream, (!saveAlpha) ? Png.Format.RGB8 : Png.Format.RGBA8);
				break;
			case ImageFileFormat.Jpg:
				Jpg.Save(image, stream, 95);
				break;
			default:
				throw new InvalidOperationException("Unsupported image file format.");
			}
		}

		public static void Save(Image image, string fileName, ImageFileFormat format, bool saveAlpha)
		{
			using (Stream stream = Storage.OpenFile(fileName, OpenFileMode.Create))
			{
				Save(image, stream, format, saveAlpha);
			}
		}
	}
}
