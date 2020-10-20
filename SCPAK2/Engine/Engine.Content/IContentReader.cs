namespace Engine.Content
{
	public interface IContentReader
	{
		object Read(ContentStream stream, object existingObject);
	}
}
