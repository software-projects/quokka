using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Castle.Core.Logging;
using Castle.Facilities.AutoTx;
using Castle.Facilities.Logging;
using Castle.Facilities.NHibernateIntegration;
using Castle.Facilities.Startable;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Services.Transaction;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.Practices.ServiceLocation;
using Quokka.Server.Internal;
using Quokka.ServiceLocation;
using Quokka.Services;
using log4net;
using log4net.Config;

namespace Quokka.Server
{
	/// <summary>
	/// Worker processes create an instance of this class
	/// </summary>
	public class ServerHost
	{
		public bool IsRunning { get; private set; }
		public event EventHandler Stopped;
		public Form MainForm { get; private set; }
		public IWindsorContainer Container { get; private set; }
		private ILog Logger { get; set; }
		private StopManager StopManager { get; set; }
		private readonly object _lockObject = new object();
		private Mutex _programMutex;
		private string _programMutexName;

		/// <summary>
		/// Starts the server application. If the program is started
		/// from an interactive session, a Window displays showing information
		/// about the progress of the application.
		/// </summary>
		/// <param name="args"></param>
		public void Run(IEnumerable<string> args)
		{
			MainForm = new HostingForm {ServerHost = this};
			Application.Run(MainForm);
			ReleaseMutex();
		}

		internal void InternalRun()
		{
			// TODO: For now arguments are ignored, but one possibility is that the
			// argument could cause us to load a separate AppDomain and use a different
			// config file. Not sure.

			IsRunning = true;
			try
			{
				ConfigureLogging();
				Logger = LogManager.GetLogger(typeof(ServerHost));
				if (Environment.UserInteractive)
				{
					var form = (HostingForm) MainForm;
					form.ShowLog();
				}
				Logger.Info("Program starting");

				if (!CreateMutex())
				{
					Logger.Fatal("Another instance of this program is running");
					throw new AnotherInstanceRunningException();
				}

				CreateContainer();
				Logger.Debug("Created container");
				ConfigureQuokka();
				Logger.Debug("Configured quokka");
				ConfigureContainer();
				Logger.Debug("Configured container");

				StartComponents();

				Logger.Info("Program started");
			}
			catch
			{
				IsRunning = false;
				throw;
			}
		}

		internal void RequestStop()
		{
			StopManager.RequestStop();
		}

		/// <summary>
		/// Override this method to configure log4net logging.
		/// </summary>
		/// <remarks>
		/// The default implementation configures log4net from the XML configuration in the
		/// application configuration file. It also defines a variable <c>LogDir</c> in the
		/// log4net global context, which is a directory suitable for log files.
		/// </remarks>
		protected virtual void ConfigureLogging()
		{
			var logDir = Path.Combine(Application.CommonAppDataPath, "Log");
			Directory.CreateDirectory(logDir);
			GlobalContext.Properties["LogDir"] = logDir;
			XmlConfigurator.Configure();
		}

		private bool CreateMutex()
		{
			_programMutexName = GetMutexName();
			if (_programMutexName == null)
			{
				// no mutex required
				return true;
			}

			var mutex = new Mutex(false, _programMutexName);
			Logger.Debug("Created mutex " + _programMutexName);
			if (!mutex.WaitOne(100, false))
			{
				// another program is already running
				Logger.Debug("Failed to wait on mutex " + _programMutexName);
				return false;
			}

			_programMutex = mutex;
			return true;
		}

		protected virtual string GetMutexName()
		{
			// The prefix Global\ ensures that the mutex is in the global namespace, which means that there is only one
			// instance across all terminal server sessions for a machine.
			return @"Global\" + GetType().FullName + ".Exclusive.Mutex";
		}

		protected virtual void CreateContainer()
		{
			Container = new WindsorContainer();
#if DEBUG
			Container.Kernel.ComponentRegistered += delegate(string key, IHandler handler)
			                                        	{
															if (key == handler.ComponentModel.Implementation.ToString())
															{
																if (handler.Service == handler.ComponentModel.Implementation)
																{
																	Logger.DebugFormat("Registered {0} ({1})",
																	                   handler.Service,
																	                   handler.ComponentModel.LifestyleType);
																}
																else
																{
																	Logger.DebugFormat("Registered {0} => {1} ({2})",
																					   handler.Service,
																					   handler.ComponentModel.Implementation,
																					   handler.ComponentModel.LifestyleType);
																}
															}
															else
															{
																Logger.DebugFormat("Registered key={0}: {1} => {2} ({3})",
																                   key, handler.Service,
																                   handler.ComponentModel.Implementation,
																                   handler.ComponentModel.LifestyleType);
															}
			                                        	};
#endif
		}

