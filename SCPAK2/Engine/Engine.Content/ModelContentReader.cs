using Engine.Graphics;
using Engine.Media;
using System.IO;

namespace Engine.Content
{
	[ContentReader("Engine.Graphics.Model")]
	public class ModelContentReader : IContentReader
	{
		public object Read(ContentStream stream, object existingObject)
		{
			bool keepSourceVertexDataInTags = new BinaryReader(stream).ReadBoolean();
			ModelData modelData = ModelDataContentReader.ReadModelData(stream);
			if (existingObject == null)
			{
				return Model.Load(modelData, keepSourceVertexDataInTags);
			}
			Model obj = (Model)existingObject;
			obj.Initialize(modelData, keepSourceVertexDataInTags);
			return obj;
		}
	}
}
