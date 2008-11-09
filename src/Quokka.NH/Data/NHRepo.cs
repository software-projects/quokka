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
using Quokka.Data;
using Quokka.Diagnostics;
using Quokka.DomainModel;

namespace Quokka.Data
{
	public class NHRepo<T> : IRepo<T> where T : DomainObject<T>
	{
		private readonly INHConfig _nhConfig;

		public NHRepo(INHConfig nhConfig)
		{
			Verify.ArgumentNotNull(nhConfig, "nhConfig", out _nhConfig);
		}

		public virtual void Save(T obj)
		{
			Session.Save(obj);
		}

		public virtual void Delete(T obj)
		{
			Session.Delete(obj);
		}

		public virtual void Update(T obj)
		{
			Session.Update(obj);
		}

		public virtual void SaveOrUpdate(T obj)
		{
			Session.SaveOrUpdate(obj);
		}

		public virtual T FindById(int id)
		{
			return (T) Session.Get(typeof (T), id);
		}

		public virtual IList<T> FindAll()
		{
			return CreateCriteria().List<T>();
		}

		public virtual ISession Session
		{
			get { return _nhConfig.SessionFactory.GetCurrentSession(); }
		}

		protected virtual ICriteria CreateCriteria()
		{
			return Session.CreateCriteria(typeof (T));
		}

		/// <summary>
		/// Returns the first item in the list, or <c>null</c> if the list is empty
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		protected static T FirstInList(IEnumerable<T> list)
		{
			foreach (T t in list)
			{
				return t;
			}
			return default(T);
		}
	}
}