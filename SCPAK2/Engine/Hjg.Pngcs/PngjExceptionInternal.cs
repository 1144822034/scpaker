using System;

namespace Hjg.Pngcs
{
	internal class PngjExceptionInternal : Exception
	{
		public const long serialVersionUID = 1L;

		public PngjExceptionInternal()
		{
		}

		public PngjExceptionInternal(string message, Exception cause)
			: base(message, cause)
		{
		}

		public PngjExceptionInternal(string message)
			: base(message)
		{
		}

		public PngjExceptionInternal(Exception cause)
			: base(cause.Message, cause)
		{
		}
	}
}
