namespace Hjg.Pngcs.Chunks
{
	internal class ChunkCopyBehaviour
	{
		public static readonly int COPY_NONE = 0;

		public static readonly int COPY_PALETTE = 1;

		public static readonly int COPY_ALL_SAFE = 4;

		public static readonly int COPY_ALL = 8;

		public static readonly int COPY_PHYS = 16;

		public static readonly int COPY_TEXTUAL = 32;

		public static readonly int COPY_TRANSPARENCY = 64;

		public static readonly int COPY_UNKNOWN = 128;

		public static readonly int COPY_ALMOSTALL = 256;
	}
}
