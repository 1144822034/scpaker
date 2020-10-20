namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkBKGD : PngChunkSingle
	{
		public const string ID = "bKGD";

		public int gray;

		public int red;

		public int green;

		public int blue;

		public int paletteIndex;

		public PngChunkBKGD(ImageInfo info)
			: base("bKGD", info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = null;
			if (ImgInfo.Greyscale)
			{
				chunkRaw = createEmptyChunk(2, alloc: true);
				PngHelperInternal.WriteInt2tobytes(gray, chunkRaw.Data, 0);
			}
			else if (ImgInfo.Indexed)
			{
				chunkRaw = createEmptyChunk(1, alloc: true);
				chunkRaw.Data[0] = (byte)paletteIndex;
			}
			else
			{
				chunkRaw = createEmptyChunk(6, alloc: true);
				PngHelperInternal.WriteInt2tobytes(red, chunkRaw.Data, 0);
				PngHelperInternal.WriteInt2tobytes(green, chunkRaw.Data, 0);
				PngHelperInternal.WriteInt2tobytes(blue, chunkRaw.Data, 0);
			}
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			if (ImgInfo.Greyscale)
			{
				gray = PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
				return;
			}
			if (ImgInfo.Indexed)
			{
				paletteIndex = (c.Data[0] & 0xFF);
				return;
			}
			red = PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
			green = PngHelperInternal.ReadInt2fromBytes(c.Data, 2);
			blue = PngHelperInternal.ReadInt2fromBytes(c.Data, 4);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkBKGD pngChunkBKGD = (PngChunkBKGD)other;
			gray = pngChunkBKGD.gray;
			red = pngChunkBKGD.red;
			green = pngChunkBKGD.red;
			blue = pngChunkBKGD.red;
			paletteIndex = pngChunkBKGD.paletteIndex;
		}

		public void SetGray(int gray)
		{
			if (!ImgInfo.Greyscale)
			{
				throw new PngjException("only gray images support this");
			}
			this.gray = gray;
		}

		public int GetGray()
		{
			if (!ImgInfo.Greyscale)
			{
				throw new PngjException("only gray images support this");
			}
			return gray;
		}

		public void SetPaletteIndex(int index)
		{
			if (!ImgInfo.Indexed)
			{
				throw new PngjException("only indexed (pallete) images support this");
			}
			paletteIndex = index;
		}

		public int GetPaletteIndex()
		{
			if (!ImgInfo.Indexed)
			{
				throw new PngjException("only indexed (pallete) images support this");
			}
			return paletteIndex;
		}

		public void SetRGB(int r, int g, int b)
		{
			if (ImgInfo.Greyscale || ImgInfo.Indexed)
			{
				throw new PngjException("only rgb or rgba images support this");
			}
			red = r;
			green = g;
			blue = b;
		}

		public int[] GetRGB()
		{
			if (ImgInfo.Greyscale || ImgInfo.Indexed)
			{
				throw new PngjException("only rgb or rgba images support this");
			}
			return new int[3]
			{
				red,
				green,
				blue
			};
		}
	}
}
