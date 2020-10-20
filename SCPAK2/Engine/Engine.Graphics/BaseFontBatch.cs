using Engine.Media;

namespace Engine.Graphics
{
	public abstract class BaseFontBatch : BaseBatch
	{
		internal static UnlitShader m_shader = new UnlitShader(useVertexColor: true, useTexture: true, useAlphaThreshold: false);

		public readonly DynamicArray<VertexPositionColorTexture> TriangleVertices = new DynamicArray<VertexPositionColorTexture>();

		public readonly DynamicArray<ushort> TriangleIndices = new DynamicArray<ushort>();

		public BitmapFont Font
		{
			get;
			internal set;
		}

		public SamplerState SamplerState
		{
			get;
			internal set;
		}

		internal BaseFontBatch()
		{
		}

		public override bool IsEmpty()
		{
			return TriangleIndices.Count == 0;
		}

		public override void Clear()
		{
			TriangleVertices.Clear();
			TriangleIndices.Clear();
		}

		public override void Flush(Matrix matrix, bool clearAfterFlush = true)
		{
			Display.DepthStencilState = base.DepthStencilState;
			Display.RasterizerState = base.RasterizerState;
			Display.BlendState = base.BlendState;
			FlushWithCurrentState(Font, SamplerState, matrix, clearAfterFlush);
		}

		public void FlushWithCurrentState(BitmapFont font, SamplerState samplerState, Matrix matrix, bool clearAfterFlush = true)
		{
			m_shader.Texture = font.Texture;
			m_shader.SamplerState = samplerState;
			m_shader.Transforms.World[0] = matrix;
			FlushWithCurrentStateAndShader(m_shader, clearAfterFlush);
		}

		public void FlushWithCurrentStateAndShader(Shader shader, bool clearAfterFlush = true)
		{
			if (TriangleIndices.Count > 0)
			{
				int num = 0;
				int num2 = TriangleIndices.Count;
				while (num2 > 0)
				{
					int num3 = MathUtils.Min(num2, 196605);
					Display.DrawUserIndexed(PrimitiveType.TriangleList, shader, VertexPositionColorTexture.VertexDeclaration, TriangleVertices.Array, 0, TriangleVertices.Count, TriangleIndices.Array, num, num3);
					num += num3;
					num2 -= num3;
				}
			}
			if (clearAfterFlush)
			{
				Clear();
			}
		}

		public Vector2 CalculateTextOffset(string text, TextAnchor anchor, Vector2 scale, Vector2 spacing)
		{
			Vector2 zero = Vector2.Zero;
			if (anchor != 0)
			{
				Vector2 vector = Font.MeasureText(text, scale, spacing);
				if ((anchor & TextAnchor.HorizontalCenter) != 0)
				{
					zero.X = (0f - vector.X) / 2f;
				}
				if ((anchor & TextAnchor.Right) != 0)
				{
					zero.X = 0f - vector.X;
				}
				if ((anchor & TextAnchor.VerticalCenter) != 0)
				{
					zero.Y = (0f - vector.Y) / 2f;
				}
				else if ((anchor & TextAnchor.Bottom) != 0)
				{
					zero.Y = 0f - vector.Y;
				}
			}
			return zero;
		}
	}
}
