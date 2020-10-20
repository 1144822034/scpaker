using System;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(Point2))]
	internal class Point2HumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			Point2 point = (Point2)value;
			return HumanReadableConverter.ValuesListToString<int>(',', point.X, point.Y);
		}

		public object ConvertFromString(Type type, string data)
		{
			int[] array = HumanReadableConverter.ValuesListFromString<int>(',', data);
			if (array.Length == 2)
			{
				return new Point2(array[0], array[1]);
			}
			throw new Exception();
		}
	}
}
