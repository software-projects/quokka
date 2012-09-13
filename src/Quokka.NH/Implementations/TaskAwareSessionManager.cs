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

		public TaskAwareSessionManager(ISessionFactoryResolver sessionFactoryResolver)
			: base(sessionFactoryResolver)
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

		protected override ISession FindCompatibleSession(string alias, ISessionFactory sessionFactory)
		{
			ISession session = null;

			var task = GetCurrentTask();
			if (task != null)
			{
				session = task.FindCompatibleSession(alias, () => CreateNewSession(alias, sessionFactory));
			}

			return session;
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