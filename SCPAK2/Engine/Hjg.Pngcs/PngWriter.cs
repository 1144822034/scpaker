using Hjg.Pngcs.Chunks;
using Hjg.Pngcs.Zlib;
using System.Collections.Generic;
using System.IO;

namespace Hjg.Pngcs
{
	internal class PngWriter
	{
		public readonly ImageInfo ImgInfo;

		public readonly string filename;

		public FilterWriteStrategy filterStrat;

		public readonly PngMetadata metadata;

		public readonly ChunksListForWrite chunksList;

		public byte[] rowb;

		public byte[] rowbprev;

		public byte[] rowbfilter;

		public int rowNum = -1;

		public readonly Stream outputStream;

		public PngIDatChunkOutputStream datStream;

		public AZlibOutputStream datStreamDeflated;

		public int[] histox = new int[256];

		public bool unpackedMode;

		public bool needsPack;

		public EDeflateCompressStrategy CompressionStrategy
		{
			get;
			set;
		}

		public int CompLevel
		{
			get;
			set;
		}

		public bool ShouldCloseStream
		{
			get;
			set;
		}

		public int IdatMaxSize
		{
			get;
			set;
		}

		public int CurrentChunkGroup
		{
			get;
			set;
		}

		public PngWriter(Stream outputStream, ImageInfo imgInfo)
			: this(outputStream, imgInfo, "[NO FILENAME AVAILABLE]")
		{
		}

		public PngWriter(Stream outputStream, ImageInfo imgInfo, string filename)
		{
			this.filename = ((filename == null) ? "" : filename);
			this.outputStream = outputStream;
			ImgInfo = imgInfo;
			CompLevel = 6;
			ShouldCloseStream = true;
			IdatMaxSize = 0;
			CompressionStrategy = EDeflateCompressStrategy.Filtered;
			rowb = new byte[imgInfo.BytesPerRow + 1];
			rowbprev = new byte[rowb.Length];
			rowbfilter = new byte[rowb.Length];
			chunksList = new ChunksListForWrite(ImgInfo);
			metadata = new PngMetadata(chunksList);
			filterStrat = new FilterWriteStrategy(ImgInfo, FilterType.FILTER_DEFAULT);
			unpackedMode = false;
			needsPack = (unpackedMode && imgInfo.Packed);
		}

		public void init()
		{
			datStream = new PngIDatChunkOutputStream(outputStream, IdatMaxSize);
			datStreamDeflated = ZlibStreamFactory.createZlibOutputStream(datStream, CompLevel, CompressionStrategy, leaveOpen: true);
			WriteSignatureAndIHDR();
			WriteFirstChunks();
		}

		public void reportResultsForFilter(int rown, FilterType type, bool tentative)
		{
			for (int i = 0; i < histox.Length; i++)
			{
				histox[i] = 0;
			}
			int num = 0;
			for (int j = 1; j <= ImgInfo.BytesPerRow; j++)
			{
				int num2 = rowbfilter[j];
				num = ((num2 >= 0) ? (num + num2) : (num - num2));
				histox[num2 & 0xFF]++;
			}
			filterStrat.fillResultsForFilter(rown, type, num, histox, tentative);
		}

		public void WriteEndChunk()
		{
			new PngChunkIEND(ImgInfo).CreateRawChunk().WriteChunk(outputStream);
		}

		public void WriteFirstChunks()
		{
			CurrentChunkGroup = 1;
			chunksList.writeChunks(outputStream, CurrentChunkGroup);
			CurrentChunkGroup = 2;
			int num = chunksList.writeChunks(outputStream, CurrentChunkGroup);
			if (num > 0 && ImgInfo.Greyscale)
			{
				throw new PngjOutputException("cannot write palette for this format");
			}
			if (num == 0 && ImgInfo.Indexed)
			{
				throw new PngjOutputException("missing palette");
			}
			CurrentChunkGroup = 3;
			chunksList.writeChunks(outputStream, CurrentChunkGroup);
			CurrentChunkGroup = 4;
		}

