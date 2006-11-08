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

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;

namespace Quokka.Reflection
{
    public class PropertyCollection : NameObjectCollectionBase 
    {
        public PropertyCollection() { }

        internal PropertyCollection(IEnumerable<Quokka.Uip.Implementation.PropertyConfig> propertyConfigs) {
            if (propertyConfigs != null) {
                foreach (Quokka.Uip.Implementation.PropertyConfig propertyConfig in propertyConfigs) {
                    Add(propertyConfig.Name, propertyConfig.Value);
                }
            }
        }

        public string this[string name] {
            get { return (string)BaseGet(name); }
            set { BaseSet(name, value); }
        }

        public string this[int index] {
            get { return (string)BaseGet(index); }
            set { BaseSet(index, value); }
        }

        public void Add(string name, string value) {
            BaseAdd(name, value);
        }

        public void Clear() {
            BaseClear();
        }

        public string[] GetAllKeys() {
            return BaseGetAllKeys();
        }

        public void Remove(string name) {
            BaseRemove(name);
        }

        public void RemoveAt(int index) {
            BaseRemoveAt(index);
        }
    }
}
