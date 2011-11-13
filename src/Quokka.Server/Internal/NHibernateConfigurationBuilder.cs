using Castle.Core.Configuration;
using Castle.Facilities.NHibernateIntegration;
using Castle.Facilities.NHibernateIntegration.Builders;
using NHibernate.Cfg;

namespace Quokka.Server.Internal
{
	internal class NHibernateConfigurationBuilder : IConfigurationBuilder 
	{
		public Configuration GetConfiguration(IConfiguration config)
		{
			var builder = new DefaultConfigurationBuilder();
			var cfg = builder.GetConfiguration(config);

			//cfg.

//			var cfg = new NHConfiguration();
//			cfg.SetProperty(NHEnvironment.Dialect, typeof (MsSql2005Dialect).AssemblyQualifiedName);
//			cfg.SetProperty(NHEnvironment.ConnectionDriver, typeof (SqlClientDriver).AssemblyQualifiedName);
//			cfg.SetProperty(NHEnvironment.ConnectionProvider, typeof(DriverConnectionProvider).AssemblyQualifiedName);
//			cfg.SetProperty(NHEnvironment.GenerateStatistics, "true");
//			cfg.SetProperty(NHEnvironment.CurrentSessionContextClass, "thread_static");
//
//			foreach (var module in ModulesSectionHandler.Instance.Config.Modules)
//			{
//				cfg.AddAssembly(module.Assembly);
//			}

			return cfg;
		}
	}
}
