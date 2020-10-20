using System.Collections.Generic;
using System.IO;

namespace Engine.Content
{
	[ContentWriter("Engine.Media.StreamingSource")]
	public class StreamingSourceContentWriter : IContentWriter
	{
		public string Sound;

		public IEnumerable<string> GetDependencies()
		{
			yield return Sound;
		}

		public void Write(string projectDirectory, Stream stream)
		{
			using (Stream stream2 = Storage.OpenFile(Storage.CombinePaths(projectDirectory, Sound), OpenFileMode.Read))
			{
				stream2.CopyTo(stream);
			}
		}
	}
}
