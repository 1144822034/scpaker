// Engine.ModelContentReader
using Engine;
using Engine.Content;
using Engine.Media;
using System.IO;

public class ModelContentReader
{
	public static ModelData Read(Stream s, out bool keepSourceVertexDataInTags)
	{
		keepSourceVertexDataInTags = new BinaryReader(s).ReadBoolean();
		return ModelDataContentReader.ReadModelData(s);
	}
}
