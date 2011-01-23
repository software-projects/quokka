using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Quokka.UI.Tasks;

namespace Quokka.UI.Fakes
{
	public class FakeViewDeck : IViewDeck
	{
		private readonly HashSet<UITask> _tasks = new HashSet<UITask>();
		private readonly HashSet<object> _views = new HashSet<object>();
		private bool _inTransition;
		private object _visibleView;

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

		public void BeginTransition()
		{
			Assert.IsFalse(_inTransition);
			_inTransition = true;
		}

		public void EndTransition()
		{
			Assert.IsTrue(_inTransition);
			_inTransition = false;
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
			Assert.IsTrue(_inTransition);
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
			//Assert.IsTrue(_inTransition);
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

		#endregion

		protected void OnViewClosed(object view)
		{
			if (ViewClosed != null)
			{
				ViewClosed(this, new ViewClosedEventArgs(view));
			}
		}
	}
}
