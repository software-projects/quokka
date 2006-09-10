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
