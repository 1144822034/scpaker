using System;

namespace Hjg.Pngcs
{
	internal class PngjUnsupportedException : Exception
	{
		public const long serialVersionUID = 1L;

		public PngjUnsupportedException()
		{
		}

		public PngjUnsupportedException(string message, Exception cause)
			: base(message, cause)
		{
		}

		public PngjUnsupportedException(string message)
			: base(message)
		{
		}

		public PngjUnsupportedException(Exception cause)
			: base(cause.Message, cause)
		{
		}
	}
}
