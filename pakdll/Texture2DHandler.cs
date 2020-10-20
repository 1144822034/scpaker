using Engine;
using Engine.Media;
using System.IO;
using System.Text;

namespace SCPAK
{
	public static class Texture2DHandler
	{
		public static void WriteTexture2D(Stream mainStream, Stream BitmapStream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(mainStream, Encoding.UTF8, leaveOpen: true);
			Image bitmap = Png.Load(BitmapStream);
			binaryWriter.Write(value: false);
			binaryWriter.Write(bitmap.Width);
			binaryWriter.Write(bitmap.Height);
			binaryWriter.Write(1);
			for (int i = 0; i < bitmap.Height; i++)
			{
				for (int j = 0; j < bitmap.Width; j++)
				{
					Color pixel = bitmap.GetPixel(j, i);
					binaryWriter.Write((uint)((pixel.A << 24) | (pixel.B << 16) | (pixel.G << 8) | pixel.R));
				}
			}
		}

		public static void RecoverTexture2D(Stream targetFileStream, Stream texture2DStream)
		{
			BinaryReader binaryReader = new BinaryReader(texture2DStream, Encoding.UTF8, leaveOpen: true);
			binaryReader.ReadByte();
			int num = binaryReader.ReadInt32();
			int num2 = binaryReader.ReadInt32();
			binaryReader.ReadInt32();
			Image bitmap = new Image(num, num2);
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					uint num3 = binaryReader.ReadUInt32();
					bitmap.SetPixel(j, i, new Color((byte)num3, (byte)(num3>>8), (byte)(num3>>16), (byte)(num3>>24)));
				}
			}
			Image.Save(bitmap,targetFileStream, ImageFileFormat.Png,true);
		}
	}
}
