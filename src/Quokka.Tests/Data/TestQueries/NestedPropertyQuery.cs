using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quokka.Data.TestQueries
{
	public class NestedPropertyQuery : SqlQuery<NestedPropertyRecord>
	{
	}

	public class NestedPropertyRecord
	{
		public int Id { get; set; }
		public NestedPropertyType1 Nested { get; private set; }

		public NestedPropertyRecord()
		{
			Nested = new NestedPropertyType1();
		}
	}

	public class NestedPropertyType1
	{
		public NestedPropertyType2 Nested { get; private set; }

		public NestedPropertyType1()
		{
			Nested = new NestedPropertyType2();
		}
	}

	public class NestedPropertyType2
	{
		public string Comment { get; set; }
	}
}
