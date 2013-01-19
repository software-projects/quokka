using System;

namespace Quokka.Config.Storage
{
	/// <summary>
	/// Interface for persistant storage of configuration items.
	/// </summary>
	public interface IConfigStorage
	{
		/// <summary>
		/// Get a single parameter value.
		/// </summary>
		ConfigValue GetValue(ConfigParameter parameter);

		/// <summary>
		/// Get a list of parameter values. This method assumes that it is more
		/// efficient to retrieve a large number of values in one operation as
		/// opposed to a large number of calls to <see cref="GetValue"/>.
		/// </summary>
		/// <param name="parameters">List of <see cref="ConfigParameter"/></param>
		/// <returns></returns>
		ConfigValue[] GetValues(params ConfigParameter[] parameters);

		/// <summary>
		/// Indicates whether the persistant storage is read only. If this property
		/// has a value of <c>true</c>, then any call to <see cref="SetValue"/> will
		/// throw a <see cref="InvalidOperationException"/> exception.
		/// </summary>
		bool IsReadOnly { get; }

		/// <summary>
		/// Set a single parameter value.
		/// </summary>
		/// <param name="parameter">The <see cref="ConfigParameter"/> assocated with the value.</param>
		/// <param name="value">The new value for the associated <see cref="ConfigParameter"/></param>
		/// <exception cref="InvalidOperationException">
		/// The configuration storage is read only.
		/// </exception>
		void SetValue(ConfigParameter parameter, string value);

		/// <summary>
		/// Call when the configuration parameters have been modified by an external source.
		/// This will clear any cached information.
		/// </summary>
		void Refresh();
	}
}