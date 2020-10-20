using Engine.Graphics;
using Engine.Media;
using Engine.Serialization;
using System.Collections.Generic;
using System.IO;

namespace Engine.Content
{
	[ContentWriter("Engine.Media.ModelData")]
	public class ModelDataContentWriter : IContentWriter
	{
		public string ModelData;

		[Optional]
		public Matrix Transform = Matrix.Identity;

		public IEnumerable<string> GetDependencies()
		{
			yield return ModelData;
		}

		public void Write(string projectDirectory, Stream stream)
		{
			ModelData modelData = Engine.Media.ModelData.Load(Storage.CombinePaths(projectDirectory, ModelData));
			WriteModelData(stream, modelData, Transform);
		}

		public static void WriteModelData(Stream stream, ModelData modelData, Matrix transform)
		{
			EngineBinaryWriter engineBinaryWriter = new EngineBinaryWriter(stream);
			engineBinaryWriter.Write(modelData.Bones.Count);
			foreach (ModelBoneData bone in modelData.Bones)
			{
				engineBinaryWriter.Write(bone.ParentBoneIndex);
				engineBinaryWriter.Write(bone.Name);
				engineBinaryWriter.Write((bone.ParentBoneIndex < 0) ? (bone.Transform * transform) : bone.Transform);
			}
			engineBinaryWriter.Write(modelData.Meshes.Count);
			foreach (ModelMeshData mesh in modelData.Meshes)
			{
				engineBinaryWriter.Write(mesh.ParentBoneIndex);
				engineBinaryWriter.Write(mesh.Name);
				engineBinaryWriter.Write(mesh.MeshParts.Count);
				engineBinaryWriter.Write(mesh.BoundingBox);
				foreach (ModelMeshPartData meshPart in mesh.MeshParts)
				{
					engineBinaryWriter.Write(meshPart.BuffersDataIndex);
					engineBinaryWriter.Write(meshPart.StartIndex);
					engineBinaryWriter.Write(meshPart.IndicesCount);
					engineBinaryWriter.Write(meshPart.BoundingBox);
				}
			}
			engineBinaryWriter.Write(modelData.Buffers.Count);
			foreach (ModelBuffersData buffer in modelData.Buffers)
			{
				engineBinaryWriter.Write(buffer.VertexDeclaration.VertexElements.Count);
				foreach (VertexElement vertexElement in buffer.VertexDeclaration.VertexElements)
				{
					engineBinaryWriter.Write(vertexElement.Offset);
					engineBinaryWriter.Write((int)vertexElement.Format);
					engineBinaryWriter.Write(vertexElement.Semantic);
				}
				engineBinaryWriter.Write(buffer.Vertices.Length);
				engineBinaryWriter.Write(buffer.Vertices);
				engineBinaryWriter.Write(buffer.Indices.Length);
				engineBinaryWriter.Write(buffer.Indices);
			}
		}
	}
}
