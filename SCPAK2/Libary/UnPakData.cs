// SCPAK.UnPakData
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hjg.Pngcs;
using SCPAK2;
internal class UnPakData
{
	private static MainActivity activity1;
	public static void _UnPakData(List<PAKInfo> listFileStream, string pakDirectory,MainActivity activity)
	{
		activity1 = activity;
		if (!Directory.Exists(pakDirectory))
		{
			Directory.CreateDirectory(pakDirectory);
		}
		foreach (PAKInfo item in listFileStream)
		{
			FileStream fileStream;
			if(activity1!=null) activity1.sendDialog("[2.1]解包中...","解包文件"+ item.fileName);
			switch (item.typeName)
			{
				case "System.String":
					fileStream = CreateFile($"{pakDirectory}/{item.fileName}.txt");
					TextSave(item.fileStream, fileStream);
					break;
				case "System.Xml.Linq.XElement":
					fileStream = CreateFile($"{pakDirectory}/{item.fileName}.xml");
					TextSave(item.fileStream, fileStream);
					break;
				case "Engine.Graphics.Texture2D":
					{
						string[] array2 = item.fileName.Split('/');
						string text2 = "";
						for (int j = 0; j < array2.Length; j++)
						{
							text2 = ((j + 1 != array2.Length) ? (text2 + "/" + array2[j]) : (text2 + "/!" + array2[j]));
						}
						if (File.Exists($"{pakDirectory}/{item.fileName}.lst") || File.Exists($"{pakDirectory}{text2}.lst"))
						{
							continue;
						}
						fileStream = CreateFile($"{pakDirectory}/{item.fileName}.png");
						if (PngSave(item.fileStream, fileStream))
						{
							fileStream.Dispose();
							if (File.Exists($"{pakDirectory}{text2}.png"))
							{
								File.Delete($"{pakDirectory}{text2}.png");
							}
							File.Move($"{pakDirectory}/{item.fileName}.png", $"{pakDirectory}{text2}.png");
						}
						break;
					}
				case "Engine.Audio.SoundBuffer":
					fileStream = CreateFile($"{pakDirectory}/{item.fileName}.wav");
					SoundSave(item.fileStream, fileStream);
					break;
				case "Engine.Graphics.Model":
					fileStream = CreateFile($"{pakDirectory}/{item.fileName}.dae");
					SCModelSave(item.fileStream, fileStream);
					break;
				case "Engine.Graphics.Shader":
					fileStream = CreateFile($"{pakDirectory}/{item.fileName}.fsh");
					item.fileStream.CopyTo(fileStream);
					break;
				case "Engine.Media.BitmapFont":
					{
						string[] array = item.fileName.Split('/');
						string text = "";
						for (int i = 0; i < array.Length; i++)
						{
							text = ((i + 1 != array.Length) ? (text + "/" + array[i]) : (text + "/!" + array[i]));
						}
						fileStream = CreateFile($"{pakDirectory}/{item.fileName}.lst");
						FontSave(item.fileStream, fileStream);
						Stream pngStream = CreateFile($"{pakDirectory}/{item.fileName}.png");
						if (PngSave(item.fileStream, pngStream))
						{
							fileStream.Dispose();
							if (File.Exists($"{pakDirectory}{text}.png"))
							{
								File.Delete($"{pakDirectory}{text}.png");
							}
							File.Move($"{pakDirectory}/{item.fileName}.png", $"{pakDirectory}{text}.png");
						}
						break;
					}
				case "Engine.Media.StreamingSource":
					fileStream = CreateFile($"{pakDirectory}/{item.fileName}.ogg");
					item.fileStream.CopyTo(fileStream);
					break;
				default:
					fileStream = CreateFile($"{pakDirectory}/{item.fileName}");
					item.fileStream.CopyTo(fileStream);
					break;
			}
			fileStream.Dispose();
			item.fileStream.Dispose();
		}
	}

	private static FileStream CreateFile(string file)
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

