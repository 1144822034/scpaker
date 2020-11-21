// Engine.ModelContentWriter
using Engine;
using Engine.Media;
using System.IO;

public class ModelContentWriter
{
	public static void Write(Stream stream, ModelData data, Vector3 scale, bool keepSourceVertexDataInTags = true)
	{
		new BinaryWriter(stream).Write(keepSourceVertexDataInTags);
		ModelDataContentWriter.WriteModelData(stream, data, scale);
	}
}
