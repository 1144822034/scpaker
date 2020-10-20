namespace Engine.Graphics
{
	public sealed class DepthStencilState : LockOnFirstUse
	{
		public bool m_depthBufferTestEnable = true;

		public bool m_depthBufferWriteEnable = true;

		public CompareFunction m_depthBufferFunction = CompareFunction.LessEqual;

		public static readonly DepthStencilState Default = new DepthStencilState
		{
			IsLocked = true
		};

		public static readonly DepthStencilState DepthRead = new DepthStencilState
		{
			DepthBufferWriteEnable = false,
			IsLocked = true
		};

		public static readonly DepthStencilState DepthWrite = new DepthStencilState
		{
			DepthBufferTestEnable = false,
			IsLocked = true
		};

		public static readonly DepthStencilState None = new DepthStencilState
		{
			DepthBufferTestEnable = false,
			DepthBufferWriteEnable = false,
			IsLocked = true
		};

		public bool DepthBufferTestEnable
		{
			get
			{
				return m_depthBufferTestEnable;
			}
			set
			{
				ThrowIfLocked();
				m_depthBufferTestEnable = value;
			}
		}

		public bool DepthBufferWriteEnable
		{
			get
			{
				return m_depthBufferWriteEnable;
			}
			set
			{
				ThrowIfLocked();
				m_depthBufferWriteEnable = value;
			}
		}

		public CompareFunction DepthBufferFunction
		{
			get
			{
				return m_depthBufferFunction;
			}
			set
			{
				ThrowIfLocked();
				m_depthBufferFunction = value;
			}
		}
	}
}
