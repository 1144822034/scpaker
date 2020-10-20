using System;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(Plane))]
	internal class PlaneHumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			Plane plane = (Plane)value;
			return HumanReadableConverter.ValuesListToString<float>(',', plane.Normal.X, plane.Normal.Y, plane.Normal.Z, plane.D);
		}

		public object ConvertFromString(Type type, string data)
		{
			float[] array = HumanReadableConverter.ValuesListFromString<float>(',', data);
			if (array.Length == 4)
			{
				return new Plane(array[0], array[1], array[2], array[3]);
			}
			throw new Exception();
		}
	}
}
