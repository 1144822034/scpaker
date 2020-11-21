// SCPAK.PakData
using Engine;
using Engine.Serialization;
using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

internal class PakData
{
	public static Stream _PakData(string fileName, string typeName)
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream();
			FileStream fileStream;
			switch (typeName)
			{
				case "System.String":
					fileStream = File.OpenRead(fileName + ".txt");
					TextWriter(memoryStream, fileStream);
					break;
				case "System.Xml.Linq.XElement":
					fileStream = File.OpenRead(fileName + ".xml");
					TextWriter(memoryStream, fileStream);
					break;
				case "Engine.Media.StreamingSource":
					fileStream = File.OpenRead(fileName + ".ogg");
					fileStream.CopyTo(memoryStream);
					break;
				case "Engine.Graphics.Model":
					fileStream = File.OpenRead(fileName + ".dae");
					SCModelWriter(memoryStream, fileStream);
					break;
				case "Engine.Graphics.Shader":
					fileStream = File.OpenRead(fileName + ".fsh");
					fileStream.CopyTo(memoryStream);
					break;
				case "Engine.Audio.SoundBuffer":
					fileStream = File.OpenRead(fileName + ".wav");
					SoundWriter(memoryStream, fileStream);
					break;
				case "Engine.Graphics.Texture2D":
					fileStream = File.OpenRead(fileName + ".png");
					if (Path.GetFileNameWithoutExtension(fileName + ".png")[0] == '!')
					{
						PngWriter(memoryStream, fileStream, modelpng: true);
					}
					else
					{
						PngWriter(memoryStream, fileStream);
					}
					break;
				case "Engine.Media.BitmapFont":
					{
						fileStream = File.OpenRead(fileName + ".lst");
						FontWriter(memoryStream, fileStream);
						int length = Path.GetFileName(fileName + ".lst").Length;
						string str = (fileName + ".lst").Substring(0, (fileName + ".lst").Length - length);
						FileStream fileStream2;
						if (File.Exists(fileName + ".png"))
						{
							fileStream2 = File.OpenRead(fileName + ".png");
							PngWriter(memoryStream, fileStream2);
						}
						else
						{
							if (!File.Exists(str + "!" + Path.GetFileNameWithoutExtension(fileName + ".lst") + ".png"))
							{
								throw new Exception("字体库错误！！！");
							}
							fileStream2 = File.OpenRead(str + "!" + Path.GetFileNameWithoutExtension(fileName + ".lst") + ".png");
							PngWriter(memoryStream, fileStream, modelpng: true);
						}
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
		catch (Exception innerException)
		{
			throw new Exception("文件写入错误 :" + fileName + "\t类型 :" + typeName, innerException);
		}
	}

	private static void TextWriter(MemoryStream memoryStream, FileStream fileStream)
	{
		byte[] array = new byte[fileStream.Length];
		fileStream.Read(array, 0, (int)fileStream.Length);
		new EngineBinaryWriter(memoryStream).Write(Encoding.UTF8.GetString(array));
	}

	private static bool IsPowerOf2(long x)
	{
		if (x > 0)
		{
			return (x & (x - 1)) == 0;
		}
		return false;
	}

	private static void PngWriter(MemoryStream memoryStream, FileStream fileStream, bool modelpng = false)
	{
		 PngReader pngReader = new PngReader(fileStream);
		EngineBinaryWriter engineBinaryWriter = new EngineBinaryWriter(memoryStream);
		engineBinaryWriter.Write(value: false);
		pngReader.ShouldCloseStream = false;
		pngReader.ChunkLoadBehaviour = ChunkLoadBehaviour.LOAD_CHUNK_NEVER;
		pngReader.MaxTotalBytesRead = long.MaxValue;
		ImageLines imageLines = pngReader.ReadRowsByte();
		pngReader.End();
		Engine.Media.Image image = new Engine.Media.Image(pngReader.ImgInfo.Cols, pngReader.ImgInfo.Rows);
		int num = 0;
		for (int i = 0; i < image.Height; i++)
		{
			byte[] array = imageLines.ScanlinesB[i];
			int num2 = 0;
			for (int j = 0; j < image.Width; j++)
			{
				byte r = array[num2++];
				byte g = array[num2++];
				byte b = array[num2++];
				byte a = array[num2++];
				image.Pixels[num++] = new Color(r, g, b, a);
			}
		}
		List<Engine.Media.Image> list = new List<Engine.Media.Image>();
		if (IsPowerOf2(image.Width) && IsPowerOf2(image.Height))
		{
			list.AddRange(Engine.Media.Image.GenerateMipmaps(image));
		}
		else
		{
			list.Add(image);
			modelpng = true;
		}
		if (!modelpng)
		{
			engineBinaryWriter.Write(image.Width);
			engineBinaryWriter.Write(image.Height);
			engineBinaryWriter.Write(1);
			for (int k = 0; k < image.Height; k++)
			{
				byte[] array2 = imageLines.ScanlinesB[k];
				int num3 = 0;
				for (int l = 0; l < image.Width; l++)
				{
					byte r2 = array2[num3++];
					byte g2 = array2[num3++];
					byte b2 = array2[num3++];
					byte a2 = array2[num3++];
					engineBinaryWriter.Write(new Color(r2, g2, b2, a2).PackedValue);
				}
			}
		}
		else
		{
			engineBinaryWriter.Write(list[0].Width);
			engineBinaryWriter.Write(list[0].Height);
			engineBinaryWriter.Write(list.Count);
			foreach (Engine.Media.Image item in list)
			{
				for (int m = 0; m < item.Pixels.Length; m++)
				{
					engineBinaryWriter.Write(item.Pixels[m].PackedValue);
				}
			}
		}
	}

	private static void SoundWriter(MemoryStream memoryStream, FileStream fileStream)
	{
		BinaryReader binaryReader = new BinaryReader(fileStream);
		binaryReader.BaseStream.Position = 22L;
		int value = binaryReader.ReadInt16();
		int value2 = binaryReader.ReadInt32();
		binaryReader.BaseStream.Position += 12L;
		int num = binaryReader.ReadInt32();
		byte[] array = new byte[num];
		if (fileStream.Read(array, 0, array.Length) != array.Length)
		{
			throw new Exception("解析wav文件错误");
		}
		binaryReader.Dispose();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(value: false);
		binaryWriter.Write(value);
		binaryWriter.Write(value2);
		binaryWriter.Write(num);
		binaryWriter.Write(array);
	}

	private static void FontWriter(MemoryStream memoryStream, FileStream fileStream)
	{
		EngineBinaryWriter engineBinaryWriter = new EngineBinaryWriter(memoryStream);
		StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8);
		int num = int.Parse(streamReader.ReadLine());
		engineBinaryWriter.Write(num);
		for (int i = 0; i < num; i++)
		{
			string[] array = streamReader.ReadLine().Split('\t');
			engineBinaryWriter.Write(char.Parse(array[0]));
			engineBinaryWriter.Write(float.Parse(array[1]));
			engineBinaryWriter.Write(float.Parse(array[2]));
			engineBinaryWriter.Write(float.Parse(array[3]));
			engineBinaryWriter.Write(float.Parse(array[4]));
			engineBinaryWriter.Write(float.Parse(array[5]));
			engineBinaryWriter.Write(float.Parse(array[6]));
			engineBinaryWriter.Write(float.Parse(array[7]));
		}
		engineBinaryWriter.Write(float.Parse(streamReader.ReadLine()));
		string[] array2 = streamReader.ReadLine().Split('\t');
		engineBinaryWriter.Write(float.Parse(array2[0]));
		engineBinaryWriter.Write(float.Parse(array2[1]));
		engineBinaryWriter.Write(float.Parse(streamReader.ReadLine()));
		engineBinaryWriter.Write(char.Parse(streamReader.ReadLine()));
	}

	private static void SCModelWriter(MemoryStream memoryStream, FileStream fileStream)
	{
		Engine.Media.ModelData data = Engine.Media.Collada.Load(fileStream);
		ModelContentWriter.Write(memoryStream, data, Vector3.One);
	}
}
