namespace Engine.Graphics
{
	public sealed class SamplerState : LockOnFirstUse
	{
		public TextureFilterMode m_filterMode;

		public TextureAddressMode m_addressModeU;

		public TextureAddressMode m_addressModeV;

		public int m_maxAnisotropy = 1;

		public float m_minLod = -1000f;

		public float m_maxLod = 1000f;

		public float m_mipLodBias;

		public static SamplerState PointClamp = new SamplerState
		{
			FilterMode = TextureFilterMode.Point,
			AddressModeU = TextureAddressMode.Clamp,
			AddressModeV = TextureAddressMode.Clamp,
			IsLocked = true
		};

		public static SamplerState PointWrap = new SamplerState
		{
			FilterMode = TextureFilterMode.Point,
			AddressModeU = TextureAddressMode.Wrap,
			AddressModeV = TextureAddressMode.Wrap,
			IsLocked = true
		};

		public static SamplerState LinearClamp = new SamplerState
		{
			FilterMode = TextureFilterMode.Linear,
			AddressModeU = TextureAddressMode.Clamp,
			AddressModeV = TextureAddressMode.Clamp,
			IsLocked = true
		};

		public static SamplerState LinearWrap = new SamplerState
		{
			FilterMode = TextureFilterMode.Linear,
			AddressModeU = TextureAddressMode.Wrap,
			AddressModeV = TextureAddressMode.Wrap,
			IsLocked = true
		};

		public static SamplerState AnisotropicClamp = new SamplerState
		{
			FilterMode = TextureFilterMode.Anisotropic,
			AddressModeU = TextureAddressMode.Clamp,
			AddressModeV = TextureAddressMode.Clamp,
			MaxAnisotropy = 16,
			IsLocked = true
		};

		public static SamplerState AnisotropicWrap = new SamplerState
		{
			FilterMode = TextureFilterMode.Anisotropic,
			AddressModeU = TextureAddressMode.Wrap,
			AddressModeV = TextureAddressMode.Wrap,
			MaxAnisotropy = 16,
			IsLocked = true
		};

		public TextureFilterMode FilterMode
		{
			get
			{
				return m_filterMode;
			}
			set
			{
				ThrowIfLocked();
				m_filterMode = value;
			}
		}

		public TextureAddressMode AddressModeU
		{
			get
			{
				return m_addressModeU;
			}
			set
			{
				ThrowIfLocked();
				m_addressModeU = value;
			}
		}

		public TextureAddressMode AddressModeV
		{
			get
			{
				return m_addressModeV;
			}
			set
			{
				ThrowIfLocked();
				m_addressModeV = value;
			}
		}

		public int MaxAnisotropy
		{
			get
			{
				return m_maxAnisotropy;
			}
			set
			{
				ThrowIfLocked();
				m_maxAnisotropy = MathUtils.Max(value, 1);
			}
		}

		public float MinLod
		{
			get
			{
				return m_minLod;
			}
			set
			{
				ThrowIfLocked();
				m_minLod = value;
			}
		}

		public float MaxLod
		{
			get
			{
				return m_maxLod;
			}
			set
			{
				ThrowIfLocked();
				m_maxLod = value;
			}
		}

		public float MipLodBias
		{
			get
			{
				return m_mipLodBias;
			}
			set
			{
				ThrowIfLocked();
				m_mipLodBias = value;
			}
		}
	}
}
