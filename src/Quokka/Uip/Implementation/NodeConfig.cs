using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Quokka.Uip.Implementation
{
    [XmlType(AnonymousType = true)]
    public class NodeConfig
    {
        private ViewConfig _view;
        private ObjectTypeConfig _controller;
        private List<NavigateToConfig> _navigateTos;
        private string _name;

        [XmlElement("View")]
        public ViewConfig View {
            get { return this._view; }
            set { this._view = value; }
        }

        [XmlElement("Controller")]
        public ObjectTypeConfig Controller {
            get { return this._controller; }
            set { this._controller = value; }
        }

        [XmlElement("NavigateTo")]
        public List<NavigateToConfig> NavigateTos {
            get { return this._navigateTos; }
            set { this._navigateTos = value; }
        }

        [XmlAttribute("Name")]
        public string Name {
            get { return this._name; }
            set { this._name = value; }
        }
    }
}