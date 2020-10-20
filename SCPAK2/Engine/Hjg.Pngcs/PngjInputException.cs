using System;

namespace Hjg.Pngcs
{
	internal class PngjInputException : PngjException
	{
		public const long serialVersionUID = 1L;

		public PngjInputException(string message, Exception cause)
			: base(message, cause)
		{
		}

		public PngjInputException(string message)
			: base(message)
		{
		}

		public PngjInputException(Exception cause)
			: base(cause)
		{
		}
	}
}
