using System;

namespace Engine.Serialization
{
	[HumanReadableConverter(typeof(BoundingSphere))]
	internal class BoundingSphereHumanReadableConverter : IHumanReadableConverter
	{
		public string ConvertToString(object value)
		{
			BoundingSphere boundingSphere = (BoundingSphere)value;
			return HumanReadableConverter.ValuesListToString<float>(',', boundingSphere.Center.X, boundingSphere.Center.Y, boundingSphere.Center.Z, boundingSphere.Radius);
		}

		public object ConvertFromString(Type type, string data)
		{
			float[] array = HumanReadableConverter.ValuesListFromString<float>(',', data);
			if (array.Length == 4)
			{
				return new BoundingSphere(new Vector3(array[0], array[1], array[2]), array[3]);
			}
			throw new Exception();
		}
	}
}
