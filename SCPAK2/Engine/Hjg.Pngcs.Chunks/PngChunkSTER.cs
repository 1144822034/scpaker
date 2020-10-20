namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkSTER : PngChunkSingle
	{
		public const string ID = "sTER";

		public byte Mode
		{
			get;
			set;
		}

		public PngChunkSTER(ImageInfo info)
			: base("sTER", info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = createEmptyChunk(1, alloc: true);
			chunkRaw.Data[0] = Mode;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw chunk)
		{
			if (chunk.Length != 1)
			{
				throw new PngjException("bad chunk length " + chunk?.ToString());
			}
			Mode = chunk.Data[0];
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkSTER pngChunkSTER = (PngChunkSTER)other;
			Mode = pngChunkSTER.Mode;
		}
	}
}
