namespace Engine.Graphics
{
	public sealed class TexturedBatch3D : BaseTexturedBatch
	{
		public void QueueTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector2 texCoord1, Vector2 texCoord2, Vector2 texCoord3, Color color)
		{
			int count = TriangleVertices.Count;
			TriangleVertices.Count += 3;
			TriangleVertices.Array[count] = new VertexPositionColorTexture(p1, color, texCoord1);
			TriangleVertices.Array[count + 1] = new VertexPositionColorTexture(p2, color, texCoord2);
			TriangleVertices.Array[count + 2] = new VertexPositionColorTexture(p3, color, texCoord3);
			int count2 = TriangleIndices.Count;
			TriangleIndices.Count += 3;
			TriangleIndices.Array[count2] = (ushort)count;
			TriangleIndices.Array[count2 + 1] = (ushort)(count + 1);
			TriangleIndices.Array[count2 + 2] = (ushort)(count + 2);
		}

		public void QueueTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector2 texCoord1, Vector2 texCoord2, Vector2 texCoord3, Color color1, Color color2, Color color3)
		{
			int count = TriangleVertices.Count;
			TriangleVertices.Count += 3;
			TriangleVertices.Array[count] = new VertexPositionColorTexture(p1, color1, texCoord1);
			TriangleVertices.Array[count + 1] = new VertexPositionColorTexture(p2, color2, texCoord2);
			TriangleVertices.Array[count + 2] = new VertexPositionColorTexture(p3, color3, texCoord3);
			int count2 = TriangleIndices.Count;
			TriangleIndices.Count += 3;
			TriangleIndices.Array[count2] = (ushort)count;
			TriangleIndices.Array[count2 + 1] = (ushort)(count + 1);
			TriangleIndices.Array[count2 + 2] = (ushort)(count + 2);
		}

		public void QueueQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector2 texCoord1, Vector2 texCoord2, Vector2 texCoord3, Vector2 texCoord4, Color color)
		{
			int count = TriangleVertices.Count;
			TriangleVertices.Count += 4;
			TriangleVertices.Array[count] = new VertexPositionColorTexture(p1, color, texCoord1);
			TriangleVertices.Array[count + 1] = new VertexPositionColorTexture(p2, color, texCoord2);
			TriangleVertices.Array[count + 2] = new VertexPositionColorTexture(p3, color, texCoord3);
			TriangleVertices.Array[count + 3] = new VertexPositionColorTexture(p4, color, texCoord4);
			int count2 = TriangleIndices.Count;
			TriangleIndices.Count += 6;
			TriangleIndices.Array[count2] = (ushort)count;
			TriangleIndices.Array[count2 + 1] = (ushort)(count + 1);
			TriangleIndices.Array[count2 + 2] = (ushort)(count + 2);
			TriangleIndices.Array[count2 + 3] = (ushort)(count + 2);
			TriangleIndices.Array[count2 + 4] = (ushort)(count + 3);
			TriangleIndices.Array[count2 + 5] = (ushort)count;
		}

		public void QueueQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector2 texCoord1, Vector2 texCoord2, Vector2 texCoord3, Vector2 texCoord4, Color color1, Color color2, Color color3, Color color4)
		{
			int count = TriangleVertices.Count;
			TriangleVertices.Count += 4;
			TriangleVertices.Array[count] = new VertexPositionColorTexture(p1, color1, texCoord1);
			TriangleVertices.Array[count + 1] = new VertexPositionColorTexture(p2, color2, texCoord2);
			TriangleVertices.Array[count + 2] = new VertexPositionColorTexture(p3, color3, texCoord3);
			TriangleVertices.Array[count + 3] = new VertexPositionColorTexture(p4, color4, texCoord4);
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
				Vector3.Transform(ref array[i].Position, ref matrix, out array[i].Position);
			}
		}
	}
}
