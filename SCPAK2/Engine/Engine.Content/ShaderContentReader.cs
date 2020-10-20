using Engine.Graphics;
using System;
using System.IO;

namespace Engine.Content
{
	[ContentReader("Engine.Graphics.Shader")]
	public class ShaderContentReader : IContentReader
	{
		public object Read(ContentStream stream, object existingObject)
		{
			if (existingObject == null)
			{
				BinaryReader binaryReader = new BinaryReader(stream);
				string vertexShaderCode = binaryReader.ReadString();
				string pixelShaderCode = binaryReader.ReadString();
				int num = binaryReader.ReadInt32();
				ShaderMacro[] array = new ShaderMacro[num];
				for (int i = 0; i < num; i++)
				{
					string name = binaryReader.ReadString();
					string value = binaryReader.ReadString();
					array[i] = new ShaderMacro(name, value);
				}
				return new Shader(vertexShaderCode, pixelShaderCode, array);
			}
			throw new NotSupportedException();
		}
	}
}
