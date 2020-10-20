using System.Collections.Generic;

namespace Engine.Serialization
{
	internal class SortedListSerializer<K, V> : ISerializer<SortedList<K, V>>
	{
		public void Serialize(InputArchive archive, ref SortedList<K, V> value)
		{
			value = new SortedList<K, V>();
			archive.SerializeDictionary(null, value);
		}

		public void Serialize(OutputArchive archive, SortedList<K, V> value)
		{
			archive.SerializeDictionary(null, value);
		}
	}
}
