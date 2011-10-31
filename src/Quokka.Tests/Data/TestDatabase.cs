using System;
using System.Data.SqlServerCe;
using System.IO;

namespace Quokka.Data
{
	public class TestDatabase : IDisposable
	{
		public string FilePath { get; set; }

		public string ConnectionString
		{
			get { return "Data Source=" + FilePath; }
		}

		public TestDatabase()
		{
			FilePath = Path.Combine(Path.GetTempPath(), "Quokka-Test-Database.sdb");
		}

		public void CreateEmpty()
		{
			File.Delete(FilePath);

			// create the database);
			using (SqlCeEngine engine = new SqlCeEngine(ConnectionString))
			{
				engine.CreateDatabase();
			}
		}

		public void Dispose()
		{
			File.Delete(FilePath);
		}
	}
}
