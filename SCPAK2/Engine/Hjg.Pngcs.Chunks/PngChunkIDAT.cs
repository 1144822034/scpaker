namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkIDAT : PngChunkMultiple
	{
		public const string ID = "IDAT";

		public PngChunkIDAT(ImageInfo i, int len, long offset)
			: base("IDAT", i)
		{
			base.Length = len;
			base.Offset = offset;
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.NA;
		}

		public override ChunkRaw CreateRawChunk()
		{
			return null;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
		}

		public override void CloneDataFromRead(PngChunk other)
		{
		}
	}
}
