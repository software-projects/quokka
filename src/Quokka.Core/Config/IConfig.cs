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
		TimeSpan GetValue(TimeSpanParameter parameter);

		/// <summary>
		/// Can configuration parameter values be written via this interace.
		/// </summary>
		/// <remarks>
		/// If this property value is <c>false</c>, any of the methods for
		/// setting config parameter values will throw <see cref="NotSupportedException"/>.
		/// </remarks>
		bool CanWrite { get; }

		/// <summary>
		/// Register the parameter with the configuration store.
		/// </summary>
		/// <param name="parameter">
		/// Register the parameter with the configuration store. If the configuration
		/// parameter is not already registered it will be stored with its default
		/// value.
		/// </param>
		/// <exception cref="NotSupportedException">
		/// Writing is not supported.
		/// </exception>
		void Register(Parameter parameter);

		void SetValue(StringParameter parameter, string value);
		void SetValue(Int32Parameter parameter, int value);
		void SetValue(BooleanParameter parameter, bool value);
		void SetValue(UrlParameter parameter, Uri value);
		void SetValue(DateParameter parameter, DateTime value);
		void SetValue(TimeSpanParameter parameter, TimeSpan value);
	}
}
