using System.IO;

namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkZTXT : PngChunkTextVar
	{
		public const string ID = "zTXt";

		public PngChunkZTXT(ImageInfo info)
			: base("zTXt", info)
		{
		}

		public override ChunkRaw CreateRawChunk()
		{
			if (key.Length == 0)
			{
				throw new PngjException("Text chunk key must be non empty");
			}
			MemoryStream memoryStream = new MemoryStream();
			ChunkHelper.WriteBytesToStream(memoryStream, ChunkHelper.ToBytes(key));
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(0);
			byte[] bytes = ChunkHelper.compressBytes(ChunkHelper.ToBytes(val), compress: true);
			ChunkHelper.WriteBytesToStream(memoryStream, bytes);
			byte[] array = memoryStream.ToArray();
			ChunkRaw chunkRaw = createEmptyChunk(array.Length, alloc: false);
			chunkRaw.Data = array;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			int num = -1;
			for (int i = 0; i < c.Data.Length; i++)
			{
				if (c.Data[i] == 0)
				{
					num = i;
					break;
				}
			}
			if (num < 0 || num > c.Data.Length - 2)
			{
				throw new PngjException("bad zTXt chunk: no separator found");
			}
			key = ChunkHelper.ToString(c.Data, 0, num);
			if (c.Data[num + 1] != 0)
			{
				throw new PngjException("bad zTXt chunk: unknown compression method");
			}
			byte[] x = ChunkHelper.compressBytes(c.Data, num + 2, c.Data.Length - num - 2, compress: false);
			val = ChunkHelper.ToString(x);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkZTXT pngChunkZTXT = (PngChunkZTXT)other;
			key = pngChunkZTXT.key;
			val = pngChunkZTXT.val;
		}
	}
}