	private static bool PngSave(Stream stream, Stream pngStream)
	{
		BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
		binaryReader.ReadByte();
		int num = binaryReader.ReadInt32();
		int num2 = binaryReader.ReadInt32();
		binaryReader.ReadInt32();
		ImageInfo imgInfo = new ImageInfo(num, num2, 8, alpha: true, grayscale: false, palette: false);
		PngWriter pngWriter = new PngWriter(pngStream, imgInfo);
		pngWriter.ShouldCloseStream = false;
		byte[] array = new byte[4 * num];
		for (int i = 0; i < num2; i++)
		{
			int num3 = 0;
			for (int j = 0; j < num; j++)
			{
				uint num4 = binaryReader.ReadUInt32();
				array[num3++] = (byte)num4;
				array[num3++] = (byte)(num4 >> 8);
				array[num3++] = (byte)(num4 >> 16);
				array[num3++] = (byte)(num4 >> 24);
			}
			pngWriter.WriteRowByte(array, i);
		}
		bool result = false;
		try
		{
			binaryReader.ReadByte();
			result = true;
		}
		catch
		{
		}
		binaryReader.Dispose();
		pngWriter.End();
		return result;
	}

	private static void TextSave(Stream stream, Stream textStream)
	{
		BinaryReader binaryReader = new BinaryReader(stream);
		string s = binaryReader.ReadString();
		binaryReader.Dispose();
		textStream.Write(Encoding.UTF8.GetBytes(s), 0, Encoding.UTF8.GetBytes(s).Length);
	}

	private static void SoundSave(Stream stream, Stream wavStream)
	{
		BinaryReader binaryReader = new BinaryReader(stream);
		binaryReader.ReadBoolean();
		int num = binaryReader.ReadInt32();
		int num2 = binaryReader.ReadInt32();
		int num3 = binaryReader.ReadInt32();
		byte[] array = new byte[num3];
		if (stream.Read(array, 0, array.Length) != array.Length)
		{
			throw new Exception("还原wav文件错误");
		}
		BinaryWriter binaryWriter = new BinaryWriter(wavStream);
		binaryWriter.Write((byte)82);
		binaryWriter.Write((byte)73);
		binaryWriter.Write((byte)70);
		binaryWriter.Write((byte)70);
		binaryWriter.Write(44 + num3 / 2);
		binaryWriter.Write((byte)87);
		binaryWriter.Write((byte)65);
		binaryWriter.Write((byte)86);
		binaryWriter.Write((byte)69);
		binaryWriter.Write((byte)102);
		binaryWriter.Write((byte)109);
		binaryWriter.Write((byte)116);
		binaryWriter.Write((byte)32);
		binaryWriter.Write(16);
		binaryWriter.Write((short)1);
		binaryWriter.Write((short)num);
		binaryWriter.Write(num2);
		binaryWriter.Write(num * 2 * num2);
		binaryWriter.Write((short)(num * 2));
		binaryWriter.Write((short)16);
		binaryWriter.Write((byte)100);
		binaryWriter.Write((byte)97);
		binaryWriter.Write((byte)116);
		binaryWriter.Write((byte)97);
		binaryWriter.Write(num3);
		binaryWriter.Write(array);
		binaryWriter.Dispose();
		wavStream.Dispose();
	}

	private static void FontSave(Stream stream, Stream lstStream)
	{
		EngineBinaryReader engineBinaryReader = new EngineBinaryReader(stream);
		int num = engineBinaryReader.ReadInt32();
		string arg = "";
		arg = arg + num + "\n";
		for (int i = 0; i < num; i++)
		{
			arg = arg + engineBinaryReader.ReadChar().ToString() + "\t";
			arg = arg + engineBinaryReader.ReadSingle() + "\t";
			arg = arg + engineBinaryReader.ReadSingle() + "\t";
			arg = arg + engineBinaryReader.ReadSingle() + "\t";
			arg = arg + engineBinaryReader.ReadSingle() + "\t";
			arg = arg + engineBinaryReader.ReadSingle() + "\t";
			arg = arg + engineBinaryReader.ReadSingle() + "\t";
			arg = arg + engineBinaryReader.ReadSingle() + "\n";
		}
		arg = arg + engineBinaryReader.ReadSingle() + "\n";
		arg = arg + engineBinaryReader.ReadSingle() + "\t" + engineBinaryReader.ReadSingle() + "\n";
		arg = arg + engineBinaryReader.ReadSingle() + "\n";
		arg += engineBinaryReader.ReadChar().ToString();
		lstStream.Write(Encoding.UTF8.GetBytes(arg), 0, Encoding.UTF8.GetBytes(arg).Length);
	}

	private static void SCModelSave(Stream stream, Stream daeStream)
	{
		bool keepSourceVertexDataInTags;
		Engine.Media.ModelData model = ModelContentReader.Read(stream, out keepSourceVertexDataInTags);
		stream.Dispose();
		ColladaExporter colladaExporter = new ColladaExporter();
		colladaExporter.AddModel(model);
		colladaExporter.Save(daeStream);
	}
}
