using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Quokka.Uip.Implementation
{
    /// <summary>
    /// Element that describes a View
    /// </summary>
    [XmlType(AnonymousType = true)]
    public class ViewConfig : ObjectTypeConfig
    {
        private bool _stayOpen;
        private bool _openModal;

        [XmlAttribute("StayOpen")]
        public bool StayOpen {
            get { return _stayOpen; }
            set { _stayOpen = value; }
        }

        [XmlAttribute("OpenModal")]
        public bool OpenModal {
            get { return _openModal; }
            set { _openModal = value; }
        }
    }
}

