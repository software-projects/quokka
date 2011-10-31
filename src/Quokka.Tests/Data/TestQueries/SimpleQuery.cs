namespace Quokka.Data.TestQueries
{
	public class SimpleQuery : SqlQuery<SimpleRecord>
	{
	}

	public class SimpleRecord
	{
		public int Id { get; set; }
		public string Comment { get; set; }
	}
}
