using System.Collections.Generic;
using System.Text;

namespace Hjg.Pngcs.Chunks
{
	internal class ChunksList
	{
		internal const int CHUNK_GROUP_0_IDHR = 0;

		internal const int CHUNK_GROUP_1_AFTERIDHR = 1;

		internal const int CHUNK_GROUP_2_PLTE = 2;

		internal const int CHUNK_GROUP_3_AFTERPLTE = 3;

		internal const int CHUNK_GROUP_4_IDAT = 4;

		internal const int CHUNK_GROUP_5_AFTERIDAT = 5;

		internal const int CHUNK_GROUP_6_END = 6;

		public List<PngChunk> chunks;

		internal readonly ImageInfo imageInfo;

		internal ChunksList(ImageInfo imfinfo)
		{
			chunks = new List<PngChunk>();
			imageInfo = imfinfo;
		}

		public Dictionary<string, int> GetChunksKeys()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (PngChunk chunk in chunks)
			{
				dictionary[chunk.Id] = ((!dictionary.ContainsKey(chunk.Id)) ? 1 : (dictionary[chunk.Id] + 1));
			}
			return dictionary;
		}

		public List<PngChunk> GetChunks()
		{
			return new List<PngChunk>(chunks);
		}

		internal static List<PngChunk> GetXById(List<PngChunk> list, string id, string innerid)
		{
			if (innerid == null)
			{
				return ChunkHelper.FilterList(list, new ChunkPredicateId(id));
			}
			return ChunkHelper.FilterList(list, new ChunkPredicateId2(id, innerid));
		}

		public void AppendReadChunk(PngChunk chunk, int chunkGroup)
		{
			chunk.ChunkGroup = chunkGroup;
			chunks.Add(chunk);
		}

		public List<PngChunk> GetById(string id)
		{
			return GetById(id, null);
		}

		public List<PngChunk> GetById(string id, string innerid)
		{
			return GetXById(chunks, id, innerid);
		}

		public PngChunk GetById1(string id)
		{
			return GetById1(id, failIfMultiple: false);
		}

		public PngChunk GetById1(string id, bool failIfMultiple)
		{
			return GetById1(id, null, failIfMultiple);
		}

		public PngChunk GetById1(string id, string innerid, bool failIfMultiple)
		{
			List<PngChunk> byId = GetById(id, innerid);
			if (byId.Count == 0)
			{
				return null;
			}
			if (byId.Count > 1 && (failIfMultiple || !byId[0].AllowsMultiple()))
			{
				throw new PngjException("unexpected multiple chunks id=" + id);
			}
			return byId[byId.Count - 1];
		}

		public List<PngChunk> GetEquivalent(PngChunk chunk)
		{
			return ChunkHelper.FilterList(chunks, new ChunkPredicateEquiv(chunk));
		}

		public override string ToString()
		{
			return "ChunkList: read: " + chunks.Count.ToString();
		}

		public string ToStringFull()
		{
			StringBuilder stringBuilder = new StringBuilder(ToString());
			stringBuilder.Append("\n Read:\n");
			foreach (PngChunk chunk in chunks)
			{
				stringBuilder.Append(chunk).Append(" G=" + chunk.ChunkGroup.ToString() + "\n");
			}
			return stringBuilder.ToString();
		}
	}
}
