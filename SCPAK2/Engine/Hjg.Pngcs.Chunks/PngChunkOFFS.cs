namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkOFFS : PngChunkSingle
	{
		public const string ID = "oFFs";

		public long posX;

		public long posY;

		public int units;

		public PngChunkOFFS(ImageInfo info)
			: base("oFFs", info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = createEmptyChunk(9, alloc: true);
			PngHelperInternal.WriteInt4tobytes((int)posX, chunkRaw.Data, 0);
			PngHelperInternal.WriteInt4tobytes((int)posY, chunkRaw.Data, 4);
			chunkRaw.Data[8] = (byte)units;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw chunk)
		{
			if (chunk.Length != 9)
			{
				throw new PngjException("bad chunk length " + chunk?.ToString());
			}
			posX = PngHelperInternal.ReadInt4fromBytes(chunk.Data, 0);
			if (posX < 0)
			{
				posX += 4294967296L;
			}
			posY = PngHelperInternal.ReadInt4fromBytes(chunk.Data, 4);
			if (posY < 0)
			{
				posY += 4294967296L;
			}
			units = PngHelperInternal.ReadInt1fromByte(chunk.Data, 8);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkOFFS pngChunkOFFS = (PngChunkOFFS)other;
			posX = pngChunkOFFS.posX;
			posY = pngChunkOFFS.posY;
			units = pngChunkOFFS.units;
		}

		public int GetUnits()
		{
			return units;
		}

		public void SetUnits(int units)
		{
			this.units = units;
		}

		public long GetPosX()
		{
			return posX;
		}

		public void SetPosX(long posX)
		{
			this.posX = posX;
		}

		public long GetPosY()
		{
			return posY;
		}

		public void SetPosY(long posY)
		{
			this.posY = posY;
		}
	}
}
