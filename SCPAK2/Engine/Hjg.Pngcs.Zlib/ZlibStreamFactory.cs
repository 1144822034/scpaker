using System.IO;

namespace Hjg.Pngcs.Zlib
{
	internal class ZlibStreamFactory
	{
		public static AZlibInputStream createZlibInputStream(Stream st, bool leaveOpen)
		{
			return new ZlibInputStreamMs(st, leaveOpen);
		}

		public static AZlibInputStream createZlibInputStream(Stream st)
		{
			return createZlibInputStream(st, leaveOpen: false);
		}

		public static AZlibOutputStream createZlibOutputStream(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen)
		{
			return new ZlibOutputStreamMs(st, compressLevel, strat, leaveOpen);
		}

		public static AZlibOutputStream createZlibOutputStream(Stream st)
		{
			return createZlibOutputStream(st, leaveOpen: false);
		}

		public static AZlibOutputStream createZlibOutputStream(Stream st, bool leaveOpen)
		{
			return createZlibOutputStream(st, 6, EDeflateCompressStrategy.Default, leaveOpen);
		}
	}
}
