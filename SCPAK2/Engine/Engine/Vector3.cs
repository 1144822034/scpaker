using System;

namespace Engine
{
	public struct Vector3 : IEquatable<Vector3>
	{
		public float X;

		public float Y;

		public float Z;

		public static readonly Vector3 Zero = new Vector3(0f);

		public static readonly Vector3 One = new Vector3(1f);

		public static readonly Vector3 UnitX = new Vector3(1f, 0f, 0f);

		public static readonly Vector3 UnitY = new Vector3(0f, 1f, 0f);

		public static readonly Vector3 UnitZ = new Vector3(0f, 0f, 1f);

		public Vector2 XY
		{
			get
			{
				return new Vector2(X, Y);
			}
			set
			{
				X = value.X;
				Y = value.Y;
			}
		}

		public Vector2 YX
		{
			get
			{
				return new Vector2(Y, X);
			}
			set
			{
				Y = value.X;
				X = value.Y;
			}
		}

		public Vector2 XZ
		{
			get
			{
				return new Vector2(X, Z);
			}
			set
			{
				X = value.X;
				Z = value.Y;
			}
		}

		public Vector2 ZX
		{
			get
			{
				return new Vector2(Z, X);
			}
			set
			{
				Z = value.X;
				X = value.Y;
			}
		}

		public Vector2 YZ
		{
			get
			{
				return new Vector2(Y, Z);
			}
			set
			{
				Y = value.X;
				Z = value.Y;
			}
		}

		public Vector2 ZY
		{
			get
			{
				return new Vector2(Z, Y);
			}
			set
			{
				Z = value.X;
				Y = value.Y;
			}
		}

		public Vector3(float v)
		{
			X = v;
			Y = v;
			Z = v;
		}

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3(Vector2 xy, float z)
		{
			X = xy.X;
			Y = xy.Y;
			Z = z;
		}

		public Vector3(Point3 p)
		{
			X = p.X;
			Y = p.Y;
			Z = p.Z;
		}

		public Vector3(Color c)
		{
			X = (float)(int)c.R / 255f;
			Y = (float)(int)c.G / 255f;
			Z = (float)(int)c.B / 255f;
		}

