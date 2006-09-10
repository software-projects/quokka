using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Quokka.Uip.Implementation
{
    /// <summary>
    /// Element that describes a namespace that is searched by default
    /// </summary>
    [XmlType(AnonymousType = true)]
    public class UsingNamespaceConfig
    {
        private string _namespace;

        [XmlAttribute]
        public string Namespace {
            get { return _namespace; }
            set { _namespace = value; }
        }
    }
}
