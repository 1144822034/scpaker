namespace Engine.Graphics
{
	public sealed class TexturedBatch2D : BaseTexturedBatch
	{
		public void QueueTriangle(Vector2 p1, Vector2 p2, Vector2 p3, float depth, Vector2 texCoord1, Vector2 texCoord2, Vector2 texCoord3, Color color)
		{
			int count = TriangleVertices.Count;
			TriangleVertices.Count += 3;
			TriangleVertices.Array[count] = new VertexPositionColorTexture(new Vector3(p1.X, p1.Y, depth), color, texCoord1);
			TriangleVertices.Array[count + 1] = new VertexPositionColorTexture(new Vector3(p2.X, p2.Y, depth), color, texCoord2);
			TriangleVertices.Array[count + 2] = new VertexPositionColorTexture(new Vector3(p3.X, p3.Y, depth), color, texCoord3);
			int count2 = TriangleIndices.Count;
			TriangleIndices.Count += 3;
			TriangleIndices.Array[count2] = (ushort)count;
			TriangleIndices.Array[count2 + 1] = (ushort)(count + 1);
			TriangleIndices.Array[count2 + 2] = (ushort)(count + 2);
		}

		public void QueueTriangle(Vector2 p1, Vector2 p2, Vector2 p3, float depth, Vector2 texCoord1, Vector2 texCoord2, Vector2 texCoord3, Color color1, Color color2, Color color3)
		{
			int count = TriangleVertices.Count;
			TriangleVertices.Count += 3;
			TriangleVertices.Array[count] = new VertexPositionColorTexture(new Vector3(p1.X, p1.Y, depth), color1, texCoord1);
			TriangleVertices.Array[count + 1] = new VertexPositionColorTexture(new Vector3(p2.X, p2.Y, depth), color2, texCoord2);
			TriangleVertices.Array[count + 2] = new VertexPositionColorTexture(new Vector3(p3.X, p3.Y, depth), color3, texCoord3);
			int count2 = TriangleIndices.Count;
			TriangleIndices.Count += 3;
			TriangleIndices.Array[count2] = (ushort)count;
			TriangleIndices.Array[count2 + 1] = (ushort)(count + 1);
			TriangleIndices.Array[count2 + 2] = (ushort)(count + 2);
		}

		public void QueueQuad(Vector2 corner1, Vector2 corner2, float depth, Vector2 texCoord1, Vector2 texCoord2, Color color)
		{
			int count = TriangleVertices.Count;
			TriangleVertices.Count += 4;
			TriangleVertices.Array[count] = new VertexPositionColorTexture(new Vector3(corner1.X, corner1.Y, depth), color, new Vector2(texCoord1.X, texCoord1.Y));
			TriangleVertices.Array[count + 1] = new VertexPositionColorTexture(new Vector3(corner1.X, corner2.Y, depth), color, new Vector2(texCoord1.X, texCoord2.Y));
			TriangleVertices.Array[count + 2] = new VertexPositionColorTexture(new Vector3(corner2.X, corner2.Y, depth), color, new Vector2(texCoord2.X, texCoord2.Y));
			TriangleVertices.Array[count + 3] = new VertexPositionColorTexture(new Vector3(corner2.X, corner1.Y, depth), color, new Vector2(texCoord2.X, texCoord1.Y));
			int count2 = TriangleIndices.Count;
			TriangleIndices.Count += 6;
			TriangleIndices.Array[count2] = (ushort)count;
			TriangleIndices.Array[count2 + 1] = (ushort)(count + 1);
			TriangleIndices.Array[count2 + 2] = (ushort)(count + 2);
			TriangleIndices.Array[count2 + 3] = (ushort)(count + 2);
			TriangleIndices.Array[count2 + 4] = (ushort)(count + 3);
			TriangleIndices.Array[count2 + 5] = (ushort)count;
		}

		public void QueueQuad(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float depth, Vector2 texCoord1, Vector2 texCoord2, Vector2 texCoord3, Vector2 texCoord4, Color color)
		{
			int count = TriangleVertices.Count;
			TriangleVertices.Count += 4;
			TriangleVertices.Array[count] = new VertexPositionColorTexture(new Vector3(p1.X, p1.Y, depth), color, texCoord1);
			TriangleVertices.Array[count + 1] = new VertexPositionColorTexture(new Vector3(p2.X, p2.Y, depth), color, texCoord2);
			TriangleVertices.Array[count + 2] = new VertexPositionColorTexture(new Vector3(p3.X, p3.Y, depth), color, texCoord3);
			TriangleVertices.Array[count + 3] = new VertexPositionColorTexture(new Vector3(p4.X, p4.Y, depth), color, texCoord4);
			int count2 = TriangleIndices.Count;
			TriangleIndices.Count += 6;
			TriangleIndices.Array[count2] = (ushort)count;
			TriangleIndices.Array[count2 + 1] = (ushort)(count + 1);
			TriangleIndices.Array[count2 + 2] = (ushort)(count + 2);
			TriangleIndices.Array[count2 + 3] = (ushort)(count + 2);
			TriangleIndices.Array[count2 + 4] = (ushort)(count + 3);
			TriangleIndices.Array[count2 + 5] = (ushort)count;
		}

		public void QueueQuad(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float depth, Vector2 texCoord1, Vector2 texCoord2, Vector2 texCoord3, Vector2 texCoord4, Color color1, Color color2, Color color3, Color color4)
		{
			int count = TriangleVertices.Count;
			TriangleVertices.Count += 4;
			TriangleVertices.Array[count] = new VertexPositionColorTexture(new Vector3(p1.X, p1.Y, depth), color1, texCoord1);
			TriangleVertices.Array[count + 1] = new VertexPositionColorTexture(new Vector3(p2.X, p2.Y, depth), color2, texCoord2);
			TriangleVertices.Array[count + 2] = new VertexPositionColorTexture(new Vector3(p3.X, p3.Y, depth), color3, texCoord3);
			TriangleVertices.Array[count + 3] = new VertexPositionColorTexture(new Vector3(p4.X, p4.Y, depth), color4, texCoord4);
			int count2 = TriangleIndices.Count;
			TriangleIndices.Count += 6;
			TriangleIndices.Array[count2] = (ushort)count;
			TriangleIndices.Array[count2 + 1] = (ushort)(count + 1);
			TriangleIndices.Array[count2 + 2] = (ushort)(count + 2);
			TriangleIndices.Array[count2 + 3] = (ushort)(count + 2);
			TriangleIndices.Array[count2 + 4] = (ushort)(count + 3);
			TriangleIndices.Array[count2 + 5] = (ushort)count;
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
