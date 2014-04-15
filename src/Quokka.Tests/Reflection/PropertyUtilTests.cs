#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Quokka.Reflection
{
    [TestFixture]
    public class PropertyUtilTests
    {
        private TestClass testObject;

        [SetUp]
        public void SetUp() {
            testObject = new TestClass();
        }

        [TearDown]
        public void TearDown() {
            testObject = null;
        }

        [Test]
        public void GetStringValue_ArgumentNull_1() {
            Assert.Throws<ArgumentNullException>(() => PropertyUtil.GetStringValue(null, "XXX"));
        }

        [Test]
        public void GetStringValue_ArgumentNull_2() {
            Assert.Throws<ArgumentNullException>(() => PropertyUtil.GetStringValue(testObject, null));
        }

        [Test]
        public void GetStringValue() {
            testObject.StringProperty = "XXX";
            string stringValue = PropertyUtil.GetStringValue(testObject, "StringProperty");
            Assert.AreEqual("XXX", stringValue);
        }

        [Test]
        public void GetValue_ArgumentNull_1() {
            Assert.Throws<ArgumentNullException>(() => PropertyUtil.GetValue(null, "XXX"));
        }

        [Test]
        public void GetValue_ArgumentNull_2() {
            Assert.Throws<ArgumentNullException>(() => PropertyUtil.GetValue(testObject, null));
        }

        [Test]
        public void GetValue_ArgumentNull_3() {
            Assert.Throws<ArgumentNullException>(() => PropertyUtil.GetValue(null, "XXX", "XYZ"));
        }

        [Test]
        public void GetValue_ArgumentNull_4() {
            Assert.Throws<ArgumentNullException>(() => PropertyUtil.GetValue(testObject, null, "XYZ"));
        }

        [Test]
        public void GetValue_String() {
            testObject.StringProperty = "XXX";
            object objectValue = PropertyUtil.GetValue(testObject, "StringProperty");
            Assert.IsNotNull(objectValue);
            Assert.IsTrue(objectValue is string);
            Assert.AreEqual("XXX", objectValue.ToString());
        }

        [Test]
        public void GetValue_Int() {
            testObject.IntProperty = 42;
            object objectValue = PropertyUtil.GetValue(testObject, "IntProperty");
            Assert.IsNotNull(objectValue);
            Assert.IsTrue(objectValue is int);
            Assert.AreEqual(42, (int)objectValue);
        }

        [Test]
        public void GetValue_Double() {
            testObject.DoubleProperty = 42.5;
            object objectValue = PropertyUtil.GetValue(testObject, "DoubleProperty");
            Assert.IsNotNull(objectValue);
            Assert.IsTrue(objectValue is double);
            Assert.AreEqual(42.5, (double)objectValue);
        }

        [Test]
        public void TrySetValue_ArgumentNull_1() {
            Assert.Throws<ArgumentNullException>(() => PropertyUtil.TrySetValue(null, "XXX", "XXX"));
        }

        [Test]
        public void TrySetValue_ArgumentNull_2() {
            Assert.Throws<ArgumentNullException>(() => PropertyUtil.TrySetValue(testObject, null, "XXX"));
        }

        [Test]
        public void TrySetValue_String() {
            Assert.IsTrue(PropertyUtil.TrySetValue(testObject, "StringProperty", "XYZ"));
            Assert.AreEqual("XYZ", testObject.StringProperty);

            Assert.IsTrue(PropertyUtil.TrySetValue(testObject, "StringProperty", null));
            Assert.IsNull(testObject.StringProperty);
        }

        [Test]
        public void TrySetValue_Int() {
            Assert.IsTrue(PropertyUtil.TrySetValue(testObject, "IntProperty", 42));
            Assert.AreEqual(42, testObject.IntProperty);

            Assert.IsFalse(PropertyUtil.TrySetValue(testObject, "IntProperty", null));
            Assert.AreEqual(42, testObject.IntProperty); // Has not changed

            Assert.IsTrue(PropertyUtil.TrySetValue(testObject, "IntProperty", "9999"));
            Assert.AreEqual(9999, testObject.IntProperty);

            Assert.IsFalse(PropertyUtil.TrySetValue(testObject, "IntProperty", "XXXX"));
            Assert.AreEqual(9999, testObject.IntProperty);
        }

        [Test]
        public void TrySetValue_Double() {
            Assert.IsTrue(PropertyUtil.TrySetValue(testObject, "DoubleProperty", 42.5));
            Assert.AreEqual(42.5, testObject.DoubleProperty);

            Assert.IsFalse(PropertyUtil.TrySetValue(testObject, "DoubleProperty", null));
            Assert.AreEqual(42.5, testObject.DoubleProperty); // Has not changed

            Assert.IsTrue(PropertyUtil.TrySetValue(testObject, "DoubleProperty", "9999.5"));
            Assert.AreEqual(9999.5, testObject.DoubleProperty);

            Assert.IsFalse(PropertyUtil.TrySetValue(testObject, "DoubleProperty", "XXXX"));
            Assert.AreEqual(9999.5, testObject.DoubleProperty);
        }

        [Test]
        public void SetValues() {
            PropertyCollection properties = new PropertyCollection();
            properties.Add("DoubleProperty", "9999.5");
            properties.Add("IntProperty", "123456");
            properties.Add("StringProperty", "AAAA");

            PropertyUtil.SetValues(testObject, properties);
            Assert.AreEqual(9999.5, testObject.DoubleProperty);
            Assert.AreEqual(123456, testObject.IntProperty);
            Assert.AreEqual("AAAA", testObject.StringProperty);
        }

        [Test]
        public void SetValues_ArgumentNull_1() {
            PropertyCollection properties = new PropertyCollection();
            Assert.Throws<ArgumentNullException>(() => PropertyUtil.SetValues(null, properties));

        }

        [Test]
        public void SetValues_ArgumentNull_2() {
            // can pass a null value for second param
            PropertyUtil.SetValues(testObject, null);
        }

        [Test]
        public void CopyProperties_ArgumentNull_1() {
            Assert.Throws<ArgumentNullException>(() => PropertyUtil.CopyProperties(null, testObject));
        }

        [Test]
        public void CopyProperties_ArgumentNull_2() {
            Assert.Throws<ArgumentNullException>(() => PropertyUtil.CopyProperties(testObject, null));
        }

        [Test]
        public void CopyProperties() {
            testObject.StringProperty = "XXXX";
            testObject.IntProperty = 42;
            testObject.DoubleProperty = 9999.5;

            TestClass other = new TestClass();

            PropertyUtil.CopyProperties(testObject, other);

            Assert.AreEqual("XXXX", testObject.StringProperty);
            Assert.AreEqual(42, testObject.IntProperty);
            Assert.AreEqual(9999.5, testObject.DoubleProperty);
        }

        public class TestClass
        {
            private int intField;

            public int IntProperty {
                get { return intField; }
                set { intField = value; }
            }

            private string stringField;

            public string StringProperty {
                get { return stringField; }
                set { stringField = value; }
            }

            public double doubleField;

            public double DoubleProperty {
                get { return doubleField; }
                set { doubleField = value; }
            }
        }
    }
}
