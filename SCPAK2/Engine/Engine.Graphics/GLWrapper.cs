#pragma warning disable CS0246 // 未能找到类型或命名空间名“OpenTK”(是否缺少 using 指令或程序集引用?)
using OpenTK.Graphics.ES20;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“OpenTK”(是否缺少 using 指令或程序集引用?)
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Graphics
{
	internal static class GLWrapper
	{
		internal static int m_mainFramebuffer;

		internal static int m_mainColorbuffer;

		public static int m_arrayBuffer;

		public static int m_elementArrayBuffer;

		public static int m_texture2D;

		public static int[] m_activeTexturesByUnit;

		public static All m_activeTextureUnit;

		public static int m_program;

		public static int m_framebuffer;

		public static Vector4? m_clearColor;

		public static float? m_clearDepth;

		public static int? m_clearStencil;

		public static All m_cullFace;

		public static All m_frontFace;

		public static All m_depthFunction;

		public static int? m_colorMask;

		public static bool? m_depthMask;

		public static float m_polygonOffsetFactor;

		public static float m_polygonOffsetUnits;

		public static Vector4 m_blendColor;

		public static All m_blendEquation;

		public static All m_blendEquationColor;

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All m_blendEquationAlpha;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All m_blendFuncSource;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All m_blendFuncSourceColor;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All m_blendFuncSourceAlpha;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All m_blendFuncDestination;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All m_blendFuncDestinationColor;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All m_blendFuncDestinationAlpha;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static Dictionary<All, bool> m_enableDisableStates;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)

		public static bool?[] m_vertexAttribArray;

		public static RasterizerState m_rasterizerState;

		public static DepthStencilState m_depthStencilState;

		public static BlendState m_blendState;

		public static Dictionary<int, SamplerState> m_textureSamplerStates;

		public static Shader m_lastShader;

		public static VertexDeclaration m_lastVertexDeclaration;

		public static IntPtr m_lastVertexOffset;

		public static int m_lastArrayBuffer;

		public static Viewport? m_viewport;

		public static Rectangle? m_scissorRectangle;

		public static bool GL_EXT_texture_filter_anisotropic;

		public static bool GL_OES_packed_depth_stencil;

		public static void Initialize()
		{
			Log.Information("GLES Vendor: " + GL.GetString(All.Vendor));
			Log.Information("GLES Renderer: " + GL.GetString(All.Renderer));
			Log.Information("GLES Version: " + GL.GetString(All.Version));
			string @string = GL.GetString(All.Extensions);
			GL_EXT_texture_filter_anisotropic = @string.Contains("GL_EXT_texture_filter_anisotropic");
			GL_OES_packed_depth_stencil = @string.Contains("GL_OES_packed_depth_stencil");
		}

		public static void InitializeCache()
		{
			m_arrayBuffer = -1;
			m_elementArrayBuffer = -1;
			m_texture2D = -1;
			m_activeTexturesByUnit = new int[8]
			{
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			};
			m_activeTextureUnit = All.AllShaderBitsExt;
			m_program = -1;
			m_framebuffer = -1;
			m_clearColor = null;
			m_clearDepth = null;
			m_clearStencil = null;
			m_cullFace = All.False;
			m_frontFace = All.False;
			m_depthFunction = All.AllShaderBitsExt;
			m_colorMask = null;
			m_depthMask = null;
			m_polygonOffsetFactor = 0f;
			m_polygonOffsetUnits = 0f;
			m_blendColor = new Vector4(float.MinValue);
			m_blendEquation = All.AllShaderBitsExt;
			m_blendEquationColor = All.AllShaderBitsExt;
			m_blendEquationAlpha = All.AllShaderBitsExt;
			m_blendFuncSource = All.AllShaderBitsExt;
			m_blendFuncSourceColor = All.AllShaderBitsExt;
			m_blendFuncSourceAlpha = All.AllShaderBitsExt;
			m_blendFuncDestination = All.AllShaderBitsExt;
			m_blendFuncDestinationColor = All.AllShaderBitsExt;
			m_blendFuncDestinationAlpha = All.AllShaderBitsExt;
			m_enableDisableStates = new Dictionary<All, bool>();
			m_vertexAttribArray = new bool?[16];
			m_rasterizerState = null;
			m_depthStencilState = null;
			m_blendState = null;
			m_textureSamplerStates = new Dictionary<int, SamplerState>();
			m_lastShader = null;
			m_lastVertexDeclaration = null;
			m_lastVertexOffset = IntPtr.Zero;
			m_lastArrayBuffer = -1;
			m_viewport = null;
			m_scissorRectangle = null;
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static bool Enable(All state)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			if (!m_enableDisableStates.TryGetValue(state, out bool value) || !value)
			{
				GL.Enable(state);
				m_enableDisableStates[state] = true;
				return true;
			}
			return false;
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static bool Disable(All state)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			if (!m_enableDisableStates.TryGetValue(state, out bool value) | value)
			{
				GL.Disable(state);
				m_enableDisableStates[state] = false;
				return true;
			}
			return false;
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static bool IsEnabled(All state)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			if (!m_enableDisableStates.TryGetValue(state, out bool value))
			{
				value = GL.IsEnabled(state);
				m_enableDisableStates[state] = value;
			}
			return value;
		}

		public static void ClearColor(Vector4 color)
		{
			Vector4 value = color;
			Vector4? clearColor = m_clearColor;
			if (value != clearColor)
			{
				GL.ClearColor(color.X, color.Y, color.Z, color.W);
				m_clearColor = color;
			}
		}

		public static void ClearDepth(float depth)
		{
			if (depth != m_clearDepth)
			{
				GL.ClearDepth(depth);
				m_clearDepth = depth;
			}
		}

		public static void ClearStencil(int stencil)
		{
			if (stencil != m_clearStencil)
			{
				GL.ClearStencil(stencil);
				m_clearStencil = stencil;
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static void CullFace(All cullFace)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			if (cullFace != m_cullFace)
			{
				GL.CullFace(cullFace);
				m_cullFace = cullFace;
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static void FrontFace(All frontFace)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			if (frontFace != m_frontFace)
			{
				GL.FrontFace(frontFace);
				m_frontFace = frontFace;
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static void DepthFunc(All depthFunction)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			if (depthFunction != m_depthFunction)
			{
				GL.DepthFunc(depthFunction);
				m_depthFunction = depthFunction;
			}
		}

		public static void ColorMask(int colorMask)
		{
			colorMask &= 0xF;
			if (colorMask != m_colorMask)
			{
				GL.ColorMask((colorMask & 8) != 0, (colorMask & 4) != 0, (colorMask & 2) != 0, (colorMask & 1) != 0);
				m_colorMask = colorMask;
			}
		}

		public static bool DepthMask(bool depthMask)
		{
			if (depthMask != m_depthMask)
			{
				GL.DepthMask(depthMask);
				m_depthMask = depthMask;
				return true;
			}
			return false;
		}

		public static void PolygonOffset(float factor, float units)
		{
			if (factor != m_polygonOffsetFactor || units != m_polygonOffsetUnits)
			{
				GL.PolygonOffset(factor, units);
				m_polygonOffsetFactor = factor;
				m_polygonOffsetUnits = units;
			}
		}

		public static void BlendColor(Vector4 blendColor)
		{
			if (blendColor != m_blendColor)
			{
				GL.BlendColor(blendColor.X, blendColor.Y, blendColor.Z, blendColor.W);
				m_blendColor = blendColor;
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static void BlendEquation(All blendEquation)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			if (blendEquation != m_blendEquation)
			{
				GL.BlendEquation(blendEquation);
				m_blendEquation = blendEquation;
				m_blendEquationColor = All.AllShaderBitsExt;
				m_blendEquationAlpha = All.AllShaderBitsExt;
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static void BlendEquationSeparate(All blendEquationColor, All blendEquationAlpha)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			if (blendEquationColor != m_blendEquationColor || blendEquationAlpha != m_blendEquationAlpha)
			{
				GL.BlendEquationSeparate(blendEquationColor, blendEquationAlpha);
				m_blendEquationColor = blendEquationColor;
				m_blendEquationAlpha = blendEquationAlpha;
				m_blendEquation = All.AllShaderBitsExt;
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static void BlendFunc(All blendFuncSource, All blendFuncDestination)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			if (blendFuncSource != m_blendFuncSource || blendFuncDestination != m_blendFuncDestination)
			{
				GL.BlendFunc(blendFuncSource, blendFuncDestination);
				m_blendFuncSource = blendFuncSource;
				m_blendFuncDestination = blendFuncDestination;
				m_blendFuncSourceColor = All.AllShaderBitsExt;
				m_blendFuncSourceAlpha = All.AllShaderBitsExt;
				m_blendFuncDestinationColor = All.AllShaderBitsExt;
				m_blendFuncDestinationAlpha = All.AllShaderBitsExt;
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static void BlendFuncSeparate(All blendFuncSourceColor, All blendFuncDestinationColor, All blendFuncSourceAlpha, All blendFuncDestinationAlpha)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			if (blendFuncSourceColor != m_blendFuncSourceColor || blendFuncDestinationColor != m_blendFuncDestinationColor || blendFuncSourceAlpha != m_blendFuncSourceAlpha || blendFuncDestinationAlpha != m_blendFuncDestinationAlpha)
			{
				GL.BlendFuncSeparate(blendFuncSourceColor, blendFuncDestinationColor, blendFuncSourceAlpha, blendFuncDestinationAlpha);
				m_blendFuncSourceColor = blendFuncSourceColor;
				m_blendFuncSourceAlpha = blendFuncSourceAlpha;
				m_blendFuncDestinationColor = blendFuncDestinationColor;
				m_blendFuncDestinationAlpha = blendFuncDestinationAlpha;
				m_blendFuncSource = All.AllShaderBitsExt;
				m_blendFuncDestination = All.AllShaderBitsExt;
			}
		}

		public static void VertexAttribArray(int index, bool enable)
		{
			if (enable && (!m_vertexAttribArray[index].HasValue || !m_vertexAttribArray[index].Value))
			{
				GL.EnableVertexAttribArray(index);
				m_vertexAttribArray[index] = true;
			}
			else if (!enable && (!m_vertexAttribArray[index].HasValue || m_vertexAttribArray[index].Value))
			{
				GL.DisableVertexAttribArray(index);
				m_vertexAttribArray[index] = false;
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static void BindTexture(All target, int texture, bool forceBind)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			if (target == All.Texture2D)
			{
				if (forceBind || texture != m_texture2D)
				{
					GL.BindTexture(target, texture);
					m_texture2D = texture;
					if (m_activeTextureUnit >= All.False)
					{
						m_activeTexturesByUnit[(int)(m_activeTextureUnit - 33984)] = texture;
					}
				}
			}
			else
			{
				GL.BindTexture(target, texture);
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static void ActiveTexture(All textureUnit)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			if (textureUnit != m_activeTextureUnit)
			{
				GL.ActiveTexture(textureUnit);
				m_activeTextureUnit = textureUnit;
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static void BindBuffer(All target, int buffer)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			switch (target)
			{
			case All.ArrayBuffer:
				if (buffer != m_arrayBuffer)
				{
					GL.BindBuffer(target, buffer);
					m_arrayBuffer = buffer;
				}
				break;
			case All.ElementArrayBuffer:
				if (buffer != m_elementArrayBuffer)
				{
					GL.BindBuffer(target, buffer);
					m_elementArrayBuffer = buffer;
				}
				break;
			default:
				GL.BindBuffer(target, buffer);
				break;
			}
		}

		public static void BindFramebuffer(int framebuffer)
		{
			if (framebuffer != m_framebuffer)
			{
				GL.BindFramebuffer(All.Framebuffer, framebuffer);
				m_framebuffer = framebuffer;
			}
		}

		public static void UseProgram(int program)
		{
			if (program != m_program)
			{
				GL.UseProgram(program);
				m_program = program;
			}
		}

		public static void DeleteProgram(int program)
		{
			if (m_program == program)
			{
				m_program = -1;
			}
			GL.DeleteProgram(program);
		}

		public static void DeleteTexture(int texture)
		{
			if (m_texture2D == texture)
			{
				m_texture2D = -1;
			}
			for (int i = 0; i < m_activeTexturesByUnit.Length; i++)
			{
				if (m_activeTexturesByUnit[i] == texture)
				{
					m_activeTexturesByUnit[i] = -1;
				}
			}
			m_textureSamplerStates.Remove(texture);
			GL.DeleteTexture(texture);
		}

		public static void DeleteFramebuffer(int framebuffer)
		{
			if (m_framebuffer == framebuffer)
			{
				m_framebuffer = -1;
			}
			GL.DeleteFramebuffers(1, ref framebuffer);
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static void DeleteBuffer(All target, int buffer)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			if (target == All.ArrayBuffer)
			{
				if (m_arrayBuffer == buffer)
				{
					m_arrayBuffer = -1;
				}
				if (m_lastArrayBuffer == buffer)
				{
					m_lastArrayBuffer = -1;
				}
			}
			if (target == All.ElementArrayBuffer && m_elementArrayBuffer == buffer)
			{
				m_elementArrayBuffer = -1;
			}
			GL.DeleteBuffers(1, ref buffer);
		}

		public static void ApplyViewportScissor(Viewport viewport, Rectangle scissorRectangle, bool isScissorEnabled)
		{
			if (!m_viewport.HasValue || viewport.X != m_viewport.Value.X || viewport.Y != m_viewport.Value.Y || viewport.Width != m_viewport.Value.Width || viewport.Height != m_viewport.Value.Height)
			{
				int y = (Display.RenderTarget == null) ? (Display.BackbufferSize.Y - viewport.Y - viewport.Height) : viewport.Y;
				GL.Viewport(viewport.X, y, viewport.Width, viewport.Height);
			}
			if (!m_viewport.HasValue || viewport.MinDepth != m_viewport.Value.MinDepth || viewport.MaxDepth != m_viewport.Value.MaxDepth)
			{
				GL.DepthRange(viewport.MinDepth, viewport.MaxDepth);
			}
			m_viewport = viewport;
			if (!isScissorEnabled)
			{
				return;
			}
			if (m_scissorRectangle.HasValue)
			{
				Rectangle value = scissorRectangle;
				Rectangle? scissorRectangle2 = m_scissorRectangle;
				if (!(value != scissorRectangle2))
				{
					return;
				}
			}
			if (Display.RenderTarget == null)
			{
				scissorRectangle.Top = Display.BackbufferSize.Y - scissorRectangle.Top - scissorRectangle.Height;
			}
			GL.Scissor(scissorRectangle.Left, scissorRectangle.Top, scissorRectangle.Width, scissorRectangle.Height);
			m_scissorRectangle = scissorRectangle;
		}

		public static void ApplyRasterizerState(RasterizerState state)
		{
			if (state != m_rasterizerState)
			{
				m_rasterizerState = state;
				switch (state.CullMode)
				{
				case CullMode.None:
					Disable(All.CullFace);
					break;
				case CullMode.CullClockwise:
					Enable(All.CullFace);
					CullFace(All.Back);
					FrontFace((Display.RenderTarget != null) ? All.Cw : All.Ccw);
					break;
				case CullMode.CullCounterClockwise:
					Enable(All.CullFace);
					CullFace(All.Back);
					FrontFace((Display.RenderTarget != null) ? All.Ccw : All.Cw);
					break;
				}
				if (state.ScissorTestEnable)
				{
					Enable(All.ScissorTest);
				}
				else
				{
					Disable(All.ScissorTest);
				}
				if (state.DepthBias != 0f || state.SlopeScaleDepthBias != 0f)
				{
					Enable(All.PolygonOffsetFill);
					PolygonOffset(state.SlopeScaleDepthBias, state.DepthBias);
				}
				else
				{
					Disable(All.PolygonOffsetFill);
				}
			}
		}

		public static void ApplyDepthStencilState(DepthStencilState state)
		{
			if (state == m_depthStencilState)
			{
				return;
			}
			m_depthStencilState = state;
			if (state.DepthBufferTestEnable || state.DepthBufferWriteEnable)
			{
				Enable(All.DepthTest);
				if (state.DepthBufferTestEnable)
				{
					DepthFunc(TranslateCompareFunction(state.DepthBufferFunction));
				}
				else
				{
					DepthFunc(All.Always);
				}
				DepthMask(state.DepthBufferWriteEnable);
			}
			else
			{
				Disable(All.DepthTest);
			}
		}

		public static void ApplyBlendState(BlendState state)
		{
			if (state == m_blendState)
			{
				return;
			}
			m_blendState = state;
			if (state.ColorBlendFunction == BlendFunction.Add && state.ColorSourceBlend == Blend.One && state.ColorDestinationBlend == Blend.Zero && state.AlphaBlendFunction == BlendFunction.Add && state.AlphaSourceBlend == Blend.One && state.AlphaDestinationBlend == Blend.Zero)
			{
				Disable(All.Blend);
				return;
			}
			All all = TranslateBlendFunction(state.ColorBlendFunction);
			All all2 = TranslateBlendFunction(state.AlphaBlendFunction);
			All all3 = TranslateBlend(state.ColorSourceBlend);
			All all4 = TranslateBlend(state.ColorDestinationBlend);
			All all5 = TranslateBlend(state.AlphaSourceBlend);
			All all6 = TranslateBlend(state.AlphaDestinationBlend);
			if (all == all2 && all3 == all5 && all4 == all6)
			{
				BlendEquation(all);
				BlendFunc(all3, all4);
			}
			else
			{
				BlendEquationSeparate(all, all2);
				BlendFuncSeparate(all3, all4, all5, all6);
			}
			BlendColor(state.BlendFactor);
			Enable(All.Blend);
		}

		public static void ApplyRenderTarget(RenderTarget2D renderTarget)
		{
			if (renderTarget != null)
			{
				BindFramebuffer(renderTarget.m_frameBuffer);
			}
			else
			{
				BindFramebuffer(m_mainFramebuffer);
			}
		}

		public static void ApplyShaderAndBuffers(Shader shader, VertexDeclaration vertexDeclaration, IntPtr vertexOffset, int arrayBuffer, int? elementArrayBuffer)
		{
			shader.PrepareForDrawing();
			BindBuffer(All.ArrayBuffer, arrayBuffer);
			if (elementArrayBuffer.HasValue)
			{
				BindBuffer(All.ElementArrayBuffer, elementArrayBuffer.Value);
			}
			UseProgram(shader.m_program);
			if (shader != m_lastShader || vertexOffset != m_lastVertexOffset || arrayBuffer != m_lastArrayBuffer || vertexDeclaration.m_elements != m_lastVertexDeclaration.m_elements)
			{
				Shader.VertexAttributeData[] vertexAttribData = shader.GetVertexAttribData(vertexDeclaration);
				for (int i = 0; i < vertexAttribData.Length; i++)
				{
					if (vertexAttribData[i].Size != 0)
					{
						GL.VertexAttribPointer(i, vertexAttribData[i].Size, vertexAttribData[i].Type, vertexAttribData[i].Normalize, vertexDeclaration.VertexStride, vertexOffset + vertexAttribData[i].Offset);
						VertexAttribArray(i, enable: true);
					}
					else
					{
						VertexAttribArray(i, enable: false);
					}
				}
				m_lastShader = shader;
				m_lastVertexDeclaration = vertexDeclaration;
				m_lastVertexOffset = vertexOffset;
				m_lastArrayBuffer = arrayBuffer;
			}
			int num = 0;
			int num2 = 0;
			ShaderParameter shaderParameter;
			while (true)
			{
				if (num2 >= shader.m_parameters.Length)
				{
					return;
				}
				shaderParameter = shader.m_parameters[num2];
				if (shaderParameter.IsChanged)
				{
					switch (shaderParameter.Type)
					{
					case ShaderParameterType.Float:
						GL.Uniform1(shaderParameter.Location, shaderParameter.Count, shaderParameter.Value);
						shaderParameter.IsChanged = false;
						break;
					case ShaderParameterType.Vector2:
						GL.Uniform2(shaderParameter.Location, shaderParameter.Count, shaderParameter.Value);
						shaderParameter.IsChanged = false;
						break;
					case ShaderParameterType.Vector3:
						GL.Uniform3(shaderParameter.Location, shaderParameter.Count, shaderParameter.Value);
						shaderParameter.IsChanged = false;
						break;
					case ShaderParameterType.Vector4:
						GL.Uniform4(shaderParameter.Location, shaderParameter.Count, shaderParameter.Value);
						shaderParameter.IsChanged = false;
						break;
					case ShaderParameterType.Matrix:
						GL.UniformMatrix4(shaderParameter.Location, shaderParameter.Count, transpose: false, shaderParameter.Value);
						shaderParameter.IsChanged = false;
						break;
					default:
						throw new InvalidOperationException("Unsupported shader parameter type.");
					case ShaderParameterType.Texture2D:
					case ShaderParameterType.Sampler2D:
						break;
					}
				}
				if (shaderParameter.Type == ShaderParameterType.Texture2D)
				{
					if (num >= 8)
					{
						throw new InvalidOperationException("Too many simultaneous textures.");
					}
					ActiveTexture((All)(33984 + num));
					if (shaderParameter.IsChanged)
					{
						GL.Uniform1(shaderParameter.Location, num);
					}
					ShaderParameter obj = shader.m_parameters[num2 + 1];
					Texture2D texture2D = (Texture2D)shaderParameter.Resource;
					SamplerState samplerState = (SamplerState)obj.Resource;
					if (texture2D != null)
					{
						if (samplerState == null)
						{
							break;
						}
						if (m_activeTexturesByUnit[num] != texture2D.m_texture)
						{
							BindTexture(All.Texture2D, texture2D.m_texture, forceBind: true);
						}
						if (!m_textureSamplerStates.TryGetValue(texture2D.m_texture, out SamplerState value) || value != samplerState)
						{
							BindTexture(All.Texture2D, texture2D.m_texture, forceBind: false);
							if (GL_EXT_texture_filter_anisotropic)
							{
								GL.TexParameter(All.Texture2D, All.TextureMaxAnisotropyExt, (samplerState.FilterMode == TextureFilterMode.Anisotropic) ? ((float)samplerState.MaxAnisotropy) : 1f);
							}
							GL.TexParameter(All.Texture2D, All.TextureMinFilter, (int)TranslateTextureFilterModeMin(samplerState.FilterMode, texture2D.MipLevelsCount > 1));
							GL.TexParameter(All.Texture2D, All.TextureMagFilter, (int)TranslateTextureFilterModeMag(samplerState.FilterMode));
							GL.TexParameter(All.Texture2D, All.TextureWrapS, (int)TranslateTextureAddressMode(samplerState.AddressModeU));
							GL.TexParameter(All.Texture2D, All.TextureWrapT, (int)TranslateTextureAddressMode(samplerState.AddressModeV));
							m_textureSamplerStates[texture2D.m_texture] = samplerState;
						}
					}
					else if (m_activeTexturesByUnit[num] != 0)
					{
						BindTexture(All.Texture2D, 0, forceBind: true);
					}
					num++;
					shaderParameter.IsChanged = false;
				}
				num2++;
			}
			throw new InvalidOperationException($"Associated SamplerState is not set for texture \"{shaderParameter.Name}\".");
		}

		public static void Clear(RenderTarget2D renderTarget, Vector4? color, float? depth, int? stencil)
		{
			All all = All.False;
			if (color.HasValue)
			{
				all |= All.ColorBufferBit;
				ClearColor(color.Value);
				ColorMask(15);
			}
			if (depth.HasValue)
			{
				all |= All.DepthBufferBit;
				ClearDepth(depth.Value);
				if (DepthMask(depthMask: true))
				{
					m_depthStencilState = null;
				}
			}
			if (stencil.HasValue)
			{
				all |= All.DepthBufferBit2Qcom;
				ClearStencil(stencil.Value);
			}
			if (all != 0)
			{
				ApplyRenderTarget(renderTarget);
				if (Disable(All.ScissorTest))
				{
					m_rasterizerState = null;
				}
				GL.Clear((ClearBufferMask)all);
			}
		}

		public static void HandleContextLost()
		{
			try
			{
				Log.Information("Device lost");
				Display.HandleDeviceLost();
				GC.Collect();
				InitializeCache();
				Display.Resize();
				Display.HandleDeviceReset();
				Log.Information("Device reset");
			}
			catch (Exception ex)
			{
				Log.Error("Failed to recreate graphics resources. Reason: {0}", ex.Message);
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static void TranslateVertexElementFormat(VertexElementFormat vertexElementFormat, out All type, out bool normalize)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			switch (vertexElementFormat)
			{
			case VertexElementFormat.Single:
				type = All.Float;
				normalize = false;
				break;
			case VertexElementFormat.Vector2:
				type = All.Float;
				normalize = false;
				break;
			case VertexElementFormat.Vector3:
				type = All.Float;
				normalize = false;
				break;
			case VertexElementFormat.Vector4:
				type = All.Float;
				normalize = false;
				break;
			case VertexElementFormat.Byte4:
				type = All.UnsignedByte;
				normalize = false;
				break;
			case VertexElementFormat.NormalizedByte4:
				type = All.UnsignedByte;
				normalize = true;
				break;
			case VertexElementFormat.Short2:
				type = All.Short;
				normalize = false;
				break;
			case VertexElementFormat.NormalizedShort2:
				type = All.Short;
				normalize = true;
				break;
			case VertexElementFormat.Short4:
				type = All.Short;
				normalize = false;
				break;
			case VertexElementFormat.NormalizedShort4:
				type = All.Short;
				normalize = true;
				break;
			default:
				throw new InvalidOperationException("Unsupported vertex element format.");
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All TranslateIndexFormat(IndexFormat indexFormat)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			if (indexFormat == IndexFormat.SixteenBits)
			{
				return All.UnsignedShort;
			}
			throw new InvalidOperationException("Unsupported index format.");
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“ActiveUniformType”(是否缺少 using 指令或程序集引用?)
		public static ShaderParameterType TranslateActiveUniformType(ActiveUniformType type)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“ActiveUniformType”(是否缺少 using 指令或程序集引用?)
		{
			switch (type)
			{
			case ActiveUniformType.Float:
				return ShaderParameterType.Float;
			case ActiveUniformType.FloatVec2:
				return ShaderParameterType.Vector2;
			case ActiveUniformType.FloatVec3:
				return ShaderParameterType.Vector3;
			case ActiveUniformType.FloatVec4:
				return ShaderParameterType.Vector4;
			case ActiveUniformType.FloatMat4:
				return ShaderParameterType.Matrix;
			case ActiveUniformType.Sampler2D:
				return ShaderParameterType.Texture2D;
			default:
				throw new InvalidOperationException("Unsupported shader parameter type.");
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All TranslatePrimitiveType(PrimitiveType primitiveType)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			switch (primitiveType)
			{
			case PrimitiveType.LineList:
				return All.ColorBufferBit0Qcom;
			case PrimitiveType.LineStrip:
				return All.LineStrip;
			case PrimitiveType.TriangleList:
				return All.ColorBufferBit2Qcom;
			case PrimitiveType.TriangleStrip:
				return All.TriangleStrip;
			default:
				throw new InvalidOperationException("Unsupported primitive type.");
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All TranslateTextureFilterModeMin(TextureFilterMode filterMode, bool isMipmapped)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			switch (filterMode)
			{
			case TextureFilterMode.Point:
				if (!isMipmapped)
				{
					return All.Nearest;
				}
				return All.NearestMipmapNearest;
			case TextureFilterMode.Linear:
				if (!isMipmapped)
				{
					return All.Linear;
				}
				return All.LinearMipmapLinear;
			case TextureFilterMode.Anisotropic:
				if (!isMipmapped)
				{
					return All.Linear;
				}
				return All.LinearMipmapLinear;
			case TextureFilterMode.PointMipLinear:
				if (!isMipmapped)
				{
					return All.Nearest;
				}
				return All.NearestMipmapLinear;
			case TextureFilterMode.LinearMipPoint:
				if (!isMipmapped)
				{
					return All.Linear;
				}
				return All.LinearMipmapNearest;
			case TextureFilterMode.MinPointMagLinearMipPoint:
				if (!isMipmapped)
				{
					return All.Nearest;
				}
				return All.NearestMipmapNearest;
			case TextureFilterMode.MinPointMagLinearMipLinear:
				if (!isMipmapped)
				{
					return All.Nearest;
				}
				return All.NearestMipmapLinear;
			case TextureFilterMode.MinLinearMagPointMipPoint:
				if (!isMipmapped)
				{
					return All.Linear;
				}
				return All.LinearMipmapNearest;
			case TextureFilterMode.MinLinearMagPointMipLinear:
				if (!isMipmapped)
				{
					return All.Linear;
				}
				return All.LinearMipmapLinear;
			default:
				throw new InvalidOperationException("Unsupported texture filter mode.");
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All TranslateTextureFilterModeMag(TextureFilterMode filterMode)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			switch (filterMode)
			{
			case TextureFilterMode.Point:
				return All.Nearest;
			case TextureFilterMode.Linear:
				return All.Linear;
			case TextureFilterMode.Anisotropic:
				return All.Linear;
			case TextureFilterMode.PointMipLinear:
				return All.Nearest;
			case TextureFilterMode.LinearMipPoint:
				return All.Nearest;
			case TextureFilterMode.MinPointMagLinearMipPoint:
				return All.Linear;
			case TextureFilterMode.MinPointMagLinearMipLinear:
				return All.Linear;
			case TextureFilterMode.MinLinearMagPointMipPoint:
				return All.Nearest;
			case TextureFilterMode.MinLinearMagPointMipLinear:
				return All.Nearest;
			default:
				throw new InvalidOperationException("Unsupported texture filter mode.");
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All TranslateTextureAddressMode(TextureAddressMode addressMode)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			switch (addressMode)
			{
			case TextureAddressMode.Clamp:
				return All.ClampToEdge;
			case TextureAddressMode.Wrap:
				return All.Repeat;
			default:
				throw new InvalidOperationException("Unsupported texture address mode.");
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All TranslateCompareFunction(CompareFunction compareFunction)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			switch (compareFunction)
			{
			case CompareFunction.Always:
				return All.Always;
			case CompareFunction.Equal:
				return All.Equal;
			case CompareFunction.Greater:
				return All.Greater;
			case CompareFunction.GreaterEqual:
				return All.Gequal;
			case CompareFunction.Less:
				return All.Less;
			case CompareFunction.LessEqual:
				return All.Lequal;
			case CompareFunction.Never:
				return All.DepthBufferBit1Qcom;
			case CompareFunction.NotEqual:
				return All.Notequal;
			default:
				throw new InvalidOperationException("Unsupported texture address mode.");
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All TranslateBlendFunction(BlendFunction blendFunction)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			switch (blendFunction)
			{
			case BlendFunction.Add:
				return All.FuncAdd;
			case BlendFunction.Subtract:
				return All.FuncSubtract;
			case BlendFunction.ReverseSubtract:
				return All.FuncReverseSubtract;
			default:
				throw new InvalidOperationException("Unsupported blend function.");
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All TranslateBlend(Blend blend)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			switch (blend)
			{
			case Blend.Zero:
				return All.False;
			case Blend.One:
				return All.ColorBufferBit0Qcom;
			case Blend.SourceColor:
				return All.SrcColor;
			case Blend.InverseSourceColor:
				return All.OneMinusSrcColor;
			case Blend.DestinationColor:
				return All.DstColor;
			case Blend.InverseDestinationColor:
				return All.OneMinusDstColor;
			case Blend.SourceAlpha:
				return All.SrcAlpha;
			case Blend.InverseSourceAlpha:
				return All.OneMinusSrcAlpha;
			case Blend.DestinationAlpha:
				return All.DstAlpha;
			case Blend.InverseDestinationAlpha:
				return All.OneMinusDstAlpha;
			case Blend.BlendFactor:
				return All.ConstantColor;
			case Blend.InverseBlendFactor:
				return All.OneMinusConstantColor;
			case Blend.SourceAlphaSaturation:
				return All.SrcAlphaSaturate;
			default:
				throw new InvalidOperationException("Unsupported blend.");
			}
		}

#pragma warning disable CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		public static All TranslateDepthFormat(DepthFormat depthFormat)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“All”(是否缺少 using 指令或程序集引用?)
		{
			switch (depthFormat)
			{
			case DepthFormat.Depth16:
				return All.DepthComponent16;
			case DepthFormat.Depth24Stencil8:
				if (GL_OES_packed_depth_stencil)
				{
					return All.Depth24Stencil8Oes;
				}
				return All.DepthComponent16;
			default:
				throw new InvalidOperationException("Unsupported DepthFormat.");
			}
		}

		[Conditional("DEBUG")]
		public static void CheckGLError()
		{
		}
	}
}
