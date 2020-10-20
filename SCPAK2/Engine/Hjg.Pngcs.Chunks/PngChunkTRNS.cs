using System;

namespace Hjg.Pngcs.Chunks
{
	internal class PngChunkTRNS : PngChunkSingle
	{
		public const string ID = "tRNS";

		public int gray;

		public int red;

		public int green;

		public int blue;

		public int[] paletteAlpha;

		public PngChunkTRNS(ImageInfo info)
			: base("tRNS", info)
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint()
		{
			return ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk()
		{
			ChunkRaw chunkRaw = null;
			if (ImgInfo.Greyscale)
			{
				chunkRaw = createEmptyChunk(2, alloc: true);
				PngHelperInternal.WriteInt2tobytes(gray, chunkRaw.Data, 0);
			}
			else if (ImgInfo.Indexed)
			{
				chunkRaw = createEmptyChunk(paletteAlpha.Length, alloc: true);
				for (int i = 0; i < chunkRaw.Length; i++)
				{
					chunkRaw.Data[i] = (byte)paletteAlpha[i];
				}
			}
			else
			{
				chunkRaw = createEmptyChunk(6, alloc: true);
				PngHelperInternal.WriteInt2tobytes(red, chunkRaw.Data, 0);
				PngHelperInternal.WriteInt2tobytes(green, chunkRaw.Data, 0);
				PngHelperInternal.WriteInt2tobytes(blue, chunkRaw.Data, 0);
			}
			return chunkRaw;
		}

		public override void ParseFromRaw(ChunkRaw c)
		{
			if (ImgInfo.Greyscale)
			{
				gray = PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
			}
			else if (ImgInfo.Indexed)
			{
				int num = c.Data.Length;
				paletteAlpha = new int[num];
				for (int i = 0; i < num; i++)
				{
					paletteAlpha[i] = (c.Data[i] & 0xFF);
				}
			}
			else
			{
				red = PngHelperInternal.ReadInt2fromBytes(c.Data, 0);
				green = PngHelperInternal.ReadInt2fromBytes(c.Data, 2);
				blue = PngHelperInternal.ReadInt2fromBytes(c.Data, 4);
			}
		}

		public override void CloneDataFromRead(PngChunk other)
		{
			PngChunkTRNS pngChunkTRNS = (PngChunkTRNS)other;
			gray = pngChunkTRNS.gray;
			red = pngChunkTRNS.red;
			green = pngChunkTRNS.red;
			blue = pngChunkTRNS.red;
			if (pngChunkTRNS.paletteAlpha != null)
			{
				paletteAlpha = new int[pngChunkTRNS.paletteAlpha.Length];
				Array.Copy(pngChunkTRNS.paletteAlpha, 0, paletteAlpha, 0, paletteAlpha.Length);
			}
		}

		public void SetRGB(int r, int g, int b)
		{
			if (ImgInfo.Greyscale || ImgInfo.Indexed)
			{
				throw new PngjException("only rgb or rgba images support this");
			}
			red = r;
			green = g;
			blue = b;
		}

		public int[] GetRGB()
		{
			if (ImgInfo.Greyscale || ImgInfo.Indexed)
			{
				throw new PngjException("only rgb or rgba images support this");
			}
			return new int[3]
			{
				red,
				green,
				blue
			};
		}

		public void SetGray(int g)
		{
			if (!ImgInfo.Greyscale)
			{
				throw new PngjException("only grayscale images support this");
			}
			gray = g;
		}

		public int GetGray()
		{
			if (!ImgInfo.Greyscale)
			{
				throw new PngjException("only grayscale images support this");
			}
			return gray;
		}

		public void SetPalletteAlpha(int[] palAlpha)
		{
			if (!ImgInfo.Indexed)
			{
				throw new PngjException("only indexed images support this");
			}
			paletteAlpha = palAlpha;
		}

		public void setIndexEntryAsTransparent(int palAlphaIndex)
		{
			if (!ImgInfo.Indexed)
			{
				throw new PngjException("only indexed images support this");
			}
			paletteAlpha = new int[1]
			{
				palAlphaIndex + 1
			};
			for (int i = 0; i < palAlphaIndex; i++)
			{
				paletteAlpha[i] = 255;
			}
			paletteAlpha[palAlphaIndex] = 0;
		}

		public int[] GetPalletteAlpha()
		{
			if (!ImgInfo.Indexed)
			{
				throw new PngjException("only indexed images support this");
			}
			return paletteAlpha;
		}
	}
}
