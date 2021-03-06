﻿#region License

// Copyright 2004-2012 John Jeffery
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
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Quokka.NH.Tests.Model
{
	public class BlogItem
	{
		public virtual Blog ParentBlog { get; set; }

		public virtual DateTime ItemDate { get; set; }

		public virtual int Id { get; set; }

		public virtual String Text { get; set; }

		public virtual String Title { get; set; }
	}

	namespace Mappers
	{
		public class BlogItemMapping : ClassMapping<BlogItem>
		{
			public BlogItemMapping()
			{
				Table("BlogItems");
				Id(x => x.Id, m => m.Generator(Generators.Native));
				Property(x => x.ItemDate);
				Property(x => x.Text);
				Property(x => x.Title);
				ManyToOne(x => x.ParentBlog, m =>
				                             	{
				                             		m.Column("BlogId");
													m.ForeignKey("FK_BlogItems_BlogId");
													m.Index("IX_BlogItems_BlogId");
				                             	});
			}
		}
	}

}
