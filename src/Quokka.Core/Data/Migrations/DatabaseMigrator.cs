using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Common.Logging;
using Quokka.Diagnostics;

namespace Quokka.Data.Migrations
{
	/// <summary>
	/// Performs database migrations.
	/// </summary>
	/// <remarks>
	/// Database migrations are embedded resources whose resource name
	/// matches the regex @"\.\d{12}_[a-zA-Z0-9_]\.sql$"
	/// </remarks>
	public class DatabaseMigrator
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Name of the database migration table. Defaults to "DatabaseMigrations".
		/// </summary>
		/// <remarks>
		/// Columns:
		/// <list type="table">
		/// <listheader>
		/// <item><term>Column</term><description>Description</description></item>
		/// </listheader>
		/// <item><term>Version</term><description>string not null unique (version of the migration must be unique)</description></item>
		/// <item><term>Module</term><description>nvarchar(64) (assembly that the migration came from)</description></item>
		/// <item><term>Description</term><description>nvarchar(64) (description of the migration)</description></item>
		/// <item><term>CreatedAt</term><description>When the migration was applied to the database</description></item>
		/// </list>
		/// </remarks>
		public string DatabaseMigrationTableName { get; set; }

		public DbConnection Connection { get; set; }
		public ICollection<Assembly> Assemblies { get; private set; }

		public DatabaseMigrator()
		{
			Assemblies = new HashSet<Assembly>();
			DatabaseMigrationTableName = "DatabaseMigrations";
		}

		// Regex that describes a valid schema migration resource name
		private static readonly Regex ResourceNameRegex = new Regex(@"\.(\d{12})_(\w+)\.sql$");

		// Regex that describes a resource name that is probably a mis-typed schema migration resource name.
		private static readonly Regex MistypedResourceNameRegex = new Regex(@"\.\d[^_-]*[_-].*\.sql$");

		public void PerformMigrations()
		{
			if (Connection == null)
			{
				throw new InvalidOperationException("Connection property has not been specified");
			}

			if (Assemblies.Count == 0)
			{
				// nothing to do
			}

			CheckForSchemaMigrationsTable();
			foreach (var schemaMigration in FindPendingMigrations())
			{
				try
				{
					using (var tx = Connection.BeginTransaction(IsolationLevel.RepeatableRead))
					{
						Log.Info("Applying schema migration: " + schemaMigration.ResourceName);
						PerformMigration(schemaMigration);
						tx.Commit();
						Log.Info("Schema migration successful: " + schemaMigration.ResourceName);
					}
				}
				catch (Exception ex)
				{
					Log.Error("Schema migration failed: " + schemaMigration.ResourceName, ex);
					throw;
				}
			}
		}

		public IEnumerable<DatabaseMigration> GetEmbeddedMigrations(Assembly migrationAssembly)
		{
			// Migration resources in the assembly
			var migrations = (from name in GetSchemaMigrationResourceNames(migrationAssembly)
							  select new DatabaseMigrationResource(migrationAssembly, name).Migration).ToList();
			migrations.Sort((m1, m2) => StringComparer.InvariantCultureIgnoreCase.Compare(m1.Version, m2.Version));

			// Check for duplicates
			for (int index = 1; index < migrations.Count; ++index)
			{
				var migration = migrations[index];
				var previousMigration = migrations[index - 1];
				if (migration.Version == previousMigration.Version)
				{
					string message = "More than one migration has version " + migration.Version;
					throw new QuokkaException(message);
				}
			}
			return migrations;
		}

		private void CheckForSchemaMigrationsTable()
		{
			try
			{
				using (var tx = Connection.BeginTransaction())
				{
					var cmd = Connection.CreateCommand();

					// TODO: this is SQL Server specific
					cmd.CommandText = string.Format("if not exists(select * from information_schema.tables where table_name = '{0}')"
					                                + " begin"
					                                + " create table DataMigrations("
					                                + " Version nvarchar(14) not null,"
					                                + " CreatedAt datetime not null,"
					                                + " Description nvarchar(255) null,"
					                                + " primary key(Version))"
					                                + " end", DatabaseMigrationTableName);

					cmd.ExecuteNonQuery();
					tx.Commit();
				}
			}
			catch (Exception ex)
			{
				var message = string.Format("Failed during check for the {0} table: {1}", DatabaseMigrationTableName, ex.Message);
				Log.Error(message, ex);
				throw new QuokkaException(message, ex);
			}
		}

