using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SCPAK2;
namespace SCPAK
{
	public class Pak
	{
		private readonly byte[] keys = Encoding.UTF8.GetBytes("tiTrKAXRpwuRhNI3gTkxIun6AyLxSZaIgEjVkyFWhD6w0QgwmN5YwykY2I79OHIolI1r4ewZ2uEfStqC7GRDM8CRTNQTdg91pkOkbnIPAiEp2EqkZWYPgPv6CNZpB3E1OuuBmR3ZzYEv8UMjQxjyXZy1CEOD8guk3uiiPvyFaf5pSznSNWXbnhmAzTbi1TEGCyhxejMTB23KUgqNiskGlrHaIVNz83DXVGkvm");
		private MainActivity activity1;
		public Pak(string PakDirectory,MainActivity activity)
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
			BinaryWriter binaryWriter = new BinaryWriter(new PadStream(fileStream, keys), Encoding.UTF8, leaveOpen: true);
			fileStream.WriteByte(80);
			fileStream.WriteByte(75);
			fileStream.WriteByte(50);
			fileStream.WriteByte(0);
			binaryWriter.Write(0L);
			binaryWriter.Write(list.Count);
			foreach (ContentFileInfo item in list)
			{
				activity1.sendDialog("[2.2]打包中...", "写入目录"+item.fileName);
				binaryWriter.Write(item.fileName.Substring(PakDirectory.Length + 1, item.fileName.Length - PakDirectory.Length - 1));
				binaryWriter.Write(item.typeName);
				list2.Add(binaryWriter.BaseStream.Position);
				binaryWriter.Write(0L);
				binaryWriter.Write(0L);
			}
			long position = binaryWriter.BaseStream.Position;
			binaryWriter.BaseStream.Position = 4L;
			binaryWriter.Write(position);
			long position2 = position;
			int num = 0;
			foreach (ContentFileInfo item2 in list)
			{
				activity1.sendDialog("[2.2]打包中...","写入文件"+ item2.fileName);
				binaryWriter.BaseStream.Position = position2;
				binaryWriter.Write((byte)222);
				binaryWriter.Write((byte)173);
				binaryWriter.Write((byte)190);
				binaryWriter.Write((byte)239);
				long position3 = binaryWriter.BaseStream.Position;
				PadStream padStream = new PadStream(Load(item2.fileName, item2.typeName), new byte[1]
				{
					63
				});
				byte[] array = new byte[padStream.Length];
				padStream.Read(array, 0, array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					fileStream.WriteByte(array[i]);
				}
				position2 = binaryWriter.BaseStream.Position;
				binaryWriter.BaseStream.Position = list2[num++];
				binaryWriter.Write(position3 - position);
				binaryWriter.Write(padStream.Length);
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
				case ".shader":
					text2 = "Engine.Graphics.Shader";
					break;
				case ".lst":
					text2 = "Engine.Media.BitmapFont";
					break;
				case ".font":
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

		public Stream Load(string fileName, string typeName)
		{
			try
			{
				MemoryStream memoryStream = new MemoryStream();
				FileStream fileStream;
				switch (typeName)
				{
				case "System.String":
					fileStream = File.OpenRead(fileName + ".txt");
					TextHandler.WriteText(memoryStream, fileStream);
					break;
				case "System.Xml.Linq.XElement":
					fileStream = File.OpenRead(fileName + ".xml");
					TextHandler.WriteText(memoryStream, fileStream);
					break;
				case "Engine.Media.StreamingSource":
					fileStream = File.OpenRead(fileName + ".ogg");
					fileStream.CopyTo(memoryStream);
					break;
				case "Engine.Graphics.Model":
					fileStream = File.OpenRead(fileName + ".dae");
					ModelHandler.WriteModel(memoryStream, fileStream);
					break;
				case "Engine.Graphics.Shader":
					fileStream = File.OpenRead(fileName + ".shader");
					fileStream.CopyTo(memoryStream);
					break;
				case "Engine.Audio.SoundBuffer":
					fileStream = File.OpenRead(fileName + ".wav");
					SoundHandler.WriteSound(memoryStream, fileStream);
					break;
				case "Engine.Graphics.Texture2D":
					fileStream = File.OpenRead(fileName + ".png");
					Texture2DHandler.WriteTexture2D(memoryStream, fileStream);
					break;
				case "Engine.Media.BitmapFont":
				{
					try
					{
						fileStream = File.OpenRead(fileName + ".lst");
					}
					catch
					{
						fileStream = File.OpenRead(fileName + ".font");
						fileStream.CopyTo(memoryStream);
						break;
					}
					FileStream fileStream2 = File.OpenRead(fileName + ".png");
					FontHandler.WriteFont(memoryStream, fileStream, fileStream2);
					fileStream2.Dispose();
					break;
				}
				default:
					throw new Exception("发现不能识别的文件 :" + fileName + "\n文件类型 :" + typeName);
				}
				fileStream.Dispose();
				memoryStream.Position = 0L;
				return memoryStream;
			}
			catch (Exception ex)
			{
				throw new Exception("文件写入错误 :" + fileName + "\t类型 :" + typeName + "\n具体错误信息 :" + ex.Message);
			}
		}
	}
}
