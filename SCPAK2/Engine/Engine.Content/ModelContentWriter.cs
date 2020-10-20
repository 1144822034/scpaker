using Engine.Media;
using System.Collections.Generic;
using System.IO;

namespace Engine.Content
{
	[ContentWriter("Engine.Graphics.Model")]
	public class ModelContentWriter : IContentWriter
	{
		public string Model;

		[Optional]
		public bool KeepSourceVertexDataInTags;

		[Optional]
		public Matrix Transform = Matrix.Identity;

		public IEnumerable<string> GetDependencies()
		{
			yield return Model;
		}

		public void Write(string projectDirectory, Stream stream)
		{
			new BinaryWriter(stream).Write(KeepSourceVertexDataInTags);
			ModelData modelData = ModelData.Load(Storage.CombinePaths(projectDirectory, Model));
			ModelDataContentWriter.WriteModelData(stream, modelData, Transform);
		}
	}
}