		public void WriteLastChunks()
		{
			CurrentChunkGroup = 5;
			chunksList.writeChunks(outputStream, CurrentChunkGroup);
			List<PngChunk> queuedChunks = chunksList.GetQueuedChunks();
			if (queuedChunks.Count > 0)
			{
				throw new PngjOutputException(queuedChunks.Count.ToString() + " chunks were not written! Eg: " + queuedChunks[0].ToString());
			}
			CurrentChunkGroup = 6;
		}

		public void WriteSignatureAndIHDR()
		{
			CurrentChunkGroup = 0;
			PngHelperInternal.WriteBytes(outputStream, PngHelperInternal.PNG_ID_SIGNATURE);
			PngChunkIHDR obj = new PngChunkIHDR(ImgInfo)
			{
				Cols = ImgInfo.Cols,
				Rows = ImgInfo.Rows,
				Bitspc = ImgInfo.BitDepth
			};
			int num = 0;
			if (ImgInfo.Alpha)
			{
				num += 4;
			}
			if (ImgInfo.Indexed)
			{
				num++;
			}
			if (!ImgInfo.Greyscale)
			{
				num += 2;
			}
			obj.Colormodel = num;
			obj.Compmeth = 0;
			obj.Filmeth = 0;
			obj.Interlaced = 0;
			obj.CreateRawChunk().WriteChunk(outputStream);
		}

		public void encodeRowFromByte(byte[] row)
		{
			if (row.Length == ImgInfo.SamplesPerRowPacked && !needsPack)
			{
				int num = 1;
				if (ImgInfo.BitDepth <= 8)
				{
					foreach (byte b in row)
					{
						rowb[num++] = b;
					}
					return;
				}
				foreach (byte b2 in row)
				{
					rowb[num] = b2;
					num += 2;
				}
				return;
			}
			if (row.Length >= ImgInfo.SamplesPerRow && needsPack)
			{
				ImageLine.packInplaceByte(ImgInfo, row, row, scaled: false);
			}
			if (ImgInfo.BitDepth <= 8)
			{
				int j = 0;
				int num2 = 1;
				for (; j < ImgInfo.SamplesPerRowPacked; j++)
				{
					rowb[num2++] = row[j];
				}
				return;
			}
			int k = 0;
			int num3 = 1;
			for (; k < ImgInfo.SamplesPerRowPacked; k++)
			{
				rowb[num3++] = row[k];
				rowb[num3++] = 0;
			}
		}

		public void encodeRowFromInt(int[] row)
		{
			if (row.Length == ImgInfo.SamplesPerRowPacked && !needsPack)
			{
				int num = 1;
				if (ImgInfo.BitDepth <= 8)
				{
					foreach (int num2 in row)
					{
						rowb[num++] = (byte)num2;
					}
					return;
				}
				foreach (int num3 in row)
				{
					rowb[num++] = (byte)(num3 >> 8);
					rowb[num++] = (byte)num3;
				}
				return;
			}
			if (row.Length >= ImgInfo.SamplesPerRow && needsPack)
			{
				ImageLine.packInplaceInt(ImgInfo, row, row, scaled: false);
			}
			if (ImgInfo.BitDepth <= 8)
			{
				int j = 0;
				int num4 = 1;
				for (; j < ImgInfo.SamplesPerRowPacked; j++)
				{
					rowb[num4++] = (byte)row[j];
				}
				return;
			}
			int k = 0;
			int num5 = 1;
			for (; k < ImgInfo.SamplesPerRowPacked; k++)
			{
				rowb[num5++] = (byte)(row[k] >> 8);
				rowb[num5++] = (byte)row[k];
			}
		}

		public void FilterRow(int rown)
		{
			if (filterStrat.shouldTestAll(rown))
			{
				FilterRowNone();
				reportResultsForFilter(rown, FilterType.FILTER_NONE, tentative: true);
				FilterRowSub();
				reportResultsForFilter(rown, FilterType.FILTER_SUB, tentative: true);
				FilterRowUp();
				reportResultsForFilter(rown, FilterType.FILTER_UP, tentative: true);
				FilterRowAverage();
				reportResultsForFilter(rown, FilterType.FILTER_AVERAGE, tentative: true);
				FilterRowPaeth();
				reportResultsForFilter(rown, FilterType.FILTER_PAETH, tentative: true);
			}
			FilterType filterType = filterStrat.gimmeFilterType(rown, useEntropy: true);
			rowbfilter[0] = (byte)filterType;
			switch (filterType)
			{
			case FilterType.FILTER_NONE:
				FilterRowNone();
				break;
			case FilterType.FILTER_SUB:
				FilterRowSub();
				break;
			case FilterType.FILTER_UP:
				FilterRowUp();
				break;
			case FilterType.FILTER_AVERAGE:
				FilterRowAverage();
				break;
			case FilterType.FILTER_PAETH:
				FilterRowPaeth();
				break;
			default:
				throw new PngjOutputException("Filter type " + filterType.ToString() + " not implemented");
			}
			reportResultsForFilter(rown, filterType, tentative: false);
		}

