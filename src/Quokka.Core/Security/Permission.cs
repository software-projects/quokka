using System;
using System.Collections.ObjectModel;
using Quokka.Diagnostics;

namespace Quokka.Security
{
	/// <summary>
	/// A permission is used to define authorization to perform a particular action.
	/// </summary>
	/// <remarks>
	/// Permissions are usually defined as readonly fields of a static class.
	/// </remarks>
	/// <example>
	/// <code>
	///		// Permissions for acessing Widgets
	///		public static class WidgetPermissions 
	///		{
	///			public static readonly Permission View = new Permission("Widget.View", "View Widgets", Roles.Staff, Roles.Clients);
	///			public static readonly Permission Edit = new Permission("Widget.Edit", "Edit Widgets", Roles.Supervisor);
	///			public static readonly Permission Delete = new Permission("Widget.Delete", "Delete Widgets", Roles.Administrator);
	///		}
	/// </code>
	/// </example>
	public class Permission
	{
		/// <summary>
		/// Uniquely identifies the permission. Used for persistent storage (eg database primary key).
		/// </summary>
		public string Id { get; private set; }

		/// <summary>
		/// Describes what the permission applies to.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		/// The default roles that have this permission. This can be useful to declare if the persistent
		/// role/permission storage is automatically generated from the code.
		/// </summary>
		public ReadOnlyCollection<DefaultRole> DefaultRoles { get; private set; }

		public Permission(string id, string description, params DefaultRole[] defaultRoles)
		{
			Id = Verify.ArgumentNotNull(id, "id");
			Description = Verify.ArgumentNotNull(description, "description");
			var copy = new DefaultRole[defaultRoles.Length];
			Array.Copy(defaultRoles, copy, defaultRoles.Length);
			DefaultRoles = new ReadOnlyCollection<DefaultRole>(copy);
		}
	}
}