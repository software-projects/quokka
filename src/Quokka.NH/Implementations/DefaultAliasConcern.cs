using System;
using Castle.Core;
using Quokka.NH.Interfaces;

namespace Quokka.NH.Implementations
{
	/// <summary>
	/// Implements <see cref="ICommissionConcern"/> in order to populate the
	/// <see cref="IDefaultAlias.DefaultAlias"/> property on components that
	/// implement the <see cref="IDefaultAlias"/> interface.
	/// </summary>
	public class DefaultAliasConcern : ICommissionConcern
	{
		private readonly Func<string> _getDefaultAlias;

		public DefaultAliasConcern(Func<string> getDefaultAlias)
		{
			_getDefaultAlias = getDefaultAlias;
		}

		public void Apply(ComponentModel model, object component)
		{
			var defaultAlias = component as IDefaultAlias;
			if (defaultAlias != null && _getDefaultAlias != null)
			{
				defaultAlias.DefaultAlias = _getDefaultAlias();
			}
		}
	}
}