		protected virtual void ConfigureQuokka()
		{
			// Next version of Quokka will not have a dependency on Microsoft.Practices.ServiceLocator
			var serviceContainer = new WindsorServiceContainer(Container);
			serviceContainer.RegisterInstance(Container);
			ServiceLocator.SetLocatorProvider(() => serviceContainer.Locator);
		}

		protected virtual void StartComponents()
		{
			foreach (var facility in Container.Kernel.GetFacilities())
			{
				var startableFacility = facility as CustomizedStartableFacility;
				if (startableFacility != null)
				{
					startableFacility.StartAll();
				}
			}
		}

		protected virtual void ConfigureContainer()
		{
			Container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.Log4net));

			var loggerFactory = Container.Resolve<ILoggerFactory>();
			var startableFacility = new CustomizedStartableFacility
			                        	{
											Logger = loggerFactory.Create(typeof (CustomizedStartableFacility))
										};
			Container.AddFacility(startableFacility.GetType().FullName, startableFacility);

			Container.Register(Component.For<IClock>().ImplementedBy<SystemClock>().LifeStyle.Singleton);
			Container.Register(Component.For<IDateTimeProvider>().ImplementedBy<DateTimeProvider>().LifeStyle.Singleton);
			Container.Register(Component.For<IGuidProvider>().ImplementedBy<GuidProvider>().LifeStyle.Singleton);

			Container.AddFacility<ConfigParamFacility>();
			Container.AddFacility<TransactionFacility>();

			// Explicitly registering the transaction manager avoids a spurious info message
			// in the event log by the nhibernate facility
			Container.Register(
				Component.For<ITransactionManager>()
					.ImplementedBy<DefaultTransactionManager>());

			Container.AddFacility("nhibernate.facility", new NHibernateFacility(new NHibernateConfigurationBuilder()));

			// Create the NHibernate facility.
#if CONFIGURE_NHIBERNATE_FACILITY_BY_CODE
			{
				// This code code is derived from this thread in the castle project users mailing list:
				// http://groups.google.com/group/castle-project-users/browse_thread/thread/2fcadf172ea099bf
				// The issue is that the current version of the NHibernate facility really wants to see XML
				// configuration, so you have to trick it a bit if you want to programatically configure it.
				var facilityConfiguration = new MutableConfiguration("facility");
				facilityConfiguration.Attribute("isWeb", "false").CreateChild("factory").Attribute("id", "nhibernate.factory");
				Container.Kernel.ConfigurationStore.AddFacilityConfiguration("nhibernate.facility", facilityConfiguration);

				var facility = new NHibernateFacility(new NHibernateConfigurationBuilder());
				Container.AddFacility("nhibernate.facility", facility);
			}
#endif

			// I've forgotten why I did this -- add one installer to a list and then turn it into an array.
			// Keeping it for now, to remind myself that this is possible.
			var installers = new List<IWindsorInstaller> {Configuration.FromAppConfig()};
			Container.Install(installers.ToArray());

			// Register the stop manager before processing the app.config -- this means that the implementation
			// cannot be overridden in the app.config. At the moment this is desirable, because we are depending
			// on implementation details of StopManager. When the StoppedWaitHandle is signaled, the StoppedCallback
			// will be invoked on a background thread.
			StopManager = new StopManager();
			Container.Register(Component.For<IStopManager>().Instance(StopManager));
			ThreadPool.RegisterWaitForSingleObject(StopManager.StoppedWaitHandle, StoppedCallback, null, -1, true);
		}

		private void StoppedCallback(object state, bool timedOut)
		{
			// During debugging I suspected that ThreadPool.RegisterWaitForSingleObject could call the callback more than
			// once. This turned out not to be the case, but it seems prudent to have a check here so that the container
			// is only disposed once.
			lock (_lockObject)
			{
				if (!IsRunning)
				{
					return;
				}
				IsRunning = false;
			}

			try
			{
				Container.Dispose();
			}
			catch (Exception ex)
			{
				Logger.Error("Error disposing of container: " + ex.Message, ex);
			}

			ReleaseMutex();
			Logger.Info("Program stopped");

			if (Stopped != null)
			{
				Stopped(this, EventArgs.Empty);
			}
		}

		private void ReleaseMutex()
		{
			if (_programMutex != null)
			{
				// Calling ReleaseMutex here caused an error.
				// Not sure why. Calling Dispose works nicely.
				_programMutex.Dispose();
				Logger.Debug("Released mutex " + _programMutexName);
				_programMutex = null;
				_programMutexName = null;
			}
		}
	}
}
