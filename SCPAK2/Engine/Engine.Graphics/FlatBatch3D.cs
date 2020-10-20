using System.Collections.Generic;

namespace Engine.Graphics
{
	public sealed class FlatBatch3D : BaseFlatBatch
	{
		public void QueueLine(Vector3 p1, Vector3 p2, Color color)
		{
			int count = LineVertices.Count;
			LineVertices.Add(new VertexPositionColor(p1, color));
			LineVertices.Add(new VertexPositionColor(p2, color));
			LineIndices.Add((ushort)count);
			LineIndices.Add((ushort)(count + 1));
		}

		public void QueueLine(Vector3 p1, Vector3 p2, Color color1, Color color2)
		{
			int count = LineVertices.Count;
			LineVertices.Add(new VertexPositionColor(p1, color1));
			LineVertices.Add(new VertexPositionColor(p2, color2));
			LineIndices.Add((ushort)count);
			LineIndices.Add((ushort)(count + 1));
		}

		public void QueueLineStrip(IEnumerable<Vector3> points, Color color)
		{
			int count = LineVertices.Count;
			int num = 0;
			foreach (Vector3 point in points)
			{
				LineVertices.Add(new VertexPositionColor(point, color));
				num++;
			}
			for (int i = 0; i < num - 1; i++)
			{
				LineIndices.Add((ushort)(count + i));
				LineIndices.Add((ushort)(count + i + 1));
			}
		}

		public void QueueTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color)
		{
			int count = TriangleVertices.Count;
			TriangleVertices.Add(new VertexPositionColor(p1, color));
			TriangleVertices.Add(new VertexPositionColor(p2, color));
			TriangleVertices.Add(new VertexPositionColor(p3, color));
			TriangleIndices.Add((ushort)count);
			TriangleIndices.Add((ushort)(count + 1));
			TriangleIndices.Add((ushort)(count + 2));
		}

		public void QueueTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color1, Color color2, Color color3)
		{
			int count = TriangleVertices.Count;
			TriangleVertices.Add(new VertexPositionColor(p1, color1));
			TriangleVertices.Add(new VertexPositionColor(p2, color2));
			TriangleVertices.Add(new VertexPositionColor(p3, color3));
			TriangleIndices.Add((ushort)count);
			TriangleIndices.Add((ushort)(count + 1));
			TriangleIndices.Add((ushort)(count + 2));
		}

		public void QueueQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Color color)
		{
			int count = TriangleVertices.Count;
			TriangleVertices.Add(new VertexPositionColor(p1, color));
			TriangleVertices.Add(new VertexPositionColor(p2, color));
			TriangleVertices.Add(new VertexPositionColor(p3, color));
			TriangleVertices.Add(new VertexPositionColor(p4, color));
			TriangleIndices.Add((ushort)count);
			TriangleIndices.Add((ushort)(count + 1));
			TriangleIndices.Add((ushort)(count + 2));
			TriangleIndices.Add((ushort)(count + 2));
			TriangleIndices.Add((ushort)(count + 3));
			TriangleIndices.Add((ushort)count);
		}

		public void QueueBoundingBox(BoundingBox boundingBox, Color color)
		{
			QueueLine(new Vector3(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Min.Z), new Vector3(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Min.Z), color);
			QueueLine(new Vector3(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Min.Z), new Vector3(boundingBox.Max.X, boundingBox.Max.Y, boundingBox.Min.Z), color);
			QueueLine(new Vector3(boundingBox.Max.X, boundingBox.Max.Y, boundingBox.Min.Z), new Vector3(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Min.Z), color);
			QueueLine(new Vector3(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Min.Z), new Vector3(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Min.Z), color);
			QueueLine(new Vector3(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Max.Z), new Vector3(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Max.Z), color);
			QueueLine(new Vector3(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Max.Z), new Vector3(boundingBox.Max.X, boundingBox.Max.Y, boundingBox.Max.Z), color);
			QueueLine(new Vector3(boundingBox.Max.X, boundingBox.Max.Y, boundingBox.Max.Z), new Vector3(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Max.Z), color);
			QueueLine(new Vector3(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Max.Z), new Vector3(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Max.Z), color);
			QueueLine(new Vector3(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Min.Z), new Vector3(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Max.Z), color);
			QueueLine(new Vector3(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Min.Z), new Vector3(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Max.Z), color);
			QueueLine(new Vector3(boundingBox.Max.X, boundingBox.Max.Y, boundingBox.Min.Z), new Vector3(boundingBox.Max.X, boundingBox.Max.Y, boundingBox.Max.Z), color);
			QueueLine(new Vector3(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Min.Z), new Vector3(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Max.Z), color);
		}

		public void QueueBoundingFrustum(BoundingFrustum boundingFrustum, Color color)
		{
			Vector3[] array = boundingFrustum.FindCorners();
			QueueLine(array[0], array[1], color);
			QueueLine(array[1], array[2], color);
			QueueLine(array[2], array[3], color);
			QueueLine(array[3], array[0], color);
			QueueLine(array[4], array[5], color);
			QueueLine(array[5], array[6], color);
			QueueLine(array[6], array[7], color);
			QueueLine(array[7], array[4], color);
			QueueLine(array[0], array[4], color);
			QueueLine(array[1], array[5], color);
			QueueLine(array[2], array[6], color);
			QueueLine(array[3], array[7], color);
		}

		public void TransformLines(Matrix matrix, int start = 0, int end = -1)
		{
			VertexPositionColor[] array = LineVertices.Array;
			if (end < 0)
			{
				end = LineVertices.Count;
			}
			for (int i = start; i < end; i++)
			{
				Vector3.Transform(ref array[i].Position, ref matrix, out array[i].Position);
			}
		}

		public void TransformTriangles(Matrix matrix, int start = 0, int end = -1)
		{
			VertexPositionColor[] array = TriangleVertices.Array;
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
