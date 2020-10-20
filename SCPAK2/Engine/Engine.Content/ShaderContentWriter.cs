using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Content
{
	[ContentWriter("Engine.Graphics.Shader")]
	public class ShaderContentWriter : IContentWriter
	{
		public string VertexShader;

		public string PixelShader;

		[Optional]
		public string Macros = string.Empty;

		public IEnumerable<string> GetDependencies()
		{
			yield return VertexShader;
			yield return PixelShader;
		}

		public void Write(string projectDirectory, Stream stream)
		{
			string value = Storage.ReadAllText(Storage.CombinePaths(projectDirectory, VertexShader));
			string value2 = Storage.ReadAllText(Storage.CombinePaths(projectDirectory, PixelShader));
			string[] array = Macros.Split(new char[1]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			ShaderMacro[] array2 = new ShaderMacro[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string[] array3 = array[i].Split('=', StringSplitOptions.None);
				if (array3.Length == 1)
				{
					array2[i] = new ShaderMacro(array3[0].Trim());
					continue;
				}
				if (array3.Length == 2)
				{
					array2[i] = new ShaderMacro(array3[0].Trim(), array3[1].Trim());
					continue;
				}
				throw new InvalidOperationException("Error parsing shader macros.");
			}
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(value);
			binaryWriter.Write(value2);
			binaryWriter.Write(array2.Length);
			for (int j = 0; j < array2.Length; j++)
			{
				binaryWriter.Write(array2[j].Name);
				binaryWriter.Write(array2[j].Value);
			}
		}
	}
}
