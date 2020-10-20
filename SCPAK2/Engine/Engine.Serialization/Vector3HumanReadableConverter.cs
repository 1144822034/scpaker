using System;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(Vector3))]
	internal class Vector3HumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			Vector3 vector = (Vector3)value;
			return HumanReadableConverter.ValuesListToString<float>(',', vector.X, vector.Y, vector.Z);
		}

		public object ConvertFromString(Type type, string data)
		{
			float[] array = HumanReadableConverter.ValuesListFromString<float>(',', data);
			if (array.Length == 3)
			{
				return new Vector3(array[0], array[1], array[2]);
			}
			throw new Exception();
		}
	}
}
