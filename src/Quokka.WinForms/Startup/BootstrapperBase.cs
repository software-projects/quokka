using System;
using System.Windows.Forms;
using Common.Logging;
using Microsoft.Practices.ServiceLocation;
using Quokka.ServiceLocation;
using Quokka.Unity;

namespace Quokka.WinForms.Startup
{
	/// <summary>
	/// Base class for application 'bootstrapper' classes
	/// </summary>
	public abstract class BootstrapperBase
	{
		private ILog _log;
		private IServiceContainer _container;
		private Form _shell;
		private string _progressMessage = String.Empty;

		/// <summary>
		/// Shell form created during <see cref="Run"/>
		/// </summary>
		public Form Shell
		{
			get { return _shell; }
		}

		/// <summary>
		/// Runs the bootstrapper, creating application artifacts in the correct order.
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

		public IServiceContainer Container
		{
			get { return _container; }
		}

		public IServiceLocator Locator
		{
			get { return _container == null ? null : _container.Locator; }
		}

		/// <summary>
		/// Called before all other initialization. Override to configure program logging.
		/// </summary>
		/// <remarks>
		/// All logging is via the Common.Logging package.
		/// </remarks>
		protected virtual void ConfigureLogging()
		{
			// TODO: some simple logging defaults to go here. Perhaps a rolling log file in application data.
		}

		/// <summary>
		/// Create a <see cref="IServiceContainer"/>.
		/// </summary>
		/// <returns>A service container.</returns>
		/// <remarks>
		/// <para>
		/// Override this method to create your own service container object. The default creates
		/// a simple container based on Unity (but this might change in future implementations).
		/// </para>
		/// <para>
		/// When this method is called, logging has been configured.
		/// </para>
		/// </remarks>
		protected virtual IServiceContainer CreateServiceContainer()
		{
			// TODO: default container should probably be the quokka simple container
			return ServiceContainerFactory.CreateContainer();
		}

		/// <summary>
		/// Configure services in the service container.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Override this method to configure services in the service container. The service container
		/// is available through the <see cref="Container"/> property.
		/// </para>
		/// <para>
		/// When this method is caled, logging has been configured and the service container has
		/// been created.
		/// </para>
		/// </remarks>
		protected virtual void ConfigureServices()
		{
		}

		protected abstract Form CreateShell();

		private void DoRun()
		{
			ConfigureLogging();
			_log = LogManager.GetLogger(typeof (BootstrapperBase));
			ProgressMessage("Program started");
			ProgressMessage("Creating service container");
			_container = CreateServiceContainer();
			ProgressMessage("Service container created");
			Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(() => _container.Locator);
			ProgressMessage("Configuring service container");
			ConfigureServices();
			ProgressMessage("Service container configured");
			ProgressMessage("Creating application shell form");
			Form form = CreateShell();
			ProgressMessage("Application shell form created");
			_shell = form;
		}

		private void ProgressMessage(string message)
		{
			if (_log != null)
			{
				_log.Debug(message);
			}
			_progressMessage = message;
		}
	}
}