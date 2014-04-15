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

namespace Quokka.UI.Regions
{
	/// <summary>
	/// Defines an interface to manage a set of <see cref="IRegion">regions</see> and to attach 
	/// regions to objects (typically controls).
	/// </summary>
	/// <remarks>
	/// This interface is based on the Prism interface with the same name.
	/// </remarks>
	public interface IRegionManager
	{
		/// <summary>
		/// Gets a collection of <see cref="IRegion"/> that identify each region by name. You can use this 
		/// collection to add or remove regions to the current region manager.
		/// </summary>
		IRegionCollection Regions { get; }
	}
}