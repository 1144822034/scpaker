using Android.OS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Engine
{
	public static class Storage
	{
		public static long FreeSpace
		{
			get
			{
				try
				{
					StatFs statFs = new StatFs(Android.OS.Environment.DataDirectory.Path);
					long num = statFs.BlockSize;
					return statFs.AvailableBlocks * num;
				}
				catch (Exception)
				{
					return long.MaxValue;
				}
			}
		}

		public static bool FileExists(string path)
		{
			return File.Exists(ProcessPath(path, writeAccess: false, failIfApp: true));
		}

		public static bool DirectoryExists(string path)
		{
			return Directory.Exists(ProcessPath(path, writeAccess: false, failIfApp: true));
		}

		public static long GetFileSize(string path)
		{
			return new FileInfo(ProcessPath(path, writeAccess: false, failIfApp: true)).Length;
		}

		public static DateTime GetFileLastWriteTime(string path)
		{
			return File.GetLastWriteTimeUtc(ProcessPath(path, writeAccess: false, failIfApp: true));
		}

		public static Stream OpenFile(string path, OpenFileMode openFileMode)
		{
			if (openFileMode != 0 && openFileMode != OpenFileMode.ReadWrite && openFileMode != OpenFileMode.Create && openFileMode != OpenFileMode.CreateOrOpen)
			{
				throw new ArgumentException("openFileMode");
			}
			bool isApp;
			string text = ProcessPath(path, openFileMode != OpenFileMode.Read, failIfApp: false, out isApp);
			if (isApp)
			{
				return EngineActivity.m_activity.ApplicationContext.Assets.Open(text);
			}
			FileMode mode;
			switch (openFileMode)
			{
			case OpenFileMode.Create:
				mode = FileMode.Create;
				break;
			case OpenFileMode.CreateOrOpen:
				mode = FileMode.OpenOrCreate;
				break;
			default:
				mode = FileMode.Open;
				break;
			}
			FileAccess access = (openFileMode == OpenFileMode.Read) ? FileAccess.Read : FileAccess.ReadWrite;
			return File.Open(text, mode, access, FileShare.Read);
		}

		public static void DeleteFile(string path)
		{
			File.Delete(ProcessPath(path, writeAccess: true, failIfApp: true));
		}

		public static void CopyFile(string sourcePath, string destinationPath)
		{
			using (Stream stream = OpenFile(sourcePath, OpenFileMode.Read))
			{
				using (Stream destination = OpenFile(destinationPath, OpenFileMode.Create))
				{
					stream.CopyTo(destination);
				}
			}
		}

		public static void MoveFile(string sourcePath, string destinationPath)
		{
			string sourceFileName = ProcessPath(sourcePath, writeAccess: true, failIfApp: true);
			string text = ProcessPath(destinationPath, writeAccess: true, failIfApp: true);
			File.Delete(text);
			File.Move(sourceFileName, text);
		}

		public static void CreateDirectory(string path)
		{
			Directory.CreateDirectory(ProcessPath(path, writeAccess: true, failIfApp: true));
		}

		public static void DeleteDirectory(string path)
		{
			Directory.Delete(ProcessPath(path, writeAccess: true, failIfApp: true));
		}

		public static IEnumerable<string> ListFileNames(string path)
		{
			return from s in Directory.EnumerateFiles(ProcessPath(path, writeAccess: false, failIfApp: true))
				select Path.GetFileName(s);
		}

		public static IEnumerable<string> ListDirectoryNames(string path)
		{
			return from s in Directory.EnumerateDirectories(ProcessPath(path, writeAccess: false, failIfApp: true))
				select Path.GetFileName(s) into s
				where s != ".__override__"
				select s;
		}

		public static string ReadAllText(string path)
		{
			return ReadAllText(path, Encoding.UTF8);
		}

		public static string ReadAllText(string path, Encoding encoding)
		{
			using (StreamReader streamReader = new StreamReader(OpenFile(path, OpenFileMode.Read), encoding))
			{
				return streamReader.ReadToEnd();
			}
		}

		public static void WriteAllText(string path, string text)
		{
			WriteAllText(path, text, Encoding.UTF8);
		}

		public static void WriteAllText(string path, string text, Encoding encoding)
		{
			using (StreamWriter streamWriter = new StreamWriter(OpenFile(path, OpenFileMode.Create), encoding))
			{
				streamWriter.Write(text);
			}
		}

		public static byte[] ReadAllBytes(string path)
		{
			using (BinaryReader binaryReader = new BinaryReader(OpenFile(path, OpenFileMode.Read)))
			{
				return binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
			}
		}

		public static void WriteAllBytes(string path, byte[] bytes)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(OpenFile(path, OpenFileMode.Create)))
			{
				binaryWriter.Write(bytes);
			}
		}

		public static string GetSystemPath(string path)
		{
			return ProcessPath(path, writeAccess: false, failIfApp: true);
		}

		public static string GetExtension(string path)
		{
			int num = path.LastIndexOf('.');
			if (num >= 0)
			{
				return path.Substring(num);
			}
			return string.Empty;
		}

		public static string GetFileName(string path)
		{
			int num = path.LastIndexOf('/');
			if (num >= 0)
			{
				return path.Substring(num + 1);
			}
			return path;
		}

		public static string GetFileNameWithoutExtension(string path)
		{
			string fileName = GetFileName(path);
			int num = fileName.LastIndexOf('.');
			if (num >= 0)
			{
				return fileName.Substring(0, num);
			}
			return fileName;
		}

		public static string GetDirectoryName(string path)
		{
			int num = path.LastIndexOf('/');
			if (num >= 0)
			{
				return path.Substring(0, num).TrimEnd('/');
			}
			return string.Empty;
		}

		public static string CombinePaths(params string[] paths)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < paths.Length; i++)
			{
				if (paths[i].Length > 0)
				{
					stringBuilder.Append(paths[i]);
					if (i < paths.Length - 1 && (stringBuilder.Length == 0 || stringBuilder[stringBuilder.Length - 1] != '/'))
					{
						stringBuilder.Append('/');
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static string ChangeExtension(string path, string extension)
		{
			return CombinePaths(GetDirectoryName(path), GetFileNameWithoutExtension(path)) + extension;
		}

		public static string ProcessPath(string path, bool writeAccess, bool failIfApp)
		{
			bool isApp;
			return ProcessPath(path, writeAccess, failIfApp, out isApp);
		}

		public static string ProcessPath(string path, bool writeAccess, bool failIfApp, out bool isApp)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (Path.DirectorySeparatorChar != '/')
			{
				path = path.Replace('/', Path.DirectorySeparatorChar);
			}
			if (Path.DirectorySeparatorChar != '\\')
			{
				path = path.Replace('\\', Path.DirectorySeparatorChar);
			}
			if (path.StartsWith("app:"))
			{
				if (failIfApp)
				{
					throw new InvalidOperationException($"Access denied to \"{path}\".");
				}
				isApp = true;
				return path.Substring(4).TrimStart(Path.DirectorySeparatorChar);
			}
			if (path.StartsWith("data:"))
			{
				isApp = false;
				return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), path.Substring(5).TrimStart(Path.DirectorySeparatorChar));
			}
			if (path.StartsWith("android:"))
			{
				isApp = false;
				return Path.Combine(Storage.CombinePaths(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, path.Substring(8).TrimStart(Path.DirectorySeparatorChar)));
			}
			throw new InvalidOperationException($"Invalid path \"{path}\".");
		}
	}
}
