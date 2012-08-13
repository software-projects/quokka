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
			return other != null && Equals(other);
		}

		public override int GetHashCode()
		{
			var id = GetId();
			return IdHelper.GetHashCode(id);
		}

		public virtual bool Equals(TEntity other)
		{
			if (other == null)
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
			object o1 = entity1;
			if (o1 == null)
			{
				object o2 = entity2;
				return o2 == null;
			}

			return entity1.Equals(entity2);
		}

		public static bool operator !=(TEntity entity1, NHEntity<TEntity, TId> entity2)
		{
			object o1 = entity1;
			if (o1 == null)
			{
				object o2 = entity2;
				return o2 != null;
			}

			return !entity1.Equals(entity2);
		}

		public virtual int CompareTo(TEntity other)
		{
			if (other == null)
			{
				return 1;
			}

			var thisId = GetId();
			var otherId = other.GetId();

			return IdHelper.Compare(thisId, otherId);
		}
	}
}
