using System;
using System.Globalization;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(int))]
	internal class Int32HumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			return ((int)value).ToString(CultureInfo.InvariantCulture);
		}

		public object ConvertFromString(Type type, string data)
		{
			return int.Parse(data, CultureInfo.InvariantCulture);
		}
	}
}
