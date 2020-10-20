using System;
using System.Collections.Generic;

namespace Engine.Serialization
{
	public abstract class InputArchive : Archive
	{
		public Dictionary<int, object> m_objectById = new Dictionary<int, object>();

		public InputArchive(int version)
			: base(version)
		{
		}

		public abstract void Serialize(string name, ref sbyte value);

		public abstract void Serialize(string name, ref byte value);

		public abstract void Serialize(string name, ref short value);

		public abstract void Serialize(string name, ref ushort value);

		public abstract void Serialize(string name, ref int value);

		public abstract void Serialize(string name, ref uint value);

		public abstract void Serialize(string name, ref long value);

		public abstract void Serialize(string name, ref ulong value);

		public abstract void Serialize(string name, ref float value);

		public abstract void Serialize(string name, ref double value);

		public abstract void Serialize(string name, ref bool value);

		public abstract void Serialize(string name, ref char value);

		public abstract void Serialize(string name, ref string value);

		public abstract void Serialize(string name, ref byte[] value);

		public abstract void Serialize(string name, int length, ref byte[] value);

		public abstract void Serialize(string name, Type type, ref object value);

		public abstract void SerializeCollection<T>(string name, ICollection<T> collection);

		public abstract void SerializeDictionary<K, V>(string name, IDictionary<K, V> dictionary);

		public void Serialize(string name, Type type, object value)
		{
			if (value == null)
			{
				throw new InvalidOperationException("Value cannot be null");
			}
			Serialize(name, type, ref value);
		}

		public void Serialize<T>(string name, T value) where T : class
		{
			object value2 = value;
			Serialize(name, typeof(T), ref value2);
		}

		public void Serialize<T>(string name, ref T value)
		{
			object value2 = value;
			Serialize(name, typeof(T), ref value2);
			value = (T)value2;
		}

		public void Serialize<T>(string name, Action<T> setter)
		{
			T value = default(T);
			Serialize(name, ref value);
			setter(value);
		}

		public T Serialize<T>(string name)
		{
			T value = default(T);
			Serialize(name, ref value);
			return value;
		}

		public void Serialize(string name, Type type, Action<object> setter)
		{
			object value = null;
			Serialize(name, type, ref value);
			setter(value);
		}

		public object Serialize(string name, Type type)
		{
			object value = null;
			Serialize(name, type, ref value);
			return value;
		}

		public List<T> SerializeCollection<T>(string name)
		{
			List<T> list = new List<T>();
			SerializeCollection(name, list);
			return list;
		}

		public Dictionary<K, V> SerializeDictionary<K, V>(string name)
		{
			Dictionary<K, V> dictionary = new Dictionary<K, V>();
			SerializeDictionary(name, dictionary);
			return dictionary;
		}

		public abstract void ReadObjectInfo(out int objectId, out bool isReference, out Type runtimeType);

		public virtual void ReadObject(SerializeData staticSerializeData, ref object value)
		{
			if (!staticSerializeData.UseObjectInfo || !base.UseObjectInfos)
			{
				ReadObjectWithoutObjectInfo(staticSerializeData, ref value);
			}
			else
			{
				ReadObjectWithObjectInfo(staticSerializeData, ref value);
			}
		}

		public void ReadObjectWithoutObjectInfo(SerializeData staticSerializeData, ref object value)
		{
			Type type = (value != null) ? value.GetType() : null;
			SerializeData serializeData = (!(type == null) && !(staticSerializeData.Type == type)) ? Archive.GetSerializeData(type, allowEmptySerializer: false) : staticSerializeData;
			if (serializeData.AutoConstructObject && value == null)
			{
				value = Activator.CreateInstance(serializeData.Type, nonPublic: true);
			}
			serializeData.Read(this, ref value);
		}

		public void ReadObjectWithObjectInfo(SerializeData staticSerializeData, ref object value)
		{
			ReadObjectInfo(out int objectId, out bool isReference, out Type runtimeType);
			if (objectId == 0)
			{
				if (value != null)
				{
					throw new InvalidOperationException("Serializing null reference into an existing object.");
				}
				return;
			}
			if (isReference)
			{
				if (value != null)
				{
					throw new InvalidOperationException("Serializing a reference into an existing object.");
				}
				value = m_objectById[objectId];
				return;
			}
			Type type = (value != null) ? value.GetType() : null;
			SerializeData serializeData;
			if (!(type != null))
			{
				serializeData = ((!(runtimeType != null)) ? staticSerializeData : Archive.GetSerializeData(runtimeType, allowEmptySerializer: false));
			}
			else
			{
				if (runtimeType != null && runtimeType != type)
				{
					throw new InvalidOperationException("Serialized object has different type than existing object.");
				}
				serializeData = Archive.GetSerializeData(type, allowEmptySerializer: false);
			}
			if (serializeData.AutoConstructObject && value == null)
			{
				value = Activator.CreateInstance(serializeData.Type, nonPublic: true);
			}
			serializeData.Read(this, ref value);
			m_objectById.Add(objectId, value);
		}
	}
}
