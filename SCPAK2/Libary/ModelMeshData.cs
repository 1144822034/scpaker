using System.Collections.Generic;

namespace SCPAK
{
	public class ModelMeshData
	{
		public string Name;

		public int ParentBoneIndex;

		public List<ModelMeshPartData> MeshParts = new List<ModelMeshPartData>();

		public BoundingBox BoundingBox;
	}
}
