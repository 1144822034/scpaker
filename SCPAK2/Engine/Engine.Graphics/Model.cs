using Engine.Media;
using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Graphics
{
	public class Model : IDisposable
	{
		internal ModelBone m_rootBone;

		internal List<ModelBone> m_bones = new List<ModelBone>();

		internal List<ModelMesh> m_meshes = new List<ModelMesh>();

		public ModelBone RootBone => m_rootBone;

		public ReadOnlyList<ModelBone> Bones => new ReadOnlyList<ModelBone>(m_bones);

		public ReadOnlyList<ModelMesh> Meshes => new ReadOnlyList<ModelMesh>(m_meshes);

		public ModelBone FindBone(string name, bool throwIfNotFound = true)
		{
			foreach (ModelBone bone in m_bones)
			{
				if (bone.Name == name)
				{
					return bone;
				}
			}
			if (throwIfNotFound)
			{
				throw new InvalidOperationException("ModelBone not found.");
			}
			return null;
		}

		public ModelMesh FindMesh(string name, bool throwIfNotFound = true)
		{
			foreach (ModelMesh mesh in m_meshes)
			{
				if (mesh.Name == name)
				{
					return mesh;
				}
			}
			if (throwIfNotFound)
			{
				throw new InvalidOperationException("ModelMesh not found.");
			}
			return null;
		}

		public ModelBone NewBone(string name, Matrix transform, ModelBone parentBone)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (parentBone == null && m_bones.Count > 0)
			{
				throw new InvalidOperationException("There can be only one root bone.");
			}
			if (parentBone != null && parentBone.Model != this)
			{
				throw new InvalidOperationException("Parent bone must belong to the same model.");
			}
			ModelBone modelBone = new ModelBone();
			modelBone.Model = this;
			modelBone.Index = m_bones.Count;
			m_bones.Add(modelBone);
			modelBone.Name = name;
			modelBone.Transform = transform;
			if (parentBone != null)
			{
				modelBone.ParentBone = parentBone;
				parentBone.m_childBones.Add(modelBone);
			}
			else
			{
				m_rootBone = modelBone;
			}
			return modelBone;
		}

		public ModelMesh NewMesh(string name, ModelBone parentBone, BoundingBox boundingBox)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (parentBone == null)
			{
				throw new ArgumentNullException("parentBone");
			}
			if (parentBone.Model != this)
			{
				throw new InvalidOperationException("Parent bone must belong to the same model.");
			}
			return new ModelMesh
			{
				Name = name,
				ParentBone = parentBone,
				BoundingBox = boundingBox
			};
		}

		public void CopyAbsoluteBoneTransformsTo(Matrix[] absoluteTransforms)
		{
			if (absoluteTransforms == null)
			{
				throw new ArgumentNullException("transforms");
			}
			if (absoluteTransforms.Length < m_bones.Count)
			{
				throw new ArgumentOutOfRangeException("transforms");
			}
			for (int i = 0; i < m_bones.Count; i++)
			{
				ModelBone modelBone = m_bones[i];
				if (modelBone.ParentBone == null)
				{
					absoluteTransforms[i] = modelBone.Transform;
				}
				else
				{
					Matrix.MultiplyRestricted(ref modelBone.m_transform, ref absoluteTransforms[modelBone.ParentBone.Index], out absoluteTransforms[i]);
				}
			}
		}

		public BoundingBox CalculateAbsoluteBoundingBox(Matrix[] absoluteTransforms)
		{
			if (absoluteTransforms == null)
			{
				throw new ArgumentNullException("transforms");
			}
			if (absoluteTransforms.Length < m_bones.Count)
			{
				throw new ArgumentOutOfRangeException("transforms");
			}
			BoundingBox result = default(BoundingBox);
			bool flag = false;
			foreach (ModelMesh mesh in Meshes)
			{
				if (flag)
				{
					BoundingBox.Transform(ref mesh.m_boundingBox, ref absoluteTransforms[mesh.ParentBone.Index], out BoundingBox result2);
					result = BoundingBox.Union(result, result2);
				}
				else
				{
					BoundingBox.Transform(ref mesh.m_boundingBox, ref absoluteTransforms[mesh.ParentBone.Index], out result);
					flag = true;
				}
			}
			return result;
		}

		public void Dispose()
		{
			InternalDispose();
		}

		public static Model Load(ModelData modelData, bool keepSourceVertexDataInTags = false)
		{
			Model model = new Model();
			model.Initialize(modelData, keepSourceVertexDataInTags);
			return model;
		}

		public static Model Load(Stream stream, bool keepSourceVertexDataInTags = false)
		{
			return Load(ModelData.Load(stream));
		}

		public static Model Load(string fileName, bool keepSourceVertexDataInTags = false)
		{
			return Load(ModelData.Load(fileName));
		}

		internal void Initialize(ModelData modelData, bool keepSourceVertexDataInTags)
		{
			if (modelData == null)
			{
				throw new ArgumentNullException("modelData");
			}
			InternalDispose();
			VertexBuffer[] array = new VertexBuffer[modelData.Buffers.Count];
			IndexBuffer[] array2 = new IndexBuffer[modelData.Buffers.Count];
			for (int i = 0; i < modelData.Buffers.Count; i++)
			{
				ModelBuffersData modelBuffersData = modelData.Buffers[i];
				array[i] = new VertexBuffer(modelBuffersData.VertexDeclaration, modelBuffersData.Vertices.Length / modelBuffersData.VertexDeclaration.VertexStride);
				array[i].SetData(modelBuffersData.Vertices, 0, modelBuffersData.Vertices.Length);
				array2[i] = new IndexBuffer(IndexFormat.SixteenBits, modelBuffersData.Indices.Length / 2);
				array2[i].SetData(modelBuffersData.Indices, 0, modelBuffersData.Indices.Length);
				if (keepSourceVertexDataInTags)
				{
					array[i].Tag = modelBuffersData.Vertices;
					array2[i].Tag = modelBuffersData.Indices;
				}
			}
			foreach (ModelBoneData bone in modelData.Bones)
			{
				NewBone(bone.Name, bone.Transform, (bone.ParentBoneIndex >= 0) ? m_bones[bone.ParentBoneIndex] : null);
			}
			foreach (ModelMeshData mesh in modelData.Meshes)
			{
				ModelMesh modelMesh = NewMesh(mesh.Name, m_bones[mesh.ParentBoneIndex], mesh.BoundingBox);
				m_meshes.Add(modelMesh);
				foreach (ModelMeshPartData meshPart in mesh.MeshParts)
				{
					modelMesh.NewMeshPart(array[meshPart.BuffersDataIndex], array2[meshPart.BuffersDataIndex], meshPart.StartIndex, meshPart.IndicesCount, meshPart.BoundingBox);
				}
			}
		}

		public void InternalDispose()
		{
			m_rootBone = null;
			m_bones.Clear();
			Utilities.DisposeCollection(m_meshes);
		}
	}
}
