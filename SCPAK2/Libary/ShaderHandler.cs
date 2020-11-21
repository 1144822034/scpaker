using System.IO;
using System.Text;

namespace SCPAK
{
	public static class ShaderHandler
	{
		public static void WriteShader(Stream mainStream, Stream vertStream, Stream fragStream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(mainStream, Encoding.UTF8, leaveOpen: true);
			byte[] array = new byte[vertStream.Length];
			vertStream.Read(array, 0, (int)vertStream.Length);
			binaryWriter.Write(Encoding.UTF8.GetString(array));
			array = new byte[fragStream.Length];
			fragStream.Read(array, 0, (int)fragStream.Length);
			binaryWriter.Write(Encoding.UTF8.GetString(array));
		}

		public static void RecoverShader(Stream vertFileStream, Stream fragFileStream, Stream shaderStream)
		{
			BinaryReader binaryReader = new BinaryReader(shaderStream, Encoding.UTF8, leaveOpen: true);
			string s = binaryReader.ReadString();
			string s2 = binaryReader.ReadString();
			vertFileStream.Write(Encoding.UTF8.GetBytes(s), 0, Encoding.UTF8.GetBytes(s).Length);
			fragFileStream.Write(Encoding.UTF8.GetBytes(s2), 0, Encoding.UTF8.GetBytes(s2).Length);
		}
	}
}
