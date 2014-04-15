using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using Castle.MicroKernel.SubSystems.Conversion;
using Quokka.NH.Interfaces;

namespace Quokka.NH.Implementations
{
	public class DefaultAliasContributor : IContributeComponentModelConstruction
	{
		private readonly DefaultAliasConcern _concern;

		public DefaultAliasContributor(Func<string> getDefaultAliasCallback)
		{
			_concern = new DefaultAliasConcern(getDefaultAliasCallback);
		}

		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (model.Implementation.Is<IDefaultAlias>())
			{
				model.Lifecycle.Add(_concern);
			}
		}
	}
}
