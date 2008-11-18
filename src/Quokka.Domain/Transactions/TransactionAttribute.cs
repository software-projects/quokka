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
using Microsoft.Practices.ServiceLocation;
using PostSharp.Laos;
using Quokka.Data;

namespace Quokka.Transactions
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	[Serializable]
	public sealed class TransactionAttribute : OnMethodInvocationAspect
	{
		public override void OnInvocation(MethodInvocationEventArgs eventArgs)
		{
			ITransactionManager txMgr = GetTransactionManager();
			ITransaction tx = txMgr.Current;

			if (tx == null)
			{
				// No transaction is current, so create one.
				tx = txMgr.CreateTransaction();
				tx.Begin();
				try
				{
					eventArgs.Proceed();

					if (tx.IsRollbackOnly)
					{
						tx.Rollback();
					}
					else
					{
						tx.Commit();
					}
				}
				catch (Exception)
				{
					tx.Rollback();
					throw;
				}
				finally
				{
					txMgr.Dispose(tx);
				}
			}
			else
			{
				// A transaction is already in progress, so do not create a new one
				try
				{
					eventArgs.Proceed();
				}
				catch (Exception)
				{
					// Exception thrown by the method, so the outer transaction should
					// not be able to be committed.
					tx.SetRollbackOnly();
					throw;
				}
			}
		}

		private static ITransactionManager GetTransactionManager()
		{
			ITransactionManager mgr = ServiceLocator.Current.GetInstance<ITransactionManager>();
			if (mgr == null)
			{
				throw new DataException("Missing transaction manager (ITransactionManager)");
			}
			return mgr;
		}
	}
}