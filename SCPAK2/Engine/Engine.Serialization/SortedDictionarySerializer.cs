using System.Collections.Generic;

namespace Engine.Serialization
{
	internal class SortedDictionarySerializer<K, V> : ISerializer<SortedDictionary<K, V>>
	{
		public void Serialize(InputArchive archive, ref SortedDictionary<K, V> value)
		{
			value = new SortedDictionary<K, V>();
			archive.SerializeDictionary(null, (IDictionary<K, V>)value);
		}

		public void Serialize(OutputArchive archive, SortedDictionary<K, V> value)
		{
			archive.SerializeDictionary(null, (IDictionary<K, V>)value);
		}
	}
}
