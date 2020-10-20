using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Hjg.Pngcs.Chunks
{
	internal abstract class PngChunk
	{
		public enum ChunkOrderingConstraint
		{
			NONE,
			BEFORE_PLTE_AND_IDAT,
			AFTER_PLTE_BEFORE_IDAT,
			BEFORE_IDAT,
			NA
		}

		public readonly string Id;

		public readonly bool Crit;

		public readonly bool Pub;

		public readonly bool Safe;

		public readonly ImageInfo ImgInfo;

		public static Dictionary<string, Type> factoryMap = initFactory();

		public bool Priority
		{
			get;
			set;
		}

		public int ChunkGroup
		{
			get;
			set;
		}

		public int Length
		{
			get;
			set;
		}

		public long Offset
		{
			get;
			set;
		}

		public PngChunk(string id, ImageInfo imgInfo)
		{
			Id = id;
			ImgInfo = imgInfo;
			Crit = ChunkHelper.IsCritical(id);
			Pub = ChunkHelper.IsPublic(id);
			Safe = ChunkHelper.IsSafeToCopy(id);
			Priority = false;
			ChunkGroup = -1;
			Length = -1;
			Offset = 0L;
		}

		public static Dictionary<string, Type> initFactory()
		{
			return new Dictionary<string, Type>
			{
				{
					"IDAT",
					typeof(PngChunkIDAT)
				},
				{
					"IHDR",
					typeof(PngChunkIHDR)
				},
				{
					"PLTE",
					typeof(PngChunkPLTE)
				},
				{
					"IEND",
					typeof(PngChunkIEND)
				},
				{
					"tEXt",
					typeof(PngChunkTEXT)
				},
				{
					"iTXt",
					typeof(PngChunkITXT)
				},
				{
					"zTXt",
					typeof(PngChunkZTXT)
				},
				{
					"bKGD",
					typeof(PngChunkBKGD)
				},
				{
					"gAMA",
					typeof(PngChunkGAMA)
				},
				{
					"pHYs",
					typeof(PngChunkPHYS)
				},
				{
					"iCCP",
					typeof(PngChunkICCP)
				},
				{
					"tIME",
					typeof(PngChunkTIME)
				},
				{
					"tRNS",
					typeof(PngChunkTRNS)
				},
				{
					"cHRM",
					typeof(PngChunkCHRM)
				},
				{
					"sBIT",
					typeof(PngChunkSBIT)
				},
				{
					"sRGB",
					typeof(PngChunkSRGB)
				},
				{
					"hIST",
					typeof(PngChunkHIST)
				},
				{
					"sPLT",
					typeof(PngChunkSPLT)
				},
				{
					"oFFs",
					typeof(PngChunkOFFS)
				},
				{
					"sTER",
					typeof(PngChunkSTER)
				}
			};
		}

		public static void FactoryRegister(string chunkId, Type type)
		{
			factoryMap.Add(chunkId, type);
		}

		internal static bool isKnown(string id)
		{
			return factoryMap.ContainsKey(id);
		}

		internal bool mustGoBeforePLTE()
		{
			return GetOrderingConstraint() == ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
		}

		internal bool mustGoBeforeIDAT()
		{
			ChunkOrderingConstraint orderingConstraint = GetOrderingConstraint();
			if (orderingConstraint != ChunkOrderingConstraint.BEFORE_IDAT && orderingConstraint != ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT)
			{
				return orderingConstraint == ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
			}
			return true;
		}

		internal bool mustGoAfterPLTE()
		{
			return GetOrderingConstraint() == ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
		}

		internal static PngChunk Factory(ChunkRaw chunk, ImageInfo info)
		{
			PngChunk pngChunk = FactoryFromId(ChunkHelper.ToString(chunk.IdBytes), info);
			pngChunk.Length = chunk.Length;
			pngChunk.ParseFromRaw(chunk);
			return pngChunk;
		}

		internal static PngChunk FactoryFromId(string cid, ImageInfo info)
		{
			PngChunk pngChunk = null;
			if (factoryMap == null)
			{
				initFactory();
			}
			if (isKnown(cid))
			{
				pngChunk = (PngChunk)factoryMap[cid].GetTypeInfo().DeclaredConstructors.First().Invoke(new object[1]
				{
					info
				});
			}
			if (pngChunk == null)
			{
				pngChunk = new PngChunkUNKNOWN(cid, info);
			}
			return pngChunk;
		}

		public ChunkRaw createEmptyChunk(int len, bool alloc)
		{
			return new ChunkRaw(len, ChunkHelper.ToBytes(Id), alloc);
		}

		public static T CloneChunk<T>(T chunk, ImageInfo info) where T : PngChunk
		{
			PngChunk pngChunk = FactoryFromId(chunk.Id, info);
			if ((object)pngChunk.GetType() != chunk.GetType())
			{
				throw new PngjException("bad class cloning chunk: " + pngChunk.GetType()?.ToString() + " " + chunk.GetType()?.ToString());
			}
			pngChunk.CloneDataFromRead(chunk);
			return (T)pngChunk;
		}

		internal void write(Stream os)
		{
			(CreateRawChunk() ?? throw new PngjException("null chunk ! creation failed for " + this?.ToString())).WriteChunk(os);
		}

		public override string ToString()
		{
			return "chunk id= " + Id + " (len=" + Length.ToString() + " off=" + Offset.ToString() + ") c=" + GetType().Name;
		}

		public abstract ChunkRaw CreateRawChunk();

		public abstract void ParseFromRaw(ChunkRaw c);

		public abstract void CloneDataFromRead(PngChunk other);

		public abstract bool AllowsMultiple();

		public abstract ChunkOrderingConstraint GetOrderingConstraint();
	}
}
