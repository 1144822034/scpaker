using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Engine.Media
{
	public static class Collada
	{
		public struct Vertex : IEquatable<Vertex>
		{
			public byte[] Data;

			public int Start;

			public int Count;

			public int m_hashCode;

			public Vertex(byte[] data, int start, int count)
			{
				Data = data;
				Start = start;
				Count = count;
				m_hashCode = 0;
				for (int i = 0; i < Count; i++)
				{
					m_hashCode += (7919 * i + 977) * Data[i + Start];
				}
			}

			public bool Equals(Vertex other)
			{
				if (m_hashCode != other.m_hashCode || Data.Length != other.Data.Length)
				{
					return false;
				}
				for (int i = 0; i < Count; i++)
				{
					if (Data[i + Start] != other.Data[i + other.Start])
					{
						return false;
					}
				}
				return true;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is Color))
				{
					return false;
				}
				return Equals((Color)obj);
			}

			public override int GetHashCode()
			{
				return m_hashCode;
			}
		}

		public class Asset
		{
			public readonly float Meter = 1f;

			public Asset(XElement node)
			{
				XElement xElement = node.Element(ColladaRoot.Namespace + "unit");
				if (xElement != null)
				{
					XAttribute xAttribute = xElement.Attribute("meter");
					if (xAttribute != null)
					{
						Meter = float.Parse(xAttribute.Value, CultureInfo.InvariantCulture);
					}
				}
			}
		}

		public class ColladaRoot
		{
			public static readonly XNamespace Namespace = "http://www.collada.org/2005/11/COLLADASchema";

			public readonly Dictionary<string, ColladaNameId> ObjectsById = new Dictionary<string, ColladaNameId>();

			public readonly Asset Asset;

			public readonly List<ColladaLibraryGeometries> LibraryGeometries = new List<ColladaLibraryGeometries>();

			public readonly List<ColladaLibraryVisualScenes> LibraryVisualScenes = new List<ColladaLibraryVisualScenes>();

			public readonly ColladaScene Scene;

			public ColladaRoot(XElement node)
			{
				Asset = new Asset(node.Element(Namespace + "asset"));
				foreach (XElement item in node.Elements(Namespace + "library_geometries"))
				{
					LibraryGeometries.Add(new ColladaLibraryGeometries(this, item));
				}
				foreach (XElement item2 in node.Elements(Namespace + "library_visual_scenes"))
				{
					LibraryVisualScenes.Add(new ColladaLibraryVisualScenes(this, item2));
				}
				Scene = new ColladaScene(this, node.Element(Namespace + "scene"));
			}
		}

		public class ColladaNameId
		{
			public string Id;

			public string Name;

			public ColladaNameId(ColladaRoot collada, XElement node, string idPostfix = "")
			{
				XAttribute xAttribute = node.Attribute("id");
				if (xAttribute != null)
				{
					Id = xAttribute.Value + idPostfix;
					collada.ObjectsById.Add(Id, this);
				}
				XAttribute xAttribute2 = node.Attribute("name");
				if (xAttribute2 != null)
				{
					Name = xAttribute2.Value;
				}
			}
		}

		public class ColladaLibraryVisualScenes
		{
			public List<ColladaVisualScene> VisualScenes = new List<ColladaVisualScene>();

			public ColladaLibraryVisualScenes(ColladaRoot collada, XElement node)
			{
				foreach (XElement item in node.Elements(ColladaRoot.Namespace + "visual_scene"))
				{
					VisualScenes.Add(new ColladaVisualScene(collada, item));
				}
			}
		}

		public class ColladaLibraryGeometries
		{
			public List<ColladaGeometry> Geometries = new List<ColladaGeometry>();

			public ColladaLibraryGeometries(ColladaRoot collada, XElement node)
			{
				foreach (XElement item in node.Elements(ColladaRoot.Namespace + "geometry"))
				{
					Geometries.Add(new ColladaGeometry(collada, item));
				}
			}
		}

		public class ColladaScene
		{
			public ColladaVisualScene VisualScene;

			public ColladaScene(ColladaRoot collada, XElement node)
			{
				XElement xElement = node.Element(ColladaRoot.Namespace + "instance_visual_scene");
				VisualScene = (ColladaVisualScene)collada.ObjectsById[xElement.Attribute("url").Value.Substring(1) + "-ColladaVisualScene"];
			}
		}

		public class ColladaVisualScene : ColladaNameId
		{
			public List<ColladaNode> Nodes = new List<ColladaNode>();

			public ColladaVisualScene(ColladaRoot collada, XElement node)
				: base(collada, node, "-ColladaVisualScene")
			{
				foreach (XElement item in node.Elements(ColladaRoot.Namespace + "node"))
				{
					Nodes.Add(new ColladaNode(collada, item));
				}
			}
		}

		public class ColladaNode : ColladaNameId
		{
			public Matrix Transform = Matrix.Identity;

			public List<ColladaNode> Nodes = new List<ColladaNode>();

			public List<ColladaGeometry> Geometries = new List<ColladaGeometry>();

			public ColladaNode(ColladaRoot collada, XElement node)
				: base(collada, node)
			{
				foreach (XElement item in node.Elements())
				{
					if (item.Name == ColladaRoot.Namespace + "matrix")
					{
						float[] array = (from s in item.Value.Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
							select float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
						Transform = Matrix.Transpose(new Matrix(array[0], array[1], array[2], array[3], array[4], array[5], array[6], array[7], array[8], array[9], array[10], array[11], array[12], array[13], array[14], array[15])) * Transform;
					}
					else if (item.Name == ColladaRoot.Namespace + "translate")
					{
						float[] array2 = (from s in item.Value.Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
							select float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
						Transform = Matrix.CreateTranslation(array2[0], array2[1], array2[2]) * Transform;
					}
					else if (item.Name == ColladaRoot.Namespace + "rotate")
					{
						float[] array3 = (from s in item.Value.Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
							select float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
						Transform = Matrix.CreateFromAxisAngle(new Vector3(array3[0], array3[1], array3[2]), MathUtils.DegToRad(array3[3])) * Transform;
					}
					else if (item.Name == ColladaRoot.Namespace + "scale")
					{
						float[] array4 = (from s in item.Value.Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
							select float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
						Transform = Matrix.CreateScale(array4[0], array4[1], array4[2]) * Transform;
					}
				}
				foreach (XElement item2 in node.Elements(ColladaRoot.Namespace + "node"))
				{
					Nodes.Add(new ColladaNode(collada, item2));
				}
				foreach (XElement item3 in node.Elements(ColladaRoot.Namespace + "instance_geometry"))
				{
					Geometries.Add((ColladaGeometry)collada.ObjectsById[item3.Attribute("url").Value.Substring(1)]);
				}
			}
		}

		public class ColladaGeometry : ColladaNameId
		{
			public ColladaMesh Mesh;

			public ColladaGeometry(ColladaRoot collada, XElement node)
				: base(collada, node)
			{
				XElement xElement = node.Element(ColladaRoot.Namespace + "mesh");
				if (xElement != null)
				{
					Mesh = new ColladaMesh(collada, xElement);
				}
			}
		}

		public class ColladaMesh
		{
			public List<ColladaSource> Sources = new List<ColladaSource>();

			public ColladaVertices Vertices;

			public List<ColladaPolygons> Polygons = new List<ColladaPolygons>();

			public ColladaMesh(ColladaRoot collada, XElement node)
			{
				foreach (XElement item in node.Elements(ColladaRoot.Namespace + "source"))
				{
					Sources.Add(new ColladaSource(collada, item));
				}
				XElement node2 = node.Element(ColladaRoot.Namespace + "vertices");
				Vertices = new ColladaVertices(collada, node2);
				foreach (XElement item2 in node.Elements(ColladaRoot.Namespace + "polygons").Concat(node.Elements(ColladaRoot.Namespace + "polylist")).Concat(node.Elements(ColladaRoot.Namespace + "triangles")))
				{
					Polygons.Add(new ColladaPolygons(collada, item2));
				}
			}
		}

		public class ColladaSource : ColladaNameId
		{
			public ColladaFloatArray FloatArray;

			public ColladaAccessor Accessor;

			public ColladaSource(ColladaRoot collada, XElement node)
				: base(collada, node)
			{
				XElement xElement = node.Element(ColladaRoot.Namespace + "float_array");
				if (xElement != null)
				{
					FloatArray = new ColladaFloatArray(collada, xElement);
				}
				XElement xElement2 = node.Element(ColladaRoot.Namespace + "technique_common");
				if (xElement2 != null)
				{
					XElement xElement3 = xElement2.Element(ColladaRoot.Namespace + "accessor");
					if (xElement3 != null)
					{
						Accessor = new ColladaAccessor(collada, xElement3);
					}
				}
			}
		}

		public class ColladaFloatArray : ColladaNameId
		{
			public float[] Array;

			public ColladaFloatArray(ColladaRoot collada, XElement node)
				: base(collada, node)
			{
				Array = (from s in node.Value.Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
					select float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
			}
		}

		public class ColladaAccessor
		{
			public ColladaFloatArray Source;

			public int Offset;

			public int Stride = 1;

			public ColladaAccessor(ColladaRoot collada, XElement node)
			{
				Source = (ColladaFloatArray)collada.ObjectsById[node.Attribute("source").Value.Substring(1)];
				XAttribute xAttribute = node.Attribute("offset");
				if (xAttribute != null)
				{
					Offset = int.Parse(xAttribute.Value, CultureInfo.InvariantCulture);
				}
				XAttribute xAttribute2 = node.Attribute("stride");
				if (xAttribute2 != null)
				{
					Stride = int.Parse(xAttribute2.Value, CultureInfo.InvariantCulture);
				}
			}
		}

		public class ColladaVertices : ColladaNameId
		{
			public string Semantic;

			public ColladaSource Source;

			public ColladaVertices(ColladaRoot collada, XElement node)
				: base(collada, node)
			{
				XElement xElement = node.Element(ColladaRoot.Namespace + "input");
				Semantic = xElement.Attribute("semantic").Value;
				Source = (ColladaSource)collada.ObjectsById[xElement.Attribute("source").Value.Substring(1)];
			}
		}

		public class ColladaPolygons
		{
			public List<ColladaInput> Inputs = new List<ColladaInput>();

			public List<int> VCount = new List<int>();

			public List<int> P = new List<int>();

			public ColladaPolygons(ColladaRoot collada, XElement node)
			{
				foreach (XElement item in node.Elements(ColladaRoot.Namespace + "input"))
				{
					Inputs.Add(new ColladaInput(collada, item));
				}
				foreach (XElement item2 in node.Elements(ColladaRoot.Namespace + "vcount"))
				{
					VCount.AddRange(from s in item2.Value.Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
						select int.Parse(s, CultureInfo.InvariantCulture));
				}
				foreach (XElement item3 in node.Elements(ColladaRoot.Namespace + "p"))
				{
					P.AddRange(from s in item3.Value.Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
						select int.Parse(s, CultureInfo.InvariantCulture));
				}
			}
		}

		public class ColladaInput
		{
			public int Offset;

			public string Semantic;

			public int Set;

			public ColladaSource Source;

			public ColladaInput(ColladaRoot collada, XElement node)
			{
				Offset = int.Parse(node.Attribute("offset").Value, CultureInfo.InvariantCulture);
				Semantic = node.Attribute("semantic").Value;
				XAttribute xAttribute = node.Attribute("set");
				if (xAttribute != null)
				{
					Set = int.Parse(xAttribute.Value, CultureInfo.InvariantCulture);
				}
				ColladaNameId colladaNameId = collada.ObjectsById[node.Attribute("source").Value.Substring(1)];
				if (colladaNameId is ColladaVertices)
				{
					ColladaVertices colladaVertices = (ColladaVertices)colladaNameId;
					Source = colladaVertices.Source;
					Semantic = colladaVertices.Semantic;
				}
				else
				{
					Source = (ColladaSource)colladaNameId;
				}
			}
		}

		public static bool IsColladaStream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			bool result = false;
			long position = stream.Position;
			try
			{
				XmlReader xmlReader = XmlReader.Create(stream, new XmlReaderSettings
				{
					IgnoreComments = true,
					IgnoreWhitespace = true
				});
				while (xmlReader.Read())
				{
					if (xmlReader.NodeType == XmlNodeType.Element)
					{
						if (xmlReader.LocalName == "COLLADA")
						{
							result = true;
						}
						break;
					}
				}
			}
			catch (XmlException)
			{
			}
			stream.Position = position;
			return result;
		}

		public static ModelData Load(Stream stream)
		{
			ModelData modelData = new ModelData();
			ColladaRoot colladaRoot = new ColladaRoot(XElement.Load(stream));
			if (colladaRoot.Scene.VisualScene.Nodes.Count > 1)
			{
				ModelBoneData modelBoneData = new ModelBoneData();
				modelData.Bones.Add(modelBoneData);
				modelBoneData.ParentBoneIndex = -1;
				modelBoneData.Name = string.Empty;
				modelBoneData.Transform = Matrix.Identity;
				foreach (ColladaNode node in colladaRoot.Scene.VisualScene.Nodes)
				{
					LoadNode(modelData, modelBoneData, node, Matrix.CreateScale(colladaRoot.Asset.Meter));
				}
			}
			else
			{
				foreach (ColladaNode node2 in colladaRoot.Scene.VisualScene.Nodes)
				{
					LoadNode(modelData, null, node2, Matrix.CreateScale(colladaRoot.Asset.Meter));
				}
			}
			foreach (ModelBuffersData buffer in modelData.Buffers)
			{
				IndexVertices(buffer.VertexDeclaration.VertexStride, buffer.Vertices, out buffer.Vertices, out buffer.Indices);
			}
			return modelData;
		}

		public static ModelBoneData LoadNode(ModelData data, ModelBoneData parentBoneData, ColladaNode node, Matrix transform)
		{
			ModelBoneData modelBoneData = new ModelBoneData();
			data.Bones.Add(modelBoneData);
			modelBoneData.ParentBoneIndex = ((parentBoneData != null) ? data.Bones.IndexOf(parentBoneData) : (-1));
			modelBoneData.Name = node.Name;
			modelBoneData.Transform = node.Transform * transform;
			foreach (ColladaNode node2 in node.Nodes)
			{
				LoadNode(data, modelBoneData, node2, Matrix.Identity);
			}
			foreach (ColladaGeometry geometry in node.Geometries)
			{
				LoadGeometry(data, modelBoneData, geometry);
			}
			return modelBoneData;
		}

		public static ModelMeshData LoadGeometry(ModelData data, ModelBoneData parentBoneData, ColladaGeometry geometry)
		{
			ModelMeshData modelMeshData = new ModelMeshData();
			data.Meshes.Add(modelMeshData);
			modelMeshData.Name = parentBoneData.Name;
			modelMeshData.ParentBoneIndex = data.Bones.IndexOf(parentBoneData);
			bool flag = false;
			foreach (ColladaPolygons polygon in geometry.Mesh.Polygons)
			{
				ModelMeshPartData modelMeshPartData = LoadPolygons(data, polygon);
				modelMeshData.MeshParts.Add(modelMeshPartData);
				modelMeshData.BoundingBox = (flag ? BoundingBox.Union(modelMeshData.BoundingBox, modelMeshPartData.BoundingBox) : modelMeshPartData.BoundingBox);
				flag = true;
			}
			return modelMeshData;
		}

		public static ModelMeshPartData LoadPolygons(ModelData data, ColladaPolygons polygons)
		{
			ModelMeshPartData modelMeshPartData = new ModelMeshPartData();
			int num = 0;
			Dictionary<VertexElement, ColladaInput> dictionary = new Dictionary<VertexElement, ColladaInput>();
			foreach (ColladaInput input in polygons.Inputs)
			{
				string str = (input.Set == 0) ? string.Empty : input.Set.ToString(CultureInfo.InvariantCulture);
				if (input.Semantic == "POSITION")
				{
					dictionary[new VertexElement(num, VertexElementFormat.Vector3, "POSITION" + str)] = input;
					num += 12;
				}
				else if (input.Semantic == "NORMAL")
				{
					dictionary[new VertexElement(num, VertexElementFormat.Vector3, "NORMAL" + str)] = input;
					num += 12;
				}
				else if (input.Semantic == "TEXCOORD")
				{
					dictionary[new VertexElement(num, VertexElementFormat.Vector2, "TEXCOORD" + str)] = input;
					num += 8;
				}
				else if (input.Semantic == "COLOR")
				{
					dictionary[new VertexElement(num, VertexElementFormat.NormalizedByte4, "COLOR" + str)] = input;
					num += 4;
				}
			}
			VertexDeclaration vertexDeclaration = new VertexDeclaration(dictionary.Keys.ToArray());
			ModelBuffersData modelBuffersData = data.Buffers.FirstOrDefault((ModelBuffersData vd) => vd.VertexDeclaration == vertexDeclaration);
			if (modelBuffersData == null)
			{
				modelBuffersData = new ModelBuffersData();
				data.Buffers.Add(modelBuffersData);
				modelBuffersData.VertexDeclaration = vertexDeclaration;
			}
			modelMeshPartData.BuffersDataIndex = data.Buffers.IndexOf(modelBuffersData);
			int num2 = polygons.P.Count / polygons.Inputs.Count;
			List<int> list = new List<int>();
			if (polygons.VCount.Count == 0)
			{
				int num3 = 0;
				for (int i = 0; i < num2 / 3; i++)
				{
					list.Add(num3);
					list.Add(num3 + 2);
					list.Add(num3 + 1);
					num3 += 3;
				}
			}
			else
			{
				int num4 = 0;
				using (List<int>.Enumerator enumerator2 = polygons.VCount.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						switch (enumerator2.Current)
						{
						case 3:
							list.Add(num4);
							list.Add(num4 + 2);
							list.Add(num4 + 1);
							num4 += 3;
							break;
						case 4:
							list.Add(num4);
							list.Add(num4 + 2);
							list.Add(num4 + 1);
							list.Add(num4 + 2);
							list.Add(num4);
							list.Add(num4 + 3);
							num4 += 4;
							break;
						default:
							throw new NotSupportedException("Collada polygons with less than 3 or more than 4 vertices are not supported.");
						}
					}
				}
			}
			int vertexStride = modelBuffersData.VertexDeclaration.VertexStride;
			int num5 = modelBuffersData.Vertices.Length;
			modelBuffersData.Vertices = ExtendArray(modelBuffersData.Vertices, list.Count * vertexStride);
			using (BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream(modelBuffersData.Vertices, num5, list.Count * vertexStride)))
			{
				bool flag = false;
				foreach (KeyValuePair<VertexElement, ColladaInput> item in dictionary)
				{
					VertexElement key = item.Key;
					ColladaInput value = item.Value;
					if (key.Semantic.StartsWith("POSITION"))
					{
						for (int j = 0; j < list.Count; j++)
						{
							float[] array = value.Source.Accessor.Source.Array;
							int offset = value.Source.Accessor.Offset;
							int stride = value.Source.Accessor.Stride;
							int num6 = polygons.P[list[j] * polygons.Inputs.Count + value.Offset];
							binaryWriter.BaseStream.Position = j * vertexStride + key.Offset;
							float num7 = array[offset + stride * num6];
							float num8 = array[offset + stride * num6 + 1];
							float num9 = array[offset + stride * num6 + 2];
							modelMeshPartData.BoundingBox = (flag ? BoundingBox.Union(modelMeshPartData.BoundingBox, new Vector3(num7, num8, num9)) : new BoundingBox(num7, num8, num9, num7, num8, num9));
							flag = true;
							binaryWriter.Write(num7);
							binaryWriter.Write(num8);
							binaryWriter.Write(num9);
						}
					}
					else if (key.Semantic.StartsWith("NORMAL"))
					{
						for (int k = 0; k < list.Count; k++)
						{
							float[] array2 = value.Source.Accessor.Source.Array;
							int offset2 = value.Source.Accessor.Offset;
							int stride2 = value.Source.Accessor.Stride;
							int num10 = polygons.P[list[k] * polygons.Inputs.Count + value.Offset];
							binaryWriter.BaseStream.Position = k * vertexStride + key.Offset;
							float num11 = array2[offset2 + stride2 * num10];
							float num12 = array2[offset2 + stride2 * num10 + 1];
							float num13 = array2[offset2 + stride2 * num10 + 2];
							float num14 = 1f / MathUtils.Sqrt(num11 * num11 + num12 * num12 + num13 * num13);
							binaryWriter.Write(num14 * num11);
							binaryWriter.Write(num14 * num12);
							binaryWriter.Write(num14 * num13);
						}
					}
					else if (key.Semantic.StartsWith("TEXCOORD"))
					{
						for (int l = 0; l < list.Count; l++)
						{
							float[] array3 = value.Source.Accessor.Source.Array;
							int offset3 = value.Source.Accessor.Offset;
							int stride3 = value.Source.Accessor.Stride;
							int num15 = polygons.P[list[l] * polygons.Inputs.Count + value.Offset];
							binaryWriter.BaseStream.Position = l * vertexStride + key.Offset;
							binaryWriter.Write(array3[offset3 + stride3 * num15]);
							binaryWriter.Write(1f - array3[offset3 + stride3 * num15 + 1]);
						}
					}
					else
					{
						if (!key.Semantic.StartsWith("COLOR"))
						{
							throw new Exception();
						}
						for (int m = 0; m < list.Count; m++)
						{
							float[] array4 = value.Source.Accessor.Source.Array;
							int offset4 = value.Source.Accessor.Offset;
							int stride4 = value.Source.Accessor.Stride;
							int num16 = polygons.P[list[m] * polygons.Inputs.Count + value.Offset];
							binaryWriter.BaseStream.Position = m * vertexStride + key.Offset;
							Color color = new Color(array4[offset4 + stride4 * num16], array4[offset4 + stride4 * num16 + 1], array4[offset4 + stride4 * num16 + 2], array4[offset4 + stride4 * num16 + 3]);
							binaryWriter.Write(color.PackedValue);
						}
					}
				}
			}
			modelMeshPartData.StartIndex = num5 / vertexStride;
			modelMeshPartData.IndicesCount = list.Count;
			return modelMeshPartData;
		}

		public static T[] ExtendArray<T>(T[] array, int extensionLength)
		{
			T[] array2 = new T[array.Length + extensionLength];
			Array.Copy(array, array2, array.Length);
			return array2;
		}

		public static void IndexVertices(int vertexStride, byte[] vertices, out byte[] resultVertices, out byte[] resultIndices)
		{
			int num = vertices.Length / vertexStride;
			Dictionary<Vertex, ushort> dictionary = new Dictionary<Vertex, ushort>();
			resultIndices = new byte[2 * num];
			for (int i = 0; i < num; i++)
			{
				Vertex key = new Vertex(vertices, i * vertexStride, vertexStride);
				if (!dictionary.TryGetValue(key, out ushort value))
				{
					value = (ushort)dictionary.Count;
					dictionary.Add(key, value);
				}
				resultIndices[i * 2] = (byte)value;
				resultIndices[i * 2 + 1] = (byte)(value >> 8);
			}
			resultVertices = new byte[dictionary.Count * vertexStride];
			foreach (KeyValuePair<Vertex, ushort> item in dictionary)
			{
				Vertex key2 = item.Key;
				ushort value2 = item.Value;
				Array.Copy(key2.Data, key2.Start, resultVertices, value2 * vertexStride, key2.Count);
			}
		}
	}
}
