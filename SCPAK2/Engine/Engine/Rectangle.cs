using System;

namespace Engine
{
	public struct Rectangle : IEquatable<Rectangle>
	{
		public int Left;

		public int Top;

		public int Width;

		public int Height;

		public static Rectangle Empty;

		public Point2 Location
		{
			get
			{
				return new Point2(Left, Top);
			}
			set
			{
				Left = value.X;
				Top = value.Y;
			}
		}

		public Point2 Size
		{
			get
			{
				return new Point2(Width, Height);
			}
			set
			{
				Width = value.X;
				Height = value.Y;
			}
		}

		public int Right => Left + Width;

		public int Bottom => Top + Height;

		public Rectangle(int left, int top, int width, int height)
		{
			Left = left;
			Top = top;
			Width = width;
			Height = height;
		}

		public static implicit operator Rectangle(ValueTuple<int, int, int, int> v)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return new Rectangle(v.Item1, v.Item2, v.Item3, v.Item4);
		}

		public bool Equals(Rectangle other)
		{
			if (Left == other.Left && Top == other.Top && Width == other.Width)
			{
				return Height == other.Height;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Rectangle))
			{
				return false;
			}
			return Equals((Rectangle)obj);
		}

		public override int GetHashCode()
		{
			return Left + Top + Width + Height;
		}

		public override string ToString()
		{
			return $"{Left},{Top},{Width},{Height}";
		}

		public bool Contains(Point2 p)
		{
			if (p.X >= Left && p.X < Left + Width && p.Y >= Top)
			{
				return p.Y < Top + Height;
			}
			return false;
		}

		public bool Intersection(Rectangle r)
		{
			int num = MathUtils.Max(Left, r.Left);
			int num2 = MathUtils.Max(Top, r.Top);
			int num3 = MathUtils.Min(Left + Width, r.Left + r.Width);
			int num4 = MathUtils.Min(Top + Height, r.Top + r.Height);
			if (num3 > num)
			{
				return num4 > num2;
			}
			return false;
		}

		public static Rectangle Intersection(Rectangle r1, Rectangle r2)
		{
			int num = MathUtils.Max(r1.Left, r2.Left);
			int num2 = MathUtils.Max(r1.Top, r2.Top);
			int num3 = MathUtils.Min(r1.Left + r1.Width, r2.Left + r2.Width);
			int num4 = MathUtils.Min(r1.Top + r1.Height, r2.Top + r2.Height);
			if (num3 <= num || num4 <= num2)
			{
				return Empty;
			}
			return new Rectangle(num, num2, num3 - num, num4 - num2);
		}

		public static Rectangle Union(Rectangle r1, Rectangle r2)
		{
			int num = MathUtils.Min(r1.Left, r2.Left);
			int num2 = MathUtils.Min(r1.Top, r2.Top);
			int num3 = MathUtils.Max(r1.Left + r1.Width, r2.Left + r2.Width);
			int num4 = MathUtils.Max(r1.Top + r1.Height, r2.Top + r2.Height);
			return new Rectangle(num, num2, num3 - num, num4 - num2);
		}

		public static bool operator ==(Rectangle r1, Rectangle r2)
		{
			return r1.Equals(r2);
		}

		public static bool operator !=(Rectangle r1, Rectangle r2)
		{
			return !r1.Equals(r2);
		}
	}
}
