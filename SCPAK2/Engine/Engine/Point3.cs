using System;

namespace Engine
{
	public struct Point3 : IEquatable<Point3>
	{
		public int X;

		public int Y;

		public int Z;

		public static readonly Point3 Zero = default(Point3);

		public static readonly Point3 One = new Point3(1, 1, 1);

		public static readonly Point3 UnitX = new Point3(1, 0, 0);

		public static readonly Point3 UnitY = new Point3(0, 1, 0);

		public static readonly Point3 UnitZ = new Point3(0, 0, 1);

		public Point3(int v)
		{
			X = v;
			Y = v;
			Z = v;
		}

		public Point3(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Point3(Vector3 v)
		{
			X = (int)v.X;
			Y = (int)v.Y;
			Z = (int)v.Z;
		}

		public static implicit operator Point3(ValueTuple<int, int, int> v)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return new Point3(v.Item1, v.Item2, v.Item3);
		}

		public override int GetHashCode()
		{
			return X + Y + Z;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Point3))
			{
				return false;
			}
			return Equals((Point3)obj);
		}

		public bool Equals(Point3 other)
		{
			if (other.X == X && other.Y == Y)
			{
				return other.Z == Z;
			}
			return false;
		}

		public override string ToString()
		{
			return $"{X},{Y},{Z}";
		}

		public static Point3 Min(Point3 p, int v)
		{
			return new Point3(MathUtils.Min(p.X, v), MathUtils.Min(p.Y, v), MathUtils.Min(p.Z, v));
		}

		public static Point3 Min(Point3 p1, Point3 p2)
		{
			return new Point3(MathUtils.Min(p1.X, p2.X), MathUtils.Min(p1.Y, p2.Y), MathUtils.Min(p1.Z, p2.Z));
		}

		public static Point3 Max(Point3 p, int v)
		{
			return new Point3(MathUtils.Max(p.X, v), MathUtils.Max(p.Y, v), MathUtils.Max(p.Z, v));
		}

		public static Point3 Max(Point3 p1, Point3 p2)
		{
			return new Point3(MathUtils.Max(p1.X, p2.X), MathUtils.Max(p1.Y, p2.Y), MathUtils.Max(p1.Z, p2.Z));
		}

		public static bool operator ==(Point3 p1, Point3 p2)
		{
			return p1.Equals(p2);
		}

		public static bool operator !=(Point3 p1, Point3 p2)
		{
			return !p1.Equals(p2);
		}

		public static Point3 operator +(Point3 p)
		{
			return p;
		}

		public static Point3 operator -(Point3 p)
		{
			return new Point3(-p.X, -p.Y, -p.Z);
		}

		public static Point3 operator +(Point3 p1, Point3 p2)
		{
			return new Point3(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
		}

		public static Point3 operator -(Point3 p1, Point3 p2)
		{
			return new Point3(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
		}

		public static Point3 operator *(int n, Point3 p)
		{
			return new Point3(p.X * n, p.Y * n, p.Z * n);
		}

		public static Point3 operator *(Point3 p, int n)
		{
			return new Point3(p.X * n, p.Y * n, p.Z * n);
		}

		public static Point3 operator *(Point3 p1, Point3 p2)
		{
			return new Point3(p1.X * p2.X, p1.Y * p2.Y, p1.Z * p2.Z);
		}

		public static Point3 operator /(Point3 p, int n)
		{
			return new Point3(p.X / n, p.Y / n, p.Z / n);
		}

		public static Point3 operator /(Point3 p1, Point3 p2)
		{
			return new Point3(p1.X / p2.X, p1.Y / p2.Y, p1.Z / p2.Z);
		}
	}
}
