using System;
using System.Collections.Generic;
using System.Linq;

namespace SCPAK
{
	public sealed class VertexDeclaration
	{
		private static List<VertexElement[]> allElements = new List<VertexElement[]>();

		public VertexElement[] VertexElements;

		public int VertexStride
		{
			get;
			set;
		}

		public VertexDeclaration(params VertexElement[] elements)
		{
			if (elements.Length == 0)
			{
				throw new ArgumentException("There must be at least one VertexElement.");
			}
			for (int i = 0; i < allElements.Count; i++)
			{
				if (elements.SequenceEqual(allElements[i]))
				{
					VertexElements = allElements[i];
					break;
				}
			}
			if (VertexElements == null)
			{
				VertexElements = elements.ToArray();
				allElements.Add(VertexElements);
			}
			for (int j = 0; j < VertexElements.Length; j++)
			{
				VertexElement vertexElement = elements[j];
				VertexStride = Math.Max(VertexStride, vertexElement.Offset + vertexElement.Format.GetSize());
			}
		}
	}
}
