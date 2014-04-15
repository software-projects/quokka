#region License

// Copyright 2004-2013 John Jeffery <john@jeffery.id.au>
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
using System.Reflection;
using Quokka.NH.Implementations;

namespace Quokka.NH
{
	/// <summary>
	/// Base class for a class that has NHibernate semantics for <see cref="Equals(object)"/> and <see cref="CompareTo"/>
	/// </summary>
	/// <typeparam name="TEntity">
	/// Type of the entity, used for implementing <see cref="IEquatable{TEntity}"/> and
	/// and <see cref="IComparable{TEntity}"/>.
	/// </typeparam>
	/// <typeparam name="TId">
	/// Type of the identifier that uniquely identifies the entity. This is usually one of
	/// <see cref="Int32"/>, <see cref="Int64"/>, <see cref="String"/> or perhaps <see cref="Guid"/>.
	/// </typeparam>
	/// <remarks>
	/// <para>
	/// The main job of this base class is to provide the required equality semantics
	/// as required by NHibernate.
	/// </para>
	/// </remarks>
	public abstract class NHEntity<TEntity, TId> : IEquatable<TEntity>, IComparable<TEntity>
		where TEntity : NHEntity<TEntity, TId>
		where TId : IComparable
	{
#pragma warning disable 649
		// This field gets set via reflection.
		protected static readonly IIdHelper<TId> IdHelper;
#pragma warning restore 649

		protected abstract TId GetId();

		static NHEntity()
		{
			if (IdHelper == null)
			{
				var type = typeof(NHEntity<TEntity, TId>);
				var field = type.GetField("IdHelper", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				if (field == null)
				{
					throw new QuokkaException("Cannot find static field IdHelper for type " + type);
				}

				var idType = typeof(TId);
				if (idType == typeof(string))
				{
					// Special case for string, as identifiers are compared
					// ordinal case-insensitive.
					var helper = new IdHelperForString();
					field.SetValue(null, helper);
				}
				else if (idType == typeof(int))
				{
					// Special case for int as many identifiers are this type.
					// This class avoids boxing altogether.
					var helper = new IdHelperForInt32();
					field.SetValue(null, helper);
				}
				else if (idType == typeof(long))
				{
					// Special case for long as some identifiers are this type.
					// This class avoids boxing altogether.
					var helper = new IdHelperForInt64();
					field.SetValue(null, helper);
				}
				else if (idType.IsValueType)
				{
					// Value types get an ID helper that does not involve boxing.
					var helperType = typeof(IdHelperForValueType<>);
					var genericType = helperType.MakeGenericType(new[] { typeof(TId) });
					var helper = Activator.CreateInstance(genericType, null);
					field.SetValue(null, helper);
				}
				else
				{
					// ID helper type for classes that can have null value
					var helperType = typeof(IdHelperForClassType<>);
					var genericType = helperType.MakeGenericType(new[] { typeof(TId) });
					var helper = Activator.CreateInstance(genericType, null);
					field.SetValue(null, helper);
				}
			}
		}

		protected NHEntity()
		{
			if (IdHelper == null)
			{
				// Should never happen
				throw new QuokkaException("Static field IdHelper is null. This indicates an internal software error.");
			}
		}

		public override bool Equals(object obj)
		{
			var other = obj as TEntity;
			return !IsNull(other) && Equals(other);
		}

		// used to check that the value of Id does not change after the first call to GetHashCode().
		private int? _firstHashCode;

		public override int GetHashCode()
		{
			var id = GetId();
			var hashCode = IdHelper.GetHashCode(id);

			// Resharper does not like us referencing a non-readonly variable inside
			// GetHashCode(). This is quite clever, but in this case we are justified,
			// as it forms part of our consistency check.
			// ReSharper disable NonReadonlyFieldInGetHashCode
			if (_firstHashCode == null)
			{
				// First time GetHashCode() has been called.
				_firstHashCode = hashCode;
			}
			else
			{
				if (_firstHashCode.Value != hashCode)
				{
					var message = string.Format(
						"Object of type {0} (Id={1}) has changed identity since the first call to GetHashCode()."
						+ "\r\nThis will cause problems if this object has been added to a collection that depends on GetHashCode()"
						+ " returning the same value every time.", typeof(TEntity).FullName, id);
					throw new InvalidOperationException(message);
				}
			}
			// ReSharper restore NonReadonlyFieldInGetHashCode

			return hashCode;
		}

		/// <summary>
		/// Test for null without triggering a fetch
		/// </summary>
		/// <remarks>
		/// Remembering that entity is probably an NHibernate object,
		/// convert to object and test for null. If entity is an
		/// NHibernate proxy, we want to be careful not to trigger
		/// an unnecessary fetch from the database.
		/// </remarks>
		private static bool IsNull(object entity)
		{
			return entity == null;
		}

		public virtual bool Equals(TEntity other)
		{
			if (IsNull(other))
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			var thisId = GetId();
			var otherId = other.GetId();

			if (IdHelper.IsDefaultValue(thisId) || IdHelper.IsDefaultValue(otherId))
			{
				// If two different objects have the same default ID value, they
				// are considered different.
				return false;
			}

			return IdHelper.AreEqual(thisId, otherId);
		}

		public static bool operator ==(TEntity entity1, NHEntity<TEntity, TId> entity2)
		{
			// Perform 'safe' tests for null first. Safe means that an NHibernate
			// proxy will not cause a fetch from the database. We want to get null
			// tests out of the way first.
			if (IsNull(entity1))
			{
				// null == null
				return IsNull(entity2);
			}

			if (IsNull(entity2))
			{
				return false;
			}

			if (ReferenceEquals(entity1, entity2))
			{
				return true;
			}

			// Resharper thinks that there is a possible null exception, because
			// it does not know about IsNull()
			// ReSharper disable PossibleNullReferenceException
			var id1 = entity1.GetId();
			var id2 = entity2.GetId();
			// ReSharper restore PossibleNullReferenceException

			if (IdHelper.IsDefaultValue(id1) || IdHelper.IsDefaultValue(id2))
			{
				// If two different objects have the same default ID value, they
				// are considered non-equal.
				return false;
			}

			return IdHelper.AreEqual(id1, id2);
		}

		public static bool operator !=(TEntity entity1, NHEntity<TEntity, TId> entity2)
		{
			// Perform 'safe' tests for null first. Safe means that an NHibernate
			// proxy will not cause a fetch from the database. We want to get null
			// tests out of the way first.
			if (IsNull(entity1))
			{
				// null == null
				return !IsNull(entity2);
			}
			if (IsNull(entity2))
			{
				return true;
			}

			if (ReferenceEquals(entity1, entity2))
			{
				return false;
			}

			// Resharper thinks that there is a possible null exception, because
			// it does not know about IsNull()
			// ReSharper disable PossibleNullReferenceException
			var id1 = entity1.GetId();
			var id2 = entity2.GetId();
			// ReSharper restore PossibleNullReferenceException

			if (IdHelper.IsDefaultValue(id1) || IdHelper.IsDefaultValue(id2))
			{
				// If two different objects have the same default ID value, they
				// are considered non-equal.
				return true;
			}

			return !IdHelper.AreEqual(id1, id2);
		}

		public virtual int CompareTo(TEntity other)
		{
			if (IsNull(other))
			{
				return 1;
			}

			var thisId = GetId();
			var otherId = other.GetId();

			return IdHelper.Compare(thisId, otherId);
		}
	}
}
