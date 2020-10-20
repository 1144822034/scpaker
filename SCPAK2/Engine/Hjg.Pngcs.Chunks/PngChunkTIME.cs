using System;

namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkTIME : PngChunkSingle
	{
		public const string ID = "tIME";

		public int year;

		public int mon;

		public int day;

		public int hour;

		public int min;

		public int sec;

		public PngChunkTIME(ImageInfo info)
			: base("tIME", info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.NONE;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = createEmptyChunk(7, alloc: true);
			PngHelperInternal.WriteInt2tobytes(year, chunkRaw.Data, 0);
			chunkRaw.Data[2] = (byte)mon;
			chunkRaw.Data[3] = (byte)day;
			chunkRaw.Data[4] = (byte)hour;
			chunkRaw.Data[5] = (byte)min;
			chunkRaw.Data[6] = (byte)sec;
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw chunk)
		{
			if (chunk.Length != 7)
			{
				throw new PngjException("bad chunk " + chunk?.ToString());
			}
			year = PngHelperInternal.ReadInt2fromBytes(chunk.Data, 0);
			mon = PngHelperInternal.ReadInt1fromByte(chunk.Data, 2);
			day = PngHelperInternal.ReadInt1fromByte(chunk.Data, 3);
			hour = PngHelperInternal.ReadInt1fromByte(chunk.Data, 4);
			min = PngHelperInternal.ReadInt1fromByte(chunk.Data, 5);
			sec = PngHelperInternal.ReadInt1fromByte(chunk.Data, 6);
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkTIME pngChunkTIME = (PngChunkTIME)other;
			year = pngChunkTIME.year;
			mon = pngChunkTIME.mon;
			day = pngChunkTIME.day;
			hour = pngChunkTIME.hour;
			min = pngChunkTIME.min;
			sec = pngChunkTIME.sec;
		}

		public void SetNow(int secsAgo)
		{
			DateTime now = DateTime.Now;
			year = now.Year;
			mon = now.Month;
			day = now.Day;
			hour = now.Hour;
			min = now.Minute;
			sec = now.Second;
		}

		internal void SetYMDHMS(int yearx, int monx, int dayx, int hourx, int minx, int secx)
		{
			year = yearx;
			mon = monx;
			day = dayx;
			hour = hourx;
			min = minx;
			sec = secx;
		}

		public int[] GetYMDHMS()
		{
			return new int[6]
			{
				year,
				mon,
				day,
				hour,
				min,
				sec
			};
		}

		public string GetAsString()
		{
			return string.Format("%04d/%02d/%02d %02d:%02d:%02d", year, mon, day, hour, min, sec);
		}
	}
}
