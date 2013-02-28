using System;

namespace Quokka.Security
{
	/// <summary>
	/// Interface used to 
	/// </summary>
	public interface ICurrentUser
	{
		/// <summary>
		/// This is either the name of the individual (eg "John CITIZEN"), or else if the user does not correspond
		/// to an individual, it is the description of the role (eg "Administration User").
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Unique identification of the user. This can be any type. Common types include integer, string and Guid.
		/// </summary>
		object UserId { get; }

		/// <summary>
		/// Does this user have the specified permission.
		/// </summary>
		bool HasPermission(Permission permission);

		/// <summary>
		/// Does the user have any of the specified permissions.
		/// </summary>
		bool HasAnyPermission(params Permission[] permissions);
	}
}