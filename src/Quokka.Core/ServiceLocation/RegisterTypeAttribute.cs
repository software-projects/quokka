using System;
using Quokka.Diagnostics;

namespace Quokka.ServiceLocation
{
	/// <summary>
	/// Attribute for conveniently registering services with the service container.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class RegisterTypeAttribute : Attribute
	{
		private readonly Type _type;
		private readonly ServiceLifecycle _lifecycle;

		public RegisterTypeAttribute(Type type, ServiceLifecycle lifecycle)
		{
			Verify.ArgumentNotNull(type, "type", out _type);
			_lifecycle = lifecycle;
		}

		public Type Type
		{
			get { return _type; }
		}

		public ServiceLifecycle Lifecycle
		{
			get { return _lifecycle; }
		}

		public string Name { get; set; }
	}
}