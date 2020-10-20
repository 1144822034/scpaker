// SCPAK.UnPak
using System.Collections.Generic;
using System.IO;
using System.Text;


public class UnPak21
{
	private static void isPakFile(BinaryReader binaryReader)
	{
		byte[] array = new byte[4];
		if (binaryReader.Read(array, 0, array.Length) != array.Length || array[0] != 80 || array[1] != 65 || array[2] != 75 || array[3] != 0)
		{
			throw new FileLoadException("该文件不是Survivalcraft2的PAK文件！");
		}
	}

	public UnPak21(string PakFile)
	{
		if (!File.Exists(PakFile))
		{
			throw new FileNotFoundException("文件不存在！");
		}
		string pakDirectory = Path.GetDirectoryName(PakFile) + "/" + Path.GetFileNameWithoutExtension(PakFile);
		FileStream fileStream = new FileStream(PakFile, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(fileStream, Encoding.UTF8, leaveOpen: true);
		isPakFile(binaryReader);
		int num = binaryReader.ReadInt32();
		int num2 = binaryReader.ReadInt32();
		List<PAKInfo> list = new List<PAKInfo>(num2);
		for (int i = 0; i < num2; i++)
		{
			string fileName = binaryReader.ReadString();
			string typeName = binaryReader.ReadString();
			int position = binaryReader.ReadInt32() + num;
			int bytesCount = binaryReader.ReadInt32();
			long position2 = binaryReader.BaseStream.Position;
			list.Add(new PAKInfo
			{
				fileStream = ContentFile(fileStream, position, bytesCount),
				fileName = fileName,
				typeName = typeName
			});
			binaryReader.BaseStream.Position = position2;
		}
		UnPakData._UnPakData(list, pakDirectory);
		binaryReader.Dispose();
		fileStream.Dispose();
	}

	public Stream ContentFile(Stream stream, int position, int bytesCount)
	{
		if (stream is MemoryStream)
		{
			return new MemoryStream(((MemoryStream)stream).ToArray(), position, bytesCount);
		}
		stream.Position = position;
		byte[] array = new byte[bytesCount];
		stream.Read(array, 0, array.Length);
		return new MemoryStream(array, writable: false);
	}
}
