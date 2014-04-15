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