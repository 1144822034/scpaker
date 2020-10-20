using System;

namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkTEXT : PngChunkTextVar
	{
		public const string ID = "tEXt";

		public PngChunkTEXT(ImageInfo info)
			: base("tEXt", info)
		{
		}

		public override ChunkRaw CreateRawChunk()
		{
			if (key.Length == 0)
			{
				throw new PngjException("Text chunk key must be non empty");
			}
			byte[] bytes = PngHelperInternal.charsetLatin1.GetBytes(key);
			byte[] bytes2 = PngHelperInternal.charsetLatin1.GetBytes(val);
			ChunkRaw chunkRaw = createEmptyChunk(bytes.Length + bytes2.Length + 1, alloc: true);
			Array.Copy(bytes, 0, chunkRaw.Data, 0, bytes.Length);
			chunkRaw.Data[bytes.Length] = 0;
			Array.Copy(bytes2, 0, chunkRaw.Data, bytes.Length + 1, bytes2.Length);
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			int i;
			for (i = 0; i < c.Data.Length && c.Data[i] != 0; i++)
			{
			}
			key = PngHelperInternal.charsetLatin1.GetString(c.Data, 0, i);
			i++;
			val = ((i < c.Data.Length) ? PngHelperInternal.charsetLatin1.GetString(c.Data, i, c.Data.Length - i) : "");
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkTEXT pngChunkTEXT = (PngChunkTEXT)other;
			key = pngChunkTEXT.key;
			val = pngChunkTEXT.val;
		}
	}
}
