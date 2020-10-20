using System;

namespace Engine.Serialization
{
	[AttributeUsage(AttributeTargets.Class)]
	public class HumanReadableConverterAttribute : Attribute
	{
		public Type Type;

		public HumanReadableConverterAttribute(Type type)
		{
			Type = type;
		}
	}
}
