using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;
using System;
using System.IO;

namespace Engine.Media
{
	public static class Png
	{
		public enum Format
		{
			RGBA8,
			RGB8,
			L8,
			LA8,
			Indexed
		}

		public struct PngInfo
		{
			public int Width;

			public int Height;

			public Format Format;
		}

		public static bool IsPngStream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			long position = stream.Position;
			int num = stream.ReadByte();
			int num2 = stream.ReadByte();
			int num3 = stream.ReadByte();
			int num4 = stream.ReadByte();
			int num5 = stream.ReadByte();
			int num6 = stream.ReadByte();
			int num7 = stream.ReadByte();
			int num8 = stream.ReadByte();
			stream.Position = position;
			if (num == 137 && num2 == 80 && num3 == 78 && num4 == 71 && num5 == 13 && num6 == 10 && num7 == 26)
			{
				return num8 == 10;
			}
			return false;
		}

		public static PngInfo GetInfo(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			PngReader pngReader = new PngReader(stream);
			pngReader.ShouldCloseStream = false;
			pngReader.End();
			PngInfo result = default(PngInfo);
			result.Width = pngReader.ImgInfo.Cols;
			result.Height = pngReader.ImgInfo.Rows;
			if (pngReader.ImgInfo.BitDepth == 8 && pngReader.ImgInfo.Channels == 4)
			{
				result.Format = Format.RGBA8;
			}
			else if (pngReader.ImgInfo.BitDepth == 8 && pngReader.ImgInfo.Channels == 3)
			{
				result.Format = Format.RGB8;
			}
			else if (pngReader.ImgInfo.BitDepth == 8 && pngReader.ImgInfo.Channels == 2 && pngReader.ImgInfo.Greyscale)
			{
				result.Format = Format.LA8;
			}
			else if (pngReader.ImgInfo.BitDepth == 8 && pngReader.ImgInfo.Channels == 1 && pngReader.ImgInfo.Greyscale)
			{
				result.Format = Format.L8;
			}
			else
			{
				if (pngReader.ImgInfo.BitDepth != 8 || pngReader.ImgInfo.Channels != 1 || !pngReader.ImgInfo.Indexed)
				{
					throw new InvalidOperationException("Unsupported PNG pixel format.");
				}
				result.Format = Format.Indexed;
			}
			return result;
		}

		public static Image Load(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			PngReader pngReader = new PngReader(stream);
			pngReader.ShouldCloseStream = false;
			pngReader.ChunkLoadBehaviour = ChunkLoadBehaviour.LOAD_CHUNK_NEVER;
			pngReader.MaxTotalBytesRead = long.MaxValue;
			ImageLines imageLines = pngReader.ReadRowsByte();
			pngReader.End();
			if (imageLines.ImgInfo.BitDepth == 8 && imageLines.ImgInfo.Channels == 4)
			{
				Image image = new Image(pngReader.ImgInfo.Cols, pngReader.ImgInfo.Rows);
				int i = 0;
				int num = 0;
				for (; i < image.Height; i++)
				{
					byte[] array = imageLines.ScanlinesB[i];
					int j = 0;
					int num2 = 0;
					for (; j < image.Width; j++)
					{
						byte r = array[num2++];
						byte g = array[num2++];
						byte b = array[num2++];
						byte a = array[num2++];
						image.Pixels[num++] = new Color(r, g, b, a);
					}
				}
				return image;
			}
			if (imageLines.ImgInfo.BitDepth == 8 && imageLines.ImgInfo.Channels == 3)
			{
				Image image2 = new Image(pngReader.ImgInfo.Cols, pngReader.ImgInfo.Rows);
				int k = 0;
				int num3 = 0;
				for (; k < image2.Height; k++)
				{
					byte[] array2 = imageLines.ScanlinesB[k];
					int l = 0;
					int num4 = 0;
					for (; l < image2.Width; l++)
					{
						byte r2 = array2[num4++];
						byte g2 = array2[num4++];
						byte b2 = array2[num4++];
						image2.Pixels[num3++] = new Color(r2, g2, b2);
					}
				}
				return image2;
			}
			if (imageLines.ImgInfo.BitDepth == 8 && imageLines.ImgInfo.Channels == 2 && imageLines.ImgInfo.Greyscale)
			{
				Image image3 = new Image(pngReader.ImgInfo.Cols, pngReader.ImgInfo.Rows);
				int m = 0;
				int num5 = 0;
				for (; m < image3.Height; m++)
				{
					byte[] array3 = imageLines.ScanlinesB[m];
					int n = 0;
					int num6 = 0;
					for (; n < image3.Width; n++)
					{
						byte b3 = array3[num6++];
						byte a2 = array3[num6++];
						image3.Pixels[num5++] = new Color(b3, b3, b3, a2);
					}
				}
				return image3;
			}
			if (imageLines.ImgInfo.BitDepth == 8 && imageLines.ImgInfo.Channels == 1 && imageLines.ImgInfo.Greyscale)
			{
				Image image4 = new Image(pngReader.ImgInfo.Cols, pngReader.ImgInfo.Rows);
				int num7 = 0;
				int num8 = 0;
				for (; num7 < image4.Height; num7++)
				{
					byte[] array4 = imageLines.ScanlinesB[num7];
					int num9 = 0;
					int num10 = 0;
					for (; num9 < image4.Width; num9++)
					{
						byte b4 = array4[num10++];
						image4.Pixels[num8++] = new Color(b4, b4, b4);
					}
				}
				return image4;
			}
			if (imageLines.ImgInfo.BitDepth == 8 && imageLines.ImgInfo.Channels == 1 && imageLines.ImgInfo.Indexed)
			{
				PngChunkPLTE pngChunkPLTE = (PngChunkPLTE)pngReader.GetChunksList().GetById1("PLTE");
				if (pngChunkPLTE == null)
				{
					throw new InvalidOperationException("PLTE chunk not found in indexed PNG.");
				}
				Image image5 = new Image(pngReader.ImgInfo.Cols, pngReader.ImgInfo.Rows);
				int num11 = 0;
				int num12 = 0;
				for (; num11 < image5.Height; num11++)
				{
					byte[] array5 = imageLines.ScanlinesB[num11];
					int num13 = 0;
					int num14 = 0;
					for (; num13 < image5.Width; num13++)
					{
						byte n2 = array5[num14++];
						int entry = pngChunkPLTE.GetEntry(n2);
						image5.Pixels[num12++] = new Color((entry >> 16) & 0xFF, (entry >> 8) & 0xFF, entry & 0xFF);
					}
				}
				return image5;
			}
			throw new InvalidOperationException("Unsupported PNG pixel format.");
		}

