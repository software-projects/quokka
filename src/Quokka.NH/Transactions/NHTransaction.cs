#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2008 John Jeffery. All rights reserved.
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

using System;
using NHibernate;
using Quokka.Data;
using Quokka.Diagnostics;

namespace Quokka.Transactions
{
	public class NHTransaction : ITransaction
	{
		private readonly ISession _session;
		private bool _rollbackOnly;

		public event EventHandler Committed;
		public event EventHandler RolledBack;

		public NHTransaction(ISession session)
		{
			Verify.ArgumentNotNull(session, "session", out _session);
		}

		public ISession Session
		{
			get { return _session; }
		}


		public bool IsActive
		{
			get { return _session.Transaction.IsActive; }
		}

		public bool IsRollbackOnly
		{
			get { return _rollbackOnly; }
		}

		public void Begin()
		{
			try
			{
				_session.Transaction.Begin();
			}
			catch (Exception ex)
			{
				throw new DataException("Cannot begin transaction", ex);
			}
		}

		public void Commit()
		{
			if (_rollbackOnly)
			{
				throw new DataException("Cannot commit transaction: it was marked rollback-only");
			}
			try
			{
				_session.Transaction.Commit();
				RaiseCommitted();
			}
			catch (Exception ex)
			{
				// failed to commit, so only rollbacks allowed from now on
				_rollbackOnly = true;

				throw new DataException("Cannot commit transaction", ex);
			}
		}

		public void Rollback()
		{
			try
			{
				_session.Transaction.Rollback();
				RaiseRolledBack();
			}
			catch (Exception ex)
			{
				throw new DataException("Cannot rollback transaction", ex);
			}
		}

		public void SetRollbackOnly()
		{
			_rollbackOnly = true;
		}

		private void RaiseRolledBack()
		{
			if (RolledBack != null)
			{
				RolledBack(this, EventArgs.Empty);
			}
		}

		private void RaiseCommitted()
		{
			if (Committed != null)
			{
				Committed(this, EventArgs.Empty);
			}
		}
	}
}