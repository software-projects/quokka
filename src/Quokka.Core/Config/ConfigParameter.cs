using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Quokka.Castle;
using Quokka.Config.Storage;
using Quokka.Diagnostics;

namespace Quokka.Config
{
	public abstract class ConfigParameter : IConfigParameter
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

		string IConfigParameter.ValidateText(string proposedValue)
		{
			throw new NotImplementedException();
		}

		void IConfigParameter.SetValueText(string value)
		{
			throw new NotImplementedException();
		}

		string IConfigParameter.GetValueText()
		{
			throw new NotImplementedException();
		}

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

	public interface IConfigParameterBuilder<T>
	{
		IConfigParameterBuilder<T> Description(string description);
		IConfigParameterBuilder<T> DefaultValue(T defaultValue);
		IConfigParameterBuilder<T> DefaultValue(Func<T> callback);
		IConfigParameterBuilder<T> Validation(Func<T, string> callback);
		IConfigParameterBuilder<T> ChangeAction(Action callback);
	}

	public abstract class ConfigParameter<T, TParameter> : ConfigParameter, IConfigParameter<T>
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

		public TParameter With(Action<IConfigParameterBuilder<T>> callback)
		{
			if (callback != null)
			{
				callback(new Builder(this));
			}
			return (TParameter)this;
		}

		private class Builder : IConfigParameterBuilder<T>
		{
			private readonly ConfigParameter<T, TParameter> _outer;

			public Builder(ConfigParameter<T, TParameter> outer)
			{
				_outer = outer;
			}

			public IConfigParameterBuilder<T> Description(string description)
			{
				_outer.Description = (description ?? string.Empty).Trim();
				return this;
			}

			public IConfigParameterBuilder<T> DefaultValue(T defaultValue)
			{
				_outer._defaultValue = defaultValue;
				_outer._defaultValueSet = true;
				return this;
			}

			public IConfigParameterBuilder<T> DefaultValue(Func<T> callback)
			{
				_outer._defaultValueCallback = callback;
				return this;
			}

			public IConfigParameterBuilder<T> Validation(Func<T, string> callback)
			{
				_outer._validationCallback = callback;
				return this;
			}

			public IConfigParameterBuilder<T> ChangeAction(Action callback)
			{
				_outer._changedCallback = callback;
				return this;
			}
		}

		public static implicit operator T(ConfigParameter<T, TParameter> configParam)
		{
			return configParam.Value;
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

		string IConfigParameter.ValidateText(string proposedValue)
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

			return DoValidate(value);
		}

		void IConfigParameter.SetValueText(string textValue)
		{
			DoSetValue((T)ConvertFromString(textValue));
		}

		string IConfigParameter.GetValueText()
		{
			return ConvertToString(Value);
		}

		string IConfigParameter<T>.Validate(T value)
		{
			return DoValidate(value);
		}

		private string DoValidate(T value)
		{
			string result = null;
			if (_validationCallback != null)
			{
				result = _validationCallback(value);
			}
			return result;
		}

		void IConfigParameter<T>.SetValue(T value)
		{
			DoSetValue(value);
		}

		private void DoSetValue(T value)
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

		T IConfigParameter<T>.DefaultValue
		{
			get { return GetDefaultValue(); }
		}

		private T GetDefaultValue()
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

		/// <summary>
		/// Convert a string value into the type required for this configuration parameter.
		/// </summary>
		/// <exception cref="FormatException">
		/// Text is the wrong format for the configuration parameter type.
		/// </exception>
		protected abstract object ConvertFromString(string text);

		/// <summary>
		/// Convert the configuration value into a string suitable for persisting.
		/// </summary>
		protected abstract string ConvertToString(object value);
	}
}