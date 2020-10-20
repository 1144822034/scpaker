using System.IO;

namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkITXT : PngChunkTextVar
	{
		public const string ID = "iTXt";

		public bool compressed;

		public string langTag = "";

		public string translatedTag = "";

		public PngChunkITXT(ImageInfo info)
			: base("iTXt", info)
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
			memoryStream.WriteByte((byte)(compressed ? 1 : 0));
			memoryStream.WriteByte(0);
			ChunkHelper.WriteBytesToStream(memoryStream, ChunkHelper.ToBytes(langTag));
			memoryStream.WriteByte(0);
			ChunkHelper.WriteBytesToStream(memoryStream, ChunkHelper.ToBytesUTF8(translatedTag));
			memoryStream.WriteByte(0);
			byte[] array = ChunkHelper.ToBytesUTF8(val);
			if (compressed)
			{
				array = ChunkHelper.compressBytes(array, compress: true);
			}
			ChunkHelper.WriteBytesToStream(memoryStream, array);
			byte[] array2 = memoryStream.ToArray();
			ChunkRaw chunkRaw = createEmptyChunk(array2.Length, alloc: false);
			chunkRaw.Data = array2;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			int num = 0;
			int[] array = new int[3];
			for (int i = 0; i < c.Data.Length; i++)
			{
				if (c.Data[i] == 0)
				{
					array[num] = i;
					num++;
					if (num == 1)
					{
						i += 2;
					}
					if (num == 3)
					{
						break;
					}
				}
			}
			if (num != 3)
			{
				throw new PngjException("Bad formed PngChunkITXT chunk");
			}
			key = ChunkHelper.ToString(c.Data, 0, array[0]);
			int num2 = array[0] + 1;
			compressed = ((c.Data[num2] != 0) ? true : false);
			num2++;
			if (compressed && c.Data[num2] != 0)
			{
				throw new PngjException("Bad formed PngChunkITXT chunk - bad compression method ");
			}
			langTag = ChunkHelper.ToString(c.Data, num2, array[1] - num2);
			translatedTag = ChunkHelper.ToStringUTF8(c.Data, array[1] + 1, array[2] - array[1] - 1);
			num2 = array[2] + 1;
			if (compressed)
			{
				byte[] x = ChunkHelper.compressBytes(c.Data, num2, c.Data.Length - num2, compress: false);
				val = ChunkHelper.ToStringUTF8(x);
			}
			else
			{
				val = ChunkHelper.ToStringUTF8(c.Data, num2, c.Data.Length - num2);
			}
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkITXT pngChunkITXT = (PngChunkITXT)other;
			key = pngChunkITXT.key;
			val = pngChunkITXT.val;
			compressed = pngChunkITXT.compressed;
			langTag = pngChunkITXT.langTag;
			translatedTag = pngChunkITXT.translatedTag;
		}

		public bool IsCompressed()
		{
			return compressed;
		}

		public void SetCompressed(bool compressed)
		{
			this.compressed = compressed;
		}

		public string GetLangtag()
		{
			return langTag;
		}

		public void SetLangtag(string langtag)
		{
			langTag = langtag;
		}

		public string GetTranslatedTag()
		{
			return translatedTag;
		}

		public void SetTranslatedTag(string translatedTag)
		{
			this.translatedTag = translatedTag;
		}
	}
}
