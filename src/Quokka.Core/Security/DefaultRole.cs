#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

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
	///			public static readonly Role Client = new Role("CLI", "Clients", "Clients of the company");
	///			public static readonly Role Staff = new Role("STF", "Staff", "All current staff of the company");
	///			public static readonly Role Supervisor = new Role("SUP", "Supervisors", "Staff supervisors in the company", Staff);
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
		/// A short name for the role.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Describes the role briefly. Use a job description or similar (eg "Shift Supervisor")
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		/// This role has all of the permissions of the role specified by this property.
		/// </summary>
		public DefaultRole Subrole { get; private set; }

		public DefaultRole(string id, string name, string description, DefaultRole subrole = null)
		{
			Id = Verify.ArgumentNotNull(id, "id");
			Name = Verify.ArgumentNotNull(name, "name");
			Description = Verify.ArgumentNotNull(description, "description");
			Subrole = subrole;
		}
	}
}