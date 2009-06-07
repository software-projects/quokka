using System;
using System.Collections.Generic;
using System.Reflection;
using Common.Logging;
using Microsoft.Practices.ServiceLocation;

namespace Quokka.ServiceLocation
{
	/// <summary>
	/// Base class for implementation of an <see cref="IServiceContainer"/>.
	/// </summary>
	public abstract class ServiceContainer : IServiceContainer
	{
		private bool _disposed;
		private bool _disposing;
		private IServiceLocator _locator;
		private readonly HashSet<Assembly> _registeredAssemblies = new HashSet<Assembly>();
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();

		public void Dispose()
		{
			// Ignore second and subsequent calls to this method
			if (!_disposed)
			{
				// When we call Dispose on the container, it is going to cause recursive calls to 
				// this method, so we need to detect and ignore them.
				if (!_disposing)
				{
					try
					{
						_disposing = true;
						DoDispose();
						_disposed = true;
					}
					finally
					{
						_disposing = false;
					}
				}
			}
		}

		public IServiceLocator Locator
		{
			get
			{
				ThrowIfDisposed();
				if (_locator == null)
				{
					// The first time we get the locator, register the interfaces with the container.
					// It is not possible to do this in the constructor, as the container is not
					// necessarily available in the constructor.
					this.RegisterInstance<IServiceContainer>(this);
					this.RegisterInstance(this);

					_locator = GetServiceLocator();
				}
				return _locator;
			}
		}

		public IServiceContainer CreateChildContainer()
		{
			ThrowIfDisposed();
			return DoCreateChildContainer();
		}

		public IServiceContainer RegisterType(Type from, Type to, string name, ServiceLifecycle lifecycle)
		{
			ThrowIfDisposed();
			DoRegisterType(from, to, name, lifecycle);
			return this;
		}

		public IServiceContainer RegisterInstance(Type type, string name, object instance)
		{
			ThrowIfDisposed();
			DoRegisterInstance(type, name, instance);
			return this;
		}

		public bool IsTypeRegistered(Type type, string name)
		{
			if (type == null)
			{
				return false;
			}

			return DoIsTypeRegistered(type, name);
		}

		public ServiceContainer RegisterTypesInAssembly(Assembly assembly)
		{
			if (assembly == null)
			{
				return this;
			}
			if (_registeredAssemblies.Contains(assembly))
			{
				// It is really easy to accidentally register the same assembly twice. This can happen
				// if you register based on a type in the assembly (eg RegisterTypesInAssembly(typeof(Widget).Assembly)),
				// and then you merge two or more assemblies into one.
				Log.Warn("Attempt to register types in the same assembly twice: " + assembly.FullName);
				return this;
			}

			// Could do this using Linq I know. This way is slightly quicker and not that much harder to understand.
			foreach (Type type in assembly.GetTypes())
			{
				if (!type.IsClass)
				{
					// only interested in the classes
					continue;
				}

				foreach (RegisterTypeAttribute attribute in type.GetCustomAttributes(typeof(RegisterTypeAttribute), false))
				{
					Type fromType = attribute.Type;
					Type toType = type;
					string name = attribute.Name;
					ServiceLifecycle lifecycle = attribute.Lifecycle;
					try
					{
						RegisterType(fromType, toType, name, lifecycle);
					}
					catch (Exception ex)
					{
						string message = String.Format("Cannot register type {0} from attribute: {1}", type.FullName, ex.Message);
						Log.Error(message, ex);
						throw;
					}
				}
			}

			return this;
		}

		protected abstract IServiceLocator GetServiceLocator();
		protected abstract void DoRegisterType(Type from, Type to, string name, ServiceLifecycle lifecycle);
		protected abstract void DoRegisterInstance(Type from, string name, object instance);
		protected abstract IServiceContainer DoCreateChildContainer();
		protected abstract void DoDispose();
		protected abstract bool DoIsTypeRegistered(Type type, string name);

		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("Service container has been disposed");
			}
		}
	}
}