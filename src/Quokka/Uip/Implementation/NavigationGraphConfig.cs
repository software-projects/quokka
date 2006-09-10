using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Quokka.Uip.Implementation
{
    [XmlType(AnonymousType = true)]
    public class NavigationGraphConfig
    {
        private List<NodeConfig> _nodes;
        private string _startNodeName;

        [XmlElement("Node")]
        public List<NodeConfig> Nodes {
            get { return this._nodes; }
            set { this._nodes = value; }
        }

        [XmlAttribute("StartNode")]
        public string StartNodeName {
            get {
                return this._startNodeName;
            }
            set {
                this._startNodeName = value;
            }
        }

        public NodeConfig GetNode(string nodeName) {
            foreach (NodeConfig node in _nodes) {
                if (node.Name == nodeName) {
                    return node;
                }
            }

            string message = String.Format("Missing node: {0}", nodeName);
            throw new UipException(message);
        }
    }
}