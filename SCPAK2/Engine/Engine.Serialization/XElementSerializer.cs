using System.Linq;
using System.Xml.Linq;

namespace Engine.Serialization
{
	internal class XElementSerializer : ISerializer<XElement>
	{
		public void Serialize(InputArchive archive, ref XElement value)
		{
			XmlInputArchive xmlInputArchive = archive as XmlInputArchive;
			if (xmlInputArchive != null)
			{
				value = xmlInputArchive.Node.Elements().First();
				return;
			}
			string value2 = null;
			archive.Serialize(null, ref value2);
			value = XElement.Parse(value2);
		}

		public void Serialize(OutputArchive archive, XElement value)
		{
			XmlOutputArchive xmlOutputArchive = archive as XmlOutputArchive;
			if (xmlOutputArchive != null)
			{
				xmlOutputArchive.Node.Add(value);
			}
			else
			{
				archive.Serialize(null, value.ToString());
			}
		}
	}
}
