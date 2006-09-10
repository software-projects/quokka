using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Quokka.Uip.Implementation
{
    /// <summary>
    ///     Element that describes a class
    /// </summary>
    [XmlType(AnonymousType = true)]
    public class ObjectTypeConfig
    {
        private string _typeName;

        [XmlAttribute("Type")]
        public string TypeName {
            get { return this._typeName; }
            set { this._typeName = value; }
        }

        [XmlElement("Property")]
        public List<PropertyConfig> Properties;
    }
}
