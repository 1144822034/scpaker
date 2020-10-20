using System;

namespace Engine
{
	public struct Box : IEquatable<Box>
	{
		public int Left;

		public int Top;

		public int Near;

		public int Width;

		public int Height;

		public int Depth;

		public static Box Empty;

		public Point3 Location
		{
			get
			{
				return new Point3(Left, Top, Near);
			}
			set
			{
				Left = value.X;
				Top = value.Y;
				Near = value.Z;
			}
		}

		public Point3 Size
		{
			get
			{
				return new Point3(Width, Height, Depth);
			}
			set
			{
				Width = value.X;
				Height = value.Y;
				Depth = value.Z;
			}
		}

		public int Right => Left + Width;

		public int Bottom => Top + Height;

		public int Far => Near + Depth;

		public Box(int left, int top, int near, int width, int height, int depth)
		{
			Left = left;
			Top = top;
			Near = near;
			Width = width;
			Height = height;
			Depth = depth;
		}

		public static implicit operator Box(ValueTuple<int, int, int, int, int, int> v)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			return new Box(v.Item1, v.Item2, v.Item3, v.Item4, v.Item5, v.Item6);
		}

		public bool Equals(Box other)
		{
			if (Left == other.Left && Top == other.Top && Near == other.Near && Width == other.Width && Height == other.Height)
			{
				return Depth == other.Depth;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Box))
			{
				return false;
			}
			return Equals((Box)obj);
		}

		public override int GetHashCode()
		{
			return Left + Top + Near + Width + Height + Depth;
		}

		public override string ToString()
		{
			return $"{Left},{Top},{Near},{Width},{Height},{Depth}";
		}

		public bool Contains(Point3 p)
		{
			if (p.X >= Left && p.X < Left + Width && p.Y >= Top && p.Y < Top + Height && p.Z >= Near)
			{
				return p.Z < Near + Depth;
			}
			return false;
		}

		public static Box Intersection(Box b1, Box b2)
		{
			int num = MathUtils.Max(b1.Left, b2.Left);
			int num2 = MathUtils.Max(b1.Top, b2.Top);
			int num3 = MathUtils.Min(b1.Near, b2.Near);
			int num4 = MathUtils.Min(b1.Left + b1.Width, b2.Left + b2.Width);
			int num5 = MathUtils.Min(b1.Top + b1.Height, b2.Top + b2.Height);
			int num6 = MathUtils.Min(b1.Near + b1.Depth, b2.Near + b2.Depth);
			if (num4 <= num || num5 <= num2 || num6 <= num3)
			{
				return Empty;
			}
			return new Box(num, num2, num3, num4 - num, num5 - num2, num6 - num3);
		}

		public static Box Union(Box b1, Box b2)
		{
			int num = MathUtils.Min(b1.Left, b2.Left);
			int num2 = MathUtils.Min(b1.Top, b2.Top);
			int num3 = MathUtils.Min(b1.Near, b2.Near);
			int num4 = MathUtils.Max(b1.Left + b1.Width, b2.Left + b2.Width);
			int num5 = MathUtils.Max(b1.Top + b1.Height, b2.Top + b2.Height);
			int num6 = MathUtils.Max(b1.Near + b1.Depth, b2.Near + b2.Depth);
			return new Box(num, num2, num3, num4 - num, num5 - num2, num6 - num3);
		}

		public static bool operator ==(Box b1, Box b2)
		{
			return b1.Equals(b2);
		}

		public static bool operator !=(Box b1, Box b2)
		{
			return !b1.Equals(b2);
		}
	}
}
