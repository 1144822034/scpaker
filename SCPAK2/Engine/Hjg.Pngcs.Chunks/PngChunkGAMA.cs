namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkGAMA : PngChunkSingle
	{
		public const string ID = "gAMA";

		public double gamma;

		public PngChunkGAMA(ImageInfo info)
			: base("gAMA", info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = createEmptyChunk(4, alloc: true);
			PngHelperInternal.WriteInt4tobytes((int)(gamma * 100000.0 + 0.5), chunkRaw.Data, 0);
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw chunk)
		{
			if (chunk.Length != 4)
			{
				throw new PngjException("bad chunk " + chunk?.ToString());
			}
			int num = PngHelperInternal.ReadInt4fromBytes(chunk.Data, 0);
			gamma = (double)num / 100000.0;
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			gamma = ((PngChunkGAMA)other).gamma;
		}

		public double GetGamma()
		{
			return gamma;
		}

		public void SetGamma(double gamma)
		{
			this.gamma = gamma;
		}
	}
}
