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

using System.Collections.Generic;
using NHibernate;
using NHibernate.Context;
using Quokka.Data;
using Quokka.Diagnostics;
using ITransaction=Quokka.Transactions.ITransaction;

namespace Quokka.Transactions
{
	/// <summary>
	/// Very simple transaction manager that just forwards everything to NHibernate.
	/// </summary>
	public class NHTransactionManager : ITransactionManager
	{
		private readonly ISessionFactory _sessionFactory;
		private readonly Dictionary<ISession, NHTransaction> _sessionDict = new Dictionary<ISession, NHTransaction>();
		private readonly object _lockObject = new object();

		private NHTransaction FindBySession(ISession session)
		{
			lock (_lockObject)
			{
				NHTransaction tx;
				if (_sessionDict.TryGetValue(session, out tx))
				{
					return tx;
				}
			}
			return null;
		}

		public NHTransactionManager(INHConfig nhConfig)
		{
			Verify.ArgumentNotNull(nhConfig, "nhConfig");
			_sessionFactory = nhConfig.SessionFactory;
			Verify.IsNotNull(_sessionFactory);
		}

		public ITransaction Current
		{
			get
			{
				if (!CurrentSessionContext.HasBind(_sessionFactory))
					return null;

				ISession session = _sessionFactory.GetCurrentSession();
				return FindBySession(session);
			}
		}

		public ITransaction CreateTransaction()
		{
			if (CurrentSessionContext.HasBind(_sessionFactory))
			{
				throw new DataException("Cannot create transaction: a transaction is already in progress");
			}

			ISession session = _sessionFactory.OpenSession();
			CurrentSessionContext.Bind(session);
			NHTransaction transaction = new NHTransaction(session);

			lock (_lockObject)
			{
				_sessionDict.Add(session, transaction);
			}

			return transaction;
		}

		public void Dispose(ITransaction transaction)
		{
			if (transaction == null)
				return;

			NHTransaction nhtx = transaction as NHTransaction;
			if (nhtx == null)
			{
				throw new DataException("Cannot dispose of transaction of type " + transaction.GetType());
			}

			if (nhtx == Current)
			{
				throw new DataException("Cannot dispose of transaction if it is not the current transaction");
			}

			if (nhtx.IsActive)
			{
				throw new DataException("Cannot dispose of transaction if it is active");
			}

			lock (_lockObject)
			{
				_sessionDict.Remove(nhtx.Session);
			}

			CurrentSessionContext.Unbind(_sessionFactory);
			nhtx.Session.Dispose();
		}
	}
}