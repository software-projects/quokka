using System;

namespace Quokka.NH.Interfaces
{
	/// <summary>
	/// Interface supported by objects that want to know the default alias for opening sessions.
	/// </summary>
	/// <remarks>
	/// The NHibernate integration facility will automatically populate this property for any
	/// component that implements this interface.
	/// </remarks>
	public interface IDefaultAlias
	{
		string DefaultAlias { get; set; }
	}
}