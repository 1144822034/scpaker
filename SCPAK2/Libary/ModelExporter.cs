using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SCPAK
{
	public class ModelExporter
	{
		public const string COLLADA = "http://www.collada.org/2005/11/COLLADASchema";

		public const string INSTANCE = "http://www.w3.org/2001/XMLSchema-instance";

		private XNamespace colladaNS = "http://www.collada.org/2005/11/COLLADASchema";

		private XNamespace instanceNS = "http://www.w3.org/2001/XMLSchema-instance";

		private XDocument document;

		private XElement root;

		private XElement visualScene;

		public ModelExporter(ModelData modelData)
		{
			document = new XDocument(new XDeclaration("1.0", "UTF-8", null));
			root = new XElement(colladaNS + "COLLADA", new XAttribute(XNamespace.Xmlns + "xsi", instanceNS), new XAttribute("version", "1.4.1"), new XElement(colladaNS + "asset", new XElement(colladaNS + "contributor", new XElement(colladaNS + "author", "Survivalcraft Moder"), new XElement(colladaNS + "authoring_tool", "Engine 0.0.0")), new XElement(colladaNS + "created", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:00")), new XElement(colladaNS + "modified", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:00")), new XElement(colladaNS + "up_axis", "Y_UP")), new XElement(colladaNS + "library_images"), new XElement(colladaNS + "library_effects"), new XElement(colladaNS + "library_materials"), new XElement(colladaNS + "library_geometries"), new XElement(colladaNS + "library_controllers"), new XElement(colladaNS + "library_visual_scenes"), new XElement(colladaNS + "scene"));
			visualScene = InitVisualScene("Scene");
			document.Add(root);
			XElement[] array = new XElement[modelData.Bones.Count];
			for (int i = 0; i < array.Length; i++)
			{
				ModelBoneData modelBoneData = modelData.Bones[i];
				array[i] = GetNode(modelBoneData.Name, modelBoneData.Transform);
			}
			for (int j = 0; j < array.Length; j++)
			{
				ModelBoneData modelBoneData2 = modelData.Bones[j];
				if (modelBoneData2.ParentBoneIndex != -1)
				{
					array[modelBoneData2.ParentBoneIndex].Add(array[j]);
				}
				else
				{
					visualScene.Add(array[j]);
				}
			}
			foreach (ModelMeshData mesh in modelData.Meshes)
			{
				XElement geometry = GetGeometry(modelData, mesh);
				array[mesh.ParentBoneIndex].Add(GetGeometryInstance(mesh.Name, geometry));
			}
		}

		public void Save(Stream stream)
		{
			document.Save(stream);
		}

		private XElement GetGeometry(ModelData model, ModelMeshData data)
		{
			ModelMeshPartData modelMeshPartData = data.MeshParts[0];
			ModelBoneData modelBoneDatum = model.Bones[data.ParentBoneIndex];
			string name = data.Name;
			ModelBuffersData modelBuffersData = model.Buffers[modelMeshPartData.BuffersDataIndex];
			int indicesCount = modelMeshPartData.IndicesCount;
			VertexDeclaration vertexDeclaration = modelBuffersData.VertexDeclaration;
			byte[] array = new byte[indicesCount * 32];
			using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(modelBuffersData.Indices)))
			{
				binaryReader.BaseStream.Position = modelMeshPartData.StartIndex * 2;
				for (int i = 0; i < indicesCount; i++)
				{
					short num = binaryReader.ReadInt16();
					Buffer.BlockCopy(modelBuffersData.Vertices, num * 32, array, i * 32, 32);
				}
			}
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			float[] array2 = new float[indicesCount * 2];
			int[] array3 = new int[indicesCount * 3];
			using (BinaryReader binaryReader2 = new BinaryReader(new MemoryStream(array)))
			{
				VertexElement[] vertexElements = vertexDeclaration.VertexElements;
				foreach (VertexElement vertexElement in vertexElements)
				{
					if (vertexElement.Semantic.StartsWith("POSITION"))
					{
						Vector3[] array4 = new Vector3[3];
						for (int k = 0; k < indicesCount; k++)
						{
							binaryReader2.BaseStream.Position = vertexDeclaration.VertexStride * k + vertexElement.Offset;
							Vector3 vector = new Vector3(binaryReader2.ReadSingle(), binaryReader2.ReadSingle(), binaryReader2.ReadSingle());
							if (!list.Contains(vector))
							{
								list.Add(vector);
							}
							int num2 = k % 3;
							array4[num2] = vector;
							if (num2 == 2)
							{
								array3[k * 3 - 6] = list.IndexOf(array4[0]);
								array3[k * 3 - 3] = list.IndexOf(array4[2]);
								array3[k * 3] = list.IndexOf(array4[1]);
							}
						}
					}
					else if (vertexElement.Semantic.StartsWith("NORMAL"))
					{
						Vector3[] array5 = new Vector3[3];
						for (int l = 0; l < indicesCount; l++)
						{
							binaryReader2.BaseStream.Position = vertexDeclaration.VertexStride * l + vertexElement.Offset;
							Vector3 vector2 = new Vector3(binaryReader2.ReadSingle(), binaryReader2.ReadSingle(), binaryReader2.ReadSingle());
							if (!list2.Contains(vector2))
							{
								list2.Add(vector2);
							}
							int num3 = l % 3;
							array5[num3] = vector2;
							if (num3 == 2)
							{
								array3[l * 3 - 5] = list2.IndexOf(array5[0]);
								array3[l * 3 - 2] = list2.IndexOf(array5[2]);
								array3[l * 3 + 1] = list2.IndexOf(array5[1]);
							}
						}
					}
					else if (vertexElement.Semantic.StartsWith("TEXCOORD"))
					{
						for (int m = 0; m < indicesCount; m++)
						{
							binaryReader2.BaseStream.Position = vertexDeclaration.VertexStride * m + vertexElement.Offset;
							array2[m * 2] = binaryReader2.ReadSingle();
							array2[m * 2 + 1] = 1f - binaryReader2.ReadSingle();
							if (m % 3 == 2)
							{
								array3[m * 3 - 4] = m - 2;
								array3[m * 3 - 1] = m;
								array3[m * 3 + 2] = m - 1;
							}
						}
					}
				}
			}
			XName name2 = colladaNS + "mesh";
			object[] array6 = new object[5];
			XElement source = (XElement)(array6[0] = GetSourceArray(name, "-mesh-positions", string.Join(" ", list.ConvertAll((Vector3 v) => string.Format("{0} {1} {2}", v.X.ToString("R"), v.Y.ToString("R"), v.Z.ToString("R")))), list.Count * 3, 3, XYZParam()));
			XElement source2 = (XElement)(array6[1] = GetSourceArray(name, "-mesh-normals", string.Join(" ", list2.ConvertAll((Vector3 v) => string.Format("{0} {1} {2}", v.X.ToString("R"), v.Y.ToString("R"), v.Z.ToString("R")))), list2.Count * 3, 3, XYZParam()));
			XElement source3 = (XElement)(array6[2] = GetSourceArray(name, "-mesh-map", string.Join(" ", array2.Select(delegate(float f)
			{
				float num4 = f;
				return num4.ToString("R");
			})), array2.Length, 2, STParam()));
			XElement source4 = (XElement)(array6[3] = new XElement(colladaNS + "vertices", new XAttribute("id", name + "-mesh-vertices"), GetInput("POSITION", source)));
			array6[4] = new XElement(colladaNS + "triangles", new XAttribute("count", indicesCount / 3), GetInput("VERTEX", source4, 0), GetInput("NORMAL", source2, 1), GetInput("TEXCOORD", source3, 2), new XElement(colladaNS + "p", string.Join(" ", array3)));
			XElement xElement = new XElement(name2, array6);
			XElement result;
			root.Element(colladaNS + "library_geometries").Add(result = new XElement(colladaNS + "geometry", new XAttribute("id", name + "-mesh"), new XAttribute("name", name), xElement));
			return result;
		}

		private XElement GetSourceArray(string id, string type, string array, int count, int stride, params XElement[] param)
		{
			return new XElement(colladaNS + "source", new XAttribute("id", id + type), new XElement(colladaNS + "float_array", new XAttribute("id", id + type + "-array"), new XAttribute("count", count), array), new XElement(colladaNS + "technique_common", new XElement(colladaNS + "accessor", new XAttribute("source", $"#{id}{type}-array"), new XAttribute("count", count / stride), new XAttribute("stride", stride), param)));
		}

		private XElement[] XYZParam()
		{
			return new XElement[3]
			{
				new XElement(colladaNS + "param", new XAttribute("name", "X"), new XAttribute("type", "float")),
				new XElement(colladaNS + "param", new XAttribute("name", "Y"), new XAttribute("type", "float")),
				new XElement(colladaNS + "param", new XAttribute("name", "Z"), new XAttribute("type", "float"))
			};
		}

		private XElement[] STParam()
		{
			return new XElement[2]
			{
				new XElement(colladaNS + "param", new XAttribute("name", "S"), new XAttribute("type", "float")),
				new XElement(colladaNS + "param", new XAttribute("name", "T"), new XAttribute("type", "float"))
			};
		}

		private XElement GetInput(string semantic, XElement source)
		{
			return new XElement(colladaNS + "input", new XAttribute("semantic", semantic), new XAttribute("source", "#" + source.Attribute("id").Value));
		}

		private XElement GetInput(string semantic, XElement source, int offset)
		{
			return new XElement(colladaNS + "input", new XAttribute("semantic", semantic), new XAttribute("source", "#" + source.Attribute("id").Value), new XAttribute("offset", offset));
		}

		private XElement InitVisualScene(string name)
		{
			XElement result;
			root.Element(colladaNS + "library_visual_scenes").Add(result = new XElement(colladaNS + "visual_scene", new XAttribute("id", name), new XAttribute("name", name)));
			root.Element(colladaNS + "scene").Add(new XElement(colladaNS + "instance_visual_scene", new XAttribute("url", "#" + name)));
			return result;
		}

		private XElement GetGeometryInstance(string id, XElement geometry)
		{
			return new XElement(colladaNS + "instance_geometry", new XAttribute("url", "#" + geometry.Attribute("id").Value), new XAttribute("name", id));
		}

		private XElement GetNode(string id, Matrix transform)
		{
			return new XElement(colladaNS + "node", new XAttribute("id", id), new XAttribute("name", id), new XAttribute("type", "NODE"), new XElement(colladaNS + "matrix", new XAttribute("sid", "transform"), $"{transform.M11} {transform.M21} {transform.M31} {transform.M41} {transform.M12} {transform.M22} {transform.M32} {transform.M42} {transform.M13} {transform.M23} {transform.M33} {transform.M43} {transform.M14} {transform.M24} {transform.M34} {transform.M44}"));
		}
	}
}
