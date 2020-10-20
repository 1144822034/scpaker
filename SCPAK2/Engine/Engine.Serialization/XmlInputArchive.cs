using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Engine.Serialization
{
	public class XmlInputArchive : InputArchive
	{
		public XElement Node
		{
			get;
			set;
		}

		public XmlInputArchive(XElement node, int version = 0)
			: base(version)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			Node = node;
		}

		public override void Serialize(string name, ref sbyte value)
		{
			string value2 = null;
			Serialize(name, ref value2);
			value = sbyte.Parse(value2, CultureInfo.InvariantCulture);
		}

		public override void Serialize(string name, ref byte value)
		{
			string value2 = null;
			Serialize(name, ref value2);
			value = byte.Parse(value2, CultureInfo.InvariantCulture);
		}

		public override void Serialize(string name, ref short value)
		{
			string value2 = null;
			Serialize(name, ref value2);
			value = short.Parse(value2, CultureInfo.InvariantCulture);
		}

		public override void Serialize(string name, ref ushort value)
		{
			string value2 = null;
			Serialize(name, ref value2);
			value = ushort.Parse(value2, CultureInfo.InvariantCulture);
		}

		public override void Serialize(string name, ref int value)
		{
			string value2 = null;
			Serialize(name, ref value2);
			value = int.Parse(value2, CultureInfo.InvariantCulture);
		}

		public override void Serialize(string name, ref uint value)
		{
			string value2 = null;
			Serialize(name, ref value2);
			value = uint.Parse(value2, CultureInfo.InvariantCulture);
		}

		public override void Serialize(string name, ref long value)
		{
			string value2 = null;
			Serialize(name, ref value2);
			value = long.Parse(value2, CultureInfo.InvariantCulture);
		}

		public override void Serialize(string name, ref ulong value)
		{
			string value2 = null;
			Serialize(name, ref value2);
			value = ulong.Parse(value2, CultureInfo.InvariantCulture);
		}

		public override void Serialize(string name, ref float value)
		{
			string value2 = null;
			Serialize(name, ref value2);
			value = float.Parse(value2, CultureInfo.InvariantCulture);
		}

		public override void Serialize(string name, ref double value)
		{
			string value2 = null;
			Serialize(name, ref value2);
			value = double.Parse(value2, CultureInfo.InvariantCulture);
		}

		public override void Serialize(string name, ref bool value)
		{
			string value2 = null;
			Serialize(name, ref value2);
			if (string.Equals(value2, "False", StringComparison.OrdinalIgnoreCase))
			{
				value = false;
				return;
			}
			if (string.Equals(value2, "True", StringComparison.OrdinalIgnoreCase))
			{
				value = true;
				return;
			}
			throw new InvalidOperationException($"Cannot convert string \"{value2}\" to a Boolean.");
		}

		public override void Serialize(string name, ref char value)
		{
			string value2 = null;
			Serialize(name, ref value2);
			if (value2.Length == 1)
			{
				value = value2[0];
				return;
			}
			throw new InvalidOperationException($"Cannot convert string \"{value2}\" to a Char.");
		}

		public override void Serialize(string name, ref string value)
		{
			if (name != null)
			{
				XAttribute xAttribute = Node.Attribute(name);
				if (xAttribute == null)
				{
					throw new InvalidOperationException($"Required XML node \"{name}\" not found.");
				}
				value = xAttribute.Value;
			}
			else
			{
				value = Node.Value;
			}
		}

		public override void Serialize(string name, ref byte[] value)
		{
			string value2 = null;
			Serialize(name, ref value2);
			value = Convert.FromBase64String(value2);
		}

		public override void Serialize(string name, int length, ref byte[] value)
		{
			string value2 = null;
			Serialize(name, ref value2);
			value = Convert.FromBase64String(value2);
			if (value.Length != length)
			{
				throw new InvalidOperationException("Invalid fixed array length.");
			}
		}

		public override void Serialize(string name, Type type, ref object value)
		{
			ReadObject(name, Archive.GetSerializeData(type, allowEmptySerializer: true), ref value);
		}

		public override void SerializeCollection<T>(string name, ICollection<T> collection)
		{
			EnterNode(name);
			SerializeData serializeData = Archive.GetSerializeData(typeof(T), allowEmptySerializer: true);
			using (IEnumerator<XElement> enumerator = Node.Elements().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					XElement xElement = Node = enumerator.Current;
					object value = null;
					ReadObject(null, serializeData, ref value);
					collection.Add((T)value);
					Node = Node.Parent;
				}
			}
			LeaveNode(name);
		}

		public override void SerializeDictionary<K, V>(string name, IDictionary<K, V> dictionary)
		{
			EnterNode(name);
			SerializeData serializeData = Archive.GetSerializeData(typeof(K), allowEmptySerializer: true);
			SerializeData serializeData2 = Archive.GetSerializeData(typeof(V), allowEmptySerializer: true);
			if (typeof(K) == typeof(string))
			{
				using (IEnumerator<XElement> enumerator = Node.Elements().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						XElement xElement = Node = enumerator.Current;
						object localName = xElement.Name.LocalName;
						object value = null;
						if (dictionary.TryGetValue((K)localName, out V value2))
						{
							value = value2;
							ReadObject(null, serializeData2, ref value);
						}
						else
						{
							ReadObject(null, serializeData2, ref value);
							dictionary.Add((K)localName, (V)value);
						}
						Node = Node.Parent;
					}
				}
			}
			else
			{
				using (IEnumerator<XElement> enumerator = Node.Elements().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						XElement xElement2 = Node = enumerator.Current;
						object value3 = null;
						object value4 = null;
						ReadObject("k", serializeData, ref value3);
						if (dictionary.TryGetValue((K)value3, out V value5))
						{
							value4 = value5;
						}
						ReadObject("v", serializeData2, ref value4);
						dictionary.Add((K)value3, (V)value4);
						Node = Node.Parent;
					}
				}
			}
			LeaveNode(name);
		}

		public override void ReadObjectInfo(out int objectId, out bool isReference, out Type runtimeType)
		{
			XAttribute xAttribute = Node.Attribute("_ref");
			if (xAttribute != null)
			{
				runtimeType = null;
				isReference = true;
				objectId = int.Parse(xAttribute.Value);
				return;
			}
			XAttribute xAttribute2 = Node.Attribute("_def");
			if (xAttribute2 != null)
			{
				XAttribute xAttribute3 = Node.Attribute("_type");
				if (xAttribute3 != null)
				{
					runtimeType = TypeCache.FindType(xAttribute3.Value, skipSystemAssemblies: false, throwIfNotFound: true);
				}
				else
				{
					runtimeType = null;
				}
				isReference = false;
				objectId = int.Parse(xAttribute2.Value);
				return;
			}
			throw new InvalidOperationException("Required XML _ref/_def attribute not found.");
		}

		public void ReadObject(string name, SerializeData staticSerializeData, ref object value)
		{
			if (staticSerializeData.IsHumanReadableSupported)
			{
				string value2 = null;
				Serialize(name, ref value2);
				value = HumanReadableConverter.ConvertFromString(staticSerializeData.Type, value2);
			}
			else
			{
				EnterNode(name);
				base.ReadObject(staticSerializeData, ref value);
				LeaveNode(name);
			}
		}

		public void EnterNode(string name)
		{
			if (name != null)
			{
				XElement xElement = Node.Element(name);
				if (xElement == null)
				{
					throw new InvalidOperationException($"XML element \"{name}\" not found.");
				}
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
