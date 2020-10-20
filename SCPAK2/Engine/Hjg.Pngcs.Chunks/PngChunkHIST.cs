using System;

namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkHIST : PngChunkSingle
	{
		public static readonly string ID = "hIST";

		public int[] hist = new int[0];

		public PngChunkHIST(ImageInfo info)
			: base(ID, info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = null;
			if (!ImgInfo.Indexed)
			{
				throw new PngjException("only indexed images accept a HIST chunk");
			}
			chunkRaw = createEmptyChunk(hist.Length * 2, alloc: true);
			for (int i = 0; i < hist.Length; i++)
			{
				PngHelperInternal.WriteInt2tobytes(hist[i], chunkRaw.Data, i * 2);
			}
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			if (!ImgInfo.Indexed)
			{
				throw new PngjException("only indexed images accept a HIST chunk");
			}
			int num = c.Data.Length / 2;
			hist = new int[num];
			for (int i = 0; i < hist.Length; i++)
			{
				hist[i] = PngHelperInternal.ReadInt2fromBytes(c.Data, i * 2);
			}
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkHIST pngChunkHIST = (PngChunkHIST)other;
			hist = new int[pngChunkHIST.hist.Length];
			Array.Copy(pngChunkHIST.hist, 0, hist, 0, pngChunkHIST.hist.Length);
		}

		public int[] GetHist()
		{
			return hist;
		}

		public void SetHist(int[] hist)
		{
			this.hist = hist;
		}
	}
}
