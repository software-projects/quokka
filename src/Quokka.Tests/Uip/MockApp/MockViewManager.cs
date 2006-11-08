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
using System.Text;

using NUnit.Framework;

namespace Quokka.Uip.MockApp
{
    public class MockViewManager : IUipViewManager
    {
        bool inTransition;
        object visibleView;
        UipTask currentTask;

        public object VisibleView {
            get { return visibleView; }
        }

        #region IUipViewManager Members

        public event EventHandler<UipViewEventArgs> ViewClosed;

        public void BeginTask(UipTask task) {
            Assert.IsNull(currentTask);
            Assert.IsNotNull(task);
            currentTask = task;
        }

        public void EndTask(UipTask task) {
            Assert.AreSame(currentTask, task);
            currentTask = null;
        }

        public void BeginTransition() {
            Assert.IsNotNull(currentTask);
            Assert.IsFalse(inTransition);
            inTransition = true;
        }

        public void EndTransition() {
            Assert.IsNotNull(currentTask);
            Assert.IsTrue(inTransition);
            inTransition = false;
        }

        public void AddView(object view, object controller) { }
        public void RemoveView(object view) { }

        public void ShowView(object view) {
            Assert.IsNotNull(currentTask);
            Assert.IsTrue(inTransition);
            Assert.IsNull(visibleView);
            Assert.IsNotNull(view);
            visibleView = view;
        }

        public void HideView(object view) {
            Assert.IsNotNull(currentTask);
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
