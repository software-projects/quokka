#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#endregion

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