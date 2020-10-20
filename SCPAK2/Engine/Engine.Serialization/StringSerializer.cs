namespace Engine.Serialization
{
	internal class StringSerializer : ISerializer<string>
	{
		public void Serialize(InputArchive archive, ref string value)
		{
			archive.Serialize(null, ref value);
		}

		public void Serialize(OutputArchive archive, string value)
		{
			archive.Serialize(null, value);
		}
	}
}
