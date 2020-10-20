using System;
using System.Globalization;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(float))]
	internal class SingleHumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			return ((float)value).ToString("R", CultureInfo.InvariantCulture);
		}

		public object ConvertFromString(Type type, string data)
		{
			return float.Parse(data, CultureInfo.InvariantCulture);
		}
	}
}
