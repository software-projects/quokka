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

namespace Quokka.Uip.Fakes
{
	using System;
	using System.Collections.Generic;
	using NUnit.Framework;

	public class FakeViewManager : IUipViewManager
    {
		private readonly List<UipTask> _tasks = new List<UipTask>();
        private bool inTransition;
        private object visibleView;

        public object VisibleView {
            get { return visibleView; }
        }

        #region IUipViewManager Members

        public event EventHandler<UipViewEventArgs> ViewClosed;

		public void BeginTask(UipTask task)
		{
			Assert.IsNotNull(task);
			Assert.IsFalse(_tasks.Contains(task));
			_tasks.Add(task);
		}

		public void EndTask(UipTask task)
		{
			Assert.IsNotNull(task);
			Assert.IsTrue(_tasks.Contains(task));
			_tasks.Remove(task);
		}

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
            //Assert.IsNull(visibleView);
            Assert.IsNotNull(view);
            visibleView = view;
//            MockViewBase mockView = view as MockViewBase;
//            if (mockView != null)
//            {
//                // simulate on load event
//                mockView.OnLoad();
//            }
        }

        public void HideView(object view) {
            Assert.IsTrue(inTransition);
            //Assert.IsNotNull(visibleView);
			if (view == visibleView) {
				visibleView = null;
			}
            //Assert.AreSame(visibleView, view);
            //visibleView = null;
        }

		public void ShowModalView(object view, object controller)
		{
			throw new NotImplementedException();
		}

		public UipAnswer AskQuestion(UipQuestion question)
		{
			throw new NotImplementedException();
		}

		#endregion

        protected void OnViewClosed(object view) {
            if (ViewClosed != null) {
                ViewClosed(this, new UipViewEventArgs(view));
            }
        }
    }
}
