using NHibernate.Cfg;

namespace Quokka.NH.Interfaces
{
	public interface IConfigurationResolver : IDefaultAlias
	{
		/// <summary>
		/// Determines whether a configuration exists, or can be created, for the
		/// specified alias.
		/// </summary>
		/// <param name="alias">Alias of the configuration (which can be <c>null</c>).</param>
		/// <returns></returns>
		bool IsAliasDefined(string alias);

		/// <summary>
		/// Returns the <see cref="Configuration"/> associated with the specified alias (which can
		/// be <c>null</c>). If the configuration has not already been created, it will be created,
		/// which can be a lengthy operation.
		/// </summary>
		/// <param name="alias">Alias of the configuration (which can be <c>null</c>).</param>
		Configuration GetConfiguration(string alias);
	}
}