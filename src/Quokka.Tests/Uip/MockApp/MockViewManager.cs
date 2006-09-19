using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Quokka.Uip.MockApp
{
    public class MockViewManager : IUipViewManager
    {
        bool inTransition;
        object visibleView;

        public object VisibleView {
            get { return visibleView; }
        }

        #region IUipViewManager Members

        public event EventHandler<UipViewEventArgs> ViewClosed;

        public void BeginTransition() {
            Assert.IsFalse(inTransition);
            inTransition = true;
        }

        public void EndTransition() {
            Assert.IsTrue(inTransition);
            inTransition = false;
        }

        public void AddView(object view, object controller) { }
        public void RemoveView(object view) { }

        public void ShowView(object view) {
            Assert.IsTrue(inTransition);
            Assert.IsNull(visibleView);
            Assert.IsNotNull(view);
            visibleView = view;
        }

        public void HideView(object view) {
            Assert.IsTrue(inTransition);
            Assert.IsNotNull(visibleView);
            Assert.AreSame(visibleView, view);
            visibleView = null;
        }

        #endregion

        protected void OnViewClosed(object view) {
            if (ViewClosed != null) {
                ViewClosed(this, new UipViewEventArgs(view));
            }
        }
    }
}
