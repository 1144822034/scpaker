// SCPAK.Pak
using SCPAK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SCPAK2;

public class Pak21
{
	public MainActivity activity1;
	public Pak21(string PakDirectory,MainActivity activity)
	{
		this.activity1 = activity;
		if (!Directory.Exists(PakDirectory))
		{
			throw new DirectoryNotFoundException("将要封包的文件夹不存在");
		}
		List<ContentFileInfo> list = new List<ContentFileInfo>();
		list = ContentFiles(list, PakDirectory);
		List<long> list2 = new List<long>();
		new DirectoryInfo(PakDirectory);
		if (PakDirectory.EndsWith("/") || PakDirectory.EndsWith("\\"))
		{
			PakDirectory = PakDirectory.Substring(0, PakDirectory.Length - 1);
		}
		if (File.Exists(PakDirectory + ".pak"))
		{
			if (File.Exists(PakDirectory + ".pak.bak"))
			{
				File.Delete(PakDirectory + ".pak.bak");
			}
			File.Move(PakDirectory + ".pak", PakDirectory + ".pak.bak");
		}
		FileStream fileStream = new FileStream(PakDirectory + ".pak", FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.UTF8, leaveOpen: true);
		binaryWriter.Write((byte)80);
		binaryWriter.Write((byte)65);
		binaryWriter.Write((byte)75);
		binaryWriter.Write((byte)0);
		binaryWriter.Write(0);
		binaryWriter.Write(list.Count);
		foreach (ContentFileInfo item in list)
		{
			activity1.sendDialog("[2.1]打包中...",item.fileName);
			if (item.typeName == "Engine.Graphics.Texture2D")
			{
				string[] array = item.fileName.Substring(PakDirectory.Length + 1, item.fileName.Length - PakDirectory.Length - 1).Split('/');
				string text = array[array.Length - 1];
				if (text[0] == '!')
				{
					string text2 = "";
					for (int i = 0; i < array.Length; i++)
					{
						text2 = ((i + 1 != array.Length) ? (text2 + "/" + array[i]) : (text2 + "/" + text.Substring(1)));
					}
					binaryWriter.Write(text2.Substring(1));
				}
				else
				{
					binaryWriter.Write(item.fileName.Substring(PakDirectory.Length + 1, item.fileName.Length - PakDirectory.Length - 1));
				}
			}
			else
			{
				binaryWriter.Write(item.fileName.Substring(PakDirectory.Length + 1, item.fileName.Length - PakDirectory.Length - 1));
			}
			binaryWriter.Write(item.typeName);
			list2.Add(binaryWriter.BaseStream.Position);
			binaryWriter.Write(0);
			binaryWriter.Write(0);
		}
		long position = binaryWriter.BaseStream.Position;
		binaryWriter.BaseStream.Position = 4L;
		binaryWriter.Write((int)position);
		long num = position;
		int num2 = 0;
		foreach (ContentFileInfo item2 in list)
		{
			activity1.sendDialog("[2.1]打包中...", item2.fileName);
			binaryWriter.BaseStream.Position = (int)num;
			binaryWriter.Write((byte)222);
			binaryWriter.Write((byte)173);
			binaryWriter.Write((byte)190);
			binaryWriter.Write((byte)239);
			long position2 = binaryWriter.BaseStream.Position;
			Stream stream = PakData._PakData(item2.fileName, item2.typeName);
			stream.CopyTo(fileStream);
			num = binaryWriter.BaseStream.Position;
			binaryWriter.BaseStream.Position = list2[num2++];
			binaryWriter.Write((int)(position2 - position));
			binaryWriter.Write((int)stream.Length);
		}
		binaryWriter.Dispose();
		fileStream.Dispose();
	}

	private List<ContentFileInfo> ContentFiles(List<ContentFileInfo> list, string PakDirectory)
	{
		string[] directories = Directory.GetDirectories(PakDirectory);
		foreach (string pakDirectory in directories)
		{
			list = ContentFiles(list, pakDirectory);
		}
		directories = Directory.GetFiles(PakDirectory);
		ContentFileInfo item = default(ContentFileInfo);
		foreach (string text in directories)
		{
			string text2;
			switch (Path.GetExtension(text))
			{
				case ".txt":
					text2 = "System.String";
					break;
				case ".xml":
					text2 = "System.Xml.Linq.XElement";
					break;
				case ".png":
					if (File.Exists(text.Substring(0, text.Length - 4) + ".lst"))
					{
						continue;
					}
					text2 = "Engine.Graphics.Texture2D";
					break;
				case ".dae":
					text2 = "Engine.Graphics.Model";
					break;
				case ".fsh":
					text2 = "Engine.Graphics.Shader";
					break;
				case ".lst":
					text2 = "Engine.Media.BitmapFont";
					break;
				case ".wav":
					text2 = "Engine.Audio.SoundBuffer";
					break;
				case ".ogg":
					text2 = "Engine.Media.StreamingSource";
					break;
				default:
					throw new Exception("发现不能识别的文件 :" + text);
			}
			if (text2 == "")
			{
				continue;
			}
			string text3 = Path.GetDirectoryName(text);
			string[] array = text3.Split('\\');
			if (array.Length > 1)
			{
				text3 = array[0];
				for (int j = 1; j < array.Length; j++)
				{
					text3 = text3 + "/" + array[j];
				}
			}
			item.fileName = text3 + "/" + Path.GetFileNameWithoutExtension(text);
			item.typeName = text2;
			list.Add(item);
		}
		return list;
	}
}
