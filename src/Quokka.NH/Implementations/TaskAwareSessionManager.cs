using System;
using Castle.Core;
using NHibernate;
using NHibernate.Context;
using NHibernate.Engine;
using Quokka.Diagnostics;
using Quokka.NH.Interfaces;
using Quokka.NH.Startup;
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
	public class TaskAwareSessionManager : ISessionManager
	{
		private class SessionHolder
		{
			public ISession Session;
			public bool ShouldHaveSession;
		}

		// Work out whether an object's type definition has a session attribute associated with it.
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

		private readonly ISessionStore<ISession> _sessionStore;
		private readonly ISessionStore<IStatelessSession> _statelessSessionStore;
		private readonly ISessionFactoryResolver _sessionFactoryResolver;
		private UITask _currentTaskAtCreation;

		// Returns the slot for storing task-scope sessions for the given alias.
		private string SlotId(string alias)
		{
			// Key for storing ISession in the UITask or UINode context. We use a constant
			// value, because multiple instances of this class want to be able to
			// access the same session.
			const string suffix = "-NHibernate-ISession-8344f95b-5b8d-48dd-8843-52b83fdb1bde";
			return alias + suffix;
		}

		public TaskAwareSessionManager(
			ISessionStore<ISession> sessionStore,
			ISessionStore<IStatelessSession> statelessSessionStore,
			ISessionFactoryResolver sessionFactoryResolver)
		{
			_sessionStore = Verify.ArgumentNotNull(sessionStore, "sessionStore");
			_statelessSessionStore = Verify.ArgumentNotNull(statelessSessionStore, "statelessSessionStore");
			_sessionFactoryResolver = Verify.ArgumentNotNull(sessionFactoryResolver, "sessionFactoryResolver");

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

		private ISession CreateSession(string alias)
		{
			var sessionFactory = _sessionFactoryResolver.GetSessionFactory(alias);
			return sessionFactory.OpenSession();
		}

		private SessionHolder GetSessionHolder(UITask task, string alias, string slotId)
		{
			var sessionHolder = task.GetData(slotId) as SessionHolder;
			if (sessionHolder == null)
			{
				sessionHolder = new SessionHolder {
				                                  	ShouldHaveSession = HasSessionAttribute(task)
				                                  };
				task.SetData(slotId, sessionHolder);
			}

			if (sessionHolder.ShouldHaveSession && sessionHolder.Session == null)
			{
				// There should be a session associated with the task.
				sessionHolder.Session = CreateSession(alias);
			}
			return sessionHolder;
		}

		private SessionHolder GetSessionHolder(UINode node, string alias, string slotId)
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

			if (sessionHolder.ShouldHaveSession && sessionHolder.Session == null)
			{
				// There should be a session associated with the task.
				sessionHolder.Session = CreateSession(alias);
			}
			return sessionHolder;
		}

		public ISession OpenSession(string alias)
		{
			alias = NormaliseAlias(alias);
			var canClose = false;
			var removeOnClose = false;
			var session = _sessionStore.FindCompatibleSession(alias);
			var slotId = SlotId(alias);

			if (session == null)
			{
				var task = GetCurrentTask();
				if (task != null)
				{
					var taskSessionHolder = GetSessionHolder(task, alias, slotId);
					if (taskSessionHolder.Session != null)
					{
						session = taskSessionHolder.Session;
						canClose = false;
						removeOnClose = true;
						_sessionStore.Add(session, alias);
					}
					else if (task.CurrentNode != null)
					{
						var nodeSessionHolder = GetSessionHolder(task.CurrentNode, alias, slotId);
						if (nodeSessionHolder.Session != null)
						{
							session = nodeSessionHolder.Session;
							canClose = false;
							removeOnClose = true;
							_sessionStore.Add(session, alias);
						}
					}
				}
			}

			if (session == null)
			{
				// This is the normal, non-task aware action -- create a session and put it
				// in the session store. The session will be disposed during the current context.
				var sessionFactory = _sessionFactoryResolver.GetSessionFactory(alias);
				session = sessionFactory.OpenSession();
				canClose = true;
				removeOnClose = true;
				_sessionStore.Add(session, alias);
			}

			var sessionDelegate = new SessionDelegate(canClose, session);
			if (removeOnClose)
			{
				sessionDelegate.Closed += (o, e) => _sessionStore.Remove(session, alias);
			}

			// Bind the (real) ISession to the session factory's current context.
			// This is going to go really, really wrong if the current session context
			// defined for the NHibernate configuration is not compatibile with the
			// session store.
			//
			// TODO: need to work out what to do here -- it would be nice to ditch
			// the session store in favour of the NHibernate current session context,
			// but that only works for ISession, not ISession store.
			var sessionFactoryImplementor = session.SessionFactory as ISessionFactoryImplementor;
			if (sessionFactoryImplementor != null
				&& (sessionFactoryImplementor.CurrentSessionContext as CurrentSessionContext) != null)
			{
				if (!CurrentSessionContext.HasBind(session.SessionFactory))
				{
					CurrentSessionContext.Bind(session);
					sessionDelegate.Closed += (o, e) => CurrentSessionContext.Unbind(session.SessionFactory);
				}
			}

			return sessionDelegate;
		}

		public IStatelessSession OpenStatelessSession(string alias)
		{
			alias = NormaliseAlias(alias);
			var canClose = false;
			var session = _statelessSessionStore.FindCompatibleSession(alias);
			if (session == null)
			{
				var sessionFactory = _sessionFactoryResolver.GetSessionFactory(alias);
				session = sessionFactory.OpenStatelessSession();
				canClose = true;
				_statelessSessionStore.Add(session, alias);
			}

			var sessionDelegate = new StatelessSessionDelegate(canClose, session);
			if (canClose)
			{
				sessionDelegate.Closed += (o, e) => _statelessSessionStore.Remove(sessionDelegate.InnerSession, alias);
			}

			return sessionDelegate;
		}

		private string NormaliseAlias(string alias)
		{
			if (alias == null)
			{
				alias = _sessionFactoryResolver.DefaultAlias;
				if (alias == null)
				{
					throw new NHibernateFacilityException("No default alias is defined");
				}
			}
			return alias;
		}

		private UITask GetCurrentTask()
		{
			if (_currentTaskAtCreation != null && !_currentTaskAtCreation.IsComplete)
			{
				return _currentTaskAtCreation;
			}
			return null;
		}
	}
}