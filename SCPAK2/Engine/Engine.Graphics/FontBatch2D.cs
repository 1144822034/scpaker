using Engine.Media;

namespace Engine.Graphics
{
	public class FontBatch2D : BaseFontBatch
	{
		public void QueueText(string text, Vector2 position, float depth, Color color, TextAnchor anchor = TextAnchor.Default)
		{
			QueueText(text, position, depth, color, anchor, Vector2.One, Vector2.Zero);
		}

		public void QueueText(string text, Vector2 position, float depth, Color color, TextAnchor anchor, Vector2 scale, Vector2 spacing, float angle = 0f)
		{
			Vector2 v;
			Vector2 v2;
			Vector2 vector3;
			if (angle != 0f)
			{
				Vector2 vector = new Vector2(MathUtils.Cos(angle), MathUtils.Sin(angle));
				v = vector;
				v2 = new Vector2(0f - vector.Y, vector.X);
				Vector2 vector2 = CalculateTextOffset(text, anchor, scale, spacing);
				Vector2 v3 = v * vector2.X + v2 * vector2.Y;
				v *= scale.X * base.Font.Scale;
				v2 *= scale.Y * base.Font.Scale;
				vector3 = position + v3;
			}
			else
			{
				v = new Vector2(scale.X * base.Font.Scale, 0f);
				v2 = new Vector2(0f, scale.Y * base.Font.Scale);
				vector3 = position + CalculateTextOffset(text, anchor, scale, spacing);
			}
			spacing += base.Font.Spacing;
			vector3 += 0.5f * (v * spacing.X + v2 * spacing.Y);
			if ((anchor & TextAnchor.DisableSnapToPixels) == 0)
			{
				vector3 = Vector2.Round(vector3);
			}
			Vector2 v4 = vector3;
			int num = 0;
			foreach (char c in text)
			{
				switch (c)
				{
				case '\n':
					num++;
					v4 = vector3 + (float)num * (base.Font.GlyphHeight + spacing.Y) * v2;
					continue;
				case '\r':
					continue;
				}
				BitmapFont.Glyph glyph = base.Font.GetGlyph(c);
				if (!glyph.IsBlank)
				{
					Vector2 v5 = v * (glyph.TexCoord2.X - glyph.TexCoord1.X) * base.Font.Texture.Width;
					Vector2 v6 = v2 * (glyph.TexCoord2.Y - glyph.TexCoord1.Y) * base.Font.Texture.Height;
					Vector2 v7 = v * glyph.Offset.X + v2 * glyph.Offset.Y;
					Vector2 v8 = v4 + v7;
					Vector2 vector4 = v8 + v5;
					Vector2 vector5 = v8 + v6;
					Vector2 vector6 = v8 + v5 + v6;
					int count = TriangleVertices.Count;
					TriangleVertices.Count += 4;
					TriangleVertices.Array[count] = new VertexPositionColorTexture(new Vector3(v8.X, v8.Y, depth), color, new Vector2(glyph.TexCoord1.X, glyph.TexCoord1.Y));
					TriangleVertices.Array[count + 1] = new VertexPositionColorTexture(new Vector3(vector4.X, vector4.Y, depth), color, new Vector2(glyph.TexCoord2.X, glyph.TexCoord1.Y));
					TriangleVertices.Array[count + 2] = new VertexPositionColorTexture(new Vector3(vector6.X, vector6.Y, depth), color, new Vector2(glyph.TexCoord2.X, glyph.TexCoord2.Y));
					TriangleVertices.Array[count + 3] = new VertexPositionColorTexture(new Vector3(vector5.X, vector5.Y, depth), color, new Vector2(glyph.TexCoord1.X, glyph.TexCoord2.Y));
					int count2 = TriangleIndices.Count;
					TriangleIndices.Count += 6;
					TriangleIndices.Array[count2] = (ushort)count;
					TriangleIndices.Array[count2 + 1] = (ushort)(count + 1);
					TriangleIndices.Array[count2 + 2] = (ushort)(count + 2);
					TriangleIndices.Array[count2 + 3] = (ushort)(count + 2);
					TriangleIndices.Array[count2 + 4] = (ushort)(count + 3);
					TriangleIndices.Array[count2 + 5] = (ushort)count;
				}
				v4 += v * (glyph.Width + spacing.X);
			}
		}

		public void TransformTriangles(Matrix matrix, int start = 0, int end = -1)
		{
			VertexPositionColorTexture[] array = TriangleVertices.Array;
			if (end < 0)
			{
				end = TriangleVertices.Count;
			}
			for (int i = start; i < end; i++)
			{
				Vector2 v = array[i].Position.XY;
				Vector2.Transform(ref v, ref matrix, out v);
				array[i].Position.X = v.X;
				array[i].Position.Y = v.Y;
			}
		}

		public void Flush(bool clearAfterFlush = true)
		{
			Flush(PrimitivesRenderer2D.ViewportMatrix(), clearAfterFlush);
		}
	}
}
