using System.Collections.Generic;
using System.IO;

namespace Hjg.Pngcs.Chunks
{
	internal class ChunksListForWrite : ChunksList
	{
		public List<PngChunk> queuedChunks;

		public Dictionary<string, int> alreadyWrittenKeys;

		internal ChunksListForWrite(ImageInfo info)
			: base(info)
		{
			queuedChunks = new List<PngChunk>();
			alreadyWrittenKeys = new Dictionary<string, int>();
		}

		public List<PngChunk> GetQueuedById(string id)
		{
			return GetQueuedById(id, null);
		}

		public List<PngChunk> GetQueuedById(string id, string innerid)
		{
			return ChunksList.GetXById(queuedChunks, id, innerid);
		}

		public PngChunk GetQueuedById1(string id, string innerid, bool failIfMultiple)
		{
			List<PngChunk> queuedById = GetQueuedById(id, innerid);
			if (queuedById.Count == 0)
			{
				return null;
			}
			if (queuedById.Count > 1 && (failIfMultiple || !queuedById[0].AllowsMultiple()))
			{
				throw new PngjException("unexpected multiple chunks id=" + id);
			}
			return queuedById[queuedById.Count - 1];
		}

		public PngChunk GetQueuedById1(string id, bool failIfMultiple)
		{
			return GetQueuedById1(id, null, failIfMultiple);
		}

		public PngChunk GetQueuedById1(string id)
		{
			return GetQueuedById1(id, failIfMultiple: false);
		}

		public bool RemoveChunk(PngChunk c)
		{
			return queuedChunks.Remove(c);
		}

		public bool Queue(PngChunk chunk)
		{
			queuedChunks.Add(chunk);
			return true;
		}

		public static bool shouldWrite(PngChunk c, int currentGroup)
		{
			if (currentGroup == 2)
			{
				return c.Id.Equals("PLTE");
			}
			if (currentGroup % 2 == 0)
			{
				throw new PngjOutputException("bad chunk group?");
			}
			int num2;
			int num;
			if (c.mustGoBeforePLTE())
			{
				num2 = (num = 1);
			}
			else if (c.mustGoBeforeIDAT())
			{
				num = 3;
				num2 = ((!c.mustGoAfterPLTE()) ? 1 : 3);
			}
			else
			{
				num = 5;
				num2 = 1;
			}
			int num3 = num;
			if (c.Priority)
			{
				num3 = num2;
			}
			if (ChunkHelper.IsUnknown(c) && c.ChunkGroup > 0)
			{
				num3 = c.ChunkGroup;
			}
			if (currentGroup == num3)
			{
				return true;
			}
			if (currentGroup > num3 && currentGroup <= num)
			{
				return true;
			}
			return false;
		}

		internal int writeChunks(Stream os, int currentGroup)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < queuedChunks.Count; i++)
			{
				PngChunk pngChunk = queuedChunks[i];
				if (shouldWrite(pngChunk, currentGroup))
				{
					if (ChunkHelper.IsCritical(pngChunk.Id) && !pngChunk.Id.Equals("PLTE"))
					{
						throw new PngjOutputException("bad chunk queued: " + pngChunk?.ToString());
					}
					if (alreadyWrittenKeys.ContainsKey(pngChunk.Id) && !pngChunk.AllowsMultiple())
					{
						throw new PngjOutputException("duplicated chunk does not allow multiple: " + pngChunk?.ToString());
					}
					pngChunk.write(os);
					chunks.Add(pngChunk);
					alreadyWrittenKeys[pngChunk.Id] = ((!alreadyWrittenKeys.ContainsKey(pngChunk.Id)) ? 1 : (alreadyWrittenKeys[pngChunk.Id] + 1));
					list.Add(i);
					pngChunk.ChunkGroup = currentGroup;
				}
			}
			for (int num = list.Count - 1; num >= 0; num--)
			{
				queuedChunks.RemoveAt(list[num]);
			}
			return list.Count;
		}

		internal List<PngChunk> GetQueuedChunks()
		{
			return queuedChunks;
		}
	}
}
