using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Quokka.Castle;
using Quokka.Config.Storage;
using Quokka.Diagnostics;

namespace Quokka.Config
{
	public abstract class ConfigParameter
	{
		/// <summary>
		/// The name of the parameter.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// String representation of the parameter type (eg "String", "Int32", "Boolean").
		/// </summary>
		public string ParameterType { get; private set; }

		/// <summary>
		/// Brief description of the parameter.
		/// </summary>
		public string Description { get; protected set; }

		/// <summary>
		/// Convert a string value into the type required for this configuration parameter.
		/// </summary>
		/// <exception cref="FormatException">
		/// Text is the wrong format for the configuration parameter type.
		/// </exception>
		public abstract object ConvertFromString(string text);

		/// <summary>
		/// Convert the configuration value into a string suitable for persisting.
		/// </summary>
		public abstract string ConvertToString(object value);

		/// <summary>
		/// Perform validation on a proposed string value.
		/// </summary>
		/// <returns>Returns <c>null</c> if value is valid for this parameter type, an error message otherwise.</returns>
		public abstract string ValidateText(string proposedValue);

		/// <summary>
		/// Update the value of the parameter.
		/// </summary>
		/// <param name="value">
		/// New value. This should be a valid text value for this type of parameter, otherwise an exception will be thrown.
		/// Call <see cref="ValidateText"/> before calling this function, as this function does not perform any validation.
		/// </param>
		public abstract void SetValueText(string value);

		/// <summary>
		/// Gets the value of this parameter as a text string.
		/// </summary>
		public abstract string GetValueText();

		protected ConfigParameter(string paramName, string paramType)
		{
			Name = Verify.ArgumentNotNull(paramName, "paramName");
			ParameterType = Verify.ArgumentNotNull(paramType, "paramType");
			Description = string.Empty;
		}

		/// <summary>
		/// The persistant storage used for configuration values.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This value is initially set to a memory only storage. This is useful for unit testing.
		/// </para>
		/// <para>
		/// If the <see cref="ConfigFacility"/> is registered with the Castle Windsor container,
		/// then this storage will be automatically set to the component registered with the container.
		/// </para>
		/// </remarks>
		public static IConfigStorage Storage = new MemoryStorage();

		/// <summary>
		/// Find all <see cref="ConfigParameter"/> parameters defined in an assembly.
		/// </summary>
		/// <param name="assembly">Assembly to look for config parameters.</param>
		/// <returns>List of <see cref="ConfigParameter"/></returns>
		/// <remarks>
		/// A config parameter should be defined as a static, readonly field.
		/// </remarks>
		public static IList<ConfigParameter> Find(Assembly assembly)
		{
			var fields = from t in assembly.GetTypes()
						 from f in t.GetFields()
						 where f.IsStatic
						 where typeof(ConfigParameter).IsAssignableFrom(f.FieldType)
						 select f;

			List<string> warnings = null;
			var configParams = new List<ConfigParameter>();

			foreach (var field in fields)
			{
				var parameter = field.GetValue(null) as ConfigParameter;
				if (parameter == null)
				{
					if (warnings == null)
					{
						warnings = new List<string>();
					}
					warnings.Add(string.Format("Ignoring parameter defined in {0}.{1} because its value is null", field.DeclaringType.FullName,
									  field.Name));
					continue;
				}

				if (!field.IsInitOnly)
				{
					if (warnings == null)
					{
						warnings = new List<string>();
					}
					warnings.Add(string.Format("Ignoring parameter {0} defined in {1}.{2} because it is not defined as readonly",
									  parameter.Name,
									  field.DeclaringType.FullName,
									  field.Name));
					continue;
				}

				configParams.Add(parameter);
			}
			
			if (warnings != null)
			{
				var logger = LoggerFactory.GetCurrentClassLogger();
				foreach (var warning in warnings)
				{
					logger.Warn(warning);
				}
			}

			return configParams;
		}
	}

	public abstract class ConfigParameter<T, TParameter> : ConfigParameter
		where TParameter : ConfigParameter<T, TParameter>
	{
		private T _defaultValue;
		private bool _defaultValueSet;
		private Func<T> _defaultValueCallback;
		private Func<T, string> _validationCallback;
		private Action _changedCallback;

		protected ConfigParameter(string paramName, string paramType) : base(paramName, paramType)
		{
		}

		public TParameter WithDescription(string description)
		{
			Description = (description ?? string.Empty).Trim();
			return (TParameter)this;
		}

		public TParameter WithDefaultValue(T defaultValue)
		{
			_defaultValue = defaultValue;
			_defaultValueSet = true;
			return (TParameter)this;
		}

		public TParameter WithDefaultValue(Func<T> callback)
		{
			_defaultValueCallback = callback;
			return (TParameter)this;
		}

		public TParameter WithValidation(Func<T, string> callback)
		{
			_validationCallback = callback;
			return (TParameter)this;
		}

		public TParameter WhenChanged(Action callback)
		{
			_changedCallback = callback;
			return (TParameter)this;
		}

		public T Value
		{
			get
			{
				var configValue = Storage.GetValue(this);
				if (configValue.HasValue)
				{
					return (T) ConvertFromString(configValue.Value);
				}
				return GetDefaultValue();
			}
		}

		public override string ValidateText(string proposedValue)
		{
			T value;
			try
			{
				value = (T)ConvertFromString(proposedValue);
			}
			catch (Exception)
			{
				return string.Format("Not a valid {0} value.", ParameterType);
			}

			return Validate(value);
		}

		public override void SetValueText(string textValue)
		{
			SetValue((T)ConvertFromString(textValue));
		}

		public override string GetValueText()
		{
			return ConvertToString(Value);
		}

		/// <summary>
		/// Check if the proposed value is valid for this configuration parameter.
		/// </summary>
		/// <param name="value">Proposed value</param>
		/// <returns>Returns <c>null</c> if the value is valid, otherwise returns an error message.</returns>
		public string Validate(T value)
		{
			string result = null;
			if (_validationCallback != null)
			{
				result = _validationCallback(value);
			}
			return result;
		}

		/// <summary>
		/// Saves the new value. No validation is performed, it is assumed that the
		/// caller has called <see cref="Validate"/> prior to calling this method.
		/// </summary>
		/// <param name="value"></param>
		public void SetValue(T value)
		{
			var newValue = ConvertToString(value);
			var oldValue = Storage.GetValue(this);
			if (oldValue.HasValue && oldValue.Value == newValue)
			{
				// no change, so do nothing
				return;
			}
			Storage.SetValue(this, ConvertToString(value));
			if (_changedCallback != null)
			{
				_changedCallback();
			}
		}

		/// <summary>
		/// Gets the default value for this parameter.
		/// </summary>
		/// <remarks>
		/// This is a method to indicate that there may be non-trivial
		/// work in determining the default value.
		/// </remarks>
		/// <returns></returns>
		public T GetDefaultValue()
		{
			if (!_defaultValueSet)
			{
				if (_defaultValueCallback != null)
				{
					_defaultValue = _defaultValueCallback();
					_defaultValueSet = true;
				}
			}
			return _defaultValue;
		}
	}
}