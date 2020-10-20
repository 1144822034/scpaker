namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkCHRM : PngChunkSingle
	{
		public const string ID = "cHRM";

		public double whitex;

		public double whitey;

		public double redx;

		public double redy;

		public double greenx;

		public double greeny;

		public double bluex;

		public double bluey;

		public PngChunkCHRM(ImageInfo info)
			: base("cHRM", info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = null;
			chunkRaw = createEmptyChunk(32, alloc: true);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(whitex), chunkRaw.Data, 0);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(whitey), chunkRaw.Data, 4);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(redx), chunkRaw.Data, 8);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(redy), chunkRaw.Data, 12);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(greenx), chunkRaw.Data, 16);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(greeny), chunkRaw.Data, 20);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(bluex), chunkRaw.Data, 24);
			PngHelperInternal.WriteInt4tobytes(PngHelperInternal.DoubleToInt100000(bluey), chunkRaw.Data, 28);
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			if (c.Length != 32)
			{
				throw new PngjException("bad chunk " + c?.ToString());
			}
			whitex = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 0));
			whitey = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 4));
			redx = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 8));
			redy = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 12));
			greenx = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 16));
			greeny = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 20));
			bluex = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 24));
			bluey = PngHelperInternal.IntToDouble100000(PngHelperInternal.ReadInt4fromBytes(c.Data, 28));
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkCHRM pngChunkCHRM = (PngChunkCHRM)other;
			whitex = pngChunkCHRM.whitex;
			whitey = pngChunkCHRM.whitex;
			redx = pngChunkCHRM.redx;
			redy = pngChunkCHRM.redy;
			greenx = pngChunkCHRM.greenx;
			greeny = pngChunkCHRM.greeny;
			bluex = pngChunkCHRM.bluex;
			bluey = pngChunkCHRM.bluey;
		}

		public void SetChromaticities(double whitex, double whitey, double redx, double redy, double greenx, double greeny, double bluex, double bluey)
		{
			this.whitex = whitex;
			this.redx = redx;
			this.greenx = greenx;
			this.bluex = bluex;
			this.whitey = whitey;
			this.redy = redy;
			this.greeny = greeny;
			this.bluey = bluey;
		}

		public double[] GetChromaticities()
		{
			return new double[8]
			{
				whitex,
				whitey,
				redx,
				redy,
				greenx,
				greeny,
				bluex,
				bluey
			};
		}
	}
}
