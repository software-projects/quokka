using System;
using System.Collections.Generic;
using System.Text;

namespace Quokka.Uip.MockApp
{
    public class MockState
    {
        private string stringField;

        public string StringProperty {
            get { return stringField; }
            set { stringField = value; }
        }
    }
}
