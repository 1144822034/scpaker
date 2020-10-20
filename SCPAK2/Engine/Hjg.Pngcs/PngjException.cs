using System;

namespace Hjg.Pngcs
{
	internal class PngjException : Exception
	{
		public const long serialVersionUID = 1L;

		public PngjException(string message, Exception cause)
			: base(message, cause)
		{
		}

		public PngjException(string message)
			: base(message)
		{
		}

		public PngjException(Exception cause)
			: base(cause.Message, cause)
		{
		}
	}
}
