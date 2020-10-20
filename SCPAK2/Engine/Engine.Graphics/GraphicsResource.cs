using System;
using System.Collections.Generic;

namespace Engine.Graphics
{
	public abstract class GraphicsResource : IDisposable
	{
		internal static HashSet<GraphicsResource> m_resources = new HashSet<GraphicsResource>();

		internal bool m_isDisposed;

		internal GraphicsResource()
		{
			m_resources.Add(this);
		}

		~GraphicsResource()
		{
			Dispatcher.Dispatch(delegate
			{
				Dispose();
			});
		}

		public virtual void Dispose()
		{
			m_isDisposed = true;
			m_resources.Remove(this);
		}

		public abstract int GetGpuMemoryUsage();

		internal abstract void HandleDeviceLost();

		internal abstract void HandleDeviceReset();

		internal void VerifyNotDisposed()
		{
			if (m_isDisposed)
			{
				throw new InvalidOperationException("GraphicsResource is disposed.");
			}
		}
	}
}