		public void prepareEncodeRow(int rown)
		{
			if (datStream == null)
			{
				init();
			}
			rowNum++;
			if (rown >= 0 && rowNum != rown)
			{
				throw new PngjOutputException("rows must be written in order: expected:" + rowNum.ToString() + " passed:" + rown.ToString());
			}
			byte[] array = rowb;
			rowb = rowbprev;
			rowbprev = array;
		}

		public void filterAndSend(int rown)
		{
			FilterRow(rown);
			datStreamDeflated.Write(rowbfilter, 0, ImgInfo.BytesPerRow + 1);
		}

		public void FilterRowAverage()
		{
			int bytesPerRow = ImgInfo.BytesPerRow;
			int num = 1 - ImgInfo.BytesPixel;
			int num2 = 1;
			while (num2 <= bytesPerRow)
			{
				rowbfilter[num2] = (byte)(rowb[num2] - (rowbprev[num2] + ((num > 0) ? rowb[num] : 0)) / 2);
				num2++;
				num++;
			}
		}

		public void FilterRowNone()
		{
			for (int i = 1; i <= ImgInfo.BytesPerRow; i++)
			{
				rowbfilter[i] = rowb[i];
			}
		}

		public void FilterRowPaeth()
		{
			int bytesPerRow = ImgInfo.BytesPerRow;
			int num = 1 - ImgInfo.BytesPixel;
			int num2 = 1;
			while (num2 <= bytesPerRow)
			{
				rowbfilter[num2] = (byte)(rowb[num2] - PngHelperInternal.FilterPaethPredictor((num > 0) ? rowb[num] : 0, rowbprev[num2], (num > 0) ? rowbprev[num] : 0));
				num2++;
				num++;
			}
		}

		public void FilterRowSub()
		{
			int i;
			for (i = 1; i <= ImgInfo.BytesPixel; i++)
			{
				rowbfilter[i] = rowb[i];
			}
			int num = 1;
			i = ImgInfo.BytesPixel + 1;
			while (i <= ImgInfo.BytesPerRow)
			{
				rowbfilter[i] = (byte)(rowb[i] - rowb[num]);
				i++;
				num++;
			}
		}

		public void FilterRowUp()
		{
			for (int i = 1; i <= ImgInfo.BytesPerRow; i++)
			{
				rowbfilter[i] = (byte)(rowb[i] - rowbprev[i]);
			}
		}

		public long SumRowbfilter()
		{
			long num = 0L;
			for (int i = 1; i <= ImgInfo.BytesPerRow; i++)
			{
				num = ((rowbfilter[i] >= 0) ? (num + rowbfilter[i]) : (num - rowbfilter[i]));
			}
			return num;
		}

