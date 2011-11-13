using System;

namespace Quokka.Config
{
	public interface IConfig
	{
		string GetValue(StringParameter parameter);
		int GetValue(Int32Parameter parameter);
		bool GetValue(BooleanParameter parameter);
		Uri GetValue(UrlParameter parameter);
		DateTime GetValue(DateParameter parameter);
	}

	/// <summary>
	/// Interface for updating configuration parameters
	/// </summary>
	public interface ISetConfig
	{
		/// <summary>
		/// Register the parameter with the configuration store.
		/// </summary>
		/// <param name="parameter">
		/// Register the parameter with the configuration store. If the configuration
		/// parameter is not already registered it will be stored with its default
		/// value.
		/// </param>
		void Register(Parameter parameter);

		/// <summary>
		/// Clear the configuration cache
		/// </summary>
		/// <param name="parameter">
		/// The individual parameter to clear the cache for, or specify <c>null</c>
		/// to clear all parameters.
		/// </param>
		void ClearCache(Parameter parameter = null);

		void SetValue(StringParameter parameter, string value);
		void SetValue(Int32Parameter parameter, int value);
		void SetValue(BooleanParameter parameter, bool value);
		void SetValue(UrlParameter parameter, Uri value);
		void SetValue(DateParameter parameter, DateTime value);
	}
}
