using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SilverConfig;

// https://weblogs.asp.net/pwelter34/444961
/// <summary>
/// A serializable dictionary for XML object
/// </summary>
/// <typeparam name="TKey">The key type of the dictionary</typeparam>
/// <typeparam name="TValue">The value type of the dictionary</typeparam>
[XmlRoot("dictionary")]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable where TKey : notnull
{
    #region IXmlSerializable Members

    public XmlSchema GetSchema()
    {
        return null!;
    }

    public void ReadXml(XmlReader reader)
    {
        if (reader is null)
        {
            throw new ArgumentNullException(nameof(reader));
        }
        XmlSerializer keySerializer = new(typeof(TKey));
        XmlSerializer valueSerializer = new(typeof(TValue));
        var wasEmpty = reader.IsEmptyElement;
        reader.Read();
        if (wasEmpty)
        {
            return;
        }

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            reader.ReadStartElement("item");
            reader.ReadStartElement("key");
            var key = (TKey)keySerializer.Deserialize(reader)!;
            reader.ReadEndElement();
            reader.ReadStartElement("value");
            var value = (TValue)valueSerializer.Deserialize(reader)!;
            reader.ReadEndElement();
            Add(key, value);
            reader.ReadEndElement();
            reader.MoveToContent();
        }

        reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
        if (writer is null)
        {
            throw new ArgumentNullException(nameof(writer));
        }
        XmlSerializer keySerializer = new(typeof(TKey));
        XmlSerializer valueSerializer = new(typeof(TValue));
        foreach (var key in Keys)
        {
            writer.WriteStartElement("item");
            writer.WriteStartElement("key");
            keySerializer.Serialize(writer, key);
            writer.WriteEndElement();
            writer.WriteStartElement("value");
            var value = this[key];
            valueSerializer.Serialize(writer, value);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }

    #endregion IXmlSerializable Members

    public override bool Equals(object? obj)
    {
        if (object.ReferenceEquals(this, obj))
            return true;
        if (obj is null)
        {
            return false;
        }
        if (obj.GetType() != this.GetType())
        {
            return false;
        }
        if (obj is not SerializableDictionary<TKey, TValue> a)
        {
            return false;
        }
        foreach (var key in Keys)
        {
            if (a.ContainsKey(key))
            {
                if (a[key]?.Equals(this[key]) == false)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}