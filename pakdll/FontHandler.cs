using System.IO;
using System.Text;

namespace SCPAK
{
	public static class FontHandler
	{
		public static void WriteFont(Stream mainStream, Stream lstStream, Stream bitmapStream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(mainStream);
			StreamReader streamReader = new StreamReader(lstStream, Encoding.UTF8);
			int num = int.Parse(streamReader.ReadLine());
			binaryWriter.Write(num);
			for (int i = 0; i < num; i++)
			{
				string[] array = streamReader.ReadLine().Split('\t');
				binaryWriter.Write(char.Parse(array[0]));
				binaryWriter.Write(float.Parse(array[1]));
				binaryWriter.Write(float.Parse(array[2]));
				binaryWriter.Write(float.Parse(array[3]));
				binaryWriter.Write(float.Parse(array[4]));
				binaryWriter.Write(float.Parse(array[5]));
				binaryWriter.Write(float.Parse(array[6]));
				binaryWriter.Write(float.Parse(array[7]));
			}
			binaryWriter.Write(float.Parse(streamReader.ReadLine()));
			string[] array2 = streamReader.ReadLine().Split('\t');
			binaryWriter.Write(float.Parse(array2[0]));
			binaryWriter.Write(float.Parse(array2[1]));
			binaryWriter.Write(float.Parse(streamReader.ReadLine()));
			binaryWriter.Write(char.Parse(streamReader.ReadLine()));
			Texture2DHandler.WriteTexture2D(mainStream, bitmapStream);
		}

		public static void RecoverFont(Stream lstFileStream, Stream bitmapFileStream, Stream fontStream)
		{
			BinaryReader binaryReader = new BinaryReader(fontStream);
			int num = binaryReader.ReadInt32();
			string str = "";
			str = str + num.ToString() + "\n";
			for (int i = 0; i < num; i++)
			{
				str = str + binaryReader.ReadChar().ToString() + "\t";
				str = str + binaryReader.ReadSingle().ToString() + "\t";
				str = str + binaryReader.ReadSingle().ToString() + "\t";
				str = str + binaryReader.ReadSingle().ToString() + "\t";
				str = str + binaryReader.ReadSingle().ToString() + "\t";
				str = str + binaryReader.ReadSingle().ToString() + "\t";
				str = str + binaryReader.ReadSingle().ToString() + "\t";
				str = str + binaryReader.ReadSingle().ToString() + "\n";
			}
			str = str + binaryReader.ReadSingle().ToString() + "\n";
			str = str + binaryReader.ReadSingle().ToString() + "\t" + binaryReader.ReadSingle().ToString() + "\n";
			str = str + binaryReader.ReadSingle().ToString() + "\n";
			str += binaryReader.ReadChar().ToString();
			lstFileStream.Write(Encoding.UTF8.GetBytes(str), 0, Encoding.UTF8.GetBytes(str).Length);
			Texture2DHandler.RecoverTexture2D(bitmapFileStream, fontStream);
		}
	}
}
