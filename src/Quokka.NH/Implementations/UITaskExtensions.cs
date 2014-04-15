using System;
using NHibernate;
using Quokka.UI.Tasks;

namespace Quokka.NH.Implementations
{
	public static class UITaskExtensions
	{
		/// <summary>
		/// Find a session associated with the <see cref="UITask"/>, if the task is configured to
		/// have a session associated with it. An <see cref="ISession"/> is created if necessary.
		/// </summary>
		/// <param name="task">A <see cref="UITask"/>, probably from <see cref="UITask.Current"/>.</param>
		/// <param name="alias">Database alias.</param>
		/// <param name="createNewSession">
		/// Callback for creating a new session if necessary. This will be called if <paramref name="task"/>
		/// does not have a session associated with it yet, but should have one. This callback creates the
		/// session, and then that session is associated with <paramref name="task"/> for as long as necessary.
		/// (Depending on the task, this will either be for the duration of the task, or the duration of the
		/// current node in the task).
		/// </param>
		/// <returns>
		/// Returns an <see cref="ISession"/> that is assocated with the task, or <c>null</c> if the
		/// task does not have a session assocated with it.
		/// </returns>
		/// <remarks>
		/// This is really for internal use only. It is not intended for use by applications.
		/// </remarks>
		public static ISession FindCompatibleSession(this UITask task, string alias, Func<ISession> createNewSession)
		{
			ISession session = null;

			var taskSessionHolder = GetSessionHolder(task, alias);
			if (taskSessionHolder.ShouldHaveSession)
			{
				if (taskSessionHolder.Session == null)
				{
					// note that code works correctly if callback returns null
					taskSessionHolder.Session = createNewSession();
				}
				session = taskSessionHolder.Session;
			}
			else if (task.CurrentNode != null)
			{
				var nodeSessionHolder = GetSessionHolder(task.CurrentNode, alias);
				if (nodeSessionHolder.ShouldHaveSession)
				{
					if (nodeSessionHolder.Session == null)
					{
						// note that code works correctly if callback returns null
						nodeSessionHolder.Session = createNewSession();
					}
					session = nodeSessionHolder.Session;
				}
			}

			return session;
		}

		private static SessionHolder GetSessionHolder(UITask task, string alias)
		{
			var sessionHolder = task.GetTaskLifetimeObject<SessionHolder>(alias);
			if (sessionHolder == null)
			{
				sessionHolder = new SessionHolder
				{
					ShouldHaveSession = HasSessionAttribute(task)
				};
				task.SetTaskLifetimeObject(alias, sessionHolder);
			}

			return sessionHolder;
		}

		private static SessionHolder GetSessionHolder(UINode node, string alias)
		{
			var sessionHolder = node.Task.GetNodeLifetimeObject<SessionHolder>(alias);
			if (sessionHolder == null)
			{
				sessionHolder = new SessionHolder
				{
					ShouldHaveSession = HasSessionAttribute(node.View)
										|| HasSessionAttribute(node.Presenter)
										|| HasSessionAttribute(node.ViewType)
										|| HasSessionAttribute(node.PresenterType)
				};
				node.Task.SetNodeLifetimeObject(alias, sessionHolder);
			}
			return sessionHolder;
		}

		/// <summary>
		/// Work out whether an object's type definition has a session attribute associated with it.
		/// </summary>
		private static bool HasSessionAttribute(object obj)
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

			var attributes = type.GetCustomAttributes(typeof(NHSessionAttribute), true);
			return attributes.Length > 0;
		}

		/// <summary>
		/// Stores a session against a <see cref="UITask"/> or <see cref="UINode"/>.
		/// </summary>
		private class SessionHolder : IDisposable
		{
			/// <summary>
			/// Should the task/node have a session associated with it.
			/// </summary>
			public bool ShouldHaveSession;

			/// <summary>
			/// The session associated with the task/node.
			/// </summary>
			public ISession Session;

			public void Dispose()
			{
				if (Session != null)
				{
					Session.Dispose();
				}
			}
		}
	}
}
