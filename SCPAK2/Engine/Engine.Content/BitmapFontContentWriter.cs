using Engine.Media;
using Engine.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine.Content
{
	[ContentWriter("Engine.Media.BitmapFont")]
	public class BitmapFontContentWriter : IContentWriter
	{
		public class BitmapFontData
		{
			public List<BitmapFont.Glyph> Glyphs = new List<BitmapFont.Glyph>();
		}

		public string Font;

		[Optional]
		public int FirstCode = 32;

		[Optional]
		public int FallbackCode = 95;

		[Optional]
		public Vector2 Spacing = Vector2.Zero;

		[Optional]
		public float Scale = 1f;

		[Optional]
		public Vector2 Offset = Vector2.Zero;

		[Optional]
		public bool GenerateMipmaps;

		[Optional]
		public bool PremultiplyAlpha = true;

		public IEnumerable<string> GetDependencies()
		{
			yield return Font;
		}

		public void Write(string projectDirectory, Stream stream)
		{
			Image image = Image.Load(Storage.OpenFile(Storage.CombinePaths(projectDirectory, Font), OpenFileMode.Read), Image.DetermineFileFormat(Storage.GetExtension(Font)));
			WriteBitmapFont(stream, image, (char)FirstCode, (char)FallbackCode, Spacing, Scale, Offset, GenerateMipmaps, PremultiplyAlpha);
		}

		public static void WriteBitmapFont(Stream stream, Image image, char firstCode, char fallbackCode, Vector2 spacing, float scale, Vector2 offset, bool generateMipmaps, bool premultiplyAlpha)
		{
			EngineBinaryWriter engineBinaryWriter = new EngineBinaryWriter(stream);
			BitmapFont bitmapFont = BitmapFont.InternalLoad(image, firstCode, fallbackCode, spacing, scale, offset, 1, premultiplyAlpha, createTexture: false);
			engineBinaryWriter.Write(bitmapFont.m_glyphsByCode.Count((BitmapFont.Glyph g) => g != null));
			for (int i = 0; i < bitmapFont.m_glyphsByCode.Length; i++)
			{
				BitmapFont.Glyph glyph = bitmapFont.m_glyphsByCode[i];
				if (glyph != null)
				{
					engineBinaryWriter.Write(glyph.Code);
					engineBinaryWriter.Write(glyph.TexCoord1);
					engineBinaryWriter.Write(glyph.TexCoord2);
					engineBinaryWriter.Write(glyph.Offset);
					engineBinaryWriter.Write(glyph.Width);
				}
			}
			engineBinaryWriter.Write(bitmapFont.GlyphHeight);
			engineBinaryWriter.Write(bitmapFont.Spacing);
			engineBinaryWriter.Write(bitmapFont.Scale);
			engineBinaryWriter.Write(bitmapFont.FallbackGlyph.Code);
			TextureContentWriter.WriteTexture(stream, bitmapFont.m_image, generateMipmaps, premultiplyAlpha: false, keepSourceImageInTag: false);
		}
	}
}
