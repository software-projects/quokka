#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
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
using System.Collections.Generic;
using System.Data;

namespace Quokka.Data.Internal
{
	/// <summary>
	/// 	Provides a mapping from a CLR type as used by a property of the
	/// 	<see cref = "SqlQuery{T}" /> class into a <see cref = "DbType" /> that
	/// 	can be used with an <see cref = "IDataParameter" />
	/// </summary>
	public class DataParameterTypeMapping
	{
		private static readonly Dictionary<Type, DataParameterTypeMapping> MapTypeToHelper =
			new Dictionary<Type, DataParameterTypeMapping>();

		static DataParameterTypeMapping()
		{
			DefineNullable<bool>(DbType.Boolean);
			DefineNullable<byte>(DbType.Byte);
			DefineNullable<char>(DbType.String);
			DefineNullable<DateTime>(DbType.DateTime);
			DefineNullable<decimal>(DbType.Decimal);
			DefineNullable<double>(DbType.Double);
			DefineNullable<float>(DbType.Single);
			DefineNullable<Guid>(DbType.Guid);
			DefineNullable<short>(DbType.Int16);
			DefineNullable<int>(DbType.Int32);
			DefineNullable<long>(DbType.Int64);
			DefineHelper<string>(DbType.String);
		}

		protected DataParameterTypeMapping(Type type, DbType dbType)
		{
			Type = type;
			DbType = dbType;
		}

		public Type Type { get; private set; }
		public DbType DbType { get; private set; }

		public virtual object GetValue(object obj)
		{
			return obj ?? DBNull.Value;
		}

		public static DataParameterTypeMapping ForType(Type type)
		{
			DataParameterTypeMapping helper;
			MapTypeToHelper.TryGetValue(type, out helper);
			return helper;
		}

		private static void DefineNullable<T>(DbType dbType) where T : struct
		{
			DefineHelper<T>(dbType);
			DefineHelper(new NullableHelper<T>(dbType));
		}

		private static void DefineHelper<T>(DbType dbType)
		{
			DefineHelper(new DataParameterTypeMapping(typeof (T), dbType));
		}

		private static void DefineHelper(DataParameterTypeMapping helper)
		{
			MapTypeToHelper.Add(helper.Type, helper);
		}

		#region Nested type: NullableHelper

		private class NullableHelper<T> : DataParameterTypeMapping where T : struct
		{
			public NullableHelper(DbType dbType) : base(typeof (T?), dbType)
			{
			}

			public override object GetValue(object obj)
			{
				var nullable = (T?) obj;
				if (nullable.HasValue)
				{
					return nullable.Value;
				}
				return DBNull.Value;
			}
		}

		#endregion
	}
}