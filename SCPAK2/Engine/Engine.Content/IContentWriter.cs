using System.Collections.Generic;
using System.IO;

namespace Engine.Content
{
	public interface IContentWriter
	{
		IEnumerable<string> GetDependencies();

		void Write(string projectDirectory, Stream stream);
	}
}
