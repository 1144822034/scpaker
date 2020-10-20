using Engine.Media;
using Engine.Serialization;
using System.Collections.Generic;
using System.IO;

namespace Engine.Content
{
	[ContentWriter("Engine.Graphics.Texture2D")]
	public class TextureContentWriter : IContentWriter
	{
		public string Texture;

		[Optional]
		public bool KeepSourceImageDataInTag;

		[Optional]
		public bool GenerateMipmaps = true;

		[Optional]
		public bool PremultiplyAlpha;

		public IEnumerable<string> GetDependencies()
		{
			yield return Texture;
		}

		public void Write(string projectDirectory, Stream stream)
		{
			Image image = Image.Load(Storage.OpenFile(Storage.CombinePaths(projectDirectory, Texture), OpenFileMode.Read), Image.DetermineFileFormat(Storage.GetExtension(Texture)));
			WriteTexture(stream, image, GenerateMipmaps, PremultiplyAlpha, KeepSourceImageDataInTag);
		}

		public static void WriteTexture(Stream stream, Image image, bool generateMipmaps, bool premultiplyAlpha, bool keepSourceImageInTag)
		{
			if (premultiplyAlpha)
			{
				image = new Image(image);
				Image.PremultiplyAlpha(image);
			}
			List<Image> list = new List<Image>();
			if (generateMipmaps)
			{
				list.AddRange(Image.GenerateMipmaps(image));
			}
			else
			{
				list.Add(image);
			}
			EngineBinaryWriter engineBinaryWriter = new EngineBinaryWriter(stream);
			engineBinaryWriter.Write(keepSourceImageInTag);
			engineBinaryWriter.Write(list[0].Width);
			engineBinaryWriter.Write(list[0].Height);
			engineBinaryWriter.Write(list.Count);
			foreach (Image item in list)
			{
				for (int i = 0; i < item.Pixels.Length; i++)
				{
					engineBinaryWriter.Write(item.Pixels[i]);
				}
			}
		}
	}
}
