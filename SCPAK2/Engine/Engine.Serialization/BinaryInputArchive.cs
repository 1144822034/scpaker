using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.Serialization
{
	public class BinaryInputArchive : InputArchive, IDisposable
	{
		public Dictionary<int, Type> m_typeIds = new Dictionary<int, Type>();

		public EngineBinaryReader m_reader;

		public bool Use7BitInts = true;

		public Stream Stream => m_reader.BaseStream;

		public BinaryInputArchive(Stream stream, int version = 0)
			: base(version)
		{
			m_reader = new EngineBinaryReader(stream);
		}

		public void Dispose()
		{
			Utilities.Dispose(ref m_reader);
		}

		public override void Serialize(string name, ref sbyte value)
		{
			value = m_reader.ReadSByte();
		}

		public override void Serialize(string name, ref byte value)
		{
			value = m_reader.ReadByte();
		}

		public override void Serialize(string name, ref short value)
		{
			value = m_reader.ReadInt16();
		}

		public override void Serialize(string name, ref ushort value)
		{
			value = m_reader.ReadUInt16();
		}

		public override void Serialize(string name, ref int value)
		{
			if (Use7BitInts)
			{
				value = m_reader.Read7BitEncodedInt();
			}
			else
			{
				value = m_reader.ReadInt32();
			}
		}

		public override void Serialize(string name, ref uint value)
		{
			value = m_reader.ReadUInt32();
		}

		public override void Serialize(string name, ref long value)
		{
			value = m_reader.ReadInt64();
		}

		public override void Serialize(string name, ref ulong value)
		{
			value = m_reader.ReadUInt64();
		}

		public override void Serialize(string name, ref float value)
		{
			value = m_reader.ReadSingle();
		}

		public override void Serialize(string name, ref double value)
		{
			value = m_reader.ReadDouble();
		}

		public override void Serialize(string name, ref bool value)
		{
			value = m_reader.ReadBoolean();
		}

		public override void Serialize(string name, ref char value)
		{
			value = m_reader.ReadChar();
		}

		public override void Serialize(string name, ref string value)
		{
			value = m_reader.ReadString();
		}

		public override void Serialize(string name, ref byte[] value)
		{
			value = new byte[m_reader.Read7BitEncodedInt()];
			if (m_reader.Read(value, 0, value.Length) != value.Length)
			{
				throw new InvalidOperationException();
			}
		}

		public override void Serialize(string name, int length, ref byte[] value)
		{
			value = new byte[length];
			if (m_reader.Read(value, 0, value.Length) != length)
			{
				throw new InvalidOperationException();
			}
		}

		public override void Serialize(string name, Type type, ref object value)
		{
			ReadObject(Archive.GetSerializeData(type, allowEmptySerializer: true), ref value);
		}

		public override void SerializeCollection<T>(string name, ICollection<T> collection)
		{
			SerializeData serializeData = Archive.GetSerializeData(typeof(T), allowEmptySerializer: true);
			int value = 0;
			Serialize(null, ref value);
			for (int i = 0; i < value; i++)
			{
				object value2 = null;
				ReadObject(serializeData, ref value2);
				collection.Add((T)value2);
			}
		}

		public override void SerializeDictionary<K, V>(string name, IDictionary<K, V> dictionary)
		{
			SerializeData serializeData = Archive.GetSerializeData(typeof(K), allowEmptySerializer: true);
			SerializeData serializeData2 = Archive.GetSerializeData(typeof(V), allowEmptySerializer: true);
			int value = 0;
			Serialize(null, ref value);
			for (int i = 0; i < value; i++)
			{
				object value2 = null;
				object value3 = null;
				ReadObject(serializeData, ref value2);
				if (dictionary.TryGetValue((K)value2, out V value4))
				{
					value3 = value4;
				}
				ReadObject(serializeData2, ref value3);
				dictionary.Add((K)value2, (V)value3);
			}
		}

		public override void ReadObjectInfo(out int objectId, out bool isReference, out Type runtimeType)
		{
			int value = 0;
			Serialize(null, ref value);
			objectId = value >> 3;
			isReference = ((value & 1) == 0);
			if ((value & 2) != 0)
			{
				int value2 = 0;
				Serialize(null, ref value2);
				if ((value & 4) != 0)
				{
					string value3 = null;
					Serialize(null, ref value3);
					runtimeType = TypeCache.FindType(value3, skipSystemAssemblies: false, throwIfNotFound: true);
					m_typeIds.Add(value2, runtimeType);
				}
				else
				{
					runtimeType = m_typeIds[value2];
				}
			}
			else
			{
				runtimeType = null;
			}
		}
	}
}
