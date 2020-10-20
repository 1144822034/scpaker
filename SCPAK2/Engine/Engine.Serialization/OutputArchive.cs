using System;
using System.Collections.Generic;

namespace Engine.Serialization
{
	public abstract class OutputArchive : Archive
	{
		public int m_nextObjectId = 1;

		public Dictionary<object, int> m_idByObject = new Dictionary<object, int>();

		public OutputArchive(int version)
			: base(version)
		{
		}

		public abstract void Serialize(string name, sbyte value);

		public abstract void Serialize(string name, byte value);

		public abstract void Serialize(string name, short value);

		public abstract void Serialize(string name, ushort value);

		public abstract void Serialize(string name, int value);

		public abstract void Serialize(string name, uint value);

		public abstract void Serialize(string name, long value);

		public abstract void Serialize(string name, ulong value);

		public abstract void Serialize(string name, float value);

		public abstract void Serialize(string name, double value);

		public abstract void Serialize(string name, bool value);

		public abstract void Serialize(string name, char value);

		public abstract void Serialize(string name, string value);

		public abstract void Serialize(string name, byte[] value);

		public abstract void Serialize(string name, int length, byte[] value);

		public abstract void Serialize(string name, Type type, object value);

		public abstract void SerializeCollection<T>(string name, string itemName, IEnumerable<T> collection);

		public abstract void SerializeDictionary<K, V>(string name, IDictionary<K, V> dictionary);

		public void Serialize<T>(string name, T value)
		{
			Serialize(name, typeof(T), value);
		}

		public abstract void WriteObjectInfo(int objectId, bool isReference, Type runtimeType);

		public virtual void WriteObject(SerializeData staticSerializeData, object value)
		{
			if (!staticSerializeData.UseObjectInfo || !base.UseObjectInfos)
			{
				staticSerializeData.Write(this, value);
				return;
			}
			if (value == null)
			{
				WriteObjectInfo(0, isReference: true, null);
				return;
			}
			if (m_idByObject.TryGetValue(value, out int value2))
			{
				WriteObjectInfo(value2, isReference: true, null);
				return;
			}
			value2 = m_nextObjectId++;
			m_idByObject.Add(value, value2);
			Type type = value.GetType();
			if (type == staticSerializeData.Type)
			{
				WriteObjectInfo(value2, isReference: false, null);
				staticSerializeData.Write(this, value);
			}
			else
			{
				SerializeData serializeData = Archive.GetSerializeData(type, allowEmptySerializer: false);
				WriteObjectInfo(value2, isReference: false, type);
				serializeData.Write(this, value);
			}
		}
	}
}
