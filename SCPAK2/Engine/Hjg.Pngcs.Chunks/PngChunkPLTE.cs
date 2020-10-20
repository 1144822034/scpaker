using System;

namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkPLTE : PngChunkSingle
	{
		public const string ID = "PLTE";

		public int nentries;

		public int[] entries;

		public PngChunkPLTE(ImageInfo info)
			: base("PLTE", info)
		{
			nentries = 0;
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.NA;
		}

		public override ChunkRaw CreateRawChunk()
		{
			int len = 3 * nentries;
			int[] array = new int[3];
			ChunkRaw chunkRaw = createEmptyChunk(len, alloc: true);
			int i = 0;
			int num = 0;
			for (; i < nentries; i++)
			{
				GetEntryRgb(i, array);
				chunkRaw.Data[num++] = (byte)array[0];
				chunkRaw.Data[num++] = (byte)array[1];
				chunkRaw.Data[num++] = (byte)array[2];
			}
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw chunk)
		{
			SetNentries(chunk.Length / 3);
			int i = 0;
			int num = 0;
			for (; i < nentries; i++)
			{
				SetEntry(i, chunk.Data[num++] & 0xFF, chunk.Data[num++] & 0xFF, chunk.Data[num++] & 0xFF);
			}
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkPLTE pngChunkPLTE = (PngChunkPLTE)other;
			SetNentries(pngChunkPLTE.GetNentries());
			Array.Copy(pngChunkPLTE.entries, 0, entries, 0, nentries);
		}

		public void SetNentries(int nentries)
		{
			this.nentries = nentries;
			if (nentries < 1 || nentries > 256)
			{
				throw new PngjException("invalid pallette - nentries=" + nentries.ToString());
			}
			if (entries == null || entries.Length != nentries)
			{
				entries = new int[nentries];
			}
		}

		public int GetNentries()
		{
			return nentries;
		}

		public void SetEntry(int n, int r, int g, int b)
		{
			entries[n] = ((r << 16) | (g << 8) | b);
		}

		public int GetEntry(int n)
		{
			return entries[n];
		}

		public void GetEntryRgb(int index, int[] rgb, int offset)
		{
			int num = entries[index];
			rgb[offset] = (num & 0xFF0000) >> 16;
			rgb[offset + 1] = (num & 0xFF00) >> 8;
			rgb[offset + 2] = (num & 0xFF);
		}

		public void GetEntryRgb(int n, int[] rgb)
		{
			GetEntryRgb(n, rgb, 0);
		}

		public int MinBitDepth()
		{
			if (nentries <= 2)
			{
				return 1;
			}
			if (nentries <= 4)
			{
				return 2;
			}
			if (nentries <= 16)
			{
				return 4;
			}
			return 8;
		}
	}
}
