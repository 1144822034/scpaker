using Engine.Media;
using Engine.Serialization;
using System.Collections.Generic;
using System.IO;

namespace Engine.Content
{
	[ContentWriter("Engine.Media.Image")]
	public class ImageContentWriter : IContentWriter
	{
		public string Image;

		public IEnumerable<string> GetDependencies()
		{
			yield return Image;
		}

		public void Write(string projectDirectory, Stream stream)
		{
			Image image = Engine.Media.Image.Load(Storage.OpenFile(Storage.CombinePaths(projectDirectory, Image), OpenFileMode.Read), Engine.Media.Image.DetermineFileFormat(Storage.GetExtension(Image)));
			WriteImage(stream, image);
		}

		public static void WriteImage(Stream stream, Image image)
		{
			EngineBinaryWriter engineBinaryWriter = new EngineBinaryWriter(stream);
			engineBinaryWriter.Write(image.Width);
			engineBinaryWriter.Write(image.Height);
			for (int i = 0; i < image.Pixels.Length; i++)
			{
				engineBinaryWriter.Write(image.Pixels[i]);
			}
		}
	}
}
