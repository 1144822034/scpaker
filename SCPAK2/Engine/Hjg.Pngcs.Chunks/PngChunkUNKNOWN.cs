using System;

namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkUNKNOWN : PngChunkMultiple
	{
		public byte[] data;

		public PngChunkUNKNOWN(string id, ImageInfo info)
			: base(id, info)
		{
		}

		public PngChunkUNKNOWN(PngChunkUNKNOWN c, ImageInfo info)
			: base(c.Id, info)
		{
			Array.Copy(c.data, 0, data, 0, c.data.Length);
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.NONE;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = createEmptyChunk(data.Length, alloc: false);
			chunkRaw.Data = data;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			data = c.Data;
		}

		public byte[] GetData()
		{
			return data;
		}

		public void SetData(byte[] data_0)
		{
			data = data_0;
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkUNKNOWN pngChunkUNKNOWN = (PngChunkUNKNOWN)other;
			data = pngChunkUNKNOWN.data;
		}
	}
}
