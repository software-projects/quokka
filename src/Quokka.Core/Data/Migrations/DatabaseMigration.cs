using System;
using System.Text;

namespace Quokka.Data.Migrations
{
	public class DatabaseMigration
	{
		public string Module { get; set; }
		public string Version { get; set; }
		public string Description { get; set; }
		public DateTime CreatedAt { get; set; }

		public override int GetHashCode()
		{
			return Version.GetHashCode()
				   ^ Module.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var other = obj as DatabaseMigration;
			if (other == null)
			{
				return false;
			}

			return StringComparer.InvariantCultureIgnoreCase.Compare(Version, other.Version) == 0
				&& StringComparer.InvariantCultureIgnoreCase.Compare(Module, other.Module) == 0;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append(Module);
			sb.Append("/");
			sb.Append(Version);
			sb.Append("_");
			sb.Append(Description);
			sb.Append(".sql");
			return sb.ToString();
		}
	}
}
