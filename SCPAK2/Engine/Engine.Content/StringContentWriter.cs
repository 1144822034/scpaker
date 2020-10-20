using System.Collections.Generic;
using System.IO;

namespace Engine.Content
{
	[ContentWriter("System.String")]
	public class StringContentWriter : IContentWriter
	{
		public string Text;

		public IEnumerable<string> GetDependencies()
		{
			yield return Text;
		}

		public void Write(string projectDirectory, Stream stream)
		{
			string value = Storage.ReadAllText(Storage.CombinePaths(projectDirectory, Text));
			new BinaryWriter(stream).Write(value);
		}
	}
}
