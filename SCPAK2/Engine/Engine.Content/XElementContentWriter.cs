using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Engine.Content
{
	[ContentWriter("System.Xml.Linq.XElement")]
	public class XElementContentWriter : IContentWriter
	{
		public string Xml;

		public IEnumerable<string> GetDependencies()
		{
			yield return Xml;
		}

		public void Write(string projectDirectory, Stream stream)
		{
			string text = Storage.ReadAllText(Storage.CombinePaths(projectDirectory, Xml));
			XElement.Load(new StringReader(text));
			new BinaryWriter(stream).Write(text);
		}
	}
}
