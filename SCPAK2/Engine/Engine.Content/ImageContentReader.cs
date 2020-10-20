using Engine.Media;
using Engine.Serialization;
using System;
using System.IO;

namespace Engine.Content
{
	[ContentReader("Engine.Media.Image")]
	public class ImageContentReader : IContentReader
	{
		public object Read(ContentStream stream, object existingObject)
		{
			if (existingObject == null)
			{
				return ReadImage(stream);
			}
			throw new NotSupportedException();
		}

		public static Image ReadImage(Stream stream)
		{
			EngineBinaryReader engineBinaryReader = new EngineBinaryReader(stream);
			int width = engineBinaryReader.ReadInt32();
			int height = engineBinaryReader.ReadInt32();
			Image image = new Image(width, height);
			for (int i = 0; i < image.Pixels.Length; i++)
			{
				image.Pixels[i] = engineBinaryReader.ReadColor();
			}
			return image;
		}
	}
}
