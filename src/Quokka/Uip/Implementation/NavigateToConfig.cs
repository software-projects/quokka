using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Quokka.Uip.Implementation
{
    [XmlType(AnonymousType = true)]
    public class NavigateToConfig
    {
        private string _navigateValue;
        private string _nodeName;

        [XmlAttribute("NavigateValue")]
        public string NavigateValue {
            get { return this._navigateValue; }
            set { this._navigateValue = value; }
        }

        [XmlAttribute("Node")]
        public string NodeName {
            get { return this._nodeName; }
            set { this._nodeName = value; }
        }
    }
}