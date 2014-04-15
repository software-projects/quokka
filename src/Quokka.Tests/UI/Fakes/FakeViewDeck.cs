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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Quokka.Diagnostics;
using Quokka.UI.Tasks;

namespace Quokka.UI.Fakes
{
	public interface IFakeViewThatLoads
	{
		void OnLoad();
	}

	public class FakeViewDeck : IViewDeck
	{
		private readonly HashSet<IUITask> _tasks = new HashSet<IUITask>();
		private readonly HashSet<object> _views = new HashSet<object>();
		private object _visibleView;
		private int _transitionReferenceCount;

		public object VisibleView
		{
			get { return _visibleView; }
		}

		#region IViewDeck Members

		public event EventHandler<ViewClosedEventArgs> ViewClosed;

		public void BeginTask(object task)
		{
			Assert.IsNotNull(task);
			Assert.IsInstanceOf<IUITask>(task);
			var uitask = (IUITask) task;
			_tasks.Add(uitask);
		}

		public void EndTask(object task)
		{
			Assert.IsNotNull(task);
			Assert.IsNotNull(task);
			Assert.IsInstanceOf<IUITask>(task);
			var uitask = (IUITask)task;
			Assert.IsTrue(_tasks.Contains(uitask));
			_tasks.Remove(uitask);
		}

		public IViewTransition BeginTransition(IUITask task)
		{
			if (Interlocked.Increment(ref _transitionReferenceCount) <= 0)
			{
				Assert.Fail("transition reference count = " + _transitionReferenceCount);
			}
			return new ViewTransition(this, task);
		}

		public void EndTransition()
		{
			if (Interlocked.Decrement(ref _transitionReferenceCount) < 0)
			{
				Assert.Fail("transition reference count = " + _transitionReferenceCount);
			}
		}

		public void AddView(object view)
		{
			Assert.IsFalse(_views.Contains(view));
			_views.Add(view);
		}

		public void RemoveView(object view)
		{
			Assert.IsTrue(_views.Contains(view));
			_views.Remove(view);
		}

		public void ShowView(object view)
		{
			Assert.IsTrue(_transitionReferenceCount > 0);
			Assert.IsNotNull(view);
			Assert.IsTrue(_views.Contains(view), "View has not been added to the view deck");
			_visibleView = view;
            var fakeView = view as IFakeViewThatLoads;
            if (fakeView != null)
            {
                // simulate on load event
                fakeView.OnLoad();
            }
		}

		public void HideView(object view)
		{
			Assert.IsNotNull(view);
			if (view == _visibleView)
			{
				_visibleView = null;
			}
		}

		public void ShowModalView(object view)
		{
			throw new NotImplementedException();
		}

		public IModalWindow CreateModalWindow()
		{
			throw new NotImplementedException();
		}

		#endregion

		protected void OnViewClosed(object view)
		{
			if (ViewClosed != null)
			{
				ViewClosed(this, new ViewClosedEventArgs(view));
			}
		}

		private class ViewTransition : IViewTransition
		{
			private readonly FakeViewDeck _viewDeck;
			private readonly IUITask _task;
			private bool _disposed;

			public ViewTransition(FakeViewDeck viewDeck, IUITask task)
			{
				_viewDeck = Verify.ArgumentNotNull(viewDeck, "viewDeck");
				_task = Verify.ArgumentNotNull(task, "task");
				_viewDeck.BeginTask(task);
			}

			public void Dispose()
			{
				if (!_disposed)
				{
					_disposed = true;
					_viewDeck.EndTransition();
				}
			}

			public void EndTask()
			{
				_viewDeck.EndTask(_task);
			}

			public void AddView(object view)
			{
				_viewDeck.AddView(view);
			}

			public void RemoveView(object view)
			{
				_viewDeck.RemoveView(view);
			}

			public void ShowView(object view)
			{
				_viewDeck.ShowView(view);
			}

			public void HideView(object view)
			{
				_viewDeck.HideView(view);
			}
		}
	}
}
