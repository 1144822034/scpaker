using System;
using System.IO;
using System.Xml.Linq;

namespace Engine.Content
{
	[ContentReader("System.Xml.Linq.XElement")]
	public class XElementContentReader : IContentReader
	{
		public object Read(ContentStream stream, object existingObject)
		{
			if (existingObject == null)
			{
				return XElement.Load(new StringReader(new BinaryReader(stream).ReadString()));
			}
			throw new NotSupportedException();
		}
	}
}
