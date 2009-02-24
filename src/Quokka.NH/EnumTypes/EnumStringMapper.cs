using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Quokka.EnumTypes
{
	[Serializable]
	public abstract class EnumStringMapper<T> : ImmutableType, IDiscriminatorType, IAuxiliaryDatabaseObject
	{
		private readonly Type _enumType;
		private string _tableName;
		private string _columnName;

		public int MaxLengthForEnumString
		{
			get { return EnumStringConverter<T>.MaxLength; }
		}

		protected EnumStringMapper()
			: base(SqlTypeFactory.GetString(EnumStringConverter<T>.MaxLength))
		{
			_enumType = typeof(T);
			if (!_enumType.IsEnum)
			{
				throw new MappingException(_enumType.Name + " does not inherit from System.Enum");
			}
		}

		/// <summary>
		/// Return the enum value from a string
		/// </summary>
		public virtual object GetInstance(object code)
		{
			try
			{
				return EnumStringConverter<T>.ConvertToEnum(code as string);
			}
			catch (ArgumentException ae)
			{
				throw new HibernateException(string.Format("Can't Parse {0} as {1}", code, _enumType.Name), ae);
			}
		}

		/// <summary>
		/// Get the string value from an enum
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public virtual object GetValue(object code)
		{
			T enumValue = (T)code;
			return EnumStringConverter<T>.ConvertToString(enumValue);
		}

		/// <summary>
		/// 
		/// </summary>
		public override System.Type ReturnedClass
		{
			get { return _enumType; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set(IDbCommand cmd, object value, int index)
		{
			IDataParameter parameter = (IDataParameter)cmd.Parameters[index];
			string stringValue = EnumStringConverter<T>.ConvertToString((T)value);
			if (stringValue == null)
			{
				parameter.Value = DBNull.Value;
			}
			else
			{
				parameter.Value = stringValue;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, int index)
		{
			object code = rs[index];
			string stringValue;
			if (code == DBNull.Value || code == null)
			{
				stringValue = null;
			}
			else
			{
				stringValue = (string)code;
			}
			return GetInstance(stringValue);

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get(IDataReader rs, string name)
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// This appends <c>enumstring - </c> to the beginning of the underlying
		/// enums name so that <see cref="System.Enum"/> could still be stored
		/// using the underlying value through the <see cref="PersistentEnumType"/>
		/// also.
		/// </remarks>
		public override string Name
		{
			get { return "enumstring - " + _enumType.Name; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ToString(object value)
		{
			if (value == null)
				return null;
			object obj = GetValue(value);
			if (obj == null)
				return null;
			return obj.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cached"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			if (cached == null)
			{
				return null;
			}
			return GetInstance(cached);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			return (value == null) ? null : GetValue(value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public string ObjectToSQLString(object value, Dialect dialect)
		{
			return GetValue(value).ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public object StringToObject(string xml)
		{
			return FromStringValue(xml);
		}

		public override object FromStringValue(string xml)
		{
			return GetInstance(xml);
		}

		public void AddDialectScope(string dialectName)
		{

		}

		public bool AppliesToDialect(Dialect dialect)
		{
			// Note that MsSqlCeDialect does not work properly
			return (dialect is MsSql2000Dialect || dialect is MsSql2005Dialect);
		}

		public void SetParameterValues(IDictionary<string, string> parameters)
		{
			throw new System.NotImplementedException();
		}

		public void SetParameterValues(IDictionary parameters)
		{
			_tableName = (string)parameters["table-name"];
			_columnName = (string)parameters["column-name"];
		}

		public string SqlCreateString(Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema)
		{
			List<string> enumNames = new List<string>();
			T[] enumValues = (T[])Enum.GetValues(typeof(T));

			foreach (T enumValue in enumValues)
			{
				string s = EnumStringConverter<T>.ConvertToString(enumValue);
				if (s != null)
				{
					enumNames.Add(s);
				}
			}


			StringBuilder sb = new StringBuilder();
			if (enumNames.Count > 0)
			{
				sb.Append("alter table ");
				sb.Append(_tableName);
				sb.Append(" add constraint ");
				sb.Append(CheckConstraintName(_tableName, _columnName));
				sb.Append(" check (");
				sb.Append(_columnName);
				sb.Append(" in ('");
				sb.Append(enumNames[0]);
				sb.Append("'");
				for (int index = 1; index < enumNames.Count; ++index)
				{
					sb.Append(", '");
					sb.Append(enumNames[index]);
					sb.Append("'");
				}
				sb.Append("))");
			}
			return sb.ToString();
		}

		public string SqlDropString(Dialect dialect, string defaultCatalog, string defaultSchema)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("alter table ");
			sb.Append(_tableName);
			sb.Append(" drop constraint ");
			sb.Append(CheckConstraintName(_tableName, _columnName));
			return sb.ToString();
		}

		private static string CheckConstraintName(string tableName, string columnName)
		{
			return "CHK_" + tableName + "_" + columnName;
		}
	}
}