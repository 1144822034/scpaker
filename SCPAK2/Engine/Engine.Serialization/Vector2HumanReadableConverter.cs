using System;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(Vector2))]
	internal class Vector2HumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			Vector2 vector = (Vector2)value;
			return HumanReadableConverter.ValuesListToString<float>(',', vector.X, vector.Y);
		}

		public object ConvertFromString(Type type, string data)
		{
			float[] array = HumanReadableConverter.ValuesListFromString<float>(',', data);
			if (array.Length == 2)
			{
				return new Vector2(array[0], array[1]);
			}
			throw new Exception();
		}
	}
}
