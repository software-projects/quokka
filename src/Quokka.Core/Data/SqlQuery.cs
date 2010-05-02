using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using Quokka.Data.Internal;

namespace Quokka.Data
{
	/// <summary>
	/// Base class for types that represent an SQL query against a database.
	/// </summary>
	/// <typeparam name="T">
	/// Each row returned from the SQL query is mapped to an instance of this type.
	/// </typeparam>
	public class SqlQuery<T> where T : class, new()
	{
		public T FindFirst(IDbCommand cmd)
		{
			T record = null;
			using (ISqlQueryReader<T> reader = QueryReader(cmd))
			{
				if (reader.Read())
				{
					record = reader.Record;
				}
			}
			return record;
		}

		public IList<T> QueryList(IDbCommand cmd)
		{
			PopulateCommand(cmd);
			using (IDataReader dataReader = cmd.ExecuteReader())
			{
				var list = new List<T>();
				DataRecordConverter converter = DataRecordConverter.CreateConverterFor(typeof(T), dataReader);

				while (dataReader.Read())
				{
					var record = new T();
					converter.CopyTo(record);
					list.Add(record);
				}

				return list;
			}
		}

		public ISqlQueryReader<T> QueryReader(IDbCommand cmd)
		{
			PopulateCommand(cmd);
			IDataReader reader = cmd.ExecuteReader();
			return new QueryReader<T>(reader);
		}

		protected virtual void SetParameters(IDbCommand cmd)
		{
			DataParameterBuilder parameterStuffer = DataParameterBuilder.GetInstance(GetType());
			parameterStuffer.PopulateCommand(cmd, this);
		}

		private void PopulateCommand(IDbCommand cmd)
		{
			SetCommandText(cmd);
			SetParameters(cmd);
		}

		protected virtual void SetCommandText(IDbCommand cmd)
		{
			cmd.CommandText = GetCommandTextFromResource();
			cmd.CommandType = CommandType.Text;
		}

		protected string GetCommandTextFromResource()
		{
			Type type = GetType();
			string fileName = type.Name + ".sql";

			Assembly assembly = type.Assembly;
			using (Stream stream = assembly.GetManifestResourceStream(type, fileName))
			{
				if (stream == null)
				{
					throw new InvalidOperationException("Cannot find embedded query file: " + fileName);
				}

				using (TextReader reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}
	}
}