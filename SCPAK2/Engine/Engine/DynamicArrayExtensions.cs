using System.Collections.Generic;

namespace Engine
{
	public static class DynamicArrayExtensions
	{
		public static DynamicArray<T> ToDynamicArray<T>(this IEnumerable<T> source)
		{
			return new DynamicArray<T>(source);
		}
	}
}
