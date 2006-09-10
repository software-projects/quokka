using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Quokka.Uip.Implementation
{
    [XmlType(AnonymousType = true)]
    public class PropertyConfig
    {
        private string _name;
        private string _value;

        [XmlAttribute]
        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        [XmlAttribute]
        public string Value {
            get { return _value; }
            set { _value = value; }
        }
    }
}
