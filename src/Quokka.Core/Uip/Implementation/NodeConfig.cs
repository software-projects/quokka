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
	using System.Collections.Generic;
	using System.Xml.Serialization;

	[XmlType(AnonymousType = true)]
    public class NodeConfig
    {
        private ViewConfig _view;
        private ObjectTypeConfig _controller;
        private List<NavigateToConfig> _navigateTos;
        private string _name;

        [XmlElement("View")]
        public ViewConfig View {
            get { return _view; }
            set { _view = value; }
        }

        [XmlElement("Controller")]
        public ObjectTypeConfig Controller {
            get { return _controller; }
            set { _controller = value; }
        }

        [XmlElement("NavigateTo")]
        public List<NavigateToConfig> NavigateTos {
            get { return _navigateTos; }
            set { _navigateTos = value; }
        }

        [XmlAttribute("Name")]
        public string Name {
            get { return _name; }
            set { _name = value; }
        }
    }
}