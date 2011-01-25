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
using System.IO;
using System.Reflection;
using Quokka.Data.Internal;

namespace Quokka.Data
{
	/// <summary>
	/// 	Base class for types that represent an SQL query against a database.
	/// </summary>
	/// <typeparam name = "T">
	/// 	Each record returned from the SQL query is mapped to an instance of this type.
	/// </typeparam>
	public class SqlQuery<T> where T : class, new()
	{
		public SqlQuery() : this(null)
		{
		}

		public SqlQuery(IDbCommand cmd)
		{
			Command = cmd;
		}

		/// <summary>
		/// 	The <see cref = "IDbCommand" /> object used to send the query to the database.
		/// </summary>
		[IgnoreParameter]
		public IDbCommand Command { get; set; }

		/// <summary>
		/// 	Execute the query and return a single record object.
		/// </summary>
		/// <returns>
		/// 	A record object, or <c>null</c> if the query resulted in no rows.
		/// </returns>
		public T ExecuteSingle()
		{
			CheckCommand();
			T record = null;
			using (ISqlQueryReader<T> reader = ExecuteReader())
			{
				if (reader.Read())
				{
					record = reader.Record;
				}
			}
			return record;
		}

		/// <summary>
		/// 	Execute the query and return a list of record objects.
		/// </summary>
		/// <returns>
		/// 	Returns an <see cref = "List{T}" /> collection containing one object for
		/// 	each row returned by the query. If no rows are returned by the query, then
		/// 	the list has no items in it.
		/// </returns>
		/// <remarks>
		/// 	Although it is often considered good practice for methods like this to return a 
		/// 	collection interface such as <see cref = "IList{T}" /> or <see cref = "ICollection{T}" />, 
		/// 	this method returns <see cref = "List{T}" /> on purpose, as a common requirement by a calling 
		/// 	program is to sort the list or filter it further. Returning a <see cref = "List{T}" /> collection
		/// 	means that the calling program can easily do this without having to make a copy of the 
		/// 	collection.
		/// </remarks>
		public List<T> ExecuteList()
		{
			CheckCommand();
			PopulateCommand(Command);
			using (IDataReader dataReader = CommandExecuteReader(Command))
			{
				var list = new List<T>();
				DataRecordConverter converter = DataRecordConverter.CreateConverterFor(typeof (T), dataReader);

				while (dataReader.Read())
				{
					var record = new T();
					converter.CopyTo(record);
					list.Add(record);
				}

				return list;
			}
		}

		/// <summary>
		/// 	Execute the query and return the result as an <see cref = "ISqlQueryReader{T}" />.
		/// </summary>
		/// <returns>
		/// 	Returns an <see cref = "ISqlQueryReader{T}" />, which can be used to iterate through the
		/// 	results. It is the responsibility of the calling program to dispose of this object.
		/// </returns>
		/// <remarks>
		/// 	This method is useful if the query will return many rows, and it is not necessary to
		/// 	keep the result as a collection. The <see cref = "ISqlQueryReader{T}" /> implementation
		/// 	saves memory by copying the data from each row into the same record object in turn.
		/// </remarks>
		public ISqlQueryReader<T> ExecuteReader()
		{
			CheckCommand();
			PopulateCommand(Command);
			IDataReader reader = CommandExecuteReader(Command);
			return new QueryReader<T>(reader);
		}

		/// <summary>
		/// 	Override this method to set the command text for the query.
		/// </summary>
		/// <param name = "cmd">
		/// 	The <see cref = "IDbCommand" /> object that will be used to execute the query.
		/// </param>
		protected virtual void SetCommandText(IDbCommand cmd)
		{
			cmd.CommandText = GetCommandTextFromResource();
			cmd.CommandType = CommandType.Text;
		}

		/// <summary>
		/// 	Override this method to set the parameters in the <see cref = "IDbCommand" />
		/// 	object prior to executing the query.
		/// </summary>
		/// <param name = "cmd">
		/// 	The <see cref = "IDbCommand" /> object that will be used to execute the query.
		/// </param>
		protected virtual void SetParameters(IDbCommand cmd)
		{
			DataParameterBuilder parameterBuilder = DataParameterBuilder.GetInstance(GetType());
			parameterBuilder.PopulateCommand(cmd, this);
		}

		/// <summary>
		/// 	This method is called to execute the <see cref = "IDbCommand" /> against the data source.
		/// 	Override this method to provide custom processing and/or error handling.
		/// </summary>
		/// <param name = "cmd">
		/// 	The <see cref = "IDbCommand" /> object that will be used to execute the query.
		/// </param>
		/// <returns>
		/// 	Returns an <see cref = "IDataReader" /> object, which is used to read the result set(s)
		/// 	from the query.
		/// </returns>
		protected virtual IDataReader CommandExecuteReader(IDbCommand cmd)
		{
			return cmd.ExecuteReader();
		}

		/// <summary>
		/// 	Retrieve the SQL command text from an embedded resource file. The file name
		/// 	has the same name as the query class with the suffix ".sql".
		/// </summary>
		/// <returns></returns>
		protected string GetCommandTextFromResource()
		{
			return GetCommandTextFromResource(null);
		}

		/// <summary>
		/// 	Retrieve the SQL command text from an embedded resource file relative to the query type.
		/// </summary>
		/// <param name = "fileName">
		/// 	The file name of the embedded resource file. If this parameter is <c>null</c> or
		/// 	empty, then the file name defaults to the name of the query class with the suffix ".sql".
		/// 	For example if the query class is <c>MyQuery</c>, then the default file name is
		/// 	<c>MyQuery.sql</c>.
		/// </param>
		/// <returns>
		/// 	SQL command text.
		/// </returns>
		protected string GetCommandTextFromResource(string fileName)
		{
			Type type = GetType();
			if (string.IsNullOrEmpty(fileName))
			{
				fileName = type.Name + ".sql";
			}
			Assembly assembly = type.Assembly;
			using (Stream stream = assembly.GetManifestResourceStream(type, fileName))
			{
				if (stream == null)
				{
					throw new InvalidOperationException("Cannot find embedded query file: " + fileName);
				}

				using (TextReader reader = new StreamReader(stream))
				{
					return SqlCommandText.Sanitize(reader.ReadToEnd());
				}
			}
		}

		private void CheckCommand()
		{
			if (Command == null)
			{
				throw new InvalidOperationException("Cannot execute query. Command must be non-null");
			}
		}

		private void PopulateCommand(IDbCommand cmd)
		{
			SetCommandText(cmd);
			SetParameters(cmd);
		}
	}
}