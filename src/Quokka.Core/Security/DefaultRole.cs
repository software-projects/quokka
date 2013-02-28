using Quokka.Diagnostics;

namespace Quokka.Security
{
	/// <summary>
	/// A role is usually associated with a job description, and is used to group together permissions (see <see cref="Permission"/>).
	/// </summary>
	/// <remarks>
	/// Roles are application-specific. An application will typically have a few pre-defined roles, and it can be useful to
	/// assign <see cref="Permission"/> objects to one or more <see cref="Permission"/> objects so that a default set of <see cref="DefaultRole"/> - <see cref="DefaultRole"/>
	/// mappings can be pre-configured in a security access database during installation of the product.
	/// </remarks>
	/// <example>
	/// <code>
	///		// Default roles referenced by permissions.
	///		public static class DefaultRoles
	///		{
	///			public static readonly Role Client = new Role("CLI", "Clients of the company");
	///			public static readonly Role Staff = new Role("STF", "All current staff of the company");
	///			public static readonly Role Supervisor = new Role("SUP", "Supervisors");
	///		}
	/// </code>
	/// </example>
	public class DefaultRole
	{
		/// <summary>
		/// Uniquely identifies the role. Used for persistent storage (eg database primary key).
		/// </summary>
		public string Id { get; private set; }

		/// <summary>
		/// Describes the role briefly. Use a job description or similar (eg "Shift Supervisor")
		/// </summary>
		public string Description { get; private set; }

		public DefaultRole(string id, string description)
		{
			Id = Verify.ArgumentNotNull(id, "id");
			Description = Verify.ArgumentNotNull(description, "description");
		}
	}
}