namespace Engine.Graphics
{
	public abstract class BaseBatch
	{
		public int Layer
		{
			get;
			internal set;
		}

		public DepthStencilState DepthStencilState
		{
			get;
			internal set;
		}

		public RasterizerState RasterizerState
		{
			get;
			internal set;
		}

		public BlendState BlendState
		{
			get;
			internal set;
		}

		internal BaseBatch()
		{
		}

		public abstract bool IsEmpty();

		public abstract void Clear();

		public abstract void Flush(Matrix matrix, bool clearAfterFlush = true);
	}
}