		public void CopyChunks(PngReader reader, int copy_mask, bool onlyAfterIdat)
		{
			bool flag = CurrentChunkGroup >= 4;
			if (onlyAfterIdat && reader.CurrentChunkGroup < 6)
			{
				throw new PngjException("tried to copy last chunks but reader has not ended");
			}
			foreach (PngChunk chunk in reader.GetChunksList().GetChunks())
			{
				if (chunk.ChunkGroup >= 4 || !flag)
				{
					bool flag2 = false;
					if (chunk.Crit)
					{
						if (chunk.Id.Equals("PLTE"))
						{
							if (ImgInfo.Indexed && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_PALETTE))
							{
								flag2 = true;
							}
							if (!ImgInfo.Greyscale && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_ALL))
							{
								flag2 = true;
							}
						}
					}
					else
					{
						bool flag3 = chunk is PngChunkTextVar;
						bool safe = chunk.Safe;
						if (ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_ALL))
						{
							flag2 = true;
						}
						if (safe && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_ALL_SAFE))
						{
							flag2 = true;
						}
						if (chunk.Id.Equals("tRNS") && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_TRANSPARENCY))
						{
							flag2 = true;
						}
						if (chunk.Id.Equals("pHYs") && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_PHYS))
						{
							flag2 = true;
						}
						if (flag3 && ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_TEXTUAL))
						{
							flag2 = true;
						}
						if (ChunkHelper.maskMatch(copy_mask, ChunkCopyBehaviour.COPY_ALMOSTALL) && !(ChunkHelper.IsUnknown(chunk) | flag3) && !chunk.Id.Equals("hIST") && !chunk.Id.Equals("tIME"))
						{
							flag2 = true;
						}
						if (chunk is PngChunkSkipped)
						{
							flag2 = false;
						}
					}
					if (flag2)
					{
						chunksList.Queue(PngChunk.CloneChunk(chunk, ImgInfo));
					}
				}
			}
		}

		public void CopyChunksFirst(PngReader reader, int copy_mask)
		{
			CopyChunks(reader, copy_mask, onlyAfterIdat: false);
		}

		public void CopyChunksLast(PngReader reader, int copy_mask)
		{
			CopyChunks(reader, copy_mask, onlyAfterIdat: true);
		}

		public double ComputeCompressionRatio()
		{
			if (CurrentChunkGroup < 6)
			{
				throw new PngjException("must be called after End()");
			}
			double num = datStream.GetCountFlushed();
			double num2 = (ImgInfo.BytesPerRow + 1) * ImgInfo.Rows;
			return num / num2;
		}

		public void End()
		{
			if (rowNum != ImgInfo.Rows - 1)
			{
				throw new PngjOutputException("all rows have not been written");
			}
			try
			{
				datStreamDeflated.Dispose();
				datStream.Close();
				WriteLastChunks();
				WriteEndChunk();
				if (ShouldCloseStream)
				{
					outputStream.Dispose();
				}
			}
			catch (IOException cause)
			{
				throw new PngjOutputException(cause);
			}
		}

		public string GetFilename()
		{
			return filename;
		}

		public void WriteRow(ImageLine imgline, int rownumber)
		{
			SetUseUnPackedMode(imgline.SamplesUnpacked);
			if (imgline.SampleType == ImageLine.ESampleType.INT)
			{
				WriteRowInt(imgline.Scanline, rownumber);
			}
			else
			{
				WriteRowByte(imgline.ScanlineB, rownumber);
			}
		}

		public void WriteRow(int[] newrow)
		{
			WriteRow(newrow, -1);
		}

		public void WriteRow(int[] newrow, int rown)
		{
			WriteRowInt(newrow, rown);
		}

		public void WriteRowInt(int[] newrow, int rown)
		{
			prepareEncodeRow(rown);
			encodeRowFromInt(newrow);
			filterAndSend(rown);
		}

		public void WriteRowByte(byte[] newrow, int rown)
		{
			prepareEncodeRow(rown);
			encodeRowFromByte(newrow);
			filterAndSend(rown);
		}

		public void WriteRowsInt(int[][] image)
		{
			for (int i = 0; i < ImgInfo.Rows; i++)
			{
				WriteRowInt(image[i], i);
			}
		}

		public void WriteRowsByte(byte[][] image)
		{
			for (int i = 0; i < ImgInfo.Rows; i++)
			{
				WriteRowByte(image[i], i);
			}
		}

		public PngMetadata GetMetadata()
		{
			return metadata;
		}

		public ChunksListForWrite GetChunksList()
		{
			return chunksList;
		}

		public void SetFilterType(FilterType filterType)
		{
			filterStrat = new FilterWriteStrategy(ImgInfo, filterType);
		}

		public bool IsUnpackedMode()
		{
			return unpackedMode;
		}

		public void SetUseUnPackedMode(bool useUnpackedMode)
		{
			unpackedMode = useUnpackedMode;
			needsPack = (unpackedMode && ImgInfo.Packed);
		}
	}
}
