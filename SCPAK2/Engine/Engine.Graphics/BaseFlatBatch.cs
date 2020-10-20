namespace Engine.Graphics
{
	public abstract class BaseFlatBatch : BaseBatch
	{
		internal static UnlitShader m_shader = new UnlitShader(useVertexColor: true, useTexture: false, useAlphaThreshold: false);

		public readonly DynamicArray<VertexPositionColor> LineVertices = new DynamicArray<VertexPositionColor>();

		public readonly DynamicArray<ushort> LineIndices = new DynamicArray<ushort>();

		public readonly DynamicArray<VertexPositionColor> TriangleVertices = new DynamicArray<VertexPositionColor>();

		public readonly DynamicArray<ushort> TriangleIndices = new DynamicArray<ushort>();

		internal BaseFlatBatch()
		{
		}

		public override bool IsEmpty()
		{
			if (LineIndices.Count == 0)
			{
				return TriangleIndices.Count == 0;
			}
			return false;
		}

		public override void Clear()
		{
			LineVertices.Clear();
			LineIndices.Clear();
			TriangleVertices.Clear();
			TriangleIndices.Clear();
		}

		public override void Flush(Matrix matrix, bool clearAfterFlush = true)
		{
			Display.DepthStencilState = base.DepthStencilState;
			Display.RasterizerState = base.RasterizerState;
			Display.BlendState = base.BlendState;
			FlushWithCurrentState(matrix, clearAfterFlush);
		}

		public void FlushWithCurrentState(Matrix matrix, bool clearAfterFlush = true)
		{
			if (!IsEmpty())
			{
				m_shader.Transforms.World[0] = matrix;
				FlushWithCurrentStateAndShader(m_shader, clearAfterFlush);
			}
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
					Display.DrawUserIndexed(PrimitiveType.TriangleList, shader, VertexPositionColor.VertexDeclaration, TriangleVertices.Array, 0, TriangleVertices.Count, TriangleIndices.Array, num, num3);
					num += num3;
					num2 -= num3;
				}
			}
			if (LineIndices.Count > 0)
			{
				int num4 = 0;
				int num5 = LineIndices.Count;
				while (num5 > 0)
				{
					int num6 = MathUtils.Min(num5, 131070);
					Display.DrawUserIndexed(PrimitiveType.LineList, shader, VertexPositionColor.VertexDeclaration, LineVertices.Array, 0, LineVertices.Count, LineIndices.Array, num4, num6);
					num4 += num6;
					num5 -= num6;
				}
			}
			if (clearAfterFlush)
			{
				Clear();
			}
		}
	}
}
