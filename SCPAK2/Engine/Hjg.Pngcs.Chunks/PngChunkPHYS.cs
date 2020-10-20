namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkPHYS : PngChunkSingle
	{
		public const string ID = "pHYs";

		public long PixelsxUnitX
		{
			get;
			set;
		}

		public long PixelsxUnitY
		{
			get;
			set;
		}

		public int Units
		{
			get;
			set;
		}

		public PngChunkPHYS(ImageInfo info)
			: base("pHYs", info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = createEmptyChunk(9, alloc: true);
			PngHelperInternal.WriteInt4tobytes((int)PixelsxUnitX, chunkRaw.Data, 0);
			PngHelperInternal.WriteInt4tobytes((int)PixelsxUnitY, chunkRaw.Data, 4);
			chunkRaw.Data[8] = (byte)Units;
			return chunkRaw;
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkPHYS pngChunkPHYS = (PngChunkPHYS)other;
			PixelsxUnitX = pngChunkPHYS.PixelsxUnitX;
			PixelsxUnitY = pngChunkPHYS.PixelsxUnitY;
			Units = pngChunkPHYS.Units;
		}

		public override void ParseFromRaw(ChunkRaw chunk)
		{
			if (chunk.Length != 9)
			{
				throw new PngjException("bad chunk length " + chunk?.ToString());
			}
			PixelsxUnitX = PngHelperInternal.ReadInt4fromBytes(chunk.Data, 0);
			if (PixelsxUnitX < 0)
			{
				PixelsxUnitX += 4294967296L;
			}
			PixelsxUnitY = PngHelperInternal.ReadInt4fromBytes(chunk.Data, 4);
			if (PixelsxUnitY < 0)
			{
				PixelsxUnitY += 4294967296L;
			}
			Units = PngHelperInternal.ReadInt1fromByte(chunk.Data, 8);
		}

		public double GetAsDpi()
		{
			if (Units != 1 || PixelsxUnitX != PixelsxUnitY)
			{
				return -1.0;
			}
			return (double)PixelsxUnitX * 0.0254;
		}

		public double[] GetAsDpi2()
		{
			if (Units == 1)
			{
				return new double[2]
				{
					(double)PixelsxUnitX * 0.0254,
					(double)PixelsxUnitY * 0.0254
				};
			}
			return new double[2]
			{
				-1.0,
				-1.0
			};
		}

		public void SetAsDpi(double dpi)
		{
			Units = 1;
			PixelsxUnitX = (long)(dpi / 0.0254 + 0.5);
			PixelsxUnitY = PixelsxUnitX;
		}

		public void SetAsDpi2(double dpix, double dpiy)
		{
			Units = 1;
			PixelsxUnitX = (long)(dpix / 0.0254 + 0.5);
			PixelsxUnitY = (long)(dpiy / 0.0254 + 0.5);
		}
	}
}
