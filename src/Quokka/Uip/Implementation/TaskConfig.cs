#region Copyright notice
//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

namespace Quokka.Uip.Implementation
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;
	using System.Xml.Serialization;

	[XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "UipTask", Namespace = "http://www.quokka.org/schemas/2006/UipTask.xsd", IsNullable = false)]
    public class TaskConfig
    {
        private ObjectTypeConfig _state;
        private string _name;
        private ObjectTypeConfig _statePersist;
        private ObjectTypeConfig _viewManager;
        private NavigationGraphConfig _navigationGraph;
        private List<UsingNamespaceConfig> _usingNamespaces;
        private static XmlSchema taskXmlSchema;
        private static XmlReaderSettings taskXmlReaderSettings;

        static TaskConfig() {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(TaskConfig), "UipTask.xsd")) {
                XmlSerializer serializer = new XmlSerializer(typeof(XmlSchema));
                taskXmlSchema = (XmlSchema)serializer.Deserialize(stream);
            }

            taskXmlReaderSettings = new XmlReaderSettings();
            taskXmlReaderSettings.ValidationType = ValidationType.Schema;
            taskXmlReaderSettings.Schemas = new XmlSchemaSet();
            taskXmlReaderSettings.Schemas.Add(taskXmlSchema);
        }

        public static TaskConfig Create(Stream stream) {
            if (stream == null) {
                throw new ArgumentNullException("stream");
            }
            XmlReader reader = XmlReader.Create(stream, taskXmlReaderSettings);
            XmlSerializer serializer = new XmlSerializer(typeof(TaskConfig));
            return (TaskConfig)serializer.Deserialize(reader);
        }

        [XmlElement("Using")]
        public List<UsingNamespaceConfig> UsingNamespaces {
            get { return this._usingNamespaces; }
            set { this._usingNamespaces = value; }
        }

        [XmlElement("State")]
        public ObjectTypeConfig State {
            get { return this._state; }
            set { this._state = value; }
        }

        [XmlElement("StatePersist")]
        public ObjectTypeConfig StatePersist {
            get { return this._statePersist; }
            set { this._statePersist = value; }
        }

        [XmlElement("ViewManager")]
        public ObjectTypeConfig ViewManager {
            get { return this._viewManager; }
            set { this._viewManager = value; }
        }

        [XmlElement("NavigationGraph")]
        public NavigationGraphConfig NavigationGraph {
            get {
                return this._navigationGraph;
            }
            set {
                this._navigationGraph = value;
            }
        }

        [XmlAttribute("Name")]
        public string Name {
            get { return this._name; }
            set { this._name = value; }
        }
    }
}
