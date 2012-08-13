using System;
using NHibernate;
using NHibernate.Transaction;
using Quokka.Diagnostics;

namespace Quokka.NH
{
	/// <summary>
	/// Useful extension methods for NHibernate <see cref="ISession"/>
	/// </summary>
	public static class SessionExtensions
	{
		/// <summary>
		/// Schedule an action to perform immediately after the transaction has committed.
		/// </summary>
		/// <param name="session">Session</param>
		/// <param name="action">Action to perform</param>
		/// <exception cref="InvalidOperationException">
		/// Not in a transaction.
		/// </exception>
		public static void AfterCommit(this ISession session, Action action)
		{
			Verify.ArgumentNotNull(session, "session");
			Verify.ArgumentNotNull(action, "action");
			var tx = session.Transaction;
			if (tx == null || !tx.IsActive)
			{
				throw new InvalidOperationException("Not in a transaction");
			}
			tx.RegisterSynchronization(new Synchronization { AfterCommitAction = action });
		}

		/// <summary>
		/// Schedule an action to perform immediately after the transaction
		/// has been rolled back.
		/// </summary>
		/// <param name="session">Session</param>
		/// <param name="action">Action to perform</param>
		/// <exception cref="InvalidOperationException">
		/// Not in a transaction.
		/// </exception>
		public static void AfterRollback(this ISession session, Action action)
		{
			Verify.ArgumentNotNull(session, "session");
			Verify.ArgumentNotNull(action, "action");
			var tx = session.Transaction;
			if (tx == null || !tx.IsActive)
			{
				throw new InvalidOperationException("Not in a transaction");
			}
			tx.RegisterSynchronization(new Synchronization { AfterRollbackAction = action });
		}

		/// <summary>
		/// Schedule an action to perform immediately after the transaction has completed.
		/// This action will be performed regardless of whether the transaction was committed
		/// or rolled-back. The boolean parameter passed to the action indicates whether the
		/// transaction succeeded or not.
		/// </summary>
		/// <param name="session">Session</param>
		/// <param name="action">
		/// Action to perform. This action accepts a boolean parameter, whose value is
		/// <c>true</c> if the transaction was committed, or <c>false</c> if the transaction
		/// was rolled back.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// Not in a transaction.
		/// </exception>
		public static void AfterCompletion(this ISession session, Action<bool> action)
		{
			Verify.ArgumentNotNull(session, "session");
			Verify.ArgumentNotNull(action, "action");
			var tx = session.Transaction;
			if (tx == null || !tx.IsActive)
			{
				throw new InvalidOperationException("Not in a transaction");
			}
			tx.RegisterSynchronization(new Synchronization { AfterCompletionAction = action });
		}

		/// <summary>
		/// Schedule an action to perform immediately prior to the transaction is complete.
		/// </summary>
		/// <param name="session">Session</param>
		/// <param name="action">Action to perform</param>
		/// <exception cref="InvalidOperationException">
		/// Not in a transaction.
		/// </exception>
		public static void BeforeCompletion(this ISession session, Action action)
		{
			Verify.ArgumentNotNull(action, "action");
			var tx = session.Transaction;
			if (tx == null || !tx.IsActive)
			{
				throw new InvalidOperationException("Not in a transaction");
			}
			tx.RegisterSynchronization(new Synchronization { BeforeCompletionAction = action });
		}

		private class Synchronization : ISynchronization
		{
			public Action AfterCommitAction;
			public Action AfterRollbackAction;
			public Action BeforeCompletionAction;
			public Action<bool> AfterCompletionAction;

			public void BeforeCompletion()
			{
				if (BeforeCompletionAction != null)
				{
					BeforeCompletionAction();
				}
			}

			public void AfterCompletion(bool success)
			{
				if (AfterCompletionAction != null)
				{
					AfterCompletionAction(success);
				}

				if (success)
				{
					if (AfterCommitAction != null)
					{
						AfterCommitAction();
					}
				}
				else
				{
					if (AfterRollbackAction != null)
					{
						AfterRollbackAction();
					}
				}
			}
		}
	}
}
