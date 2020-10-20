using System;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(bool))]
	internal class BooleanHumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			if (!(bool)value)
			{
				return "False";
			}
			return "True";
		}

		public object ConvertFromString(Type type, string data)
		{
			if (string.Equals(data, "True", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (string.Equals(data, "False", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			throw new Exception();
		}
	}
}
