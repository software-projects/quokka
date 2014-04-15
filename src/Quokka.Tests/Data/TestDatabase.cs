#region License

// Copyright 2004-2014 John Jeffery
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
