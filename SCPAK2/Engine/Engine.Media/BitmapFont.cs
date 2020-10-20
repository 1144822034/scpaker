using Engine.Content;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Engine.Media
{
	public class BitmapFont : IDisposable
	{
		public class Glyph
		{
			public readonly char Code;

			public readonly bool IsBlank;

			public readonly Vector2 TexCoord1;

			public readonly Vector2 TexCoord2;

			public readonly Vector2 Offset;

			public readonly float Width;

			public Glyph(char code, Vector2 texCoord1, Vector2 texCoord2, Vector2 offset, float width)
			{
				Code = code;
				IsBlank = (texCoord1 == texCoord2);
				TexCoord1 = texCoord1;
				TexCoord2 = texCoord2;
				Offset = offset;
				Width = width;
			}
		}

		public static BitmapFont m_debugFont;

		internal Glyph[] m_glyphsByCode;

		internal Image m_image;

		public Texture2D Texture
		{
			get;
			set;
		}

		public float GlyphHeight
		{
			get;
			set;
		}

		public float LineHeight
		{
			get;
			set;
		}

		public Vector2 Spacing
		{
			get;
			set;
		}

		public float Scale
		{
			get;
			set;
		}

		public Glyph FallbackGlyph
		{
			get;
			set;
		}

		public char MaxGlyphCode
		{
			get;
			set;
		}

		public static BitmapFont DebugFont
		{
			get
			{
				if (m_debugFont == null)
				{
					m_debugFont = BitmapFontContentReader.ReadBitmapFont(Storage.OpenFile("app:DebugFont.dat",OpenFileMode.Read));
				}
				return m_debugFont;
			}
		}

		public BitmapFont(Texture2D texture, IEnumerable<Glyph> glyphs, char fallbackCode, float glyphHeight, Vector2 spacing, float scale)
		{
			Initialize(texture, null, glyphs, fallbackCode, glyphHeight, spacing, scale);
		}

		public void Dispose()
		{
			if (Texture != null)
			{
				Texture.Dispose();
				Texture = null;
			}
		}

		public Glyph GetGlyph(char code)
		{
			if (code >= m_glyphsByCode.Length)
			{
				return FallbackGlyph;
			}
			return m_glyphsByCode[code];
		}

		public Vector2 MeasureText(string text, Vector2 scale, Vector2 spacing)
		{
			return MeasureText(text, 0, text.Length, scale, spacing);
		}

		public Vector2 MeasureText(string text, int start, int length, Vector2 scale, Vector2 spacing)
		{
			scale *= Scale;
			spacing += Spacing;
			Vector2 vector = new Vector2(0f, (GlyphHeight + spacing.Y) * scale.Y);
			Vector2 result = vector;
			for (int i = start; i < start + length; i++)
			{
				char c = text[i];
				switch (c)
				{
				case '\n':
					vector.X = 0f;
					vector.Y += (GlyphHeight + spacing.Y) * scale.Y;
					if (vector.Y > result.Y)
					{
						result.Y = vector.Y;
					}
					break;
				default:
				{
					Glyph glyph = GetGlyph(c);
					vector.X += (glyph.Width + spacing.X) * scale.X;
					if (vector.X > result.X)
					{
						result.X = vector.X;
					}
					break;
				}
				case '\r':
					break;
				}
			}
			return result;
		}

		public int FitText(float width, string text, float scale, float spacing)
		{
			return FitText(width, text, 0, text.Length, scale, spacing);
		}

		public int FitText(float width, string text, int start, int length, float scale, float spacing)
		{
			scale *= Scale;
			spacing += Spacing.X;
			float num = 0f;
			for (int i = start; i < start + length; i++)
			{
				char c = text[i];
				switch (c)
				{
				case '\n':
					num = 0f;
					continue;
				case '\r':
					continue;
				}
				Glyph glyph = GetGlyph(c);
				num += (glyph.Width + spacing) * scale;
				if (num > width)
				{
					return i - start;
				}
			}
			return length;
		}

		public float CalculateCharacterPosition(string text, int characterIndex, Vector2 scale, Vector2 spacing)
		{
			characterIndex = MathUtils.Clamp(characterIndex, 0, text.Length);
			return MeasureText(text, 0, characterIndex, scale, spacing).X;
		}

		public static BitmapFont Load(Image image, char firstCode, char fallbackCode, Vector2 spacing, float scale, Vector2 offset, int mipLevelsCount = 1, bool premultiplyAlpha = true)
		{
			return InternalLoad(image, firstCode, fallbackCode, spacing, scale, offset, mipLevelsCount, premultiplyAlpha, createTexture: true);
		}

		public static BitmapFont Load(Stream stream, char firstCode, char fallbackCode, Vector2 spacing, float scale, Vector2 offset, int mipLevelsCount = 1, bool premultiplyAlpha = true)
		{
			return Load(Image.Load(stream), firstCode, fallbackCode, spacing, scale, offset, mipLevelsCount, premultiplyAlpha);
		}

		public static BitmapFont Load(string fileName, char firstCode, char fallbackCode, Vector2 spacing, float scale, Vector2 offset, int mipLevelsCount = 1, bool premultiplyAlpha = true)
		{
			using (Stream stream = Storage.OpenFile(fileName, OpenFileMode.Read))
			{
				return Load(stream, firstCode, fallbackCode, spacing, scale, offset, mipLevelsCount, premultiplyAlpha);
			}
		}

		static BitmapFont()
		{
			Display.DeviceReset += delegate
			{
				if (m_debugFont != null)
				{
					BitmapFontContentReader.InitializeBitmapFont(typeof(BitmapFont).GetTypeInfo().Assembly.GetManifestResourceStream("Engine.Resources.Embedded.DebugFont.dat"), m_debugFont);
				}
			};
		}

		internal BitmapFont()
		{
		}

		internal static BitmapFont InternalLoad(Image image, char firstCode, char fallbackCode, Vector2 spacing, float scale, Vector2 offset, int mipLevelsCount, bool premultiplyAlpha, bool createTexture)
		{
			List<Rectangle> list = new List<Rectangle>(FindGlyphs(image));
			List<Rectangle> list2 = new List<Rectangle>(list.Select((Rectangle r) => CropGlyph(image, r)));
			if (list.Count == 0)
			{
				throw new InvalidOperationException("No glyphs found in BitmapFont image.");
			}
			int num = int.MaxValue;
			int num2 = int.MaxValue;
			int num3 = int.MaxValue;
			int num4 = int.MaxValue;
			for (int i = 0; i < list2.Count; i++)
			{
				if (list2[i].Width > 0 && list2[i].Height > 0)
				{
					num = Math.Min(num, list2[i].Left - list[i].Left);
					num2 = Math.Min(num2, list2[i].Top - list[i].Top);
					num3 = Math.Min(num3, list[i].Right - list2[i].Right);
					num4 = Math.Min(num4, list[i].Bottom - list2[i].Bottom);
				}
			}
			int num5 = firstCode;
			float num6 = 0f;
			List<Glyph> list3 = new List<Glyph>();
			for (int j = 0; j < list2.Count; j++)
			{
				Vector2 texCoord;
				Vector2 texCoord2;
				Vector2 offset2;
				if (list2[j].Width > 0 && list2[j].Height > 0)
				{
					texCoord = new Vector2(((float)list2[j].Left - 0.5f) / (float)image.Width, ((float)list2[j].Top - 0.5f) / (float)image.Height);
					texCoord2 = new Vector2(((float)list2[j].Right + 0.5f) / (float)image.Width, ((float)list2[j].Bottom + 0.5f) / (float)image.Height);
					offset2 = new Vector2((float)(list2[j].Left - list[j].Left - num) - 0.5f, (float)(list2[j].Top - list[j].Top - num2) - 0.5f);
				}
				else
				{
					texCoord = Vector2.Zero;
					texCoord2 = Vector2.Zero;
					offset2 = Vector2.Zero;
				}
				offset2 += offset;
				float width = list[j].Width - num - num3;
				num6 = MathUtils.Max(num6, list[j].Height - num2 - num4);
				list3.Add(new Glyph((char)num5, texCoord, texCoord2, offset2, width));
				num5++;
			}
			Image image2 = new Image(image.Width, image.Height);
			for (int k = 0; k < image.Pixels.Length; k++)
			{
				image2.Pixels[k] = ((image.Pixels[k] == Color.Magenta) ? Color.Transparent : image.Pixels[k]);
			}
			if (premultiplyAlpha)
			{
				Image.PremultiplyAlpha(image2);
			}
			Texture2D texture = createTexture ? Texture2D.Load(image2, mipLevelsCount) : null;
			Image image3 = createTexture ? null : image2;
			BitmapFont bitmapFont = new BitmapFont();
			bitmapFont.Initialize(texture, image3, list3, fallbackCode, num6, spacing, scale);
			return bitmapFont;
		}

		internal void Initialize(Texture2D texture, Image image, IEnumerable<Glyph> glyphs, char fallbackCode, float glyphHeight, Vector2 spacing, float scale)
		{
			Dispose();
			Texture = texture;
			m_image = image;
			GlyphHeight = glyphHeight;
			LineHeight = glyphHeight + spacing.Y;
			Spacing = spacing;
			Scale = scale;
			FallbackGlyph = glyphs.First((Glyph g) => g.Code == fallbackCode);
			MaxGlyphCode = glyphs.Max((Glyph g) => g.Code);
			m_glyphsByCode = new Glyph[MaxGlyphCode + 1];
			for (int i = 0; i < m_glyphsByCode.Length; i++)
			{
				m_glyphsByCode[i] = FallbackGlyph;
			}
			foreach (Glyph glyph in glyphs)
			{
				m_glyphsByCode[glyph.Code] = glyph;
			}
		}

		public static IEnumerable<Rectangle> FindGlyphs(Image image)
		{
			int y = 1;
			while (y < image.Height)
			{
				int num;
				for (int x = 1; x < image.Width; x = num)
				{
					if (image.GetPixel(x, y) != Color.Magenta && image.GetPixel(x - 1, y) == Color.Magenta && image.GetPixel(x, y - 1) == Color.Magenta)
					{
						int i = 1;
						int j = 1;
						for (; x + i < image.Width && image.GetPixel(x + i, y) != Color.Magenta; i++)
						{
						}
						for (; y + j < image.Height && image.GetPixel(x, y + j) != Color.Magenta; j++)
						{
						}
						yield return new Rectangle(x, y, i, j);
					}
					num = x + 1;
				}
				num = y + 1;
				y = num;
			}
		}

		public static Rectangle CropGlyph(Image image, Rectangle rectangle)
		{
			int num = int.MaxValue;
			int num2 = int.MaxValue;
			int num3 = int.MinValue;
			int num4 = int.MinValue;
			for (int i = rectangle.Left; i < rectangle.Left + rectangle.Width; i++)
			{
				for (int j = rectangle.Top; j < rectangle.Top + rectangle.Height; j++)
				{
					if (image.GetPixel(i, j).A != 0)
					{
						num = MathUtils.Min(num, i);
						num2 = MathUtils.Min(num2, j);
						num3 = MathUtils.Max(num3, i);
						num4 = MathUtils.Max(num4, j);
					}
				}
			}
			if (num == int.MaxValue)
			{
				return new Rectangle(rectangle.Left, rectangle.Top, 0, 0);
			}
			return new Rectangle(num, num2, num3 - num + 1, num4 - num2 + 1);
		}
	}
}
