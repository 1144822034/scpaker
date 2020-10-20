using System;
using System.Globalization;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(byte))]
	internal class ByteHumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			return ((byte)value).ToString(CultureInfo.InvariantCulture);
		}

		public object ConvertFromString(Type type, string data)
		{
			return byte.Parse(data, CultureInfo.InvariantCulture);
		}
	}
}
