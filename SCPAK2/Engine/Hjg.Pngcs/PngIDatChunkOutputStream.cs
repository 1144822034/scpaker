using Hjg.Pngcs.Chunks;
using System.IO;

namespace Hjg.Pngcs
{
	internal class PngIDatChunkOutputStream : ProgressiveOutputStream
	{
		public const int SIZE_DEFAULT = 32768;

		public readonly Stream outputStream;

		public PngIDatChunkOutputStream(Stream outputStream_0)
			: this(outputStream_0, 32768)
		{
		}

		public PngIDatChunkOutputStream(Stream outputStream_0, int size)
			: base((size > 8) ? size : 32768)
		{
			outputStream = outputStream_0;
		}

		public override void FlushBuffer(byte[] b, int len)
		{
			ChunkRaw chunkRaw = new ChunkRaw(len, ChunkHelper.b_IDAT, alloc: false);
			chunkRaw.Data = b;
			chunkRaw.WriteChunk(outputStream);
		}

		public override void Close()
		{
			Flush();
		}
	}
}
