using FluxJpeg.Core;
using FluxJpeg.Core.Decoder;
using FluxJpeg.Core.Encoder;
using System;
using System.IO;

namespace Engine.Media
{
	public static class Jpg
	{
		public static bool IsJpgStream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			long position = stream.Position;
			int num = stream.ReadByte();
			int num2 = stream.ReadByte();
			int num3 = stream.ReadByte();
			stream.Position = position;
			if (num == 255 && num2 == 216)
			{
				return num3 == 255;
			}
			return false;
		}

		public static Image Load(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			DecodedJpeg decodedJpeg = new JpegDecoder(stream).Decode();
			int width = decodedJpeg.Image.Width;
			int height = decodedJpeg.Image.Height;
			byte[][,] raster = decodedJpeg.Image.Raster;
			Image image = new Image(width, height);
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					image.Pixels[j + i * width] = new Color(raster[0][j, i], raster[1][j, i], raster[2][j, i]);
				}
			}
			return image;
		}

		public static void Save(Image image, Stream stream, int quality)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (quality < 0 || quality > 100)
			{
				throw new ArgumentOutOfRangeException("quality");
			}
			int width = image.Width;
			int height = image.Height;
			Color[] pixels = image.Pixels;
			byte[][,] array = new byte[3][,]
			{
				new byte[width, height],
				new byte[width, height],
				new byte[width, height]
			};
			byte[,] array2 = array[0];
			byte[,] array3 = array[1];
			byte[,] array4 = array[2];
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					Color color = pixels[j + i * width];
					array2[j, i] = color.R;
					array3[j, i] = color.G;
					array4[j, i] = color.B;
				}
			}
			ColorModel cm = default(ColorModel);
			cm.colorspace = ColorSpace.RGB;
			cm.Opaque = true;
			new JpegEncoder(new FluxJpeg.Core.Image(cm, array), quality, stream).Encode();
		}
	}
}
