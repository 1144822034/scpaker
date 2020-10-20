using System;
using System.Collections.Generic;

namespace Engine.Graphics
{
	public class ModelMesh : IDisposable
	{
		internal List<ModelMeshPart> m_meshParts = new List<ModelMeshPart>();

		internal BoundingBox m_boundingBox;

		public string Name
		{
			get;
			internal set;
		}

		public ModelBone ParentBone
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

		public ReadOnlyList<ModelMeshPart> MeshParts => new ReadOnlyList<ModelMeshPart>(m_meshParts);

		internal ModelMesh()
		{
		}

		public void Dispose()
		{
			Utilities.DisposeCollection(m_meshParts);
		}

		public ModelMeshPart NewMeshPart(VertexBuffer vertexBuffer, IndexBuffer indexBuffer, int startIndex, int indicesCount, BoundingBox boundingBox)
		{
			if (vertexBuffer == null)
			{
				throw new ArgumentNullException("vertexBuffer");
			}
			if (indexBuffer == null)
			{
				throw new ArgumentNullException("indexBuffer");
			}
			if (startIndex < 0 || indicesCount < 0 || startIndex + indicesCount > indexBuffer.IndicesCount)
			{
				throw new InvalidOperationException("Specified range is outside of index buffer.");
			}
			ModelMeshPart modelMeshPart = new ModelMeshPart();
			m_meshParts.Add(modelMeshPart);
			modelMeshPart.VertexBuffer = vertexBuffer;
			modelMeshPart.IndexBuffer = indexBuffer;
			modelMeshPart.StartIndex = startIndex;
			modelMeshPart.IndicesCount = indicesCount;
			modelMeshPart.BoundingBox = boundingBox;
			return modelMeshPart;
		}
	}
}
