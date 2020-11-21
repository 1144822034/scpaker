using System.IO;
using System.Text;

namespace SCPAK
{
	public static class TextHandler
	{
		public static void RecoverText(Stream targetFileStream, Stream textStream)
		{
			BinaryReader binaryReader = new BinaryReader(textStream, Encoding.UTF8, leaveOpen: true);
			binaryReader.BaseStream.Position = 0L;
			string s = binaryReader.ReadString();
			targetFileStream.Write(Encoding.UTF8.GetBytes(s), 0, Encoding.UTF8.GetBytes(s).Length);
		}

		public static void WriteText(Stream mainStream, Stream textStream)
		{
			byte[] array = new byte[textStream.Length];
			textStream.Read(array, 0, (int)textStream.Length);
			new BinaryWriter(mainStream, Encoding.UTF8, leaveOpen: true).Write(Encoding.UTF8.GetString(array));
		}
	}
}