		public static implicit operator Vector3(ValueTuple<float, float, float> v)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(v.Item1, v.Item2, v.Item3);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Vector3))
			{
				return false;
			}
			return Equals((Vector3)obj);
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
		}

		public override string ToString()
		{
			return $"{X},{Y},{Z}";
		}

		public bool Equals(Vector3 other)
		{
			if (X == other.X && Y == other.Y)
			{
				return Z == other.Z;
			}
			return false;
		}

		public static float Distance(Vector3 v1, Vector3 v2)
		{
			return MathUtils.Sqrt(DistanceSquared(v1, v2));
		}

		public static float DistanceSquared(Vector3 v1, Vector3 v2)
		{
			return MathUtils.Sqr(v1.X - v2.X) + MathUtils.Sqr(v1.Y - v2.Y) + MathUtils.Sqr(v1.Z - v2.Z);
		}

		public static float Dot(Vector3 v1, Vector3 v2)
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
		}

		public static Vector3 Cross(Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
		}

		public float Length()
		{
			return MathUtils.Sqrt(LengthSquared());
		}

		public float LengthSquared()
		{
			return X * X + Y * Y + Z * Z;
		}

		public static Vector3 Floor(Vector3 v)
		{
			return new Vector3(MathUtils.Floor(v.X), MathUtils.Floor(v.Y), MathUtils.Floor(v.Z));
		}

		public static Vector3 Ceiling(Vector3 v)
		{
			return new Vector3(MathUtils.Ceiling(v.X), MathUtils.Ceiling(v.Y), MathUtils.Ceiling(v.Z));
		}

		public static Vector3 Round(Vector3 v)
		{
			return new Vector3(MathUtils.Round(v.X), MathUtils.Round(v.Y), MathUtils.Round(v.Z));
		}

		public static Vector3 Min(Vector3 v, float f)
		{
			return new Vector3(MathUtils.Min(v.X, f), MathUtils.Min(v.Y, f), MathUtils.Min(v.Z, f));
		}

		public static Vector3 Min(Vector3 v1, Vector3 v2)
		{
			return new Vector3(MathUtils.Min(v1.X, v2.X), MathUtils.Min(v1.Y, v2.Y), MathUtils.Min(v1.Z, v2.Z));
		}

		public static Vector3 Max(Vector3 v, float f)
		{
			return new Vector3(MathUtils.Max(v.X, f), MathUtils.Max(v.Y, f), MathUtils.Max(v.Z, f));
		}

		public static Vector3 Max(Vector3 v1, Vector3 v2)
		{
			return new Vector3(MathUtils.Max(v1.X, v2.X), MathUtils.Max(v1.Y, v2.Y), MathUtils.Max(v1.Z, v2.Z));
		}

		public static float MinElement(Vector3 v)
		{
			return MathUtils.Min(v.X, v.Y, v.Z);
		}

		public static float MaxElement(Vector3 v)
		{
			return MathUtils.Max(v.X, v.Y, v.Z);
		}

		public static Vector3 Clamp(Vector3 v, float min, float max)
		{
			return new Vector3(MathUtils.Clamp(v.X, min, max), MathUtils.Clamp(v.Y, min, max), MathUtils.Clamp(v.Z, min, max));
		}

		public static Vector3 Saturate(Vector3 v)
		{
			return new Vector3(MathUtils.Saturate(v.X), MathUtils.Saturate(v.Y), MathUtils.Saturate(v.Z));
		}

		public static Vector3 Lerp(Vector3 v1, Vector3 v2, float f)
		{
			return new Vector3(MathUtils.Lerp(v1.X, v2.X, f), MathUtils.Lerp(v1.Y, v2.Y, f), MathUtils.Lerp(v1.Z, v2.Z, f));
		}

		public static Vector3 CatmullRom(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float f)
		{
			return new Vector3(MathUtils.CatmullRom(v1.X, v2.X, v3.X, v4.X, f), MathUtils.CatmullRom(v1.Y, v2.Y, v3.Y, v4.Y, f), MathUtils.CatmullRom(v1.Z, v2.Z, v3.Z, v4.Z, f));
		}

		public static Vector3 Normalize(Vector3 v)
		{
			float num = v.Length();
			if (!(num > 0f))
			{
				return UnitX;
			}
			return v / num;
		}

		public static Vector3 LimitLength(Vector3 v, float maxLength)
		{
			float num = v.LengthSquared();
			if (num > maxLength * maxLength)
			{
				return v * (maxLength / MathUtils.Sqrt(num));
			}
			return v;
		}

		public static Vector3 Transform(Vector3 v, Matrix m)
		{
			return new Vector3(v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31 + m.M41, v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32 + m.M42, v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33 + m.M43);
		}

		public static void Transform(ref Vector3 v, ref Matrix m, out Vector3 result)
		{
			result = new Vector3(v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31 + m.M41, v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32 + m.M42, v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33 + m.M43);
		}

		public static Vector3 Transform(Vector3 v, Quaternion q)
		{
			float num = q.X + q.X;
			float num2 = q.Y + q.Y;
			float num3 = q.Z + q.Z;
			float num4 = q.W * num;
			float num5 = q.W * num2;
			float num6 = q.W * num3;
			float num7 = q.X * num;
			float num8 = q.X * num2;
			float num9 = q.X * num3;
			float num10 = q.Y * num2;
			float num11 = q.Y * num3;
			float num12 = q.Z * num3;
			return new Vector3(v.X * (1f - num10 - num12) + v.Y * (num8 - num6) + v.Z * (num9 + num5), v.X * (num8 + num6) + v.Y * (1f - num7 - num12) + v.Z * (num11 - num4), v.X * (num9 - num5) + v.Y * (num11 + num4) + v.Z * (1f - num7 - num10));
		}

		public static void Transform(ref Vector3 v, ref Quaternion q, out Vector3 result)
		{
			float num = q.X + q.X;
			float num2 = q.Y + q.Y;
			float num3 = q.Z + q.Z;
			float num4 = q.W * num;
			float num5 = q.W * num2;
			float num6 = q.W * num3;
			float num7 = q.X * num;
			float num8 = q.X * num2;
			float num9 = q.X * num3;
			float num10 = q.Y * num2;
			float num11 = q.Y * num3;
			float num12 = q.Z * num3;
			result = new Vector3(v.X * (1f - num10 - num12) + v.Y * (num8 - num6) + v.Z * (num9 + num5), v.X * (num8 + num6) + v.Y * (1f - num7 - num12) + v.Z * (num11 - num4), v.X * (num9 - num5) + v.Y * (num11 + num4) + v.Z * (1f - num7 - num10));
		}

		public static void Transform(Vector3[] sourceArray, int sourceIndex, ref Matrix m, Vector3[] destinationArray, int destinationIndex, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Vector3 vector = sourceArray[sourceIndex + i];
				destinationArray[destinationIndex + i] = new Vector3(vector.X * m.M11 + vector.Y * m.M21 + vector.Z * m.M31 + m.M41, vector.X * m.M12 + vector.Y * m.M22 + vector.Z * m.M32 + m.M42, vector.X * m.M13 + vector.Y * m.M23 + vector.Z * m.M33 + m.M43);
			}
		}

		public static Vector3 TransformNormal(Vector3 v, Matrix m)
		{
			return new Vector3(v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31, v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32, v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33);
		}

		public static void TransformNormal(ref Vector3 v, ref Matrix m, out Vector3 result)
		{
			result = new Vector3(v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31, v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32, v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33);
		}

		public static void TransformNormal(Vector3[] sourceArray, int sourceIndex, ref Matrix m, Vector3[] destinationArray, int destinationIndex, int count)
		{
			for (int i = 0; i < count; i++)
			{
				Vector3 vector = sourceArray[sourceIndex + i];
				destinationArray[destinationIndex + i] = new Vector3(vector.X * m.M11 + vector.Y * m.M21 + vector.Z * m.M31, vector.X * m.M12 + vector.Y * m.M22 + vector.Z * m.M32, vector.X * m.M13 + vector.Y * m.M23 + vector.Z * m.M33);
			}
		}

		public static bool operator ==(Vector3 v1, Vector3 v2)
		{
			return v1.Equals(v2);
		}

		public static bool operator !=(Vector3 v1, Vector3 v2)
		{
			return !v1.Equals(v2);
		}

		public static Vector3 operator +(Vector3 v)
		{
			return v;
		}

		public static Vector3 operator -(Vector3 v)
		{
			return new Vector3(0f - v.X, 0f - v.Y, 0f - v.Z);
		}

		public static Vector3 operator +(Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
		}

		public static Vector3 operator -(Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
		}

		public static Vector3 operator *(Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
		}

		public static Vector3 operator *(Vector3 v, float s)
		{
			return new Vector3(v.X * s, v.Y * s, v.Z * s);
		}

		public static Vector3 operator *(float s, Vector3 v)
		{
			return new Vector3(v.X * s, v.Y * s, v.Z * s);
		}

		public static Vector3 operator /(Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
		}

		public static Vector3 operator /(Vector3 v, float d)
		{
			float num = 1f / d;
			return new Vector3(v.X * num, v.Y * num, v.Z * num);
		}
	}
}
