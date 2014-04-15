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

namespace Quokka.ServiceLocation
{
	/// <summary>
	/// Code adapted from Microsoft.Practices.ServiceLocation.
	/// http://commonservicelocator.codeplex.com/
	/// </summary>
	public static class ServiceLocator
	{
		private static ServiceLocatorProvider _currentProvider;

		/// <summary>
		/// The current ambient container.
		/// </summary>
		public static IServiceLocator Current
		{
			get { return _currentProvider(); }
		}

		/// <summary>
		/// Set the delegate that is used to retrieve the current container.
		/// 
		/// </summary>
		/// <param name="newProvider">Delegate that, when called, will return
		///             the current ambient container.</param>
		public static void SetLocatorProvider(ServiceLocatorProvider newProvider)
		{
			_currentProvider = newProvider;
		}
	}

	/// <summary>
	/// This delegate type is used to provide a method that will
	/// return the current container. Used with the <see cref="ServiceLocator"/>
	/// static accessor class.
	/// </summary>
	/// <returns>
	/// An <see cref="IServiceLocator"/>.
	/// </returns>
	/// <remarks>
	/// Code adapted from Microsoft.Practices.ServiceLocation.
	/// http://commonservicelocator.codeplex.com/
	/// </remarks>
	public delegate IServiceLocator ServiceLocatorProvider();
}