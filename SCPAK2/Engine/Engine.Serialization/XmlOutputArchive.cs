using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Engine.Serialization
{
	public class XmlOutputArchive : OutputArchive
	{
		public XElement Node
		{
			get;
			set;
		}

		public XmlOutputArchive(string rootNodeName, int version = 0)
			: this(new XElement(rootNodeName), version)
		{
		}

		public XmlOutputArchive(XElement node, int version = 0)
			: base(version)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			Node = node;
		}

		public override void Serialize(string name, sbyte value)
		{
			Serialize(name, value.ToString(CultureInfo.InvariantCulture));
		}

		public override void Serialize(string name, byte value)
		{
			Serialize(name, value.ToString(CultureInfo.InvariantCulture));
		}

		public override void Serialize(string name, short value)
		{
			Serialize(name, value.ToString(CultureInfo.InvariantCulture));
		}

		public override void Serialize(string name, ushort value)
		{
			Serialize(name, value.ToString(CultureInfo.InvariantCulture));
		}

		public override void Serialize(string name, int value)
		{
			Serialize(name, value.ToString(CultureInfo.InvariantCulture));
		}

		public override void Serialize(string name, uint value)
		{
			Serialize(name, value.ToString(CultureInfo.InvariantCulture));
		}

		public override void Serialize(string name, long value)
		{
			Serialize(name, value.ToString(CultureInfo.InvariantCulture));
		}

		public override void Serialize(string name, ulong value)
		{
			Serialize(name, value.ToString(CultureInfo.InvariantCulture));
		}

		public override void Serialize(string name, float value)
		{
			Serialize(name, value.ToString("R", CultureInfo.InvariantCulture));
		}

		public override void Serialize(string name, double value)
		{
			Serialize(name, value.ToString("R", CultureInfo.InvariantCulture));
		}

		public override void Serialize(string name, bool value)
		{
			Serialize(name, value ? "True" : "False");
		}

		public override void Serialize(string name, char value)
		{
			Serialize(name, value.ToString());
		}

		public override void Serialize(string name, string value)
		{
			if (name == null)
			{
				Node.SetValue(value ?? string.Empty);
			}
			else
			{
				Node.SetAttributeValue(name, value ?? string.Empty);
			}
		}

		public override void Serialize(string name, byte[] value)
		{
			Serialize(name, Convert.ToBase64String(value));
		}

		public override void Serialize(string name, int length, byte[] value)
		{
			if (value.Length != length)
			{
				throw new InvalidOperationException("Invalid fixed array length.");
			}
			Serialize(name, Convert.ToBase64String(value));
		}

		public override void Serialize(string name, Type type, object value)
		{
			WriteObject(name, Archive.GetSerializeData(type, allowEmptySerializer: true), value);
		}

		public override void SerializeCollection<T>(string name, string itemName, IEnumerable<T> collection)
		{
			EnterNode(name);
			SerializeData serializeData = Archive.GetSerializeData(typeof(T), allowEmptySerializer: true);
			foreach (T item in collection)
			{
				EnterNode(itemName);
				WriteObject(null, serializeData, item);
				LeaveNode(itemName);
			}
			LeaveNode(name);
		}

		public override void SerializeDictionary<K, V>(string name, IDictionary<K, V> dictionary)
		{
			EnterNode(name);
			if (typeof(K) == typeof(string))
			{
				SerializeData serializeData = Archive.GetSerializeData(typeof(V), allowEmptySerializer: true);
				foreach (KeyValuePair<K, V> item in dictionary)
				{
					string name2 = item.Key as string;
					EnterNode(name2);
					WriteObject(null, serializeData, item.Value);
					LeaveNode(name2);
				}
			}
			else
			{
				SerializeData serializeData2 = Archive.GetSerializeData(typeof(K), allowEmptySerializer: true);
				SerializeData serializeData3 = Archive.GetSerializeData(typeof(V), allowEmptySerializer: true);
				foreach (KeyValuePair<K, V> item2 in dictionary)
				{
					EnterNode("e");
					WriteObject("k", serializeData2, item2.Key);
					WriteObject("v", serializeData3, item2.Value);
					LeaveNode("e");
				}
			}
			LeaveNode(name);
		}

		public override void WriteObjectInfo(int objectId, bool isReference, Type runtimeType)
		{
			if (isReference)
			{
				Node.SetAttributeValue("_ref", objectId.ToString(CultureInfo.InvariantCulture));
				return;
			}
			Node.SetAttributeValue("_def", objectId.ToString(CultureInfo.InvariantCulture));
			if (runtimeType != null)
			{
				Node.SetAttributeValue("_type", runtimeType.FullName);
			}
		}

		public void WriteObject(string name, SerializeData staticSerializeData, object value)
		{
			if (staticSerializeData.IsHumanReadableSupported)
			{
				Serialize(name, (value != null) ? HumanReadableConverter.ConvertToString(value) : string.Empty);
				return;
			}
			EnterNode(name);
			base.WriteObject(staticSerializeData, value);
			LeaveNode(name);
		}

		public void EnterNode(string name)
		{
			if (name != null)
			{
				XElement xElement = new XElement(name);
				Node.Add(xElement);
				Node = xElement;
			}
		}

		public void LeaveNode(string name)
		{
			if (name != null)
			{
				Node = Node.Parent;
			}
		}
	}
}
