using System;

namespace Engine.Graphics
{
	public class ModelMeshPart : IDisposable
	{
		internal BoundingBox m_boundingBox;

		public VertexBuffer VertexBuffer
		{
			get;
			internal set;
		}

		public IndexBuffer IndexBuffer
		{
			get;
			internal set;
		}

		public int StartIndex
		{
			get;
			internal set;
		}

		public int IndicesCount
		{
			get;
			internal set;
		}

		public BoundingBox BoundingBox
		{
			get
			{
				return m_boundingBox;
			}
			internal set
			{
				m_boundingBox = value;
			}
		}

		internal ModelMeshPart()
		{
		}

		public void Dispose()
		{
			if (VertexBuffer != null)
			{
				VertexBuffer.Dispose();
				VertexBuffer = null;
			}
			if (IndexBuffer != null)
			{
				IndexBuffer.Dispose();
				IndexBuffer = null;
			}
		}
	}
}
