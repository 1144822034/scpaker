using System;

namespace Engine
{
	public struct Ray3 : IEquatable<Ray3>
	{
		public Vector3 Position;

		public Vector3 Direction;

		public Ray3(Vector3 position, Vector3 direction)
		{
			Position = position;
			Direction = direction;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Ray3))
			{
				return false;
			}
			return Equals((Ray3)obj);
		}

		public override int GetHashCode()
		{
			return Position.GetHashCode() + Direction.GetHashCode();
		}

		public override string ToString()
		{
			return $"{Position.ToString()},{Direction.ToString()}";
		}

		public bool Equals(Ray3 other)
		{
			if (Position == other.Position)
			{
				return Direction == other.Direction;
			}
			return false;
		}

		public float? Intersection(BoundingBox box)
		{
			if (Position.X >= box.Min.X && Position.X <= box.Max.X && Position.Y >= box.Min.Y && Position.Y <= box.Max.Y && Position.Z >= box.Min.Z && Position.Z <= box.Max.Z)
			{
				return 0f;
			}
			Vector3 vector = new Vector3(-1f);
			if (Direction.X != 0f)
			{
				if (Position.X < box.Min.X)
				{
					vector.X = (box.Min.X - Position.X) / Direction.X;
				}
				else if (Position.X > box.Max.X)
				{
					vector.X = (box.Max.X - Position.X) / Direction.X;
				}
			}
			if (Direction.Y != 0f)
			{
				if (Position.Y < box.Min.Y)
				{
					vector.Y = (box.Min.Y - Position.Y) / Direction.Y;
				}
				else if (Position.Y > box.Max.Y)
				{
					vector.Y = (box.Max.Y - Position.Y) / Direction.Y;
				}
			}
			if (Direction.Z != 0f)
			{
				if (Position.Z < box.Min.Z)
				{
					vector.Z = (box.Min.Z - Position.Z) / Direction.Z;
				}
				else if (Position.Z > box.Max.Z)
				{
					vector.Z = (box.Max.Z - Position.Z) / Direction.Z;
				}
			}
			if (vector.X > vector.Y && vector.X > vector.Z)
			{
				if (vector.X < 0f)
				{
					return null;
				}
				float num = Position.Z + vector.X * Direction.Z;
				if (num < box.Min.Z || num > box.Max.Z)
				{
					return null;
				}
				num = Position.Y + vector.X * Direction.Y;
				if (num < box.Min.Y || num > box.Max.Y)
				{
					return null;
				}
				return vector.X;
			}
			if (vector.Y > vector.X && vector.Y > vector.Z)
			{
				if (vector.Y < 0f)
				{
					return null;
				}
				float num2 = Position.Z + vector.Y * Direction.Z;
				if (num2 < box.Min.Z || num2 > box.Max.Z)
				{
					return null;
				}
				num2 = Position.X + vector.Y * Direction.X;
				if (num2 < box.Min.X || num2 > box.Max.X)
				{
					return null;
				}
				return vector.Y;
			}
			if (vector.Z < 0f)
			{
				return null;
			}
			float num3 = Position.X + vector.Z * Direction.X;
			if (num3 < box.Min.X || num3 > box.Max.X)
			{
				return null;
			}
			num3 = Position.Y + vector.Z * Direction.Y;
			if (num3 < box.Min.Y || num3 > box.Max.Y)
			{
				return null;
			}
			return vector.Z;
		}

		public float? Intersection(BoundingSphere sphere)
		{
			Vector3 v = sphere.Center - Position;
			float num = v.LengthSquared();
			float num2 = sphere.Radius * sphere.Radius;
			if (num < num2)
			{
				return 0f;
			}
			float num3 = Vector3.Dot(Direction, v);
			if (num3 < 0f)
			{
				return null;
			}
			float num4 = num2 + num3 * num3 - num;
			if (!(num4 < 0f))
			{
				return num3 - MathUtils.Sqrt(num4);
			}
			return null;
		}

		public float? Intersection(Plane plane)
		{
			float num = Vector3.Dot(Direction, plane.Normal);
			if (num == 0f)
			{
				return null;
			}
			return (0f - (Vector3.Dot(Position, plane.Normal) + plane.D)) / num;
		}

		public static bool operator ==(Ray3 a, Ray3 b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Ray3 a, Ray3 b)
		{
			return !a.Equals(b);
		}
	}
}
