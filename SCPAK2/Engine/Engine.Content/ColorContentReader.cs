using Engine.Serialization;
using System;

namespace Engine.Content
{
	[ContentReader("Engine.Color")]
	public class ColorContentReader : IContentReader
	{
		public object Read(ContentStream stream, object existingObject)
		{
			if (existingObject == null)
			{
				return new EngineBinaryReader(stream).ReadColor();
			}
			throw new NotSupportedException();
		}
	}
}
