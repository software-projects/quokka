using System;
using System.Threading;
using System.Windows.Forms;
using Castle.Core.Logging;
using Castle.Windsor;
using Quokka.Castle;
using Quokka.Diagnostics;
using Quokka.Events;
using Quokka.Events.Internal;
using Quokka.ServiceLocation;
using Quokka.Services;
using Quokka.Threading;
using Quokka.UI.Messages;
using Quokka.UI.Regions;
using Quokka.UI.Tasks;
using Quokka.WinForms.Regions;

namespace Quokka.WinForms.Startup
{
	/// <summary>
	/// 	Base class for application 'bootstrapper' classes
	/// </summary>
	public abstract class BootstrapperBase
	{
		protected ILogger Logger { get; set; }
		private ILogger _log;
		private IServiceContainer _serviceContainer;
		private IWindsorContainer _windsorContainer;
		private Form _shell;
		private string _progressMessage = String.Empty;

		protected BootstrapperBase()
		{
			Logger = NullLogger.Instance;
		}

		/// <summary>
		/// 	Shell form created during <see cref = "Run" />
		/// </summary>
		public Form Shell
		{
			get { return _shell; }
		}

		/// <summary>
		/// 	Runs the bootstrapper, creating application artifacts in the correct order.
		/// </summary>
		public Form Run()
		{
			try
			{
				DoRun();
			}
			catch (OutOfMemoryException)
			{
				throw;
			}
			catch (Exception ex)
			{
				string message = "Error during startup: " + _progressMessage + ": " + ex.Message;
				if (_log != null)
				{
					_log.Error(message, ex);
				}
				throw new StartupException(message, ex);
			}

			return Shell;
		}

		public IWindsorContainer Container
		{
			get { return _windsorContainer; }
		}

		public IServiceContainer ServiceContainer
		{
			get { return _serviceContainer; }
		}

		public IServiceLocator Locator
		{
			get { return _serviceContainer == null ? null : _serviceContainer.Locator; }
		}

		/// <summary>
		/// 	Called before all other initialization. Override to configure program logging.
		/// </summary>
		/// <remarks>
		/// 	All logging is via the Common.Logging package.
		/// </remarks>
		protected virtual void ConfigureLogging()
		{
		}

		/// <summary>
		/// 	Create a <see cref = "IServiceContainer" />.
		/// </summary>
		/// <returns>A service container.</returns>
		/// <remarks>
		/// 	<para>
		/// 		Override this method to create your own service container object. The default creates
		/// 		a simple container based on Unity (but this might change in future implementations).
		/// 	</para>
		/// 	<para>
		/// 		When this method is called, logging has been configured.
		/// 	</para>
		/// </remarks>
		private IServiceContainer CreateServiceContainer()
		{
			_windsorContainer = CreateContainer(false);

			// Set up logging if it has not already been done.
			// This is because the Windsor Container might have a logging facility defined.
			if (!LoggerFactory.IsConfigured && _windsorContainer.Kernel.HasComponent(typeof (ILoggerFactory)))
			{
				LoggerFactory.SetLoggerFactory(_windsorContainer.Resolve<ILoggerFactory>());
			}

			var serviceContainer = new WindsorServiceContainer(_windsorContainer, CreateChildContainer);
			return serviceContainer;
		}

		private IWindsorContainer CreateChildContainer()
		{
			return CreateContainer(true);
		}

		/// <summary>
		///	Create a <see cref="IWindsorContainer" />. This method gets called at program startup,
		/// and whenever Quokka needs to create a child container.
		/// </summary>
		/// <param name="isChildContainer">
		/// Indicates whether the container is going to be used as a child container. In some cases
		/// a child container may have a slightly different configuration to a parent container.
		/// </param>
		/// <returns>A Castle Windsor container.</returns>
		protected abstract IWindsorContainer CreateContainer(bool isChildContainer);

