using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine.Serialization
{
	public class BinaryOutputArchive : OutputArchive, IDisposable
	{
		public int m_nextTypeId;

		public Dictionary<Type, int> m_typeIds = new Dictionary<Type, int>();

		public EngineBinaryWriter m_writer;

		public bool Use7BitInts = true;

		public Stream Stream => m_writer.BaseStream;

		public BinaryOutputArchive(Stream stream, int version = 0)
			: base(version)
		{
			m_writer = new EngineBinaryWriter(stream);
		}

		public void Dispose()
		{
			Utilities.Dispose(ref m_writer);
		}

		public override void Serialize(string name, sbyte value)
		{
			m_writer.Write(value);
		}

		public override void Serialize(string name, byte value)
		{
			m_writer.Write(value);
		}

		public override void Serialize(string name, short value)
		{
			m_writer.Write(value);
		}

		public override void Serialize(string name, ushort value)
		{
			m_writer.Write(value);
		}

		public override void Serialize(string name, int value)
		{
			if (Use7BitInts)
			{
				m_writer.Write7BitEncodedInt(value);
			}
			else
			{
				m_writer.Write(value);
			}
		}

		public override void Serialize(string name, uint value)
		{
			m_writer.Write(value);
		}

		public override void Serialize(string name, long value)
		{
			m_writer.Write(value);
		}

		public override void Serialize(string name, ulong value)
		{
			m_writer.Write(value);
		}

		public override void Serialize(string name, float value)
		{
			m_writer.Write(value);
		}

		public override void Serialize(string name, double value)
		{
			m_writer.Write(value);
		}

		public override void Serialize(string name, bool value)
		{
			m_writer.Write(value);
		}

		public override void Serialize(string name, char value)
		{
			m_writer.Write(value);
		}

		public override void Serialize(string name, string value)
		{
			m_writer.Write(value ?? string.Empty);
		}

		public override void Serialize(string name, byte[] value)
		{
			m_writer.Write7BitEncodedInt(value.Length);
			m_writer.Write(value);
		}

		public override void Serialize(string name, int length, byte[] value)
		{
			if (value.Length != length)
			{
				throw new InvalidOperationException("Invalid fixed array length.");
			}
			m_writer.Write(value, 0, length);
		}

		public override void Serialize(string name, Type type, object value)
		{
			WriteObject(Archive.GetSerializeData(type, allowEmptySerializer: true), value);
		}

		public override void SerializeCollection<T>(string name, string itemName, IEnumerable<T> collection)
		{
			SerializeData serializeData = Archive.GetSerializeData(typeof(T), allowEmptySerializer: true);
			Serialize(null, collection.Count());
			foreach (T item in collection)
			{
				WriteObject(serializeData, item);
			}
		}

		public override void SerializeDictionary<K, V>(string name, IDictionary<K, V> dictionary)
		{
			SerializeData serializeData = Archive.GetSerializeData(typeof(K), allowEmptySerializer: true);
			SerializeData serializeData2 = Archive.GetSerializeData(typeof(V), allowEmptySerializer: true);
			Serialize(null, dictionary.Count());
			foreach (KeyValuePair<K, V> item in dictionary)
			{
				WriteObject(serializeData, item.Key);
				WriteObject(serializeData2, item.Value);
			}
		}

		public override void WriteObjectInfo(int objectId, bool isReference, Type runtimeType)
		{
			if (isReference)
			{
				Serialize(null, objectId << 3);
			}
			else if (runtimeType != null)
			{
				if (m_typeIds.TryGetValue(runtimeType, out int value))
				{
					Serialize(null, 3 | (objectId << 3));
					Serialize(null, value);
					return;
				}
				value = m_nextTypeId++;
				Serialize(null, 7 | (objectId << 3));
				Serialize(null, value);
				Serialize(null, runtimeType.FullName);
				m_typeIds.Add(runtimeType, value);
			}
			else
			{
				Serialize(null, 1 | (objectId << 3));
			}
		}
	}
}
