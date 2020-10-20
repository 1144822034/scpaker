using System;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(BoundingRectangle))]
	internal class BoundingRectangleHumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			BoundingRectangle boundingRectangle = (BoundingRectangle)value;
			return HumanReadableConverter.ValuesListToString<float>(',', boundingRectangle.Min.X, boundingRectangle.Min.Y, boundingRectangle.Max.X, boundingRectangle.Max.Y);
		}

		public object ConvertFromString(Type type, string data)
		{
			float[] array = HumanReadableConverter.ValuesListFromString<float>(',', data);
			if (array.Length == 4)
			{
				return new BoundingRectangle(array[0], array[1], array[2], array[3]);
			}
			throw new Exception();
		}
	}
}
