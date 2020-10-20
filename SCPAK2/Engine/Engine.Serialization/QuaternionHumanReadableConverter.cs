using System;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(Quaternion))]
	internal class QuaternionHumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			Quaternion quaternion = (Quaternion)value;
			return HumanReadableConverter.ValuesListToString<float>(',', quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
		}

		public object ConvertFromString(Type type, string data)
		{
			float[] array = HumanReadableConverter.ValuesListFromString<float>(',', data);
			if (array.Length == 4)
			{
				return new Quaternion(array[0], array[1], array[2], array[3]);
			}
			throw new Exception();
		}
	}
}
