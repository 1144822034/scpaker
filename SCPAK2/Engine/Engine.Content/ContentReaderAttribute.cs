using System;

namespace Engine.Content
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ContentReaderAttribute : Attribute
	{
		public string ContentTypeName;

		public ContentReaderAttribute(string contentTypeName)
		{
			ContentTypeName = contentTypeName;
		}
	}
}
