namespace Engine.Graphics
{
	public sealed class RasterizerState : LockOnFirstUse
	{
		public CullMode m_cullMode = CullMode.CullCounterClockwise;

		public bool m_scissorTestEnable;

		public float m_depthBias;

		public float m_slopeScaleDepthBias;

		public static readonly RasterizerState CullNone = new RasterizerState
		{
			CullMode = CullMode.None,
			IsLocked = true
		};

		public static readonly RasterizerState CullNoneScissor = new RasterizerState
		{
			CullMode = CullMode.None,
			ScissorTestEnable = true,
			IsLocked = true
		};

		public static readonly RasterizerState CullClockwise = new RasterizerState
		{
			CullMode = CullMode.CullClockwise,
			IsLocked = true
		};

		public static readonly RasterizerState CullClockwiseScissor = new RasterizerState
		{
			CullMode = CullMode.CullClockwise,
			ScissorTestEnable = true,
			IsLocked = true
		};

		public static readonly RasterizerState CullCounterClockwise = new RasterizerState
		{
			CullMode = CullMode.CullCounterClockwise,
			IsLocked = true
		};

		public static readonly RasterizerState CullCounterClockwiseScissor = new RasterizerState
		{
			CullMode = CullMode.CullCounterClockwise,
			ScissorTestEnable = true,
			IsLocked = true
		};

		public CullMode CullMode
		{
			get
			{
				return m_cullMode;
			}
			set
			{
				ThrowIfLocked();
				m_cullMode = value;
			}
		}

		public bool ScissorTestEnable
		{
			get
			{
				return m_scissorTestEnable;
			}
			set
			{
				ThrowIfLocked();
				m_scissorTestEnable = value;
			}
		}

		public float DepthBias
		{
			get
			{
				return m_depthBias;
			}
			set
			{
				ThrowIfLocked();
				m_depthBias = value;
			}
		}

		public float SlopeScaleDepthBias
		{
			get
			{
				return m_slopeScaleDepthBias;
			}
			set
			{
				ThrowIfLocked();
				m_slopeScaleDepthBias = value;
			}
		}
	}
}
