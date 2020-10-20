using System;
using System.IO;

namespace Engine.Content
{
	[ContentReader("System.String")]
	public class StringContentReader : IContentReader
	{
		public object Read(ContentStream stream, object existingObject)
		{
			if (existingObject == null)
			{
				return new BinaryReader(stream).ReadString();
			}
			throw new NotSupportedException();
		}
	}
}
