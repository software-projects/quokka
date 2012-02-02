using System.Data.SqlClient;
using System.IO;
using NUnit.Framework;

namespace Quokka.Data
{
	[TestFixture]
	[Ignore("This class will be obsolete soon")]
	public class SqlExpressUtilTests
	{
		private string _dbFile;
		private string _logFile;
		private const string _dbName = "TestDatabaseUsedForTestingQuokkaSqlExpressUtilClass";

		[SetUp]
		public void SetUp()
		{
			_dbFile = Path.GetTempFileName();
			File.Delete(_dbFile);
			_logFile = SqlExpressUtil.LogFilePath(_dbFile);			
		}

		[TearDown]
		public void TearDown()
		{
			File.Delete(_dbFile);
			File.Delete(_logFile);
		}

		[Test]
		public void CreateConnectionString()
		{
			string cs = SqlExpressUtil.CreateConnectionString(@"c:\temp\abc.mdf", "xyz");

			SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(cs);
			Assert.AreEqual(@"c:\temp\abc.mdf", sb.AttachDBFilename);
			Assert.AreEqual("xyz", sb.InitialCatalog);
			Assert.AreEqual(@".\SQLEXPRESS", sb.DataSource);
			Assert.IsTrue(sb.IntegratedSecurity);
			Assert.IsTrue(sb.UserInstance);
		}

		[Test]
		public void OpenMaster()
		{
			using (SqlExpressUtil.OpenMaster())
			{
			}
		}

		[Test]
		public void CreateDatabase()
		{
			string dbFile = Path.GetTempFileName();
			string logFile = SqlExpressUtil.LogFilePath(dbFile);
			try
			{
				File.Delete(dbFile);
				Assert.IsFalse(File.Exists(dbFile));
				Assert.IsFalse(File.Exists(logFile));
				SqlExpressUtil.CreateDatabaseFile(dbFile);
				Assert.IsTrue(File.Exists(dbFile));
				Assert.IsTrue(File.Exists(logFile));
			}
			finally
			{
				File.Delete(dbFile);
				File.Delete(logFile);
			}
		}

		[Test]
		public void AttachAndDetach()
		{
			string cs = SqlExpressUtil.CreateConnectionString(_dbFile, _dbName);
			SqlExpressUtil.CreateDatabaseFile(_dbFile);

			using (SqlConnection conn = new SqlConnection(cs))
			{
				conn.Open();
			}

			SqlExpressUtil.DetachDatabase(_dbName);
		}
	}
}