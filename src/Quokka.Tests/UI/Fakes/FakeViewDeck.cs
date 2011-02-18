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
	public class FakeViewDeck : IViewDeck
	{
		private readonly HashSet<UITask> _tasks = new HashSet<UITask>();
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
			Assert.IsInstanceOfType(typeof(UITask), task);
			var uitask = (UITask) task;
			Assert.IsFalse(_tasks.Contains(uitask));
			_tasks.Add(uitask);
		}

		public void EndTask(object task)
		{
			Assert.IsNotNull(task);
			Assert.IsNotNull(task);
			Assert.IsInstanceOfType(typeof(UITask), task);
			var uitask = (UITask)task;
			Assert.IsTrue(_tasks.Contains(uitask));
			_tasks.Remove(uitask);
		}

		public IViewTransition BeginTransition()
		{
			if (Interlocked.Increment(ref _transitionReferenceCount) <= 0)
			{
				Assert.Fail("transition reference count = " + _transitionReferenceCount);
			}
			return new ViewTransition(this);
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
			//            MockViewBase mockView = view as MockViewBase;
			//            if (mockView != null)
			//            {
			//                // simulate on load event
			//                mockView.OnLoad();
			//            }
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
			private bool _disposed;

			public ViewTransition(FakeViewDeck viewDeck)
			{
				_viewDeck = Verify.ArgumentNotNull(viewDeck, "viewDeck");
			}

			public void Dispose()
			{
				if (!_disposed)
				{
					_disposed = true;
					_viewDeck.EndTransition();
				}
			}

			public void BeginTask(object task)
			{
				_viewDeck.BeginTask(task);
			}

			public void EndTask(object task)
			{
				_viewDeck.EndTask(task);
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
