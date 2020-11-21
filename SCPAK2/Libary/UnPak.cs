using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SCPAK2;
namespace SCPAK
{
	public class UnPak
	{
		private byte[] keys = Encoding.UTF8.GetBytes("tiTrKAXRpwuRhNI3gTkxIun6AyLxSZaIgEjVkyFWhD6w0QgwmN5YwykY2I79OHIolI1r4ewZ2uEfStqC7GRDM8CRTNQTdg91pkOkbnIPAiEp2EqkZWYPgPv6CNZpB3E1OuuBmR3ZzYEv8UMjQxjyXZy1CEOD8guk3uiiPvyFaf5pSznSNWXbnhmAzTbi1TEGCyhxejMTB23KUgqNiskGlrHaIVNz83DXVGkvm");
		private MainActivity activity1;
		public UnPak(string pakFile,MainActivity activity)
		{
			this.activity1 = activity;
			if (!File.Exists(pakFile))
			{
				throw new FileNotFoundException("文件不存在！");
			}
			string pakDirectory = Path.GetDirectoryName(pakFile) + "/" + Path.GetFileNameWithoutExtension(pakFile);
			using (FileStream fileStream = new FileStream(pakFile, FileMode.Open))
			{
				PadStream padStream = new PadStream(fileStream, keys);
				BinaryReader binaryReader = new BinaryReader(padStream, Encoding.UTF8, leaveOpen: true);
				byte[] array = new byte[4];
				if (fileStream.Read(array, 0, array.Length) != array.Length || array[0] != 80 || array[1] != 75 || array[2] != 50 || array[3] != 0)
				{
					throw new FileLoadException("该文件不是Survivalcraft2(2.2)的PAK文件！");
				}
				long num = binaryReader.ReadInt64();
				int num2 = binaryReader.ReadInt32();
				List<PakInfo> list = new List<PakInfo>(num2);
				for (int i = 0; i < num2; i++)
				{
					string fileName = binaryReader.ReadString();
					string typeName = binaryReader.ReadString();
					long position = binaryReader.ReadInt64() + num;
					long bytesCount = binaryReader.ReadInt64();
					long position2 = binaryReader.BaseStream.Position;
					list.Add(new PakInfo
					{
						fileStream = ContentFile(padStream, position, bytesCount),
						fileName = fileName,
						typeName = typeName
					});
					binaryReader.BaseStream.Position = position2;
				}
				Load(list, pakDirectory);
				binaryReader.Dispose();
				padStream.Dispose();
			}
		}

		public Stream ContentFile(PadStream padStream, long position, long bytesCount)
		{
			padStream.keys = new byte[1]
			{
				63
			};
			padStream.Position = position;
			byte[] array = new byte[bytesCount];
			for (long num = 0L; num < bytesCount; num++)
			{
				array[num] = (byte)padStream.ReadByte();
			}
			MemoryStream result = new MemoryStream(array, writable: false);
			padStream.keys = keys;
			return result;
		}

		public void Load(List<PakInfo> listFileStream, string pakDirectory)
		{
			if (!Directory.Exists(pakDirectory))
			{
				Directory.CreateDirectory(pakDirectory);
			}
			foreach (PakInfo item in listFileStream)
			{
				activity1.sendDialog("[2.2]解包中...","解包文件"+ item.fileName);
				Stream stream;
				switch (item.typeName)
				{
				case "System.String":
					stream = CreateFile(pakDirectory + "/" + item.fileName + ".txt");
					TextHandler.RecoverText(stream, item.fileStream);
					break;
				case "System.Xml.Linq.XElement":
					stream = CreateFile(pakDirectory + "/" + item.fileName + ".xml");
					TextHandler.RecoverText(stream, item.fileStream);
					break;
				case "Engine.Media.StreamingSource":
					stream = CreateFile(pakDirectory + "/" + item.fileName + ".ogg");
					item.fileStream.CopyTo(stream);
					break;
				case "Engine.Graphics.Texture2D":
					stream = CreateFile(pakDirectory + "/" + item.fileName + ".png");
					Texture2DHandler.RecoverTexture2D(stream, item.fileStream);
					break;
				case "Engine.Audio.SoundBuffer":
					stream = CreateFile(pakDirectory + "/" + item.fileName + ".wav");
					SoundHandler.RecoverSound(stream, item.fileStream);
					break;
				case "Engine.Graphics.Model":
					stream = CreateFile(pakDirectory + "/" + item.fileName + ".dae");
					ModelHandler.RecoverModel(stream, item.fileStream);
					break;
				case "Engine.Graphics.Shader":
					stream = CreateFile(pakDirectory + "/" + item.fileName + ".shader");
					item.fileStream.CopyTo(stream);
					break;
				case "Engine.Media.BitmapFont":
					try
					{
						stream = CreateFile(pakDirectory + "/" + item.fileName + ".lst");
						Stream stream2 = CreateFile(pakDirectory + "/" + item.fileName + ".png");
						FontHandler.RecoverFont(stream, stream2, item.fileStream);
						stream2.Dispose();
					}
					catch
					{
						item.fileStream.Position = 0L;
						stream = CreateFile(pakDirectory + "/" + item.fileName + ".font");
						item.fileStream.CopyTo(stream);
					}
					break;
				default:
					throw new Exception("发现无法识别的文件类型:" + item.typeName + "\t文件名称:" + item.fileName);
				}
				stream.Dispose();
				item.fileStream.Dispose();
			}
		}

		public FileStream CreateFile(string file)
		{
			try
			{
				return new FileStream(file, FileMode.Create);
			}
			catch (DirectoryNotFoundException)
			{
				string[] array = file.Split('/');
				string text = "";
				for (int i = 0; i < array.Length - 1; i++)
				{
					if (i == array.Length - 1)
					{
						text += array[i];
						break;
					}
					text = text + array[i] + "/";
				}
				Directory.CreateDirectory(text);
				return new FileStream(file, FileMode.Create);
			}
		}
	}
}
