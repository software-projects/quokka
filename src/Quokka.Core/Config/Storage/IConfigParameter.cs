using System;

namespace Quokka.Config.Storage
{
	/// <summary>
	/// Interface which provides all information required to persist a configuration
	/// parameter.
	/// </summary>
	public interface IConfigParameter
	{
		string Name { get; }
		string ParameterType { get; }
		string Description { get; }

		/// <summary>
		/// Perform validation on a proposed string value.
		/// </summary>
		/// <returns>
		/// Returns <c>null</c> if value is valid for this parameter type, an error message otherwise.
		/// </returns>
		string ValidateText(string proposedValue);

		/// <summary>
		/// Update the value of the parameter.
		/// </summary>
		/// <param name="value">
		/// New value. This should be a valid text value for this type of parameter, otherwise an exception will be thrown.
		/// Call <see cref="ValidateText"/> before calling this function, as this function does not perform any validation.
		/// </param>
		void SetValueText(string value);

		/// <summary>
		/// Gets the value of this parameter as a text string.
		/// </summary>
		string GetValueText();
	}

	public interface IConfigParameter<T> : IConfigParameter
	{
		/// <summary>
		/// The current value for this parameter.
		/// </summary>
		T Value { get; }

		/// <summary>
		/// The default value for this parameter.
		/// </summary>
		T DefaultValue { get; }

		/// <summary>
		/// Saves the new value. No validation is performed, it is assumed that the
		/// caller has called <see cref="Validate"/> prior to calling this method.
		/// </summary>
		/// <param name="value"></param>
		void SetValue(T value);

		/// <summary>
		/// Check if the proposed value is valid for this configuration parameter.
		/// </summary>
		/// <param name="value">Proposed value</param>
		/// <returns>Returns <c>null</c> if the value is valid, otherwise returns an error message.</returns>
		string Validate(T value);
	}
}