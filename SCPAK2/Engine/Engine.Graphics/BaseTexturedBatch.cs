namespace Engine.Graphics
{
	public abstract class BaseTexturedBatch : BaseBatch
	{
		internal static UnlitShader m_shader = new UnlitShader(useVertexColor: true, useTexture: true, useAlphaThreshold: false);

		internal static UnlitShader m_shaderAlphaTest = new UnlitShader(useVertexColor: true, useTexture: true, useAlphaThreshold: true);

		public readonly DynamicArray<VertexPositionColorTexture> TriangleVertices = new DynamicArray<VertexPositionColorTexture>();

		public readonly DynamicArray<ushort> TriangleIndices = new DynamicArray<ushort>();

		public Texture2D Texture
		{
			get;
			internal set;
		}

		public bool UseAlphaTest
		{
			get;
			internal set;
		}

		public SamplerState SamplerState
		{
			get;
			internal set;
		}

		internal BaseTexturedBatch()
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
			FlushWithCurrentState(UseAlphaTest, Texture, SamplerState, matrix, clearAfterFlush);
		}

		public void FlushWithCurrentState(bool useAlphaTest, Texture2D texture, SamplerState samplerState, Matrix matrix, bool clearAfterFlush = true)
		{
			if (useAlphaTest)
			{
				m_shaderAlphaTest.Texture = texture;
				m_shaderAlphaTest.SamplerState = samplerState;
				m_shaderAlphaTest.Transforms.World[0] = matrix;
				m_shaderAlphaTest.AlphaThreshold = 0f;
				FlushWithCurrentStateAndShader(m_shaderAlphaTest, clearAfterFlush);
			}
			else
			{
				m_shader.Texture = texture;
				m_shader.SamplerState = samplerState;
				m_shader.Transforms.World[0] = matrix;
				FlushWithCurrentStateAndShader(m_shader, clearAfterFlush);
			}
		}

		public void FlushWithCurrentStateAndShader(Shader shader, bool clearAfterFlush = true)
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
			if (clearAfterFlush)
			{
				Clear();
			}
		}
	}
}
