using System.IO;
using System.Text;

namespace SCPAK
{
	public static class ModelHandler
	{
		public static void WriteModel(Stream mainStream, Stream daeStream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(mainStream, Encoding.UTF8, leaveOpen: true);
			binaryWriter.Write(value: true);
			Matrix m = Matrix.CreateScale(new Vector3(1f, 1f, 1f));
			ModelData modelData = Collada.Load(daeStream);
			binaryWriter.Write(modelData.Bones.Count);
			foreach (ModelBoneData bone in modelData.Bones)
			{
				Matrix matrix = (bone.ParentBoneIndex < 0) ? (bone.Transform * m) : bone.Transform;
				binaryWriter.Write(bone.ParentBoneIndex);
				binaryWriter.Write(bone.Name);
				binaryWriter.Write(matrix.M11);
				binaryWriter.Write(matrix.M12);
				binaryWriter.Write(matrix.M13);
				binaryWriter.Write(matrix.M14);
				binaryWriter.Write(matrix.M21);
				binaryWriter.Write(matrix.M22);
				binaryWriter.Write(matrix.M23);
				binaryWriter.Write(matrix.M24);
				binaryWriter.Write(matrix.M31);
				binaryWriter.Write(matrix.M32);
				binaryWriter.Write(matrix.M33);
				binaryWriter.Write(matrix.M34);
				binaryWriter.Write(matrix.M41);
				binaryWriter.Write(matrix.M42);
				binaryWriter.Write(matrix.M43);
				binaryWriter.Write(matrix.M44);
			}
			binaryWriter.Write(modelData.Meshes.Count);
			foreach (ModelMeshData mesh in modelData.Meshes)
			{
				binaryWriter.Write(mesh.ParentBoneIndex);
				binaryWriter.Write(mesh.Name);
				binaryWriter.Write(mesh.MeshParts.Count);
				binaryWriter.Write(mesh.BoundingBox.Min.X);
				binaryWriter.Write(mesh.BoundingBox.Min.Y);
				binaryWriter.Write(mesh.BoundingBox.Min.Z);
				binaryWriter.Write(mesh.BoundingBox.Max.X);
				binaryWriter.Write(mesh.BoundingBox.Max.Y);
				binaryWriter.Write(mesh.BoundingBox.Max.Z);
				foreach (ModelMeshPartData meshPart in mesh.MeshParts)
				{
					binaryWriter.Write(meshPart.BuffersDataIndex);
					binaryWriter.Write(meshPart.StartIndex);
					binaryWriter.Write(meshPart.IndicesCount);
					binaryWriter.Write(meshPart.BoundingBox.Min.X);
					binaryWriter.Write(meshPart.BoundingBox.Min.Y);
					binaryWriter.Write(meshPart.BoundingBox.Min.Z);
					binaryWriter.Write(meshPart.BoundingBox.Max.X);
					binaryWriter.Write(meshPart.BoundingBox.Max.Y);
					binaryWriter.Write(meshPart.BoundingBox.Max.Z);
				}
			}
			binaryWriter.Write(modelData.Buffers.Count);
			foreach (ModelBuffersData buffer in modelData.Buffers)
			{
				binaryWriter.Write(buffer.VertexDeclaration.VertexElements.Length);
				VertexElement[] vertexElements = buffer.VertexDeclaration.VertexElements;
				foreach (VertexElement vertexElement in vertexElements)
				{
					binaryWriter.Write(vertexElement.Offset);
					binaryWriter.Write((int)vertexElement.Format);
					binaryWriter.Write(vertexElement.Semantic);
				}
				binaryWriter.Write(buffer.Vertices.Length);
				binaryWriter.Write(buffer.Vertices);
				binaryWriter.Write(buffer.Indices.Length);
				binaryWriter.Write(buffer.Indices);
			}
		}

		public static void RecoverModel(Stream targetFileStream, Stream modelStream)
		{
			new ModelExporter(new ModelData(modelStream)).Save(targetFileStream);
		}
	}
}
