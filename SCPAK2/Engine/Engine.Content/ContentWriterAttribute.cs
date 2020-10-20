using System;

namespace Engine.Content
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ContentWriterAttribute : Attribute
	{
		public string ContentTypeName;

		public ContentWriterAttribute(string contentTypeName)
		{
			ContentTypeName = contentTypeName;
		}
	}
}
