using System;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(string))]
	internal class StringHumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			return (string)value;
		}

		public object ConvertFromString(Type type, string data)
		{
			return data;
		}
	}
}
