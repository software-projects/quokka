using System;
using Quokka.Diagnostics;

namespace Quokka.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ParameterAttribute : Attribute
	{
		public string ParameterName { get; protected set; }
		public int ParameterOrder { get; protected set; }
		public bool Ignore { get; protected set; }

		public ParameterAttribute(string parameterName)
		{
			ParameterName = Verify.ArgumentNotNull(parameterName, "parameterName");
		}

		public ParameterAttribute(int parameterOrder)
		{
			ParameterOrder = parameterOrder;
		}

		protected ParameterAttribute() { }
	}
}