namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkSRGB : PngChunkSingle
	{
		public const string ID = "sRGB";

		public const int RENDER_INTENT_Perceptual = 0;

		public const int RENDER_INTENT_Relative_colorimetric = 1;

		public const int RENDER_INTENT_Saturation = 2;

		public const int RENDER_INTENT_Absolute_colorimetric = 3;

		public int Intent
		{
			get;
			set;
		}

		public PngChunkSRGB(ImageInfo info)
			: base("sRGB", info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = createEmptyChunk(1, alloc: true);
			chunkRaw.Data[0] = (byte)Intent;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			if (c.Length != 1)
			{
				throw new PngjException("bad chunk length " + c?.ToString());
			}
			Intent = PngHelperInternal.ReadInt1fromByte(c.Data, 0);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkSRGB pngChunkSRGB = (PngChunkSRGB)other;
			Intent = pngChunkSRGB.Intent;
		}
	}
}
