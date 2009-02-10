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

namespace Quokka.DomainModel
{
	/// <summary>
	/// Base class for domain model objects. Provides correct semantics for comparing
	/// </summary>
	/// <typeparam name="T">Derived class type</typeparam>
	public abstract class DomainObject<T> : DomainObject, IEquatable<T> where T : DomainObject<T>
	{
		public override bool Equals(object obj)
		{
			return Equals(obj as T);
		}

		public override int GetHashCode()
		{
			// Zero Id means that the domain object has not been saved to the database yet.
			// A domain object with a non-zero Id has been saved to the database.
			//
			// The purpose of this test is if a number of unsaved domain objects are used as
			// the key to a hash table or a set -- that set will still work efficiently.
			// If all of the objects had a hash code of zero then that hash table would
			// be very inefficient.
			return Id == 0 ? base.GetHashCode() : Id.GetHashCode();
		}

		public virtual bool Equals(T other)
		{
			// Never equals null
			if (other == null)
				return false;

			// If Id == 0, then the domain object has not been saved yet.
			// In this case, equality means the same object only.
			if (Id == 0)
				return ReferenceEquals(this, other);

			// If Id != 0, then equality means the same Id
			return Id == other.Id;
		}
	}
}