		private void PerformMigration(DatabaseMigrationResource migrationResource)
		{
			// First save the migration entry in the database. By performing this first we will ensure
			// that no other process or thread is trying to apply the same schema migration at the same
			// time, as one of them will get a duplicate key exception.
			migrationResource.Migration.CreatedAt = DateTime.Now;
			var sql = string.Format("insert into {0}(Module, Version, Description, CreatedAt)"
			                        + " values (@Module, @Version, @Description, @CreatedAt)", DatabaseMigrationTableName);
			Connection.Execute(sql, migrationResource.Migration);

			// Perform the actual SQL
			using (var textReader = migrationResource.GetSql())
			{
				var cmd = Connection.CreateCommand();
				cmd.CommandTimeout = 300;
				cmd.RunMigrationScript(textReader);
			}
		}

		private IEnumerable<DatabaseMigrationResource> FindPendingMigrations()
		{
			var list = new List<DatabaseMigrationResource>();
			var hash = new HashSet<Assembly>();

			// Find all migrations in the database
			var sql = string.Format("select Module, Version, Description, CreatedAt from {0}", DatabaseMigrationTableName);
			// create dictionary mapping version to migration
			var existingMigrations = Connection.Query<DatabaseMigration>(sql).ToDictionary(m => m.Version);

			foreach (var assembly in Assemblies)
			{
				// check for duplicate assemblies
				if (hash.Contains(assembly))
				{
					continue;
				}

				hash.Add(assembly);
				list.AddRange(FindPendingMigrationsForAssembly(existingMigrations, assembly));
			}

			Comparison<DatabaseMigrationResource> comparison = delegate(DatabaseMigrationResource r1, DatabaseMigrationResource r2)
			{
				var result = StringComparer.InvariantCultureIgnoreCase.Compare(r1.Migration.Version,
																			   r2.Migration.Version);
				if (result == 0)
				{
					result = StringComparer.InvariantCultureIgnoreCase.Compare(r1.Migration.Module, r2.Migration.Module);
				}

				return result;
			};

			list.Sort(comparison);
			return list;
		}

		/// <summary>
		/// Finds migrations that have not yet been applied to the database
		/// </summary>
		private IEnumerable<DatabaseMigrationResource> FindPendingMigrationsForAssembly(
			Dictionary<string, DatabaseMigration> existingMigrations,
			Assembly migrationAssembly)
		{
			// Migration resources in the assembly
			var migrationResources = from name in GetSchemaMigrationResourceNames(migrationAssembly)
			                         select new DatabaseMigrationResource(migrationAssembly, name);

			// Migration resources that have not been run yet
			var pendingMigrations = (from migrationResource in migrationResources
			                         where !existingMigrations.ContainsKey(migrationResource.Migration.Version)
			                         orderby migrationResource.Migration.Version
			                         select migrationResource).ToList();

			return pendingMigrations;
		}

		public static IEnumerable<string> GetSchemaMigrationResourceNames(Assembly migrationAssembly)
		{
			var resourceNames = (from name in migrationAssembly.GetManifestResourceNames()
								 where IsMigrationResourceName(name)
								 select name).ToList();
			return resourceNames;
		}

		/// <summary>
		/// Used during unit testing to ensure that there are no resources that look pretty similar to
		/// a migration, but are mistyped.
		/// </summary>
		/// <returns></returns>
		public ICollection<string> GetInvalidMigrationResourceNames(Assembly migrationAssembly)
		{
			var resourceNames = (from name in migrationAssembly.GetManifestResourceNames()
								 where IsMistypedMigrationResourceName(name)
								 select name).ToList();
			return resourceNames;
		}

		public static bool IsMigrationResourceName(string resourceName)
		{
			return ResourceNameRegex.IsMatch(resourceName);
		}

		public static bool IsMistypedMigrationResourceName(string resourceName)
		{
			return !IsMigrationResourceName(resourceName)
				   && MistypedResourceNameRegex.IsMatch(resourceName);
		}

		public class DatabaseMigrationResource
		{
			public DatabaseMigrationResource(Assembly assembly, string resourceName)
			{
				Assembly = Verify.ArgumentNotNull(assembly, "assembly");
				ResourceName = Verify.ArgumentNotNull(resourceName, "resourceName");

				var match = ResourceNameRegex.Match(resourceName);
				if (!match.Success)
				{
					throw new ArgumentException("Resource name is not a valid schema migration: " + resourceName);
				}

				var version = match.Groups[1].Value;
				var description = match.Groups[2].Value;

				Migration = new DatabaseMigration
				{
					Module = assembly.GetName().Name,
					Version = version,
					Description = description,
				};
			}

			public Assembly Assembly { get; private set; }
			public DatabaseMigration Migration { get; private set; }
			public string ResourceName { get; private set; }

			public TextReader GetSql()
			{
				var stream = Assembly.GetManifestResourceStream(ResourceName);
				if (stream == null)
				{
					throw new InvalidOperationException("Cannot find embedded resource: " + ResourceName);
				}
				var textReader = new StreamReader(stream);
				return textReader;
			}
		}
	}
}
