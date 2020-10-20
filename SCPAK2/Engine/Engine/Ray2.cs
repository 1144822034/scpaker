using System;

namespace Engine
{
	public struct Ray2 : IEquatable<Ray2>
	{
		public Vector2 Position;

		public Vector2 Direction;

		public Ray2(Vector2 position, Vector2 direction)
		{
			Position = position;
			Direction = direction;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Ray2))
			{
				return false;
			}
			return Equals((Ray2)obj);
		}

		public override int GetHashCode()
		{
			return Position.GetHashCode() + Direction.GetHashCode();
		}

		public override string ToString()
		{
			return $"{Position.ToString()},{Direction.ToString()}";
		}

		public bool Equals(Ray2 other)
		{
			if (Position == other.Position)
			{
				return Direction == other.Direction;
			}
			return false;
		}

		public static bool operator ==(Ray2 a, Ray2 b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Ray2 a, Ray2 b)
		{
			return !a.Equals(b);
		}
	}
}
