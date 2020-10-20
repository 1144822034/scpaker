using System.Collections.Generic;

namespace Engine.Serialization
{
	internal class ListSerializer<T> : ISerializer<List<T>>
	{
		public void Serialize(InputArchive archive, ref List<T> value)
		{
			value = new List<T>();
			archive.SerializeCollection(null, value);
		}

		public void Serialize(OutputArchive archive, List<T> value)
		{
			archive.SerializeCollection(null, "e", value);
		}
	}
}
