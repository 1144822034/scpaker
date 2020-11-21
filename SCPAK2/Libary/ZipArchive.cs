﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace pakdll
{
	public class ZipArchive : IDisposable
	{
		public enum Compression : ushort
		{
			Store = 0,
			Deflate = 8
		}

		public bool ForceDeflating;

		public bool KeepStreamOpen;

		public List<ZipArchiveEntry> Files = new List<ZipArchiveEntry>();

		public Stream ZipFileStream;

		public string Comment = "";

		public byte[] CentralDirImage;

		public ushort ExistingFiles;

		public bool ReadOnly;

		public static uint[] CrcTable;

		static ZipArchive()
		{
			CrcTable = new uint[256];
			for (int i = 0; i < CrcTable.Length; i++)
			{
				uint num = (uint)i;
				for (int j = 0; j < 8; j++)
				{
					num = (uint)(((num & 1) == 0) ? ((int)(num >> 1)) : (-306674912 ^ (int)(num >> 1)));
				}
				CrcTable[i] = num;
			}
		}

		private ZipArchive()
		{
		}

		public static ZipArchive Create(Stream stream, bool keepStreamOpen = false)
		{
			return new ZipArchive
			{
				Comment = string.Empty,
				ZipFileStream = stream,
				ReadOnly = false,
				KeepStreamOpen = keepStreamOpen
			};
		}

		public static ZipArchive Open(Stream stream, bool keepStreamOpen = false)
		{
			ZipArchive zipArchive = new ZipArchive();
			zipArchive.ZipFileStream = stream;
			zipArchive.ReadOnly = true;
			zipArchive.KeepStreamOpen = keepStreamOpen;
			if (zipArchive.ReadFileInfo())
			{
				return zipArchive;
			}
			throw new InvalidDataException();
		}

		public void AddStream(string filenameInZip, Stream source)
		{
			if (ReadOnly)
			{
				throw new InvalidOperationException("Writing is not allowed");
			}
			ZipArchiveEntry zipArchiveEntry = new ZipArchiveEntry();
			zipArchiveEntry.Method = Compression.Deflate;
			zipArchiveEntry.FilenameInZip = NormalizedFilename(filenameInZip);
			zipArchiveEntry.Comment = string.Empty;
			zipArchiveEntry.Crc32 = 0u;
			zipArchiveEntry.HeaderOffset = (uint)ZipFileStream.Position;
			zipArchiveEntry.ModifyTime = DateTime.Now;
			WriteLocalHeader(zipArchiveEntry);
			zipArchiveEntry.FileOffset = (uint)ZipFileStream.Position;
			Store(zipArchiveEntry, source);
			UpdateCrcAndSizes(zipArchiveEntry);
			Files.Add(zipArchiveEntry);
		}

		public void Close()
		{
			if (!ReadOnly)
			{
				uint offset = (uint)ZipFileStream.Position;
				uint num = 0u;
				if (CentralDirImage != null)
				{
					ZipFileStream.Write(CentralDirImage, 0, CentralDirImage.Length);
				}
				for (int i = 0; i < Files.Count; i++)
				{
					long position = ZipFileStream.Position;
					WriteCentralDirRecord(Files[i]);
					num = (uint)((int)num + (int)(ZipFileStream.Position - position));
				}
				if (CentralDirImage != null)
				{
					WriteEndRecord((uint)((int)num + CentralDirImage.Length), offset);
				}
				else
				{
					WriteEndRecord(num, offset);
				}
			}
			if (ZipFileStream != null && !KeepStreamOpen)
			{
				ZipFileStream.Flush();
				ZipFileStream.Dispose();
			}
			ZipFileStream = null;
		}

		public List<ZipArchiveEntry> ReadCentralDir()
		{
			if (CentralDirImage == null)
			{
				throw new InvalidOperationException("Central directory currently does not exist");
			}
			List<ZipArchiveEntry> list = new List<ZipArchiveEntry>();
			ushort num;
			ushort num2;
			ushort num3;
			for (int i = 0; i < CentralDirImage.Length && BitConverter.ToUInt32(CentralDirImage, i) == 33639248; i += 46 + num + num2 + num3)
			{
				ushort method = BitConverter.ToUInt16(CentralDirImage, i + 10);
				uint dt = BitConverter.ToUInt32(CentralDirImage, i + 12);
				uint crc = BitConverter.ToUInt32(CentralDirImage, i + 16);
				uint compressedSize = BitConverter.ToUInt32(CentralDirImage, i + 20);
				uint fileSize = BitConverter.ToUInt32(CentralDirImage, i + 24);
				num = BitConverter.ToUInt16(CentralDirImage, i + 28);
				num2 = BitConverter.ToUInt16(CentralDirImage, i + 30);
				num3 = BitConverter.ToUInt16(CentralDirImage, i + 32);
				uint headerOffset = BitConverter.ToUInt32(CentralDirImage, i + 42);
				uint headerSize = (uint)(46 + num + num2 + num3);
				Encoding uTF = Encoding.UTF8;
				ZipArchiveEntry zipArchiveEntry = new ZipArchiveEntry();
				zipArchiveEntry.Method = (Compression)method;
				zipArchiveEntry.FilenameInZip = NormalizedFilename(uTF.GetString(CentralDirImage, i + 46, num));
				zipArchiveEntry.FileOffset = GetFileOffset(headerOffset);
				zipArchiveEntry.FileSize = fileSize;
				zipArchiveEntry.CompressedSize = compressedSize;
				zipArchiveEntry.HeaderOffset = headerOffset;
				zipArchiveEntry.HeaderSize = headerSize;
				zipArchiveEntry.Crc32 = crc;
				zipArchiveEntry.ModifyTime = DosTimeToDateTime(dt);
				if (num3 > 0)
				{
					zipArchiveEntry.Comment = uTF.GetString(CentralDirImage, i + 46 + num + num2, num3);
				}
				list.Add(zipArchiveEntry);
			}
			return list;
		}

		public void ExtractFile(ZipArchiveEntry zfe, Stream stream)
		{
			if (!stream.CanWrite)
			{
				throw new InvalidOperationException("Stream cannot be written");
			}
			byte[] array = new byte[4];
			ZipFileStream.Seek(zfe.HeaderOffset, SeekOrigin.Begin);
			ZipFileStream.Read(array, 0, 4);
			if (BitConverter.ToUInt32(array, 0) != 67324752)
			{
				throw new InvalidOperationException("Unsupported zip archive.");
			}
			Stream stream2;
			if (zfe.Method == Compression.Store)
			{
				stream2 = ZipFileStream;
			}
			else
			{
				if (zfe.Method != Compression.Deflate)
				{
					throw new InvalidOperationException("Unsupported zip archive.");
				}
				stream2 = new DeflateStream(ZipFileStream, CompressionMode.Decompress, leaveOpen: true);
			}
			byte[] array2 = new byte[16384];
			ZipFileStream.Seek(zfe.FileOffset, SeekOrigin.Begin);
			uint num = zfe.FileSize;
			while (num != 0)
			{
				int num2 = stream2.Read(array2, 0, (int)Math.Min(num, array2.Length));
				stream.Write(array2, 0, num2);
				num = (uint)((int)num - num2);
			}
			stream.Flush();
			if (zfe.Method == Compression.Deflate)
			{
				stream2.Dispose();
			}
		}

		public uint GetFileOffset(uint _headerOffset)
		{
			byte[] array = new byte[2];
			ZipFileStream.Seek(_headerOffset + 26, SeekOrigin.Begin);
			ZipFileStream.Read(array, 0, 2);
			ushort num = BitConverter.ToUInt16(array, 0);
			ZipFileStream.Read(array, 0, 2);
			ushort num2 = BitConverter.ToUInt16(array, 0);
			return (uint)(30 + num + num2 + _headerOffset);
		}

		public void WriteLocalHeader(ZipArchiveEntry _zfe)
		{
			long position = ZipFileStream.Position;
			byte[] bytes = Encoding.UTF8.GetBytes(_zfe.FilenameInZip);
			ZipFileStream.Write(new byte[6]
			{
			80,
			75,
			3,
			4,
			20,
			0
			}, 0, 6);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)(_zfe.EncodeUTF8 ? 2048 : 0)), 0, 2);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);
			ZipFileStream.Write(BitConverter.GetBytes(DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);
			ZipFileStream.Write(new byte[12], 0, 12);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);
			ZipFileStream.Write(bytes, 0, bytes.Length);
			_zfe.HeaderSize = (uint)(ZipFileStream.Position - position);
		}

		public void WriteCentralDirRecord(ZipArchiveEntry _zfe)
		{
			Encoding uTF = Encoding.UTF8;
			byte[] bytes = uTF.GetBytes(_zfe.FilenameInZip);
			byte[] bytes2 = uTF.GetBytes(_zfe.Comment);
			ZipFileStream.Write(new byte[8]
			{
			80,
			75,
			1,
			2,
			23,
			11,
			20,
			0
			}, 0, 8);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)(_zfe.EncodeUTF8 ? 2048 : 0)), 0, 2);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);
			ZipFileStream.Write(BitConverter.GetBytes(DateTimeToDosTime(_zfe.ModifyTime)), 0, 4);
			ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);
			ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);
			ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes2.Length), 0, 2);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)0), 0, 2);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)33024), 0, 2);
			ZipFileStream.Write(BitConverter.GetBytes(_zfe.HeaderOffset), 0, 4);
			ZipFileStream.Write(bytes, 0, bytes.Length);
			ZipFileStream.Write(bytes2, 0, bytes2.Length);
		}

		public void WriteEndRecord(uint _size, uint _offset)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(Comment);
			ZipFileStream.Write(new byte[8]
			{
			80,
			75,
			5,
			6,
			0,
			0,
			0,
			0
			}, 0, 8);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)Files.Count + ExistingFiles), 0, 2);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)Files.Count + ExistingFiles), 0, 2);
			ZipFileStream.Write(BitConverter.GetBytes(_size), 0, 4);
			ZipFileStream.Write(BitConverter.GetBytes(_offset), 0, 4);
			ZipFileStream.Write(BitConverter.GetBytes((ushort)bytes.Length), 0, 2);
			ZipFileStream.Write(bytes, 0, bytes.Length);
		}

		public void Store(ZipArchiveEntry _zfe, Stream _source)
		{
			byte[] array = new byte[16384];
			uint num = 0u;
			long position = ZipFileStream.Position;
			long position2 = _source.Position;
			Stream stream = (_zfe.Method != 0) ? new DeflateStream(ZipFileStream, CompressionMode.Compress, leaveOpen: true) : ZipFileStream;
			_zfe.Crc32 = uint.MaxValue;
			int num2;
			do
			{
				num2 = _source.Read(array, 0, array.Length);
				num = (uint)((int)num + num2);
				if (num2 > 0)
				{
					stream.Write(array, 0, num2);
					for (uint num3 = 0u; num3 < num2; num3++)
					{
						_zfe.Crc32 = (CrcTable[(_zfe.Crc32 ^ array[num3]) & 0xFF] ^ (_zfe.Crc32 >> 8));
					}
				}
			}
			while (num2 == array.Length);
			stream.Flush();
			if (_zfe.Method == Compression.Deflate)
			{
				stream.Dispose();
			}
			_zfe.Crc32 ^= uint.MaxValue;
			_zfe.FileSize = num;
			_zfe.CompressedSize = (uint)(ZipFileStream.Position - position);
			if (_zfe.Method == Compression.Deflate && !ForceDeflating && _source.CanSeek && _zfe.CompressedSize > _zfe.FileSize)
			{
				_zfe.Method = Compression.Store;
				ZipFileStream.Position = position;
				ZipFileStream.SetLength(position);
				_source.Position = position2;
				Store(_zfe, _source);
			}
		}

		public uint DateTimeToDosTime(DateTime _dt)
		{
			return (uint)((_dt.Second / 2) | (_dt.Minute << 5) | (_dt.Hour << 11) | (_dt.Day << 16) | (_dt.Month << 21) | (_dt.Year - 1980 << 25));
		}

		public DateTime DosTimeToDateTime(uint _dt)
		{
			return new DateTime((int)((_dt >> 25) + 1980), (int)((_dt >> 21) & 0xF), (int)((_dt >> 16) & 0x1F), (int)((_dt >> 11) & 0x1F), (int)((_dt >> 5) & 0x3F), (int)((_dt & 0x1F) * 2));
		}

		public void UpdateCrcAndSizes(ZipArchiveEntry _zfe)
		{
			long position = ZipFileStream.Position;
			ZipFileStream.Position = _zfe.HeaderOffset + 8;
			ZipFileStream.Write(BitConverter.GetBytes((ushort)_zfe.Method), 0, 2);
			ZipFileStream.Position = _zfe.HeaderOffset + 14;
			ZipFileStream.Write(BitConverter.GetBytes(_zfe.Crc32), 0, 4);
			ZipFileStream.Write(BitConverter.GetBytes(_zfe.CompressedSize), 0, 4);
			ZipFileStream.Write(BitConverter.GetBytes(_zfe.FileSize), 0, 4);
			ZipFileStream.Position = position;
		}

		public string NormalizedFilename(string _filename)
		{
			string text = _filename.Replace('\\', '/');
			int num = text.IndexOf(':');
			if (num >= 0)
			{
				text = text.Remove(0, num + 1);
			}
			return text.Trim('/');
		}

		public bool ReadFileInfo()
		{
			if (ZipFileStream.Length < 22)
			{
				return false;
			}
			try
			{
				ZipFileStream.Seek(-17L, SeekOrigin.End);
				BinaryReader binaryReader = new BinaryReader(ZipFileStream);
				do
				{
					ZipFileStream.Seek(-5L, SeekOrigin.Current);
					if (binaryReader.ReadUInt32() == 101010256)
					{
						ZipFileStream.Seek(6L, SeekOrigin.Current);
						ushort existingFiles = binaryReader.ReadUInt16();
						int num = binaryReader.ReadInt32();
						uint num2 = binaryReader.ReadUInt32();
						ushort num3 = binaryReader.ReadUInt16();
						if (ZipFileStream.Position + num3 == ZipFileStream.Length)
						{
							ExistingFiles = existingFiles;
							CentralDirImage = new byte[num];
							ZipFileStream.Seek(num2, SeekOrigin.Begin);
							ZipFileStream.Read(CentralDirImage, 0, num);
							ZipFileStream.Seek(num2, SeekOrigin.Begin);
							return true;
						}
						return false;
					}
				}
				while (ZipFileStream.Position > 0);
			}
			catch
			{
			}
			return false;
		}

		public void Dispose()
		{
			Close();
		}
	}

}