		/// <summary>
		/// 	Configure services in the service container.
		/// </summary>
		/// <remarks>
		/// 	<para>
		/// 		Override this method to configure services in the container. The container
		/// 		is available through the <see cref="Container" /> property.
		/// 	</para>
		/// 	<para>
		/// 		When this method is caled, logging has been configured and the container has
		/// 		been created.
		/// 	</para>
		/// </remarks>
		protected virtual void ConfigureServices()
		{
		}

		protected abstract Form CreateShell();

		private void DoRun()
		{
			ConfigureLogging();

			ProgressMessage("Program started");
			ProgressMessage("Creating service container");
			_serviceContainer = CreateServiceContainer();
			ProgressMessage("Service container created");
			ServiceLocator.SetLocatorProvider(() => _serviceContainer.Locator);
			ProgressMessage("Configuring service container");
			ConfigureServices();
			ProgressMessage("Service container configured");
			ConfigureDefaultServices();
			ProgressMessage("Creating application shell form");
			Form form = CreateShell();
			ProgressMessage("Application shell form created");
			_shell = form;
		}

		/// <summary>
		/// 	Configures default services provided by Quokka.
		/// </summary>
		/// <remarks>
		/// 	Normally you do not have to override this method. Only override if
		/// 	you especially do not want these services available to your program.
		/// </remarks>
		protected virtual void ConfigureDefaultServices()
		{
			if (!ServiceContainer.IsTypeRegistered<IEventBroker>())
			{
				ServiceContainer.RegisterType<IEventBroker, EventBrokerImpl>(ServiceLifecycle.Singleton);
			}

			if (!ServiceContainer.IsTypeRegistered<IDateTimeProvider>())
			{
				ServiceContainer.RegisterType<IDateTimeProvider, DateTimeProvider>(ServiceLifecycle.Singleton);
			}

			if (!ServiceContainer.IsTypeRegistered<IGuidProvider>())
			{
				ServiceContainer.RegisterType<IGuidProvider, GuidProvider>(ServiceLifecycle.Singleton);
			}

			if (!ServiceContainer.IsTypeRegistered<IRegionManager>())
			{
				ServiceContainer.RegisterType<IRegionManager, RegionManager>(ServiceLifecycle.Singleton);
			}

			if (!ServiceContainer.IsTypeRegistered<SynchronizationContext>())
			{
				if (SynchronizationContext.Current == null)
				{
					SynchronizationContext ctx = new WindowsFormsSynchronizationContext();
					ServiceContainer.RegisterInstance(ctx);
				}
				else
				{
					ServiceContainer.RegisterInstance(SynchronizationContext.Current);
				}
			}

			if (!ServiceContainer.IsTypeRegistered<IModalWindowFactory>())
			{
				ServiceContainer.RegisterType<IModalWindowFactory, ModalWindowFactory>(ServiceLifecycle.Singleton);
			}

			if (!ServiceContainer.IsTypeRegistered<IUIMessageBoxView>())
			{
				ServiceContainer.RegisterType<IUIMessageBoxView, MessageBoxView>(ServiceLifecycle.PerRequest);
			}

			if (!ServiceContainer.IsTypeRegistered<Worker>())
			{
				ServiceContainer.RegisterType<Worker>(ServiceLifecycle.PerRequest);
			}

			if (!ServiceContainer.IsTypeRegistered<IErrorReportView>())
			{
				ServiceContainer.RegisterType<IErrorReportView, ErrorReportView>(ServiceLifecycle.PerRequest);
			}
		}

		private void ProgressMessage(string message)
		{
			// Create the logger for this (base) class to use
			if (_log == null && LoggerFactory.IsConfigured)
			{
				_log = LoggerFactory.GetLogger(typeof (BootstrapperBase));

				// Create the logger for the derived class code to use
				if (Logger == NullLogger.Instance)
				{
					Logger = LoggerFactory.GetLogger(GetType());	
				}
			}
			if (_log != null)
			{
				_log.Debug(message);
			}
			_progressMessage = message;
		}
	}
}