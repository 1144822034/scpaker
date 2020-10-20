namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkIEND : PngChunkSingle
	{
		public const string ID = "IEND";

		public PngChunkIEND(ImageInfo info)
			: base("IEND", info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.NA;
		}

		public override ChunkRaw CreateRawChunk()
		{
			return new ChunkRaw(0, ChunkHelper.b_IEND, alloc: false);
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
		}

		public override void CloneDataFromRead(PngChunk other)
		{
		}
	}
}
