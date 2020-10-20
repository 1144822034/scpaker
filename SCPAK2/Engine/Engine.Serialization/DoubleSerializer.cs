namespace Engine.Serialization
{
	internal class DoubleSerializer : ISerializer<double>
	{
		public void Serialize(InputArchive archive, ref double value)
		{
			archive.Serialize(null, ref value);
		}

		public void Serialize(OutputArchive archive, double value)
		{
			archive.Serialize(null, value);
		}
	}
}
