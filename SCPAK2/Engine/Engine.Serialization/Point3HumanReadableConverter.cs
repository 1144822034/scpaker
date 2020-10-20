using System;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(Point3))]
	internal class Point3HumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			Point3 point = (Point3)value;
			return HumanReadableConverter.ValuesListToString<int>(',', point.X, point.Y, point.Z);
		}

		public object ConvertFromString(Type type, string data)
		{
			int[] array = HumanReadableConverter.ValuesListFromString<int>(',', data);
			if (array.Length == 3)
			{
				return new Point3(array[0], array[1], array[2]);
			}
			throw new Exception();
		}
	}
}
