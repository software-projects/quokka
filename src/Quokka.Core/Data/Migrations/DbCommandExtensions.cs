using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Common.Logging;

namespace Quokka.Data.Migrations
{
	internal static class DbCommandExtensions
	{
		private static readonly ILog Logger = LogManager.GetCurrentClassLogger();

		private static readonly Regex RowcountRegex = new Regex(@"^\s*--\s*repeat\s+until\s+rowcount\s+=+\s+0",
														RegexOptions.IgnoreCase | RegexOptions.Multiline);

		private static readonly Regex RowcountRegex2 = new Regex(@"^\s*set\s+rowcount\s+\d+", RegexOptions.IgnoreCase | RegexOptions.Multiline);

		public static void RunMigrationScript(this IDbCommand cmd, TextReader reader)
		{
			var sb = new StringBuilder();

			for (; ; )
			{
				string line = reader.ReadLine();
				if (line == null)
				{
					RunMigrationSql(cmd, sb.ToString());
					break;
				}

				if (line.Trim().ToLower() == "go")
				{
					RunMigrationSql(cmd, sb.ToString());
					sb = new StringBuilder();
				}
				else
				{
					sb.AppendLine(line);
				}
			}
		}

		/// <summary>
		/// Run an SQL script for a database migration. If the script starts with 'set rowcount nn', then the sql will be run
		/// repeatedly until there are no rows updated.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="sql"></param>
		public static void RunMigrationSql(this IDbCommand cmd, string sql)
		{
			if (String.IsNullOrEmpty(sql))
			{
				return;
			}

			if (sql.Length == 0)
			{
				return;
			}

			cmd.CommandType = CommandType.Text;
			cmd.CommandText = sql;

			if (RowcountRegex.IsMatch(sql) || RowcountRegex2.IsMatch(sql))
			{
				int totalRowCount = 0;
				for (; ; )
				{
					int rowCount = cmd.ExecuteNonQuery();
					if (rowCount == 0)
					{
						break;
					}
					totalRowCount += rowCount;
					Logger.InfoFormat("... {0} rows updated", totalRowCount);
				}

				// restore any rowcount settting
				cmd.CommandText = "set rowcount 0";
				cmd.ExecuteNonQuery();
			}
			else
			{
				cmd.ExecuteNonQuery();
			}
		}
	}
}
