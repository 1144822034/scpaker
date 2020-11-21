using System;
using System.IO;

namespace SCPAK
{
	public static class SoundHandler
	{
		public static void WriteSound(Stream mainStream, Stream soundStream)
		{
			BinaryReader binaryReader = new BinaryReader(soundStream);
			binaryReader.BaseStream.Position = 22L;
			int value = binaryReader.ReadInt16();
			int value2 = binaryReader.ReadInt32();
			binaryReader.BaseStream.Position += 12L;
			int num = binaryReader.ReadInt32();
			byte[] array = new byte[num];
			if (soundStream.Read(array, 0, array.Length) != array.Length)
			{
				throw new Exception("解析wav文件错误");
			}
			binaryReader.Dispose();
			BinaryWriter binaryWriter = new BinaryWriter(mainStream);
			binaryWriter.Write(value: false);
			binaryWriter.Write(value);
			binaryWriter.Write(value2);
			binaryWriter.Write(num);
			binaryWriter.Write(array);
		}

		public static void RecoverSound(Stream targetFileStream, Stream soundStream)
		{
			BinaryReader binaryReader = new BinaryReader(soundStream);
			binaryReader.ReadBoolean();
			int num = binaryReader.ReadInt32();
			int num2 = binaryReader.ReadInt32();
			int num3 = binaryReader.ReadInt32();
			byte[] array = new byte[num3];
			if (soundStream.Read(array, 0, array.Length) != array.Length)
			{
				throw new Exception("还原wav文件错误");
			}
			BinaryWriter binaryWriter = new BinaryWriter(targetFileStream);
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
		}
	}
}
