using System;
using Castle.Core;
using NHibernate;
using Quokka.NH.Interfaces;
using Quokka.UI.Tasks;

namespace Quokka.NH.Implementations
{
	/// <summary>
	/// Implementation if <see cref="ISessionManager"/> that is aware of the associated <see cref="UITask"/>
	/// and <see cref="UINode"/>, if relevant.
	/// </summary>
	/// <remarks>
	/// <para>
	/// If the presenter or view is decorated with a <see cref="NHSessionAttribute"/>, then this
	/// session manager will know to keep the same <see cref="ISession"/> for the lifetime of the
	/// presenter and view.
	/// </para>
	/// <para>
	/// If the <see cref="UITask"/> is decorated with a <see cref="NHSessionAttribute"/>, then this
	/// session manager will know to keep the same <see cref="ISession"/> for the lifetime of the
	/// <see cref="UITask"/>.
	/// </para>
	/// </remarks>
	[Transient]
	public class TaskAwareSessionManager : SessionManager
	{
		private UITask _currentTaskAtCreation;

		protected override ISession FindCompatibleSession(string alias, ISessionFactory sessionFactory)
		{
			ISession session = null;
			var slotId = SlotId(alias);

			var task = GetCurrentTask();
			if (task != null)
			{
				var taskSessionHolder = GetSessionHolder(task, slotId);
				if (taskSessionHolder.ShouldHaveSession)
				{
					if (taskSessionHolder.Session == null)
					{
						taskSessionHolder.Session = CreateNewSession(alias, sessionFactory);
					}
					session = taskSessionHolder.Session;
				}
				else if (task.CurrentNode != null)
				{
					var nodeSessionHolder = GetSessionHolder(task.CurrentNode, slotId);
					if (nodeSessionHolder.ShouldHaveSession)
					{
						if (nodeSessionHolder.Session == null)
						{
							nodeSessionHolder.Session = CreateNewSession(alias, sessionFactory);
						}
						session = nodeSessionHolder.Session;
					}
				}
			}

			return session;
		}

		private string SlotId(string alias)
		{
			// Key for storing ISession in the UITask or UINode context. We use a constant
			// value, because multiple instances of this class want to be able to
			// access the same session.
			const string suffix = "-NHibernate-ISession-8344f95b-5b8d-48dd-8843-52b83fdb1bde";
			return alias + suffix;
		}

		private UITask GetCurrentTask()
		{
			if (_currentTaskAtCreation != null && !_currentTaskAtCreation.IsComplete)
			{
				return _currentTaskAtCreation;
			}
			return null;
		}

		// Returns the slot for storing task-scope sessions for the given alias.

		public TaskAwareSessionManager(ISessionFactoryResolver sessionFactoryResolver) : base(sessionFactoryResolver)
		{
			// Keep track of the task that was current when this object was created.
			// For this reason this class needs to be registered as Transient with the
			// Castle container.
			_currentTaskAtCreation = UITask.Current;
			if (_currentTaskAtCreation != null)
			{
				// When the task completes, forget about it.
				_currentTaskAtCreation.TaskComplete += (sender, args) => _currentTaskAtCreation = null;
			}
		}

		private SessionHolder GetSessionHolder(UITask task, string slotId)
		{
			var sessionHolder = task.GetData(slotId) as SessionHolder;
			if (sessionHolder == null)
			{
				sessionHolder = new SessionHolder {
				                                  	ShouldHaveSession = HasSessionAttribute(task)
				                                  };
				task.SetData(slotId, sessionHolder);
			}

			return sessionHolder;
		}

		private SessionHolder GetSessionHolder(UINode node, string slotId)
		{
			var sessionHolder = node.GetData(slotId) as SessionHolder;
			if (sessionHolder == null)
			{
				sessionHolder = new SessionHolder {
				                                  	ShouldHaveSession = HasSessionAttribute(node.View)
				                                  	                    || HasSessionAttribute(node.Presenter)
				                                  	                    || HasSessionAttribute(node.ViewType)
				                                  	                    || HasSessionAttribute(node.PresenterType)
				                                  };
				node.SetData(slotId, sessionHolder);
			}
			return sessionHolder;
		}

		/// <summary>
		/// Work out whether an object's type definition has a session attribute associated with it.
		/// </summary>
		private bool HasSessionAttribute(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			var type = obj as Type;
			if (type == null)
			{
				type = obj.GetType();
			}

			var attributes = type.GetCustomAttributes(typeof (NHSessionAttribute), true);
			return attributes.Length > 0;
		}

		/// <summary>
		/// Stores a session against a <see cref="UITask"/> or <see cref="UINode"/>.
		/// </summary>
		private class SessionHolder
		{
			/// <summary>
			/// Should the task/node have a session associated with it.
			/// </summary>
			public bool ShouldHaveSession;

			/// <summary>
			/// The session associated with the task/node.
			/// </summary>
			public ISession Session;
		}
	}
}