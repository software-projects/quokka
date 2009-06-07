using System;

namespace Quokka.ServiceLocation
{
	public class PerRequestAttribute : RegisterTypeAttribute
	{
		public PerRequestAttribute(Type type) : base(type, ServiceLifecycle.PerRequest)
		{
		}
	}
}