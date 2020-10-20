using System;

namespace Engine
{
	public struct Point2 : IEquatable<Point2>
	{
		public int X;

		public int Y;

		public static readonly Point2 Zero = default(Point2);

		public static readonly Point2 One = new Point2(1, 1);

		public static readonly Point2 UnitX = new Point2(1, 0);

		public static readonly Point2 UnitY = new Point2(0, 1);

		public Point2(int v)
		{
			X = v;
			Y = v;
		}

		public Point2(int x, int y)
		{
			X = x;
			Y = y;
		}

		public Point2(Vector2 v)
		{
			X = (int)v.X;
			Y = (int)v.Y;
		}

		public static implicit operator Point2(ValueTuple<int, int> v)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return new Point2(v.Item1, v.Item2);
		}

		public override int GetHashCode()
		{
			return X + Y;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Point2))
			{
				return false;
			}
			return Equals((Point2)obj);
		}

		public bool Equals(Point2 other)
		{
			if (other.X == X)
			{
				return other.Y == Y;
			}
			return false;
		}

		public override string ToString()
		{
			return $"{X},{Y}";
		}

		public static Point2 Min(Point2 p, int v)
		{
			return new Point2(MathUtils.Min(p.X, v), MathUtils.Min(p.Y, v));
		}

		public static Point2 Min(Point2 p1, Point2 p2)
		{
			return new Point2(MathUtils.Min(p1.X, p2.X), MathUtils.Min(p1.Y, p2.Y));
		}

		public static Point2 Max(Point2 p, int v)
		{
			return new Point2(MathUtils.Max(p.X, v), MathUtils.Max(p.Y, v));
		}

		public static Point2 Max(Point2 p1, Point2 p2)
		{
			return new Point2(MathUtils.Max(p1.X, p2.X), MathUtils.Max(p1.Y, p2.Y));
		}

		public static bool operator ==(Point2 p1, Point2 p2)
		{
			return p1.Equals(p2);
		}

		public static bool operator !=(Point2 p1, Point2 p2)
		{
			return !p1.Equals(p2);
		}

		public static Point2 operator +(Point2 p)
		{
			return p;
		}

		public static Point2 operator -(Point2 p)
		{
			return new Point2(-p.X, -p.Y);
		}

		public static Point2 operator +(Point2 p1, Point2 p2)
		{
			return new Point2(p1.X + p2.X, p1.Y + p2.Y);
		}

		public static Point2 operator -(Point2 p1, Point2 p2)
		{
			return new Point2(p1.X - p2.X, p1.Y - p2.Y);
		}

		public static Point2 operator *(int n, Point2 p)
		{
			return new Point2(p.X * n, p.Y * n);
		}

		public static Point2 operator *(Point2 p, int n)
		{
			return new Point2(p.X * n, p.Y * n);
		}

		public static Point2 operator *(Point2 p1, Point2 p2)
		{
			return new Point2(p1.X * p2.X, p1.Y * p2.Y);
		}

		public static Point2 operator /(Point2 p, int n)
		{
			return new Point2(p.X / n, p.Y / n);
		}

		public static Point2 operator /(Point2 p1, Point2 p2)
		{
			return new Point2(p1.X / p2.X, p1.Y / p2.Y);
		}
	}
}
