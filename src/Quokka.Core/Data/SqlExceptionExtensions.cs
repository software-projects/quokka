using System;
using System.Data.SqlClient;

namespace Quokka.Data
{
	public static class SqlExceptionExtensions
	{
		private static class SqlErrorCode
		{
			public const int DeadLock = 1205;
			public const int DuplicateKey = 2627;
		}

		/// <summary>
		///   Is the exception a deadlock exception.
		/// </summary>
		/// <param name = "ex"></param>
		/// <returns></returns>
		public static bool IsDeadlockException(this Exception ex)
		{
			return ex.HasSqlError(SqlErrorCode.DeadLock);
		}

		/// <summary>
		///   Is the exception a duplicate key exception.
		/// </summary>
		/// <param name = "ex"></param>
		/// <returns></returns>
		public static bool IsDuplicateKeyException(this Exception ex)
		{
			return ex.HasSqlError(SqlErrorCode.DuplicateKey);
		}

		public static bool HasSqlError(this Exception ex, params int[] errorCodes)
		{
			var sqlEx = ex as SqlException;
			if (sqlEx == null)
			{
				if (ex.InnerException == null)
				{
					return false;
				}

				return HasSqlError(ex.InnerException, errorCodes);
			}

			foreach (var errorCode in errorCodes)
			{
				if (sqlEx.Number == errorCode)
				{
					return true;
				}

				foreach (SqlError error in sqlEx.Errors)
				{
					if (error.Number == errorCode)
					{
						return true;
					}
				}
			}
			return false;
		}

	}
}
