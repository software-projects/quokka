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
		/// to an individual, it is the description of the role (eg "Administration User"). Set to <c>null</c> if
		/// no user is logged in.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// This is normally the name that the user enters during login. Set to <c>null</c> if no user is logged in.
		/// </summary>
		string Username { get; }

		/// <summary>
		/// Unique identification of the user. This can be any type. Common types include integer, string and Guid.
		/// Set to <c>null</c> if no user is logged in.
		/// </summary>
		/// <remarks>
		/// This might be the same as <see cref="Username"/>, or it might be the underlying identifier/guid that is used to 
		/// uniquely identify the user.
		/// </remarks>
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

	public static class CurrentUserExtensions
	{
		/// <summary>
		/// Is there a current user logged in.
		/// </summary>
		/// <param name="currentUser"></param>
		/// <returns></returns>
		public static bool IsLoggedIn(this ICurrentUser currentUser)
		{
			return (currentUser != null && currentUser.UserId != null);
		}
	}
}