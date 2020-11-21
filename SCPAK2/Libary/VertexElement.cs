using System;

namespace SCPAK
{
	public class VertexElement
	{
		public readonly int Offset;

		public readonly VertexElementFormat Format;

		public readonly string Semantic;

		public int HashCode;

		public VertexElement(int offset, VertexElementFormat format, string semantic)
		{
			if (offset < 0)
			{
				throw new ArgumentException("offset cannot be negative.");
			}
			if (string.IsNullOrEmpty(semantic))
			{
				throw new ArgumentException("semantic cannot be empty or null.");
			}
			Offset = offset;
			Format = format;
			Semantic = semantic;
			HashCode = Offset.GetHashCode() + Format.GetHashCode() + Semantic.GetHashCode();
		}
	}
}
