using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Engine.Media
{
	public static class Bmp
	{
		public enum Format
		{
			RGBA8,
			RGB8
		}

		public struct BmpInfo
		{
			public int Width;

			public int Height;

			public Format Format;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BitmapHeader
		{
			public byte Type1;

			public byte Type2;

			public int Size;

			public short Reserved1;

			public short Reserved2;

			public int OffBits;

			public int Size2;

			public int Width;

			public int Height;

			public short Planes;

			public short BitCount;

			public int Compression;

			public int SizeImage;

			public int XPelsPerMeter;

			public int YPelsPerMeter;

			public int ClrUsed;

			public int ClrImportant;
		}

		public static bool IsBmpStream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			long position = stream.Position;
			int num = stream.ReadByte();
			int num2 = stream.ReadByte();
			stream.Position = position;
			if (num == 66)
			{
				return num2 == 77;
			}
			return false;
		}

		public static BmpInfo GetInfo(Stream stream)
		{
			BitmapHeader bitmapHeader = ReadHeader(stream);
			BmpInfo result = default(BmpInfo);
			result.Width = bitmapHeader.Width;
			result.Height = bitmapHeader.Height;
			if (bitmapHeader.BitCount == 32)
			{
				result.Format = Format.RGBA8;
			}
			else
			{
				if (bitmapHeader.BitCount != 24)
				{
					throw new InvalidOperationException("Unsupported BMP pixel format.");
				}
				result.Format = Format.RGB8;
			}
			return result;
		}

		public static Image Load(Stream stream)
		{
			BitmapHeader bitmapHeader = ReadHeader(stream);
			Image image = new Image(bitmapHeader.Width, MathUtils.Abs(bitmapHeader.Height));
			if (bitmapHeader.BitCount == 32)
			{
				byte[] array = new byte[4 * image.Width];
				for (int i = 0; i < image.Height; i++)
				{
					if (stream.Read(array, 0, array.Length) != array.Length)
					{
						throw new InvalidOperationException("BMP data truncated.");
					}
					int num = (bitmapHeader.Height < 0) ? (image.Width * (image.Height - i - 1)) : (image.Width * i);
					int j = 0;
					int num2 = 0;
					for (; j < image.Width; j++)
					{
						byte b = array[num2++];
						byte g = array[num2++];
						byte r = array[num2++];
						byte a = array[num2++];
						image.Pixels[num++] = new Color(r, g, b, a);
					}
				}
			}
			else
			{
				if (bitmapHeader.BitCount != 24)
				{
					throw new InvalidOperationException("Unsupported BMP pixel format.");
				}
				byte[] array2 = new byte[(3 * image.Width + 3) / 4 * 4];
				for (int k = 0; k < image.Height; k++)
				{
					if (stream.Read(array2, 0, array2.Length) != array2.Length)
					{
						throw new InvalidOperationException("BMP data truncated.");
					}
					int num3 = (bitmapHeader.Height < 0) ? (image.Width * (image.Height - k - 1)) : (image.Width * k);
					int l = 0;
					int num4 = 0;
					for (; l < image.Width; l++)
					{
						byte b2 = array2[num4++];
						byte g2 = array2[num4++];
						byte r2 = array2[num4++];
						image.Pixels[num3++] = new Color(r2, g2, b2);
					}
				}
			}
			return image;
		}

		public static void Save(Image image, Stream stream, Format format)
		{
			BitmapHeader structure = default(BitmapHeader);
			structure.Type1 = 66;
			structure.Type2 = 77;
			structure.Reserved1 = 0;
			structure.Reserved2 = 0;
			structure.OffBits = 54;
			structure.Size2 = 40;
			structure.Width = image.Width;
			structure.Height = -image.Height;
			structure.Planes = 1;
			structure.Compression = 0;
			structure.SizeImage = 0;
			structure.XPelsPerMeter = 3780;
			structure.YPelsPerMeter = 3780;
			structure.ClrUsed = 0;
			structure.ClrImportant = 0;
			if (format == Format.RGBA8)
			{
				structure.Size = 54 + 4 * image.Width * image.Height;
				structure.BitCount = 32;
			}
			else
			{
				structure.Size = 54 + (3 * image.Width + 3) / 4 * 4 * image.Height;
				structure.BitCount = 24;
			}
			byte[] array = Utilities.StructureToArray(structure);
			stream.Write(array, 0, array.Length);
			if (format == Format.RGBA8)
			{
				byte[] array2 = new byte[4 * image.Width];
				for (int i = 0; i < image.Height; i++)
				{
					int num = image.Width * i;
					int j = 0;
					int num2 = 0;
					for (; j < image.Width; j++)
					{
						Color color = image.Pixels[num++];
						array2[num2++] = color.B;
						array2[num2++] = color.G;
						array2[num2++] = color.R;
						array2[num2++] = color.A;
					}
					stream.Write(array2, 0, array2.Length);
				}
				return;
			}
			byte[] array3 = new byte[(3 * image.Width + 3) / 4 * 4];
			for (int k = 0; k < image.Height; k++)
			{
				int num3 = image.Width * k;
				int l = 0;
				int num4 = 0;
				for (; l < image.Width; l++)
				{
					Color color2 = image.Pixels[num3++];
					array3[num4++] = color2.B;
					array3[num4++] = color2.G;
					array3[num4++] = color2.R;
				}
				stream.Write(array3, 0, array3.Length);
			}
		}

		public static BitmapHeader ReadHeader(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!BitConverter.IsLittleEndian)
			{
				throw new InvalidOperationException("Unsupported system endianness.");
			}
			byte[] array = new byte[54];
			if (stream.Read(array, 0, array.Length) != array.Length)
			{
				throw new InvalidOperationException("Invalid BMP header.");
			}
			BitmapHeader result = Utilities.ArrayToStructure<BitmapHeader>(array);
			if (result.Type1 != 66 || result.Type2 != 77)
			{
				throw new InvalidOperationException("Invalid BMP header.");
			}
			if (result.Compression != 0)
			{
				throw new InvalidOperationException("Unsupported BMP compression.");
			}
			return result;
		}
	}
}
