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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using Quokka.Uip.Implementation;

namespace Quokka.Uip
{
    [TestFixture]
    public class TaskConfigTests
    {
        [Test]
        public void XmlSerialization_Success() {
            string[] taskNames = {
                "UipTask_1.xml",
                "UipTask_2.xml",
                "UipTask_3.xml",
                "UipTask_4.xml",
                "UipTask_5.xml",
                "UipTask_6.xml",
            };

            foreach (string taskName in taskNames) {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(this.GetType(), taskName)) {
                    TaskConfig taskConfig = TaskConfig.Create(stream);

                    Assert.Less(1, taskConfig.NavigationGraph.Nodes.Count);
                }
            }
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void XmlSerialization_Failure() {
        }
    }
}
