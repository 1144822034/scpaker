using Engine.Graphics;
using Engine.Media;
using Engine.Serialization;
using System.IO;

namespace Engine.Content
{
	[ContentReader("Engine.Graphics.Texture2D")]
	public class TextureContentReader : IContentReader
	{
		public object Read(ContentStream stream, object existingObject)
		{
			if (existingObject == null)
			{
				return ReadTexture(stream);
			}
			Texture2D texture2D = (Texture2D)existingObject;
			stream.Position += 12L;
			LoadTextureData(stream, texture2D, keepSourceImageInTag: false);
			return texture2D;
		}

		public static Texture2D ReadTexture(Stream stream)
		{
			EngineBinaryReader engineBinaryReader = new EngineBinaryReader(stream);
			bool keepSourceImageInTag = engineBinaryReader.ReadBoolean();
			int width = engineBinaryReader.ReadInt32();
			int height = engineBinaryReader.ReadInt32();
			int mipLevelsCount = engineBinaryReader.ReadInt32();
			Texture2D texture2D = new Texture2D(width, height, mipLevelsCount, ColorFormat.Rgba8888);
			LoadTextureData(stream, texture2D, keepSourceImageInTag);
			return texture2D;
		}

		public static void LoadTextureData(Stream stream, Texture2D texture, bool keepSourceImageInTag)
		{
			EngineBinaryReader engineBinaryReader = new EngineBinaryReader(stream);
			int num = texture.Width;
			int num2 = texture.Height;
			byte[] array = new byte[4 * num * num2];
			for (int i = 0; i < texture.MipLevelsCount; i++)
			{
				engineBinaryReader.Read(array, 0, 4 * num * num2);
				if (keepSourceImageInTag && i == 0)
				{
					Image image = new Image(num, num2);
					for (int j = 0; j < image.Pixels.Length; j++)
					{
						byte r = array[4 * j];
						byte g = array[4 * j + 1];
						byte b = array[4 * j + 2];
						byte a = array[4 * j + 3];
						image.Pixels[j] = new Color(r, g, b, a);
					}
					texture.Tag = image;
				}
				texture.SetData(i, array);
				num = MathUtils.Max(num / 2, 1);
				num2 = MathUtils.Max(num2 / 2, 1);
			}
		}
	}
}
