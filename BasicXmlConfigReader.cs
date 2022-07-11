using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SilverConfig
{
    public class BasicXmlConfigReader<T> : IConfigReader<T>
    {
        private readonly XmlSerializer serializer;

        public BasicXmlConfigReader()
        {
            serializer = new XmlSerializer(typeof(T));
        }

        public virtual T? Read(string path)
        {
            using var streamWriter = new StreamReader(path);
            using var xmlReader = XmlReader.Create(streamWriter);
            return (T?)serializer.Deserialize(xmlReader);
        }

        public virtual bool SupportsComments()
        {
            return false;
        }

        public virtual void Write(T config, string path)
        {
            using var streamWriter = new StreamWriter(path, false);
            serializer.Serialize(streamWriter, config);
        }
    }
}