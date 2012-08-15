#region License

// Copyright 2004-2012 John Jeffery <john@jeffery.id.au>
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
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace Quokka.Data
{
	/// <summary>
	/// Utility class for accessing SQL Server Express User Instance databases
	/// </summary>
	public static class SqlExpressUtil
	{
		/// <summary>
		/// Open a connection to the master database of the local user instance.
		/// </summary>
		/// <returns>SQL Server Connection</returns>
		public static SqlConnection OpenMaster()
		{
			const string masterConnectionString = @"Data Source=.\SQLEXPRESS"
			                                      + ";Integrated Security=SSPI"
			                                      + ";User Instance=true"
			                                      + ";Initial Catalog=master";

			SqlConnection conn = new SqlConnection(masterConnectionString);
			conn.Open();
			return conn;
		}

		/// <summary>
		/// Create a connection string to attach a file and use as a database on the local user instance
		/// </summary>
		/// <param name="dbfile">Path of the database file</param>
		/// <param name="dbname">The database name to attach as</param>
		/// <returns>Connection string</returns>
		public static string CreateConnectionString(string dbfile, string dbname)
		{
			SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
			builder.DataSource = @".\SQLEXPRESS";
			builder.IntegratedSecurity = true;
			builder.UserInstance = true;
			builder.AttachDBFilename = Path.GetFullPath(dbfile);
			builder.InitialCatalog = dbname;
			return builder.ToString();
		}

		/// <summary>
		/// Given the database file name, returns the transaction log file path
		/// </summary>
		/// <param name="databaseFilePath">Database file path</param>
		/// <returns>Transaction log file path</returns>
		public static string LogFilePath(string databaseFilePath)
		{
			string directory = Path.GetDirectoryName(databaseFilePath);
			string baseName = Path.GetFileNameWithoutExtension(databaseFilePath);
			string filePath = Path.Combine(directory, baseName) + "_log.ldf";
			return filePath;
		}

		/// <summary>
		/// Creates a new database file at the specified path. The database file remains detached.
		/// </summary>
		/// <param name="filePath"></param>
		public static void CreateDatabaseFile(string filePath)
		{
			SqlConnection connection = OpenMaster();
			try
			{
				string escapedName = EscapeSqlName(Path.GetFileNameWithoutExtension(filePath));
				string escapedFilePath = Path.GetFullPath(filePath).Replace("'", "''''");
				try
				{
					const string sql =
						"DECLARE @databaseName sysname"
						+ " SET @databaseName = CONVERT(sysname, NEWID())"
						+ " WHILE EXISTS (SELECT name FROM sys.databases WHERE name = @databaseName)"
						+ " BEGIN"
						+ "   SET @databaseName = CONVERT(sysname, NEWID())"
						+ " END"
						+ " SET @databaseName = '[' + @databaseName + ']'"
						+ " DECLARE @sqlString nvarchar(MAX)"
						+ " SET @sqlString = 'CREATE DATABASE ' + @databaseName + N' ON ( NAME = [{0}], FILENAME = N''{1}'')'"
						+ " EXEC sp_executesql @sqlString"
						+ " SET @sqlString = 'ALTER DATABASE ' + @databaseName + ' SET AUTO_SHRINK ON'"
						+ " EXEC sp_executesql @sqlString"
						+ " SET @sqlString = 'ALTER DATABASE ' + @databaseName + ' SET OFFLINE WITH ROLLBACK IMMEDIATE'"
						+ " EXEC sp_executesql @sqlString"
						+ " SET @sqlString = 'EXEC sp_detach_db ' + @databaseName"
						+ " EXEC sp_executesql @sqlString";

					SqlCommand command = connection.CreateCommand();
					command.CommandText = string.Format(CultureInfo.InvariantCulture, sql, new object[] {escapedName, escapedFilePath});
					command.ExecuteNonQuery();
				}
				catch (Exception exception)
				{
					throw new QuokkaException("An error occurred while creating a new DB file: " + exception.Message, exception);
				}
			}
			finally
			{
				connection.Close();
			}
		}

		/// <summary>
		/// Detach a database
		/// </summary>
		/// <param name="dbname">Name of the database to detach</param>
		public static void DetachDatabase(string dbname)
		{
			using (SqlConnection conn = OpenMaster())
			{
				DetachDatabase(conn, dbname);
			}
		}

		/// <summary>
		/// Detach at database
		/// </summary>
		/// <param name="conn">Existing SQL Server connection</param>
		/// <param name="dbname">Name of the datbase</param>
		public static void DetachDatabase(SqlConnection conn, string dbname)
		{
			const string fmt =
				"use master"
				+ " if exists(select * from sysdatabases where name = N'{0}')"
				+ " begin"
				+ "   alter database[{1}] set offline with rollback immediate"
				+ "   exec sp_detach_db [{1}]"
				+ " end";

			string sql = string.Format(fmt, EscapeSqlString(dbname), EscapeSqlName(dbname));
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = sql;
			cmd.ExecuteNonQuery();
		}

		public static string EscapeSqlName(string name)
		{
			return name.Replace("]", "]]").Replace("'", "''");
		}

		public static string EscapeSqlString(string s)
		{
			return s.Replace("'", "''");
		}
	}
}