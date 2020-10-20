using System;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(BoundingCircle))]
	internal class BoundingCircleHumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			BoundingCircle boundingCircle = (BoundingCircle)value;
			return HumanReadableConverter.ValuesListToString<float>(',', boundingCircle.Center.X, boundingCircle.Center.Y, boundingCircle.Radius);
		}

		public object ConvertFromString(Type type, string data)
		{
			float[] array = HumanReadableConverter.ValuesListFromString<float>(',', data);
			if (array.Length == 3)
			{
				return new BoundingCircle(new Vector2(array[0], array[1]), array[2]);
			}
			throw new Exception();
		}
	}
}
