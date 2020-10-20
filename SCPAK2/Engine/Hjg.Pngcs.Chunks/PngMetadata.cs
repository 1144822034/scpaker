using System.Collections.Generic;

namespace Hjg.Pngcs.Chunks
{
	internal class PngMetadata
	{
		public readonly ChunksList chunkList;

		public readonly bool ReadOnly;

		internal PngMetadata(ChunksList chunks)
		{
			chunkList = chunks;
			if (chunks is ChunksListForWrite)
			{
				ReadOnly = false;
			}
			else
			{
				ReadOnly = true;
			}
		}

		public void QueueChunk(PngChunk chunk, bool lazyOverwrite)
		{
			ChunksListForWrite chunkListW = getChunkListW();
			if (ReadOnly)
			{
				throw new PngjException("cannot set chunk : readonly metadata");
			}
			if (lazyOverwrite)
			{
				ChunkHelper.TrimList(chunkListW.GetQueuedChunks(), new ChunkPredicateEquiv(chunk));
			}
			chunkListW.Queue(chunk);
		}

		public void QueueChunk(PngChunk chunk)
		{
			QueueChunk(chunk, lazyOverwrite: true);
		}

		public ChunksListForWrite getChunkListW()
		{
			return (ChunksListForWrite)chunkList;
		}

		public double[] GetDpi()
		{
			PngChunk byId = chunkList.GetById1("pHYs", failIfMultiple: true);
			if (byId == null)
			{
				return new double[2]
				{
					-1.0,
					-1.0
				};
			}
			return ((PngChunkPHYS)byId).GetAsDpi2();
		}

		public void SetDpi(double dpix, double dpiy)
		{
			PngChunkPHYS pngChunkPHYS = new PngChunkPHYS(chunkList.imageInfo);
			pngChunkPHYS.SetAsDpi2(dpix, dpiy);
			QueueChunk(pngChunkPHYS);
		}

		public void SetDpi(double dpi)
		{
			SetDpi(dpi, dpi);
		}

		public PngChunkTIME SetTimeNow(int nsecs)
		{
			PngChunkTIME pngChunkTIME = new PngChunkTIME(chunkList.imageInfo);
			pngChunkTIME.SetNow(nsecs);
			QueueChunk(pngChunkTIME);
			return pngChunkTIME;
		}

		public PngChunkTIME SetTimeNow()
		{
			return SetTimeNow(0);
		}

		public PngChunkTIME SetTimeYMDHMS(int year, int mon, int day, int hour, int min, int sec)
		{
			PngChunkTIME pngChunkTIME = new PngChunkTIME(chunkList.imageInfo);
			pngChunkTIME.SetYMDHMS(year, mon, day, hour, min, sec);
			QueueChunk(pngChunkTIME, lazyOverwrite: true);
			return pngChunkTIME;
		}

		public PngChunkTIME GetTime()
		{
			return (PngChunkTIME)chunkList.GetById1("tIME");
		}

		public string GetTimeAsString()
		{
			PngChunkTIME time = GetTime();
			if (time != null)
			{
				return time.GetAsString();
			}
			return "";
		}

		public PngChunkTextVar SetText(string key, string val, bool useLatin1, bool compress)
		{
			if (compress && !useLatin1)
			{
				throw new PngjException("cannot compress non latin text");
			}
			PngChunkTextVar pngChunkTextVar;
			if (useLatin1)
			{
				pngChunkTextVar = ((!compress) ? ((PngChunkTextVar)new PngChunkTEXT(chunkList.imageInfo)) : ((PngChunkTextVar)new PngChunkZTXT(chunkList.imageInfo)));
			}
			else
			{
				pngChunkTextVar = new PngChunkITXT(chunkList.imageInfo);
				((PngChunkITXT)pngChunkTextVar).SetLangtag(key);
			}
			pngChunkTextVar.SetKeyVal(key, val);
			QueueChunk(pngChunkTextVar, lazyOverwrite: true);
			return pngChunkTextVar;
		}

		public PngChunkTextVar SetText(string key, string val)
		{
			return SetText(key, val, useLatin1: false, compress: false);
		}

		public List<PngChunkTextVar> GetTxtsForKey(string key)
		{
			List<PngChunkTextVar> list = new List<PngChunkTextVar>();
			foreach (PngChunk item in chunkList.GetById("tEXt", key))
			{
				list.Add((PngChunkTextVar)item);
			}
			foreach (PngChunk item2 in chunkList.GetById("zTXt", key))
			{
				list.Add((PngChunkTextVar)item2);
			}
			foreach (PngChunk item3 in chunkList.GetById("iTXt", key))
			{
				list.Add((PngChunkTextVar)item3);
			}
			return list;
		}

		public string GetTxtForKey(string key)
		{
			string text = "";
			List<PngChunkTextVar> txtsForKey = GetTxtsForKey(key);
			if (txtsForKey.Count == 0)
			{
				return text;
			}
			foreach (PngChunkTextVar item in txtsForKey)
			{
				text = text + item.GetVal() + "\n";
			}
			return text.Trim();
		}

		public PngChunkPLTE GetPLTE()
		{
			return (PngChunkPLTE)chunkList.GetById1("PLTE");
		}

		public PngChunkPLTE CreatePLTEChunk()
		{
			PngChunkPLTE pngChunkPLTE = new PngChunkPLTE(chunkList.imageInfo);
			QueueChunk(pngChunkPLTE);
			return pngChunkPLTE;
		}

		public PngChunkTRNS GetTRNS()
		{
			return (PngChunkTRNS)chunkList.GetById1("tRNS");
		}

		public PngChunkTRNS CreateTRNSChunk()
		{
			PngChunkTRNS pngChunkTRNS = new PngChunkTRNS(chunkList.imageInfo);
			QueueChunk(pngChunkTRNS);
			return pngChunkTRNS;
		}
	}
}
