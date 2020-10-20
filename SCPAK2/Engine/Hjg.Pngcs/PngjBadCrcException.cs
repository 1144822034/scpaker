using System;

namespace Hjg.Pngcs
{
	internal class PngjBadCrcException : PngjException
	{
		public const long serialVersionUID = 1L;

		public PngjBadCrcException(string message, Exception cause)
			: base(message, cause)
		{
		}

		public PngjBadCrcException(string message)
			: base(message)
		{
		}

		public PngjBadCrcException(Exception cause)
			: base(cause)
		{
		}
	}
}
