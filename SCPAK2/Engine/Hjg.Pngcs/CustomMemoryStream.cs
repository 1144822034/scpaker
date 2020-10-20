using System.IO;

namespace Hjg.Pngcs
{
	internal class CustomMemoryStream : MemoryStream
	{
		public new virtual void Close()
		{
		}
	}
}
