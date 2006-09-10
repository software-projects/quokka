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
