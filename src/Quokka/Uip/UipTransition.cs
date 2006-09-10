using System;
using System.Collections.Generic;
using System.Text;

namespace Quokka.Uip
{
    public class UipTransition
    {
        private readonly UipNode node;
        private readonly string navigateValue;
        private readonly UipNode nextNode;

        internal UipTransition(UipNode node, string navigateValue, UipNode nextNode) {
            if (node == null)
                throw new ArgumentNullException("node");
            if (navigateValue == null)
                throw new ArgumentNullException("navigateValue");
            if (nextNode == null)
                throw new ArgumentNullException("nextNode");
            this.node = node;
            this.navigateValue = navigateValue;
            this.nextNode = nextNode;
        }

        public UipNode Node {
            get { return node; }
        }

        public string NavigateValue {
            get { return navigateValue; }
        }

        public UipNode NextNode {
            get { return nextNode; }
        }
    }
}
