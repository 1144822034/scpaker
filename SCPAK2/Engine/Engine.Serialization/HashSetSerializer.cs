using System.Collections.Generic;

namespace Engine.Serialization
{
	internal class HashSetSerializer<T> : ISerializer<HashSet<T>>
	{
		public void Serialize(InputArchive archive, ref HashSet<T> value)
		{
			value = new HashSet<T>();
			archive.SerializeCollection(null, value);
		}

		public void Serialize(OutputArchive archive, HashSet<T> value)
		{
			archive.SerializeCollection(null, "e", value);
		}
	}
}
