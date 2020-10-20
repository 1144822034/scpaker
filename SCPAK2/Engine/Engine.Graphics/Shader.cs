#pragma warning disable CS0246 // 未能找到类型或命名空间名“OpenTK”(是否缺少 using 指令或程序集引用?)
using OpenTK.Graphics.ES20;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“OpenTK”(是否缺少 using 指令或程序集引用?)
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Engine.Graphics
{
	public class Shader : GraphicsResource
	{
		internal struct ShaderAttributeData
		{
			public string Semantic;

			public int Location;
		}

		internal struct VertexAttributeData
		{
			public int Size;

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
			public All Type;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)

			public bool Normalize;

			public int Offset;
		}

		public Dictionary<string, ShaderParameter> m_parametersByName;

		internal ShaderParameter[] m_parameters;

		public string m_vertexShaderCode;

		public string m_pixelShaderCode;

		public ShaderMacro[] m_shaderMacros;

		internal int m_program;

		internal int m_vertexShader;

		internal int m_pixelShader;

		internal Dictionary<VertexDeclaration, VertexAttributeData[]> m_vertexAttributeDataByDeclaration = new Dictionary<VertexDeclaration, VertexAttributeData[]>();

		internal List<ShaderAttributeData> m_shaderAttributeData = new List<ShaderAttributeData>();

		public ShaderParameter m_glymulParameter;

		public object Tag
		{
			get;
			set;
		}

		public ReadOnlyList<ShaderParameter> Parameters => new ReadOnlyList<ShaderParameter>(m_parameters);

		public string DebugName
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		public ShaderParameter GetParameter(string name, bool allowNull = false)
		{
			if (!m_parametersByName.TryGetValue(name, out ShaderParameter value) && !allowNull)
			{
				throw new InvalidOperationException($"Parameter \"{name}\" not found.");
			}
			return value;
		}

		public override int GetGpuMemoryUsage()
		{
			return 16384;
		}

		public virtual void PrepareForDrawingOverride()
		{
		}

		public void InitializeShader(string vertexShaderCode, string pixelShaderCode, ShaderMacro[] shaderMacros)
		{
			if (vertexShaderCode == null)
			{
				throw new ArgumentNullException("vertexShaderCode");
			}
			if (pixelShaderCode == null)
			{
				throw new ArgumentNullException("pixelShaderCode");
			}
			if (shaderMacros == null)
			{
				throw new ArgumentNullException("shaderMacros");
			}
			m_vertexShaderCode = vertexShaderCode;
			m_pixelShaderCode = pixelShaderCode;
			m_shaderMacros = (ShaderMacro[])shaderMacros.Clone();
		}

		public Shader(string vertexShaderCode, string pixelShaderCode, params ShaderMacro[] shaderMacros)
		{
			try
			{
				InitializeShader(vertexShaderCode, pixelShaderCode, shaderMacros);
				CompileShaders();
			}
			catch
			{
				Dispose();
				throw;
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			DeleteShaders();
		}

		internal void PrepareForDrawing()
		{
			m_glymulParameter.SetValue((Display.RenderTarget != null) ? (-1f) : 1f);
			PrepareForDrawingOverride();
		}

		internal VertexAttributeData[] GetVertexAttribData(VertexDeclaration vertexDeclaration)
		{
			if (!m_vertexAttributeDataByDeclaration.TryGetValue(vertexDeclaration, out VertexAttributeData[] value))
			{
				value = new VertexAttributeData[8];
				foreach (ShaderAttributeData shaderAttributeDatum in m_shaderAttributeData)
				{
					VertexElement vertexElement = null;
					for (int i = 0; i < vertexDeclaration.m_elements.Length; i++)
					{
						if (vertexDeclaration.m_elements[i].Semantic == shaderAttributeDatum.Semantic)
						{
							vertexElement = vertexDeclaration.m_elements[i];
							break;
						}
					}
					if (!(vertexElement != null))
					{
						throw new InvalidOperationException($"VertexElement not found for shader attribute \"{shaderAttributeDatum.Semantic}\".");
					}
					value[shaderAttributeDatum.Location] = new VertexAttributeData
					{
						Size = vertexElement.Format.GetElementsCount(),
						Offset = vertexElement.Offset
					};
					GLWrapper.TranslateVertexElementFormat(vertexElement.Format, out value[shaderAttributeDatum.Location].Type, out value[shaderAttributeDatum.Location].Normalize);
				}
				m_vertexAttributeDataByDeclaration.Add(vertexDeclaration, value);
			}
			return value;
		}

		public static void ParseShaderMetadata(string shaderCode, Dictionary<string, string> semanticsByAttribute, Dictionary<string, string> samplersByTexture)
		{
			string[] array = shaderCode.Split('\n', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				try
				{
					string text = array[i];
					text = text.Trim();
					if (text.StartsWith("//"))
					{
						text = text.Substring(2).TrimStart();
						if (text.StartsWith("<") && text.EndsWith("/>"))
						{
							XElement xElement = XElement.Parse(text);
							if (xElement.Name == "Semantic")
							{
								if (xElement.Attribute("Attribute") == null)
								{
									throw new InvalidOperationException("Missing \"Attribute\" attribute in shader metadata.");
								}
								if (xElement.Attribute("Name") == null)
								{
									throw new InvalidOperationException("Missing \"Name\" attribute in shader metadata.");
								}
								semanticsByAttribute.Add(xElement.Attribute("Attribute").Value, xElement.Attribute("Name").Value);
							}
							else
							{
								if (!(xElement.Name == "Sampler"))
								{
									throw new InvalidOperationException("Unrecognized shader metadata node.");
								}
								if (xElement.Attribute("Texture") == null)
								{
									throw new InvalidOperationException("Missing \"Texture\" attribute in shader metadata.");
								}
								if (xElement.Attribute("Name") == null)
								{
									throw new InvalidOperationException("Missing \"Name\" attribute in shader metadata.");
								}
								samplersByTexture.Add(xElement.Attribute("Texture").Value, xElement.Attribute("Name").Value);
							}
						}
					}
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException($"Error in shader metadata, line {i + 1}. {ex.Message}");
				}
			}
		}

		public string PrependShaderMacros(string shaderCode, ShaderMacro[] shaderMacros, bool isVertexShader)
		{
			string str = "#version 100" + Environment.NewLine;
			str = str + "#define GLSL" + Environment.NewLine;
			if (isVertexShader)
			{
				str = ((!Display.UseReducedZRange) ? (str + "#define OPENGL_POSITION_FIX gl_Position.y *= u_glymul; gl_Position.z = 2.0 * gl_Position.z - gl_Position.w;" + Environment.NewLine) : (str + "#define OPENGL_POSITION_FIX gl_Position.y *= u_glymul;" + Environment.NewLine));
				str = str + "uniform float u_glymul;" + Environment.NewLine;
			}
			foreach (ShaderMacro shaderMacro in shaderMacros)
			{
				str = str + "#define " + shaderMacro.Name + " " + shaderMacro.Value + Environment.NewLine;
			}
			str = str + "#line 1" + Environment.NewLine;
			return str + shaderCode;
		}

		internal override void HandleDeviceLost()
		{
			DeleteShaders();
		}

		internal override void HandleDeviceReset()
		{
			CompileShaders();
		}

		internal void CompileShaders()
		{
			DeleteShaders();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			ParseShaderMetadata(m_vertexShaderCode, dictionary, dictionary2);
			ParseShaderMetadata(m_pixelShaderCode, dictionary, dictionary2);
			string @string = PrependShaderMacros(m_vertexShaderCode, m_shaderMacros, isVertexShader: true);
			string string2 = PrependShaderMacros(m_pixelShaderCode, m_shaderMacros, isVertexShader: false);
			m_vertexShader = GL.CreateShader(All.VertexShader);
			GL.ShaderSource(m_vertexShader, @string);
			GL.CompileShader(m_vertexShader);
			GL.GetShader(m_vertexShader, OpenTK.Graphics.ES20.ShaderParameter.CompileStatus, out int @params);
			if (@params != 1)
			{
				string shaderInfoLog = GL.GetShaderInfoLog(m_vertexShader);
				throw new InvalidOperationException($"Error compiling vertex shader.\n{shaderInfoLog}");
			}
			m_pixelShader = GL.CreateShader(All.FragmentShader);
			GL.ShaderSource(m_pixelShader, string2);
			GL.CompileShader(m_pixelShader);
			GL.GetShader(m_pixelShader, OpenTK.Graphics.ES20.ShaderParameter.CompileStatus, out int params2);
			if (params2 != 1)
			{
				string shaderInfoLog2 = GL.GetShaderInfoLog(m_pixelShader);
				throw new InvalidOperationException($"Error compiling pixel shader.\n{shaderInfoLog2}");
			}
			m_program = GL.CreateProgram();
			GL.AttachShader(m_program, m_vertexShader);
			GL.AttachShader(m_program, m_pixelShader);
			GL.LinkProgram(m_program);
			GL.GetProgram(m_program, ProgramParameter.LinkStatus, out int params3);
			if (params3 != 1)
			{
				string programInfoLog = GL.GetProgramInfoLog(m_program);
				throw new InvalidOperationException($"Error linking program.\n{programInfoLog}");
			}
			GL.GetProgram(m_program, ProgramParameter.ActiveAttributes, out int params4);
			for (int i = 0; i < params4; i++)
			{
				StringBuilder stringBuilder = new StringBuilder(256);
				GL.GetActiveAttrib(m_program, i, stringBuilder.Capacity, out int _, out int _, out ActiveAttribType _, stringBuilder);
				int attribLocation = GL.GetAttribLocation(m_program, stringBuilder.ToString());
				if (!dictionary.TryGetValue(stringBuilder.ToString(), out string value))
				{
					throw new InvalidOperationException($"Attribute \"{stringBuilder.ToString()}\" has no semantic defined in shader metadata.");
				}
				m_shaderAttributeData.Add(new ShaderAttributeData
				{
					Location = attribLocation,
					Semantic = value
				});
			}
			GL.GetProgram(m_program, ProgramParameter.ActiveUniforms, out int params5);
			List<ShaderParameter> list = new List<ShaderParameter>();
			Dictionary<string, ShaderParameter> dictionary3 = new Dictionary<string, ShaderParameter>();
			for (int j = 0; j < params5; j++)
			{
				StringBuilder stringBuilder2 = new StringBuilder(256);
				GL.GetActiveUniform(m_program, j, stringBuilder2.Capacity, out int _, out int size2, out ActiveUniformType type2, stringBuilder2);
				int uniformLocation = GL.GetUniformLocation(m_program, stringBuilder2.ToString());
				ShaderParameterType shaderParameterType = GLWrapper.TranslateActiveUniformType(type2);
				int num = stringBuilder2.ToString().IndexOf('[');
				if (num >= 0)
				{
					stringBuilder2.Remove(num, stringBuilder2.Length - num);
				}
				ShaderParameter shaderParameter = new ShaderParameter(this, stringBuilder2.ToString(), shaderParameterType, size2);
				shaderParameter.Location = uniformLocation;
				dictionary3.Add(shaderParameter.Name, shaderParameter);
				list.Add(shaderParameter);
				if (shaderParameterType == ShaderParameterType.Texture2D)
				{
					if (!dictionary2.TryGetValue(shaderParameter.Name, out string value2))
					{
						throw new InvalidOperationException($"Texture \"{shaderParameter.Name}\" has no sampler defined in shader metadata.");
					}
					ShaderParameter shaderParameter2 = new ShaderParameter(this, value2, ShaderParameterType.Sampler2D, 1);
					shaderParameter2.Location = int.MaxValue;
					dictionary3.Add(value2, shaderParameter2);
					list.Add(shaderParameter2);
				}
			}
			if (m_parameters != null)
			{
				foreach (KeyValuePair<string, ShaderParameter> item in dictionary3)
				{
					if (m_parametersByName.TryGetValue(item.Key, out ShaderParameter value3))
					{
						value3.Location = item.Value.Location;
					}
				}
				ShaderParameter[] parameters = m_parameters;
				for (int k = 0; k < parameters.Length; k++)
				{
					parameters[k].IsChanged = true;
				}
			}
			else
			{
				m_parameters = list.ToArray();
				m_parametersByName = dictionary3;
			}
			m_glymulParameter = GetParameter("u_glymul");
			if (m_glymulParameter.Type != 0)
			{
				throw new InvalidOperationException("u_glymul parameter has invalid type.");
			}
		}

		internal void DeleteShaders()
		{
			if (m_program != 0)
			{
				if (m_vertexShader != 0)
				{
					GL.DetachShader(m_program, m_vertexShader);
				}
				if (m_pixelShader != 0)
				{
					GL.DetachShader(m_program, m_pixelShader);
				}
				GLWrapper.DeleteProgram(m_program);
				m_program = 0;
			}
			if (m_vertexShader != 0)
			{
				GL.DeleteShader(m_vertexShader);
				m_vertexShader = 0;
			}
			if (m_pixelShader != 0)
			{
				GL.DeleteShader(m_pixelShader);
				m_pixelShader = 0;
			}
		}
	}
}
