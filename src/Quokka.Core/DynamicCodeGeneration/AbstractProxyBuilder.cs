namespace Quokka.DynamicCodeGeneration
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Text;
	using Quokka.Diagnostics;

	public abstract class AbstractProxyBuilder
	{
		private readonly ModuleBuilder _moduleBuilder;
		private readonly Type _interfaceType;
		private readonly Type _innerType;
		private readonly string _className;
		private readonly TypeBuilder _typeBuilder;
		private readonly FieldBuilder _innerFieldBuilder;
		private readonly List<string> _errorMessages;
		private Type _proxyType;

		public AbstractProxyBuilder(ModuleBuilder moduleBuilder, string className, Type interfaceType, Type innerType)
		{
			Verify.ArgumentNotNull(moduleBuilder, "moduleBuilder");
			Verify.ArgumentNotNull(className, "className");
			Verify.ArgumentNotNull(interfaceType, "interfaceType");
			Verify.ArgumentNotNull(innerType, "innerType");

			if (!interfaceType.IsInterface) {
				throw new ArgumentException("must be an interface type", "interfaceType");
			}

			_errorMessages = new List<string>();
			_moduleBuilder = moduleBuilder;
			_className = className;
			_interfaceType = interfaceType;
			_innerType = innerType;

			_typeBuilder = _moduleBuilder.DefineType(
				_className,
				TypeAttributes.Public |
				TypeAttributes.Class |
				TypeAttributes.AutoClass |
				TypeAttributes.AnsiClass |
				TypeAttributes.BeforeFieldInit |
				TypeAttributes.AutoLayout,
				typeof(object),
				new Type[] {_interfaceType});

			_innerFieldBuilder = _typeBuilder.DefineField("inner", _innerType, FieldAttributes.Private);
		}

		public Type CreateType()
		{
			if (_proxyType != null) {
				return _proxyType;
			}

			if (!CanCreateType) {
				ThrowCannotCreateClassException();
			}

			DoCreateType();

			_proxyType = _typeBuilder.CreateType();
			return _proxyType;
		}

		public IList<string> ErrorMessages
		{
			get { return _errorMessages.AsReadOnly(); }
		}

		public bool CanCreateType
		{
			get { return _errorMessages.Count == 0; }
		}

		protected abstract void DoCreateType();

		protected void AddErrorMessage(string format, params object[] args)
		{
			string message = String.Format(format, args);
			_errorMessages.Add(message);
		}

		protected ModuleBuilder ModuleBuilder
		{
			get { return _moduleBuilder; }
		}

		protected Type InterfaceType
		{
			get { return _interfaceType; }
		}

		protected Type InnerType
		{
			get { return _innerType; }
		}

		protected string ClassName
		{
			get { return _className; }
		}

		protected TypeBuilder TypeBuilder
		{
			get { return _typeBuilder; }
		}

		protected FieldBuilder InnerFieldBuilder
		{
			get { return _innerFieldBuilder; }
		}

		protected Type ProxyType
		{
			get { return _proxyType; }
		}

		private void ThrowCannotCreateClassException()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Cannot create proxy class for {0}:", InterfaceType);
			sb.AppendLine();
			foreach (string errorMessage in _errorMessages) {
				sb.Append(" - ");
				sb.AppendLine(errorMessage);
			}
			throw new InvalidOperationException(sb.ToString());

		}
	}
}