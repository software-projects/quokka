using System;

namespace Quokka.ServiceLocation
{
	public class SingletonAttribute : RegisterTypeAttribute
	{
		public SingletonAttribute(Type type) : base(type, ServiceLifecycle.Singleton)
		{
		}
	}
}