		public static void Save(Image image, Stream stream, Format format)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			switch (format)
			{
			case Format.RGBA8:
			{
				ImageInfo imgInfo3 = new ImageInfo(image.Width, image.Height, 8, alpha: true, grayscale: false, palette: false);
				PngWriter pngWriter3 = new PngWriter(stream, imgInfo3);
				pngWriter3.ShouldCloseStream = false;
				byte[] array3 = new byte[4 * image.Width];
				int m = 0;
				int num5 = 0;
				for (; m < image.Height; m++)
				{
					int n = 0;
					int num6 = 0;
					for (; n < image.Width; n++)
					{
						Color color3 = image.Pixels[num5++];
						array3[num6++] = color3.R;
						array3[num6++] = color3.G;
						array3[num6++] = color3.B;
						array3[num6++] = color3.A;
					}
					pngWriter3.WriteRowByte(array3, m);
				}
				pngWriter3.End();
				break;
			}
			case Format.RGB8:
			{
				ImageInfo imgInfo2 = new ImageInfo(image.Width, image.Height, 8, alpha: false, grayscale: false, palette: false);
				PngWriter pngWriter2 = new PngWriter(stream, imgInfo2);
				pngWriter2.ShouldCloseStream = false;
				byte[] array2 = new byte[3 * image.Width];
				int k = 0;
				int num3 = 0;
				for (; k < image.Height; k++)
				{
					int l = 0;
					int num4 = 0;
					for (; l < image.Width; l++)
					{
						Color color2 = image.Pixels[num3++];
						array2[num4++] = color2.R;
						array2[num4++] = color2.G;
						array2[num4++] = color2.B;
					}
					pngWriter2.WriteRowByte(array2, k);
				}
				pngWriter2.End();
				break;
			}
			case Format.LA8:
			{
				ImageInfo imgInfo4 = new ImageInfo(image.Width, image.Height, 8, alpha: true, grayscale: true, palette: false);
				PngWriter pngWriter4 = new PngWriter(stream, imgInfo4);
				pngWriter4.ShouldCloseStream = false;
				byte[] array4 = new byte[2 * image.Width];
				int num7 = 0;
				int num8 = 0;
				for (; num7 < image.Height; num7++)
				{
					int num9 = 0;
					int num10 = 0;
					for (; num9 < image.Width; num9++)
					{
						Color color4 = image.Pixels[num8++];
						array4[num10++] = (byte)((color4.R + color4.G + color4.B) / 3);
						array4[num10++] = color4.A;
					}
					pngWriter4.WriteRowByte(array4, num7);
				}
				pngWriter4.End();
				break;
			}
			case Format.L8:
			{
				ImageInfo imgInfo = new ImageInfo(image.Width, image.Height, 8, alpha: false, grayscale: true, palette: false);
				PngWriter pngWriter = new PngWriter(stream, imgInfo);
				pngWriter.ShouldCloseStream = false;
				byte[] array = new byte[image.Width];
				int i = 0;
				int num = 0;
				for (; i < image.Height; i++)
				{
					int j = 0;
					int num2 = 0;
					for (; j < image.Width; j++)
					{
						Color color = image.Pixels[num++];
						array[num2++] = (byte)((color.R + color.G + color.B) / 3);
					}
					pngWriter.WriteRowByte(array, i);
				}
				pngWriter.End();
				break;
			}
			default:
				throw new InvalidOperationException("Unsupported PNG pixel format.");
			}
		}
	}
}
