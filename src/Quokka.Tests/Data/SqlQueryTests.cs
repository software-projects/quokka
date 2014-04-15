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
using NUnit.Framework;
using Quokka.Data.TestQueries;

// ReSharper disable InconsistentNaming
namespace Quokka.Data
{
	public class SqlQueryTests
	{
		private TestDatabase _testDB;

		[SetUp]
		public void SetUp()
		{
			_testDB = new TestDatabase();
			_testDB.CreateEmpty();
		}

		[TearDown]
		public void TearDown()
		{
			if (_testDB != null)
			{
				_testDB.Dispose();
				_testDB = null;
			}
		}

		[Test]
		public void Simple_query()
		{
			using (var conn = new SqlCeConnection(_testDB.ConnectionString))
			{
				conn.Open();
				var cmd = conn.CreateCommand();
				
				cmd.CommandText = @"create table Table1(Id int not null, Comment nvarchar(30) null)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = @"insert into Table1(Id, Comment) values (1, 'Text1')";
				cmd.ExecuteNonQuery();
				cmd.CommandText = @"insert into Table1(Id, Comment) values (2, 'Text2')";
				cmd.ExecuteNonQuery();

				var query = new SimpleQuery {Command = cmd};
				var list = query.ExecuteList();

				Assert.AreEqual(2, list.Count);
				var item0 = list[0];
				var item1 = list[1];

				Assert.AreEqual(1, item0.Id);
				Assert.AreEqual("Text1", item0.Comment);
				Assert.AreEqual(2, item1.Id);
				Assert.AreEqual("Text2", item1.Comment);
			}
		}

		[Test]
		public void Missing_property()
		{
			using (var conn = new SqlCeConnection(_testDB.ConnectionString))
			{
				conn.Open();
				var cmd = conn.CreateCommand();

				cmd.CommandText = @"create table Table1(Id int not null, Comment nvarchar(30) null, AnotherColumn int null)";
				cmd.ExecuteNonQuery();

				var query = new TestQuery<TestRecord>
				            	{
				            		Command = cmd,
									CommandText = "select Id, Comment, AnotherColumn from Table1"
				            	};

				try
				{
					query.ExecuteList();
					Assert.Fail("Expected exception");
				}
				catch (InvalidOperationException ex)
				{
					string expectedMessage = "Cannot build converter for Quokka.Data.SqlQueryTests+TestRecord:"
					                               + Environment.NewLine
					                               +
					                               "Type Quokka.Data.SqlQueryTests+TestRecord does not have a property named 'AnotherColumn'"
												   + Environment.NewLine;
					Assert.AreEqual(expectedMessage, ex.Message);
				}

			}
		}

		public class TestRecord1
		{
			public int Id { get; set; }
			public int? NullableInt32 { get; set; }
			public byte Byte { get; set; }
			public byte? NullableByte { get; set; }
//			public char Char { get; set; }
//			public char? NullableChar { get; set; }
			public short Int16 { get; set; }
			public short? NullableInt16 { get; set; }
			public float Single { get; set; }
			public float? NullableSingle { get; set; }
//			public double Double { get; set; } // double not supported by sqlce
//			public double? NullableDouble { get; set; }
			public decimal Decimal { get; set; }
			public decimal? NullableDecimal { get; set; }
			public bool Boolean { get; set; }
			public bool? NullableBoolean { get; set; }
		}


		[Test]
		public void Query_with_many_columns()
		{
			using (var conn = new SqlCeConnection(_testDB.ConnectionString))
			{
				conn.Open();
				var cmd = conn.CreateCommand();
				cmd.CommandText =
					@"
create table Table1(
	Id int not null,
	NullableInt32 int null,
	Byte tinyint not null,
	NullableByte tinyint null,
--	[Char] nchar(1) not null,
--	NullableChar nchar(1) null,
	Int16 smallint not null,
	NullableInt16 smallint null,
	Single real not null,
	NullableSingle real null,
--	[Double] double precision not null, -- double precision 
--	NullableDouble double precision not null,
	[Decimal] decimal(10,2) not null,
	NullableDecimal decimal(10,4) null,
	Boolean bit not null,
	NullableBoolean bit null,


	primary key(Id)
)
";

				cmd.ExecuteNonQuery();

				var query = new TestQuery<TestRecord1> {Command = cmd, CommandText = "select * from Table1"};
				query.ExecuteList();
			}
		}

		[Test]
		public void Nested_property_query()
		{
			using (var conn = new SqlCeConnection(_testDB.ConnectionString))
			{
				conn.Open();
				var cmd = conn.CreateCommand();

				cmd.CommandText = @"create table Table1(Id int not null, Comment nvarchar(30) null)";
				cmd.ExecuteNonQuery();
				cmd.CommandText = @"insert into Table1(Id, Comment) values (1, 'Text1')";
				cmd.ExecuteNonQuery();
				cmd.CommandText = @"insert into Table1(Id, Comment) values (2, 'Text2')";
				cmd.ExecuteNonQuery();

				var query = new NestedPropertyQuery { Command = cmd };
				var list = query.ExecuteList();

				Assert.AreEqual(2, list.Count);
				var item0 = list[0];
				var item1 = list[1];

				Assert.AreEqual(1, item0.Id);
				Assert.AreEqual("Text1", item0.Nested.Nested.Comment);
				Assert.AreEqual(2, item1.Id);
				Assert.AreEqual("Text2", item1.Nested.Nested.Comment);
			}
		}

		public class TestQuery<T> : SqlQuery<T> where T : class, new()
		{
			public string CommandText { get; set; }

			protected override void SetCommandText(System.Data.IDbCommand cmd)
			{
				cmd.CommandText = CommandText;
			}
		}

		public class TestRecord
		{
			public int Id { get; set; }
			public string Comment { get; set; }
		}
	}
}
