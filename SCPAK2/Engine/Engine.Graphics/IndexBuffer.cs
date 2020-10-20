#pragma warning disable CS0246 // 未能找到类型或命名空间名“OpenTK”(是否缺少 using 指令或程序集引用?)
using OpenTK.Graphics.ES20;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“OpenTK”(是否缺少 using 指令或程序集引用?)
using System;
using System.Runtime.InteropServices;

namespace Engine.Graphics
{
	public sealed class IndexBuffer : GraphicsResource
	{
		internal int m_buffer;

		public IndexFormat IndexFormat
		{
			get;
			set;
		}

		public int IndicesCount
		{
			get;
			set;
		}

		public object Tag
		{
			get;
			set;
		}

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

		public override int GetGpuMemoryUsage()
		{
			return IndicesCount * IndexFormat.GetSize();
		}

		public void InitializeIndexBuffer(IndexFormat indexFormat, int indicesCount)
		{
			if (indicesCount <= 0)
			{
				throw new ArgumentException("Indices count must be greater than 0.");
			}
			IndexFormat = indexFormat;
			IndicesCount = indicesCount;
		}

		public void VerifyParametersSetData<T>(T[] source, int sourceStartIndex, int sourceCount, int targetStartIndex = 0) where T : struct
		{
			VerifyNotDisposed();
			int num = Utilities.SizeOf<T>();
			int size = IndexFormat.GetSize();
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (sourceStartIndex < 0 || sourceCount < 0 || sourceStartIndex + sourceCount > source.Length)
			{
				throw new ArgumentException("Range is out of source bounds.");
			}
			if (targetStartIndex < 0 || targetStartIndex * size + sourceCount * num > IndicesCount * size)
			{
				throw new ArgumentException("Range is out of target bounds.");
			}
		}

		public IndexBuffer(IndexFormat indexFormat, int indicesCount)
		{
			InitializeIndexBuffer(indexFormat, indicesCount);
			AllocateBuffer();
		}

		public override void Dispose()
		{
			base.Dispose();
			DeleteBuffer();
		}

		public void SetData<T>(T[] source, int sourceStartIndex, int sourceCount, int targetStartIndex = 0) where T : struct
		{
			VerifyParametersSetData(source, sourceStartIndex, sourceCount, targetStartIndex);
			GCHandle gCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
			try
			{
				int num = Utilities.SizeOf<T>();
				int size = IndexFormat.GetSize();
				GLWrapper.BindBuffer(All.ElementArrayBuffer, m_buffer);
				GL.BufferSubData(All.ElementArrayBuffer, new IntPtr(targetStartIndex * size), new IntPtr(num * sourceCount), gCHandle.AddrOfPinnedObject() + sourceStartIndex * num);
			}
			finally
			{
				gCHandle.Free();
			}
		}

		internal override void HandleDeviceLost()
		{
			DeleteBuffer();
		}

		internal override void HandleDeviceReset()
		{
			AllocateBuffer();
		}

		public void AllocateBuffer()
		{
			GL.GenBuffers(1, out m_buffer);
			GLWrapper.BindBuffer(All.ElementArrayBuffer, m_buffer);
			GL.BufferData(All.ElementArrayBuffer, new IntPtr(IndexFormat.GetSize() * IndicesCount), IntPtr.Zero, All.StaticDraw);
		}

		public void DeleteBuffer()
		{
			if (m_buffer != 0)
			{
				GLWrapper.DeleteBuffer(All.ElementArrayBuffer, m_buffer);
				m_buffer = 0;
			}
		}
	}
}
