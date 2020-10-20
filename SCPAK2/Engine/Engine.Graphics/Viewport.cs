using System;

namespace Engine.Graphics
{
	public struct Viewport : IEquatable<Viewport>
	{
		public int X;

		public int Y;

		public int Width;

		public int Height;

		public float MinDepth;

		public float MaxDepth;

		public Rectangle Rectangle => new Rectangle(X, Y, Width, Height);

		public float AspectRatio => (float)Width / (float)Height;

		public Viewport(int x, int y, int width, int height, float minDepth = 0f, float maxDepth = 1f)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
			MinDepth = minDepth;
			MaxDepth = maxDepth;
		}

		public bool Equals(Viewport other)
		{
			if (X == other.X && Y == other.Y && Width == other.Width && Height == other.Height && MinDepth == other.MinDepth)
			{
				return MaxDepth == other.MaxDepth;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Viewport))
			{
				return false;
			}
			return Equals(this);
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Width.GetHashCode() + Height.GetHashCode() + MinDepth.GetHashCode() + MaxDepth.GetHashCode();
		}

		public override string ToString()
		{
			return $"{X}, {Y}, {Width}, {Height}, {MinDepth}, {MaxDepth}";
		}

		public Vector3 Project(Vector3 source, Matrix worldViewProjection)
		{
			Vector3 result = Vector3.Transform(source, worldViewProjection);
			result /= source.X * worldViewProjection.M14 + source.Y * worldViewProjection.M24 + source.Z * worldViewProjection.M34 + worldViewProjection.M44;
			result.X = (result.X + 1f) * 0.5f * (float)Width + (float)X;
			result.Y = (0f - result.Y + 1f) * 0.5f * (float)Height + (float)Y;
			result.Z = result.Z * (MaxDepth - MinDepth) + MinDepth;
			return result;
		}

		public Vector3 Project(Vector3 source, Matrix projection, Matrix view, Matrix world)
		{
			return Project(source, world * view * projection);
		}

		public Vector3 Unproject(Vector3 source, Matrix worldViewProjection)
		{
			Matrix m = Matrix.Invert(worldViewProjection);
			source.X = (source.X - (float)X) / (float)Width * 2f - 1f;
			source.Y = 0f - ((source.Y - (float)Y) / (float)Height * 2f - 1f);
			source.Z = (source.Z - MinDepth) / (MaxDepth - MinDepth);
			return Vector3.Transform(source, m) / (source.X * m.M14 + source.Y * m.M24 + source.Z * m.M34 + m.M44);
		}

		public Vector3 Unproject(Vector3 source, Matrix projection, Matrix view, Matrix world)
		{
			return Unproject(source, world * view * projection);
		}

		public static bool operator ==(Viewport v1, Viewport v2)
		{
			return v1.Equals(v2);
		}

		public static bool operator !=(Viewport v1, Viewport v2)
		{
			return !v1.Equals(v2);
		}
	}
}
