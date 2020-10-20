using System;
using System.IO;

namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkSPLT : PngChunkMultiple
	{
		public const string ID = "sPLT";

		public string PalName
		{
			get;
			set;
		}

		public int SampleDepth
		{
			get;
			set;
		}

		public int[] Palette
		{
			get;
			set;
		}

		public PngChunkSPLT(ImageInfo info)
			: base("sPLT", info)
		{
			PalName = "";
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			MemoryStream memoryStream = new MemoryStream();
			ChunkHelper.WriteBytesToStream(memoryStream, ChunkHelper.ToBytes(PalName));
			memoryStream.WriteByte(0);
			memoryStream.WriteByte((byte)SampleDepth);
			int nentries = GetNentries();
			for (int i = 0; i < nentries; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (SampleDepth == 8)
					{
						PngHelperInternal.WriteByte(memoryStream, (byte)Palette[i * 5 + j]);
					}
					else
					{
						PngHelperInternal.WriteInt2(memoryStream, Palette[i * 5 + j]);
					}
				}
				PngHelperInternal.WriteInt2(memoryStream, Palette[i * 5 + 4]);
			}
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
			if (num <= 0 || num > c.Data.Length - 2)
			{
				throw new PngjException("bad sPLT chunk: no separator found");
			}
			PalName = ChunkHelper.ToString(c.Data, 0, num);
			SampleDepth = PngHelperInternal.ReadInt1fromByte(c.Data, num + 1);
			num += 2;
			int num2 = (c.Data.Length - num) / ((SampleDepth == 8) ? 6 : 10);
			Palette = new int[num2 * 5];
			int num3 = 0;
			for (int j = 0; j < num2; j++)
			{
				int num4;
				int num5;
				int num6;
				int num7;
				if (SampleDepth == 8)
				{
					num4 = PngHelperInternal.ReadInt1fromByte(c.Data, num++);
					num5 = PngHelperInternal.ReadInt1fromByte(c.Data, num++);
					num6 = PngHelperInternal.ReadInt1fromByte(c.Data, num++);
					num7 = PngHelperInternal.ReadInt1fromByte(c.Data, num++);
				}
				else
				{
					num4 = PngHelperInternal.ReadInt2fromBytes(c.Data, num);
					num += 2;
					num5 = PngHelperInternal.ReadInt2fromBytes(c.Data, num);
					num += 2;
					num6 = PngHelperInternal.ReadInt2fromBytes(c.Data, num);
					num += 2;
					num7 = PngHelperInternal.ReadInt2fromBytes(c.Data, num);
					num += 2;
				}
				int num8 = PngHelperInternal.ReadInt2fromBytes(c.Data, num);
				num += 2;
				Palette[num3++] = num4;
				Palette[num3++] = num5;
				Palette[num3++] = num6;
				Palette[num3++] = num7;
				Palette[num3++] = num8;
			}
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkSPLT pngChunkSPLT = (PngChunkSPLT)other;
			PalName = pngChunkSPLT.PalName;
			SampleDepth = pngChunkSPLT.SampleDepth;
			Palette = new int[pngChunkSPLT.Palette.Length];
			Array.Copy(pngChunkSPLT.Palette, 0, Palette, 0, Palette.Length);
		}

		public int GetNentries()
		{
			return Palette.Length / 5;
		}
	}
}
