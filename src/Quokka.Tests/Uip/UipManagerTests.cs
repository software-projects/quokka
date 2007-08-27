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
using System.Reflection;
using NUnit.Framework;

using Quokka.Uip.MockApp;

namespace Quokka.Uip
{
    [TestFixture]
    public class UipManagerTests
    {
        [SetUp]
        public void Setup() {
            UipManager.Clear();
        }

        [TearDown]
        public void Teardown() {
            UipManager.Clear();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadTaskDefinition_ArgumentNull_1a() {
            UipManager.DefineTask(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadTaskDefinition_ArgumentNull_1b() {
            UipManager.DefineTask(null, "task.xml");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadTaskDefinition_ArgumentNull_2() {
            UipManager.DefineTask(GetType(), null);
        }

        [Test]
        [ExpectedException(typeof(UipException))]
        public void LoadTaskDefinition_NotFound() {
            UipManager.DefineTask(GetType(), "UndefinedTask.xml");
        }

        [Test]
        public void LoadTaskDefinitions() {
            UipManager.AddAssembly(Assembly.GetExecutingAssembly());
            UipManager.DefineTasks(typeof(MockState), "MockTask.xml", "MockTask2.xml");
            IUipViewManager viewManager = new MockViewManager();

            UipTask task1 = UipManager.CreateTask("MockTask", viewManager);
            Assert.IsNotNull(task1);
            UipTask task2 = UipManager.CreateTask("MockTask2", viewManager);
            Assert.IsNotNull(task2);
        }
    }
}
