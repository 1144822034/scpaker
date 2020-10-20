using System.IO;

namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkIHDR : PngChunkSingle
	{
		public const string ID = "IHDR";

		public int Cols
		{
			get;
			set;
		}

		public int Rows
		{
			get;
			set;
		}

		public int Bitspc
		{
			get;
			set;
		}

		public int Colormodel
		{
			get;
			set;
		}

		public int Compmeth
		{
			get;
			set;
		}

		public int Filmeth
		{
			get;
			set;
		}

		public int Interlaced
		{
			get;
			set;
		}

		public PngChunkIHDR(ImageInfo info)
			: base("IHDR", info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.NA;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = new ChunkRaw(13, ChunkHelper.b_IHDR, alloc: true);
			int num = 0;
			PngHelperInternal.WriteInt4tobytes(Cols, chunkRaw.Data, num);
			num += 4;
			PngHelperInternal.WriteInt4tobytes(Rows, chunkRaw.Data, num);
			num += 4;
			chunkRaw.Data[num++] = (byte)Bitspc;
			chunkRaw.Data[num++] = (byte)Colormodel;
			chunkRaw.Data[num++] = (byte)Compmeth;
			chunkRaw.Data[num++] = (byte)Filmeth;
			chunkRaw.Data[num++] = (byte)Interlaced;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			if (c.Length != 13)
			{
				throw new PngjException("Bad IDHR len " + c.Length.ToString());
			}
			MemoryStream asByteStream = c.GetAsByteStream();
			Cols = PngHelperInternal.ReadInt4(asByteStream);
			Rows = PngHelperInternal.ReadInt4(asByteStream);
			Bitspc = PngHelperInternal.ReadByte(asByteStream);
			Colormodel = PngHelperInternal.ReadByte(asByteStream);
			Compmeth = PngHelperInternal.ReadByte(asByteStream);
			Filmeth = PngHelperInternal.ReadByte(asByteStream);
			Interlaced = PngHelperInternal.ReadByte(asByteStream);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkIHDR pngChunkIHDR = (PngChunkIHDR)other;
			Cols = pngChunkIHDR.Cols;
			Rows = pngChunkIHDR.Rows;
			Bitspc = pngChunkIHDR.Bitspc;
			Colormodel = pngChunkIHDR.Colormodel;
			Compmeth = pngChunkIHDR.Compmeth;
			Filmeth = pngChunkIHDR.Filmeth;
			Interlaced = pngChunkIHDR.Interlaced;
		}
	}
}
