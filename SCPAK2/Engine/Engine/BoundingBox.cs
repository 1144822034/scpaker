using System;
using System.Collections.Generic;

namespace Engine
{
	public struct BoundingBox : IEquatable<BoundingBox>
	{
		public Vector3 Min;

		public Vector3 Max;

		public BoundingBox(float x1, float y1, float z1, float x2, float y2, float z2)
		{
			Min = new Vector3(x1, y1, z1);
			Max = new Vector3(x2, y2, z2);
		}

		public BoundingBox(Vector3 min, Vector3 max)
		{
			Min = min;
			Max = max;
		}

		public BoundingBox(IEnumerable<Vector3> points)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			Min = new Vector3(float.MaxValue);
			Max = new Vector3(float.MinValue);
			foreach (Vector3 point in points)
			{
				Min.X = MathUtils.Min(Min.X, point.X);
				Min.Y = MathUtils.Min(Min.Y, point.Y);
				Min.Z = MathUtils.Min(Min.Z, point.Z);
				Max.X = MathUtils.Max(Max.X, point.X);
				Max.Y = MathUtils.Max(Max.Y, point.Y);
				Max.Z = MathUtils.Max(Max.Z, point.Z);
			}
			if (Min.X == float.MaxValue)
			{
				throw new ArgumentException("points");
			}
		}

		public static implicit operator BoundingBox(ValueTuple<float, float, float, float, float, float> v)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			return new BoundingBox(v.Item1, v.Item2, v.Item3, v.Item4, v.Item5, v.Item6);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is BoundingBox))
			{
				return false;
			}
			return Equals((BoundingBox)obj);
		}

		public override int GetHashCode()
		{
			return Min.GetHashCode() + Max.GetHashCode();
		}

		public override string ToString()
		{
			return $"{Min},{Max}";
		}

		public bool Equals(BoundingBox other)
		{
			if (Min == other.Min)
			{
				return Max == other.Max;
			}
			return false;
		}

		public Vector3 Center()
		{
			return new Vector3(0.5f * (Min.X + Max.X), 0.5f * (Min.Y + Max.Y), 0.5f * (Min.Z + Max.Z));
		}

		public Vector3 Size()
		{
			return Max - Min;
		}

		public float Volume()
		{
			Vector3 vector = Size();
			return vector.X * vector.Y * vector.Z;
		}

		public bool Contains(Vector3 p)
		{
			if (p.X >= Min.X && p.X <= Max.X && p.Y >= Min.Y && p.Y <= Max.Y && p.Z >= Min.Z)
			{
				return p.Z <= Max.Z;
			}
			return false;
		}

		public bool Intersection(BoundingBox box)
		{
			if (box.Max.X >= Min.X && box.Min.X <= Max.X && box.Max.Y >= Min.Y && box.Min.Y <= Max.Y && box.Max.Z >= Min.Z)
			{
				return box.Min.Z <= Max.Z;
			}
			return false;
		}

		public bool Intersection(BoundingSphere sphere)
		{
			if (sphere.Center.X - Min.X > sphere.Radius && sphere.Center.Y - Min.Y > sphere.Radius && sphere.Center.Z - Min.Z > sphere.Radius && Max.X - sphere.Center.X > sphere.Radius && Max.Y - sphere.Center.Y > sphere.Radius && Max.Z - sphere.Center.Z > sphere.Radius)
			{
				return true;
			}
			float num = 0f;
			if (sphere.Center.X - Min.X <= sphere.Radius)
			{
				num += (sphere.Center.X - Min.X) * (sphere.Center.X - Min.X);
			}
			else if (Max.X - sphere.Center.X <= sphere.Radius)
			{
				num += (sphere.Center.X - Max.X) * (sphere.Center.X - Max.X);
			}
			if (sphere.Center.Y - Min.Y <= sphere.Radius)
			{
				num += (sphere.Center.Y - Min.Y) * (sphere.Center.Y - Min.Y);
			}
			else if (Max.Y - sphere.Center.Y <= sphere.Radius)
			{
				num += (sphere.Center.Y - Max.Y) * (sphere.Center.Y - Max.Y);
			}
			if (sphere.Center.Z - Min.Z <= sphere.Radius)
			{
				num += (sphere.Center.Z - Min.Z) * (sphere.Center.Z - Min.Z);
			}
			else if (Max.Z - sphere.Center.Z <= sphere.Radius)
			{
				num += (sphere.Center.Z - Max.Z) * (sphere.Center.Z - Max.Z);
			}
			if (num <= sphere.Radius * sphere.Radius)
			{
				return true;
			}
			return false;
		}

		public static BoundingBox Intersection(BoundingBox b1, BoundingBox b2)
		{
			Vector3 min = Vector3.Max(b1.Min, b2.Min);
			Vector3 max = Vector3.Min(b1.Max, b2.Max);
			if (!(max.X > min.X) || !(max.Y > min.Y) || !(max.Z > min.Z))
			{
				return default(BoundingBox);
			}
			return new BoundingBox(min, max);
		}

		public static BoundingBox Union(BoundingBox b1, BoundingBox b2)
		{
			Vector3 min = Vector3.Min(b1.Min, b2.Min);
			Vector3 max = Vector3.Max(b1.Max, b2.Max);
			return new BoundingBox(min, max);
		}

		public static BoundingBox Union(BoundingBox b, Vector3 p)
		{
			Vector3 min = Vector3.Min(b.Min, p);
			Vector3 max = Vector3.Max(b.Max, p);
			return new BoundingBox(min, max);
		}

		public static float Distance(BoundingBox b, Vector3 p)
		{
			float num = MathUtils.Max(b.Min.X - p.X, 0f, p.X - b.Max.X);
			float num2 = MathUtils.Max(b.Min.Y - p.Y, 0f, p.Y - b.Max.Y);
			float num3 = MathUtils.Max(b.Min.Z - p.Z, 0f, p.Z - b.Max.Z);
			return MathUtils.Sqrt(num * num + num2 * num2 + num3 * num3);
		}

		public static BoundingBox Transform(BoundingBox b, Matrix m)
		{
			Transform(ref b, ref m, out BoundingBox result);
			return result;
		}

		public static void Transform(ref BoundingBox b, ref Matrix m, out BoundingBox result)
		{
			Vector3[] sourceArray = new Vector3[8]
			{
				new Vector3(b.Min.X, b.Min.Y, b.Min.Z),
				new Vector3(b.Max.X, b.Min.Y, b.Min.Z),
				new Vector3(b.Min.X, b.Max.Y, b.Min.Z),
				new Vector3(b.Max.X, b.Max.Y, b.Min.Z),
				new Vector3(b.Min.X, b.Min.Y, b.Max.Z),
				new Vector3(b.Max.X, b.Min.Y, b.Max.Z),
				new Vector3(b.Min.X, b.Max.Y, b.Max.Z),
				new Vector3(b.Max.X, b.Max.Y, b.Max.Z)
			};
			Vector3[] array = new Vector3[8];
			Vector3.Transform(sourceArray, 0, ref m, array, 0, 8);
			result = new BoundingBox(array);
		}

		public static bool operator ==(BoundingBox a, BoundingBox b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(BoundingBox a, BoundingBox b)
		{
			return !a.Equals(b);
		}
	}
}
