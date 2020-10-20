namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkSBIT : PngChunkSingle
	{
		public const string ID = "sBIT";

		public int Graysb
		{
			get;
			set;
		}

		public int Alphasb
		{
			get;
			set;
		}

		public int Redsb
		{
			get;
			set;
		}

		public int Greensb
		{
			get;
			set;
		}

		public int Bluesb
		{
			get;
			set;
		}

		public PngChunkSBIT(ImageInfo info)
			: base("sBIT", info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			if (c.Length != GetLen())
			{
				throw new PngjException("bad chunk length " + c?.ToString());
			}
			if (ImgInfo.Greyscale)
			{
				Graysb = PngHelperInternal.ReadInt1fromByte(c.Data, 0);
				if (ImgInfo.Alpha)
				{
					Alphasb = PngHelperInternal.ReadInt1fromByte(c.Data, 1);
				}
				return;
			}
			Redsb = PngHelperInternal.ReadInt1fromByte(c.Data, 0);
			Greensb = PngHelperInternal.ReadInt1fromByte(c.Data, 1);
			Bluesb = PngHelperInternal.ReadInt1fromByte(c.Data, 2);
			if (ImgInfo.Alpha)
			{
				Alphasb = PngHelperInternal.ReadInt1fromByte(c.Data, 3);
			}
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = null;
			chunkRaw = createEmptyChunk(GetLen(), alloc: true);
			if (ImgInfo.Greyscale)
			{
				chunkRaw.Data[0] = (byte)Graysb;
				if (ImgInfo.Alpha)
				{
					chunkRaw.Data[1] = (byte)Alphasb;
				}
			}
			else
			{
				chunkRaw.Data[0] = (byte)Redsb;
				chunkRaw.Data[1] = (byte)Greensb;
				chunkRaw.Data[2] = (byte)Bluesb;
				if (ImgInfo.Alpha)
				{
					chunkRaw.Data[3] = (byte)Alphasb;
				}
			}
			return chunkRaw;
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkSBIT pngChunkSBIT = (PngChunkSBIT)other;
			Graysb = pngChunkSBIT.Graysb;
			Redsb = pngChunkSBIT.Redsb;
			Greensb = pngChunkSBIT.Greensb;
			Bluesb = pngChunkSBIT.Bluesb;
			Alphasb = pngChunkSBIT.Alphasb;
		}

		public int GetLen()
		{
			int num = ImgInfo.Greyscale ? 1 : 3;
			if (ImgInfo.Alpha)
			{
				num++;
			}
			return num;
		}
	}
}
