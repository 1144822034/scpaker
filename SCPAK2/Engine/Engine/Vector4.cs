using System;

namespace Engine
{
	public struct Vector4 : IEquatable<Vector4>
	{
		public float X;

		public float Y;

		public float Z;

		public float W;

		public static readonly Vector4 Zero = new Vector4(0f);

		public static readonly Vector4 One = new Vector4(1f);

		public static readonly Vector4 UnitX = new Vector4(1f, 0f, 0f, 0f);

		public static readonly Vector4 UnitY = new Vector4(0f, 1f, 0f, 0f);

		public static readonly Vector4 UnitZ = new Vector4(0f, 0f, 1f, 0f);

		public static readonly Vector4 UnitW = new Vector4(0f, 0f, 0f, 1f);

		public Vector4(float v)
		{
			X = v;
			Y = v;
			Z = v;
			W = v;
		}

		public Vector4(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Vector4(Vector3 xyz, float w)
		{
			X = xyz.X;
			Y = xyz.Y;
			Z = xyz.Z;
			W = w;
		}

		public Vector4(Color c)
		{
			X = (float)(int)c.R / 255f;
			Y = (float)(int)c.G / 255f;
			Z = (float)(int)c.B / 255f;
			W = (float)(int)c.A / 255f;
		}

		public static implicit operator Vector4(ValueTuple<float, float, float, float> v)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return new Vector4(v.Item1, v.Item2, v.Item3, v.Item4);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Vector4))
			{
				return false;
			}
			return Equals((Vector4)obj);
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode();
		}

		public override string ToString()
		{
			return $"{X},{Y},{Z},{W}";
		}

		public bool Equals(Vector4 other)
		{
			if (X == other.X && Y == other.Y && Z == other.Z)
			{
				return W == other.W;
			}
			return false;
		}

		public static float Distance(Vector4 v1, Vector4 v2)
		{
			return MathUtils.Sqrt(DistanceSquared(v1, v2));
		}

		public static float DistanceSquared(Vector4 v1, Vector4 v2)
		{
			return MathUtils.Sqr(v1.X - v2.X) + MathUtils.Sqr(v1.Y - v2.Y) + MathUtils.Sqr(v1.Z - v2.Z) + MathUtils.Sqr(v1.W - v2.W);
		}

		public static float Dot(Vector4 v1, Vector4 v2)
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z + v1.W * v2.W;
		}

		public float Length()
		{
			return MathUtils.Sqrt(LengthSquared());
		}

		public float LengthSquared()
		{
			return X * X + Y * Y + Z * Z;
		}

		public static Vector4 Floor(Vector4 v)
		{
			return new Vector4(MathUtils.Floor(v.X), MathUtils.Floor(v.Y), MathUtils.Floor(v.Z), MathUtils.Floor(v.W));
		}

		public static Vector4 Ceiling(Vector4 v)
		{
			return new Vector4(MathUtils.Ceiling(v.X), MathUtils.Ceiling(v.Y), MathUtils.Ceiling(v.Z), MathUtils.Ceiling(v.W));
		}

		public static Vector4 Round(Vector4 v)
		{
			return new Vector4(MathUtils.Round(v.X), MathUtils.Round(v.Y), MathUtils.Round(v.Z), MathUtils.Round(v.W));
		}

		public static Vector4 Min(Vector4 v, float f)
		{
			return new Vector4(MathUtils.Min(v.X, f), MathUtils.Min(v.Y, f), MathUtils.Min(v.Z, f), MathUtils.Min(v.W, f));
		}

		public static Vector4 Min(Vector4 v1, Vector4 v2)
		{
			return new Vector4(MathUtils.Min(v1.X, v2.X), MathUtils.Min(v1.Y, v2.Y), MathUtils.Min(v1.Z, v2.Z), MathUtils.Min(v1.W, v2.W));
		}

		public static Vector4 Max(Vector4 v, float f)
		{
			return new Vector4(MathUtils.Max(v.X, f), MathUtils.Max(v.Y, f), MathUtils.Max(v.Z, f), MathUtils.Max(v.W, f));
		}

		public static Vector4 Max(Vector4 v1, Vector4 v2)
		{
			return new Vector4(MathUtils.Max(v1.X, v2.X), MathUtils.Max(v1.Y, v2.Y), MathUtils.Max(v1.Z, v2.Z), MathUtils.Max(v1.W, v2.W));
		}

		public static float MinElement(Vector4 v)
		{
			return MathUtils.Min(v.X, v.Y, v.Z, v.W);
		}

		public static float MaxElement(Vector4 v)
		{
			return MathUtils.Max(v.X, v.Y, v.Z, v.W);
		}

		public static Vector4 Clamp(Vector4 v, float min, float max)
		{
			return new Vector4(MathUtils.Clamp(v.X, min, max), MathUtils.Clamp(v.Y, min, max), MathUtils.Clamp(v.Z, min, max), MathUtils.Clamp(v.W, min, max));
		}

		public static Vector4 Saturate(Vector4 v)
		{
			return new Vector4(MathUtils.Saturate(v.X), MathUtils.Saturate(v.Y), MathUtils.Saturate(v.Z), MathUtils.Saturate(v.W));
		}

		public static Vector4 Lerp(Vector4 v1, Vector4 v2, float f)
		{
			return new Vector4(MathUtils.Lerp(v1.X, v2.X, f), MathUtils.Lerp(v1.Y, v2.Y, f), MathUtils.Lerp(v1.Z, v2.Z, f), MathUtils.Lerp(v1.W, v2.W, f));
		}

		public static Vector4 CatmullRom(Vector4 v1, Vector4 v2, Vector4 v3, Vector4 v4, float f)
		{
			return new Vector4(MathUtils.CatmullRom(v1.X, v2.X, v3.X, v4.X, f), MathUtils.CatmullRom(v1.Y, v2.Y, v3.Y, v4.Y, f), MathUtils.CatmullRom(v1.Z, v2.Z, v3.Z, v4.Z, f), MathUtils.CatmullRom(v1.W, v2.W, v3.W, v4.W, f));
		}

		public static Vector4 Normalize(Vector4 v)
		{
			float num = v.Length();
			if (!(num > 0f))
			{
				return UnitX;
			}
			return v / num;
		}

		public static Vector4 LimitLength(Vector4 v, float maxLength)
		{
			float num = v.LengthSquared();
			if (num > maxLength * maxLength)
			{
				return v * (maxLength / MathUtils.Sqrt(num));
			}
			return v;
		}

		public static Vector4 Transform(Vector4 v, Matrix m)
		{
			return new Vector4(v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31 + m.M41, v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32 + m.M42, v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33 + m.M43, v.X * m.M14 + v.Y * m.M24 + v.Z * m.M34 + m.M44);
		}

		public static void Transform(ref Vector4 v, ref Matrix m, out Vector4 result)
		{
			result = new Vector4(v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31 + m.M41, v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32 + m.M42, v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33 + m.M43, v.X * m.M14 + v.Y * m.M24 + v.Z * m.M34 + m.M44);
		}

		public static void Transform(Vector4[] sourceArray, int sourceIndex, ref Matrix m, Vector4[] destinationArray, int destinationIndex, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Vector4 vector = sourceArray[sourceIndex + i];
				destinationArray[destinationIndex + i] = new Vector4(vector.X * m.M11 + vector.Y * m.M21 + vector.Z * m.M31 + m.M41, vector.X * m.M12 + vector.Y * m.M22 + vector.Z * m.M32 + m.M42, vector.X * m.M13 + vector.Y * m.M23 + vector.Z * m.M33 + m.M43, vector.X * m.M14 + vector.Y * m.M24 + vector.Z * m.M34 + m.M44);
			}
		}

		public static bool operator ==(Vector4 v1, Vector4 v2)
		{
			return v1.Equals(v2);
		}

		public static bool operator !=(Vector4 v1, Vector4 v2)
		{
			return !v1.Equals(v2);
		}

		public static Vector4 operator +(Vector4 v)
		{
			return v;
		}

		public static Vector4 operator -(Vector4 v)
		{
			return new Vector4(0f - v.X, 0f - v.Y, 0f - v.Z, 0f - v.W);
		}

		public static Vector4 operator +(Vector4 v1, Vector4 v2)
		{
			return new Vector4(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, v1.W + v2.W);
		}

		public static Vector4 operator -(Vector4 v1, Vector4 v2)
		{
			return new Vector4(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, v1.W - v2.W);
		}

		public static Vector4 operator *(Vector4 v1, Vector4 v2)
		{
			return new Vector4(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z, v1.W * v2.W);
		}

		public static Vector4 operator *(Vector4 v, float s)
		{
			return new Vector4(v.X * s, v.Y * s, v.Z * s, v.W * s);
		}

		public static Vector4 operator *(float s, Vector4 v)
		{
			return new Vector4(v.X * s, v.Y * s, v.Z * s, v.W * s);
		}

		public static Vector4 operator /(Vector4 v1, Vector4 v2)
		{
			return new Vector4(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z, v1.W / v2.W);
		}

		public static Vector4 operator /(Vector4 v, float d)
		{
			float num = 1f / d;
			return new Vector4(v.X * num, v.Y * num, v.Z * num, v.W * num);
		}
	}
}
