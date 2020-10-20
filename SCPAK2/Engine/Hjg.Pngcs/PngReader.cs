using Hjg.Pngcs.Chunks;
using Hjg.Pngcs.Zlib;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hjg.Pngcs
{
	internal class PngReader
	{
		public readonly string filename;

		public Dictionary<string, int> skipChunkIdsSet;

		public readonly PngMetadata metadata;

		public readonly ChunksList chunksList;

		public ImageLine imgLine;

		public byte[] rowb;

		public byte[] rowbprev;

		public byte[] rowbfilter;

		public readonly bool interlaced;

		public readonly PngDeinterlacer deinterlacer;

		public bool crcEnabled = true;

		public bool unpackedMode;

		public int rowNum = -1;

		public long offset;

		public int bytesChunksLoaded;

		public readonly Stream inputStream;

		internal AZlibInputStream idatIstream;

		internal PngIDatChunkInputStream iIdatCstream;

		public Adler32 crctest;

		public ImageInfo ImgInfo
		{
			get;
			set;
		}

		public ChunkLoadBehaviour ChunkLoadBehaviour
		{
			get;
			set;
		}

		public bool ShouldCloseStream
		{
			get;
			set;
		}

		public long MaxBytesMetadata
		{
			get;
			set;
		}

		public long MaxTotalBytesRead
		{
			get;
			set;
		}

		public int SkipChunkMaxSize
		{
			get;
			set;
		}

		public string[] SkipChunkIds
		{
			get;
			set;
		}

		public int CurrentChunkGroup
		{
			get;
			set;
		}

		public PngReader(Stream inputStream)
			: this(inputStream, "[NO FILENAME AVAILABLE]")
		{
		}

		public PngReader(Stream inputStream, string filename)
		{
			this.filename = ((filename == null) ? "" : filename);
			this.inputStream = inputStream;
			chunksList = new ChunksList(null);
			metadata = new PngMetadata(chunksList);
			offset = 0L;
			CurrentChunkGroup = -1;
			ShouldCloseStream = true;
			MaxBytesMetadata = 5242880L;
			MaxTotalBytesRead = 209715200L;
			SkipChunkMaxSize = 2097152;
			SkipChunkIds = new string[1]
			{
				"fdAT"
			};
			ChunkLoadBehaviour = ChunkLoadBehaviour.LOAD_CHUNK_ALWAYS;
			byte[] array = new byte[8];
			PngHelperInternal.ReadBytes(inputStream, array, 0, array.Length);
			offset += array.Length;
			if (!PngCsUtils.arraysEqual(array, PngHelperInternal.PNG_ID_SIGNATURE))
			{
				throw new PngjInputException("Bad PNG signature");
			}
			CurrentChunkGroup = 0;
			int num = PngHelperInternal.ReadInt4(inputStream);
			offset += 4L;
			if (num != 13)
			{
				throw new Exception("IDHR chunk len != 13 ?? " + num.ToString());
			}
			byte[] array2 = new byte[4];
			PngHelperInternal.ReadBytes(inputStream, array2, 0, 4);
			if (!PngCsUtils.arraysEqual4(array2, ChunkHelper.b_IHDR))
			{
				throw new PngjInputException("IHDR not found as first chunk??? [" + ChunkHelper.ToString(array2) + "]");
			}
			offset += 4L;
			PngChunkIHDR pngChunkIHDR = (PngChunkIHDR)ReadChunk(array2, num, skipforced: false);
			bool alpha = (pngChunkIHDR.Colormodel & 4) != 0;
			bool palette = (pngChunkIHDR.Colormodel & 1) != 0;
			bool grayscale = pngChunkIHDR.Colormodel == 0 || pngChunkIHDR.Colormodel == 4;
			ImgInfo = new ImageInfo(pngChunkIHDR.Cols, pngChunkIHDR.Rows, pngChunkIHDR.Bitspc, alpha, grayscale, palette);
			rowb = new byte[ImgInfo.BytesPerRow + 1];
			rowbprev = new byte[rowb.Length];
			rowbfilter = new byte[rowb.Length];
			interlaced = (pngChunkIHDR.Interlaced == 1);
			deinterlacer = (interlaced ? new PngDeinterlacer(ImgInfo) : null);
			if (pngChunkIHDR.Filmeth != 0 || pngChunkIHDR.Compmeth != 0 || (pngChunkIHDR.Interlaced & 0xFFFE) != 0)
			{
				throw new PngjInputException("compmethod or filtermethod or interlaced unrecognized");
			}
			if (pngChunkIHDR.Colormodel < 0 || pngChunkIHDR.Colormodel > 6 || pngChunkIHDR.Colormodel == 1 || pngChunkIHDR.Colormodel == 5)
			{
				throw new PngjInputException("Invalid colormodel " + pngChunkIHDR.Colormodel.ToString());
			}
			if (pngChunkIHDR.Bitspc != 1 && pngChunkIHDR.Bitspc != 2 && pngChunkIHDR.Bitspc != 4 && pngChunkIHDR.Bitspc != 8 && pngChunkIHDR.Bitspc != 16)
			{
				throw new PngjInputException("Invalid bit depth " + pngChunkIHDR.Bitspc.ToString());
			}
		}

		public bool FirstChunksNotYetRead()
		{
			return CurrentChunkGroup < 1;
		}

		public void ReadLastAndClose()
		{
			if (CurrentChunkGroup < 5)
			{
				try
				{
					idatIstream.Dispose();
				}
				catch (Exception)
				{
				}
				ReadLastChunks();
			}
			Close();
		}

		public void Close()
		{
			if (CurrentChunkGroup < 6)
			{
				try
				{
					idatIstream.Dispose();
				}
				catch (Exception)
				{
				}
				CurrentChunkGroup = 6;
			}
			if (ShouldCloseStream)
			{
				inputStream.Dispose();
			}
		}

		public void UnfilterRow(int nbytes)
		{
			int num = rowbfilter[0];
			switch (num)
			{
			case 0:
				UnfilterRowNone(nbytes);
				break;
			case 1:
				UnfilterRowSub(nbytes);
				break;
			case 2:
				UnfilterRowUp(nbytes);
				break;
			case 3:
				UnfilterRowAverage(nbytes);
				break;
			case 4:
				UnfilterRowPaeth(nbytes);
				break;
			default:
				throw new PngjInputException("Filter type " + num.ToString() + " not implemented");
			}
			if (crctest != null)
			{
				crctest.Update(rowb, 1, nbytes);
			}
		}

		public void UnfilterRowAverage(int nbytes)
		{
			int num = 1 - ImgInfo.BytesPixel;
			int num2 = 1;
			while (num2 <= nbytes)
			{
				int num3 = (num > 0) ? rowb[num] : 0;
				rowb[num2] = (byte)(rowbfilter[num2] + (num3 + (rowbprev[num2] & 0xFF)) / 2);
				num2++;
				num++;
			}
		}

		public void UnfilterRowNone(int nbytes)
		{
			for (int i = 1; i <= nbytes; i++)
			{
				rowb[i] = rowbfilter[i];
			}
		}

		public void UnfilterRowPaeth(int nbytes)
		{
			int num = 1 - ImgInfo.BytesPixel;
			int num2 = 1;
			while (num2 <= nbytes)
			{
				int a = (num > 0) ? rowb[num] : 0;
				int c = (num > 0) ? rowbprev[num] : 0;
				rowb[num2] = (byte)(rowbfilter[num2] + PngHelperInternal.FilterPaethPredictor(a, rowbprev[num2], c));
				num2++;
				num++;
			}
		}

		public void UnfilterRowSub(int nbytes)
		{
			int i;
			for (i = 1; i <= ImgInfo.BytesPixel; i++)
			{
				rowb[i] = rowbfilter[i];
			}
			int num = 1;
			i = ImgInfo.BytesPixel + 1;
			while (i <= nbytes)
			{
				rowb[i] = (byte)(rowbfilter[i] + rowb[num]);
				i++;
				num++;
			}
		}

		public void UnfilterRowUp(int nbytes)
		{
			for (int i = 1; i <= nbytes; i++)
			{
				rowb[i] = (byte)(rowbfilter[i] + rowbprev[i]);
			}
		}

		public void ReadFirstChunks()
		{
			if (!FirstChunksNotYetRead())
			{
				return;
			}
			int num = 0;
			bool flag = false;
			byte[] array = new byte[4];
			CurrentChunkGroup = 1;
			while (!flag)
			{
				num = PngHelperInternal.ReadInt4(inputStream);
				offset += 4L;
				if (num < 0)
				{
					break;
				}
				PngHelperInternal.ReadBytes(inputStream, array, 0, 4);
				offset += 4L;
				if (PngCsUtils.arraysEqual4(array, ChunkHelper.b_IDAT))
				{
					flag = true;
					CurrentChunkGroup = 4;
					chunksList.AppendReadChunk(new PngChunkIDAT(ImgInfo, num, offset - 8), CurrentChunkGroup);
					break;
				}
				if (PngCsUtils.arraysEqual4(array, ChunkHelper.b_IEND))
				{
					throw new PngjInputException("END chunk found before image data (IDAT) at offset=" + offset.ToString());
				}
				string text = ChunkHelper.ToString(array);
				if (text.Equals("PLTE"))
				{
					CurrentChunkGroup = 2;
				}
				ReadChunk(array, num, skipforced: false);
				if (text.Equals("PLTE"))
				{
					CurrentChunkGroup = 3;
				}
			}
			int num2 = flag ? num : (-1);
			if (num2 < 0)
			{
				throw new PngjInputException("first idat chunk not found!");
			}
			iIdatCstream = new PngIDatChunkInputStream(inputStream, num2, offset);
			idatIstream = ZlibStreamFactory.createZlibInputStream(iIdatCstream, leaveOpen: true);
			if (!crcEnabled)
			{
				iIdatCstream.DisableCrcCheck();
			}
		}

		public void ReadLastChunks()
		{
			CurrentChunkGroup = 5;
			if (!iIdatCstream.IsEnded())
			{
				iIdatCstream.ForceChunkEnd();
			}
			int num = iIdatCstream.GetLenLastChunk();
			byte[] idLastChunk = iIdatCstream.GetIdLastChunk();
			bool flag = false;
			bool flag2 = true;
			bool flag3 = false;
			while (!flag)
			{
				flag3 = false;
				if (!flag2)
				{
					num = PngHelperInternal.ReadInt4(inputStream);
					offset += 4L;
					if (num < 0)
					{
						throw new PngjInputException("bad len " + num.ToString());
					}
					PngHelperInternal.ReadBytes(inputStream, idLastChunk, 0, 4);
					offset += 4L;
				}
				flag2 = false;
				if (PngCsUtils.arraysEqual4(idLastChunk, ChunkHelper.b_IDAT))
				{
					flag3 = true;
				}
				else if (PngCsUtils.arraysEqual4(idLastChunk, ChunkHelper.b_IEND))
				{
					CurrentChunkGroup = 6;
					flag = true;
				}
				ReadChunk(idLastChunk, num, flag3);
			}
			if (!flag)
			{
				throw new PngjInputException("end chunk not found - offset=" + offset.ToString());
			}
		}

		public PngChunk ReadChunk(byte[] chunkid, int clen, bool skipforced)
		{
			if (clen < 0)
			{
				throw new PngjInputException("invalid chunk lenght: " + clen.ToString());
			}
			if (skipChunkIdsSet == null && CurrentChunkGroup > 0)
			{
				skipChunkIdsSet = new Dictionary<string, int>();
				if (SkipChunkIds != null)
				{
					string[] skipChunkIds = SkipChunkIds;
					foreach (string key in skipChunkIds)
					{
						skipChunkIdsSet.Add(key, 1);
					}
				}
			}
			string text = ChunkHelper.ToString(chunkid);
			PngChunk pngChunk = null;
			bool flag = ChunkHelper.IsCritical(text);
			bool flag2 = skipforced;
			if (MaxTotalBytesRead > 0 && clen + offset > MaxTotalBytesRead)
			{
				throw new PngjInputException("Maximum total bytes to read exceeeded: " + MaxTotalBytesRead.ToString() + " offset:" + offset.ToString() + " clen=" + clen.ToString());
			}
			if (CurrentChunkGroup > 0 && !ChunkHelper.IsCritical(text))
			{
				flag2 = (flag2 || (SkipChunkMaxSize > 0 && clen >= SkipChunkMaxSize) || skipChunkIdsSet.ContainsKey(text) || (MaxBytesMetadata > 0 && clen > MaxBytesMetadata - bytesChunksLoaded) || !ChunkHelper.ShouldLoad(text, ChunkLoadBehaviour));
			}
			if (flag2)
			{
				PngHelperInternal.SkipBytes(inputStream, clen);
				PngHelperInternal.ReadInt4(inputStream);
				pngChunk = new PngChunkSkipped(text, ImgInfo, clen);
			}
			else
			{
				ChunkRaw chunkRaw = new ChunkRaw(clen, chunkid, alloc: true);
				chunkRaw.ReadChunkData(inputStream, crcEnabled | flag);
				pngChunk = PngChunk.Factory(chunkRaw, ImgInfo);
				if (!pngChunk.Crit)
				{
					bytesChunksLoaded += chunkRaw.Length;
				}
			}
			pngChunk.Offset = offset - 8;
			chunksList.AppendReadChunk(pngChunk, CurrentChunkGroup);
			offset += (long)clen + 4L;
			return pngChunk;
		}

		internal void logWarn(string warn)
		{
		}

		public ChunksList GetChunksList()
		{
			if (FirstChunksNotYetRead())
			{
				ReadFirstChunks();
			}
			return chunksList;
		}

		public PngMetadata GetMetadata()
		{
			if (FirstChunksNotYetRead())
			{
				ReadFirstChunks();
			}
			return metadata;
		}

		public ImageLine ReadRow(int nrow)
		{
			if (imgLine != null && imgLine.SampleType == ImageLine.ESampleType.BYTE)
			{
				return ReadRowByte(nrow);
			}
			return ReadRowInt(nrow);
		}

		public ImageLine ReadRowInt(int nrow)
		{
			if (imgLine == null)
			{
				imgLine = new ImageLine(ImgInfo, ImageLine.ESampleType.INT, unpackedMode);
			}
			if (imgLine.Rown == nrow)
			{
				return imgLine;
			}
			ReadRowInt(imgLine.Scanline, nrow);
			imgLine.FilterUsed = (FilterType)rowbfilter[0];
			imgLine.Rown = nrow;
			return imgLine;
		}

		public ImageLine ReadRowByte(int nrow)
		{
			if (imgLine == null)
			{
				imgLine = new ImageLine(ImgInfo, ImageLine.ESampleType.BYTE, unpackedMode);
			}
			if (imgLine.Rown == nrow)
			{
				return imgLine;
			}
			ReadRowByte(imgLine.ScanlineB, nrow);
			imgLine.FilterUsed = (FilterType)rowbfilter[0];
			imgLine.Rown = nrow;
			return imgLine;
		}

		public int[] ReadRow(int[] buffer, int nrow)
		{
			return ReadRowInt(buffer, nrow);
		}

		public int[] ReadRowInt(int[] buffer, int nrow)
		{
			if (buffer == null)
			{
				buffer = new int[unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked];
			}
			if (!interlaced)
			{
				if (nrow <= rowNum)
				{
					throw new PngjInputException("rows must be read in increasing order: " + nrow.ToString());
				}
				int bytesRead = 0;
				while (rowNum < nrow)
				{
					bytesRead = ReadRowRaw(rowNum + 1);
				}
				decodeLastReadRowToInt(buffer, bytesRead);
			}
			else
			{
				if (deinterlacer.getImageInt() == null)
				{
					deinterlacer.setImageInt(ReadRowsInt().Scanlines);
				}
				Array.Copy(deinterlacer.getImageInt()[nrow], 0, buffer, 0, unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked);
			}
			return buffer;
		}

		public byte[] ReadRowByte(byte[] buffer, int nrow)
		{
			if (buffer == null)
			{
				buffer = new byte[unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked];
			}
			if (!interlaced)
			{
				if (nrow <= rowNum)
				{
					throw new PngjInputException("rows must be read in increasing order: " + nrow.ToString());
				}
				int bytesRead = 0;
				while (rowNum < nrow)
				{
					bytesRead = ReadRowRaw(rowNum + 1);
				}
				decodeLastReadRowToByte(buffer, bytesRead);
			}
			else
			{
				if (deinterlacer.getImageByte() == null)
				{
					deinterlacer.setImageByte(ReadRowsByte().ScanlinesB);
				}
				Array.Copy(deinterlacer.getImageByte()[nrow], 0, buffer, 0, unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked);
			}
			return buffer;
		}

		[Obsolete("GetRow is deprecated,  use ReadRow/ReadRowInt/ReadRowByte instead.")]
		public ImageLine GetRow(int nrow)
		{
			return ReadRow(nrow);
		}

		public void decodeLastReadRowToInt(int[] buffer, int bytesRead)
		{
			if (ImgInfo.BitDepth <= 8)
			{
				int i = 0;
				int num = 1;
				for (; i < bytesRead; i++)
				{
					buffer[i] = rowb[num++];
				}
			}
			else
			{
				int num2 = 0;
				int num3 = 1;
				while (num3 < bytesRead)
				{
					buffer[num2] = (rowb[num3++] << 8) + rowb[num3++];
					num2++;
				}
			}
			if (ImgInfo.Packed && unpackedMode)
			{
				ImageLine.unpackInplaceInt(ImgInfo, buffer, buffer, Scale: false);
			}
		}

		public void decodeLastReadRowToByte(byte[] buffer, int bytesRead)
		{
			if (ImgInfo.BitDepth <= 8)
			{
				Array.Copy(rowb, 1, buffer, 0, bytesRead);
			}
			else
			{
				int num = 0;
				for (int i = 1; i < bytesRead; i += 2)
				{
					buffer[num] = rowb[i];
					num++;
				}
			}
			if (ImgInfo.Packed && unpackedMode)
			{
				ImageLine.unpackInplaceByte(ImgInfo, buffer, buffer, scale: false);
			}
		}

		public ImageLines ReadRowsInt(int rowOffset, int nRows, int rowStep)
		{
			if (nRows < 0)
			{
				nRows = (ImgInfo.Rows - rowOffset) / rowStep;
			}
			if (rowStep < 1 || rowOffset < 0 || nRows * rowStep + rowOffset > ImgInfo.Rows)
			{
				throw new PngjInputException("bad args");
			}
			ImageLines imageLines = new ImageLines(ImgInfo, ImageLine.ESampleType.INT, unpackedMode, rowOffset, nRows, rowStep);
			if (!interlaced)
			{
				for (int i = 0; i < ImgInfo.Rows; i++)
				{
					int bytesRead = ReadRowRaw(i);
					int num = imageLines.ImageRowToMatrixRowStrict(i);
					if (num >= 0)
					{
						decodeLastReadRowToInt(imageLines.Scanlines[num], bytesRead);
					}
				}
			}
			else
			{
				int[] array = new int[unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked];
				for (int j = 1; j <= 7; j++)
				{
					deinterlacer.setPass(j);
					for (int k = 0; k < deinterlacer.getRows(); k++)
					{
						int bytesRead2 = ReadRowRaw(k);
						int currRowReal = deinterlacer.getCurrRowReal();
						int num2 = imageLines.ImageRowToMatrixRowStrict(currRowReal);
						if (num2 >= 0)
						{
							decodeLastReadRowToInt(array, bytesRead2);
							deinterlacer.deinterlaceInt(array, imageLines.Scanlines[num2], !unpackedMode);
						}
					}
				}
			}
			End();
			return imageLines;
		}

		public ImageLines ReadRowsInt()
		{
			return ReadRowsInt(0, ImgInfo.Rows, 1);
		}

		public ImageLines ReadRowsByte(int rowOffset, int nRows, int rowStep)
		{
			if (nRows < 0)
			{
				nRows = (ImgInfo.Rows - rowOffset) / rowStep;
			}
			if (rowStep < 1 || rowOffset < 0 || nRows * rowStep + rowOffset > ImgInfo.Rows)
			{
				throw new PngjInputException("bad args");
			}
			ImageLines imageLines = new ImageLines(ImgInfo, ImageLine.ESampleType.BYTE, unpackedMode, rowOffset, nRows, rowStep);
			if (!interlaced)
			{
				for (int i = 0; i < ImgInfo.Rows; i++)
				{
					int bytesRead = ReadRowRaw(i);
					int num = imageLines.ImageRowToMatrixRowStrict(i);
					if (num >= 0)
					{
						decodeLastReadRowToByte(imageLines.ScanlinesB[num], bytesRead);
					}
				}
			}
			else
			{
				byte[] array = new byte[unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked];
				for (int j = 1; j <= 7; j++)
				{
					deinterlacer.setPass(j);
					for (int k = 0; k < deinterlacer.getRows(); k++)
					{
						int bytesRead2 = ReadRowRaw(k);
						int currRowReal = deinterlacer.getCurrRowReal();
						int num2 = imageLines.ImageRowToMatrixRowStrict(currRowReal);
						if (num2 >= 0)
						{
							decodeLastReadRowToByte(array, bytesRead2);
							deinterlacer.deinterlaceByte(array, imageLines.ScanlinesB[num2], !unpackedMode);
						}
					}
				}
			}
			End();
			return imageLines;
		}

		public ImageLines ReadRowsByte()
		{
			return ReadRowsByte(0, ImgInfo.Rows, 1);
		}

		public int ReadRowRaw(int nrow)
		{
			if (nrow == 0 && FirstChunksNotYetRead())
			{
				ReadFirstChunks();
			}
			if (nrow == 0 && interlaced)
			{
				Array.Clear(rowb, 0, rowb.Length);
			}
			int num = ImgInfo.BytesPerRow;
			if (interlaced)
			{
				if (nrow < 0 || nrow > deinterlacer.getRows() || (nrow != 0 && nrow != deinterlacer.getCurrRowSubimg() + 1))
				{
					throw new PngjInputException("invalid row in interlaced mode: " + nrow.ToString());
				}
				deinterlacer.setRow(nrow);
				num = (ImgInfo.BitspPixel * deinterlacer.getPixelsToRead() + 7) / 8;
				if (num < 1)
				{
					throw new PngjExceptionInternal("wtf??");
				}
			}
			else if (nrow < 0 || nrow >= ImgInfo.Rows || nrow != rowNum + 1)
			{
				throw new PngjInputException("invalid row: " + nrow.ToString());
			}
			rowNum = nrow;
			byte[] array = rowb;
			rowb = rowbprev;
			rowbprev = array;
			PngHelperInternal.ReadBytes(idatIstream, rowbfilter, 0, num + 1);
			offset = iIdatCstream.GetOffset();
			if (offset < 0)
			{
				throw new PngjExceptionInternal("bad offset ??" + offset.ToString());
			}
			if (MaxTotalBytesRead > 0 && offset >= MaxTotalBytesRead)
			{
				throw new PngjInputException("Reading IDAT: Maximum total bytes to read exceeeded: " + MaxTotalBytesRead.ToString() + " offset:" + offset.ToString());
			}
			rowb[0] = 0;
			UnfilterRow(num);
			rowb[0] = rowbfilter[0];
			if ((rowNum == ImgInfo.Rows - 1 && !interlaced) || (interlaced && deinterlacer.isAtLastRow()))
			{
				ReadLastAndClose();
			}
			return num;
		}

		public void ReadSkippingAllRows()
		{
			if (FirstChunksNotYetRead())
			{
				ReadFirstChunks();
			}
			iIdatCstream.DisableCrcCheck();
			try
			{
				int num;
				do
				{
					num = iIdatCstream.Read(rowbfilter, 0, rowbfilter.Length);
				}
				while (num >= 0);
			}
			catch (IOException cause)
			{
				throw new PngjInputException("error in raw read of IDAT", cause);
			}
			offset = iIdatCstream.GetOffset();
			if (offset < 0)
			{
				throw new PngjExceptionInternal("bad offset ??" + offset.ToString());
			}
			if (MaxTotalBytesRead > 0 && offset >= MaxTotalBytesRead)
			{
				throw new PngjInputException("Reading IDAT: Maximum total bytes to read exceeeded: " + MaxTotalBytesRead.ToString() + " offset:" + offset.ToString());
			}
			ReadLastAndClose();
		}

		public override string ToString()
		{
			return "filename=" + filename + " " + ImgInfo.ToString();
		}

		public void End()
		{
			if (CurrentChunkGroup < 6)
			{
				Close();
			}
		}

		public bool IsInterlaced()
		{
			return interlaced;
		}

		public void SetUnpackedMode(bool unPackedMode)
		{
			unpackedMode = unPackedMode;
		}

		public bool IsUnpackedMode()
		{
			return unpackedMode;
		}

		public void SetCrcCheckDisabled()
		{
			crcEnabled = false;
		}

		internal long GetCrctestVal()
		{
			return crctest.GetValue();
		}

		internal void InitCrctest()
		{
			crctest = new Adler32();
		}
	}
}
