using System.Collections.Generic;

namespace Engine.Media
{
	public class ModelMeshData
	{
		public string Name;

		public int ParentBoneIndex;

		public List<ModelMeshPartData> MeshParts = new List<ModelMeshPartData>();

		public BoundingBox BoundingBox;
	}
}
