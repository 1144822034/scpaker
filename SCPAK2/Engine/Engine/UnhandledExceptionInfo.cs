using System;

namespace Engine
{
	public class UnhandledExceptionInfo
	{
		public readonly Exception Exception;

		public bool IsHandled;

		internal UnhandledExceptionInfo(Exception e)
		{
			Exception = e;
		}
	}
}
