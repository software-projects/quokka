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
	/// A 'strawman' interface if all interactions were via UITasks
	/// </summary>
	public interface IRegion
	{
		/// <summary>
		/// Gets a readonly view of the collection of views in the region.
		/// </summary>
		/// <value>An <see cref="IViewsCollection"/> of all the added views.</value>
		IViewsCollection Views { get; }

		/// <summary>
		/// Gets a readonly view of the collection of all the active views in the region.
		/// </summary>
		/// <value>An <see cref="IViewsCollection"/> of all the active views.</value>
		IViewsCollection ActiveViews { get; }

		/// <summary>
		/// Gets the name of the region that uniquely identifies the region.
		/// </summary>
		/// <value>The name of the region.</value>
		string Name { get; set; }

		/// <summary>
		/// Adds a new view or UI task to the region.
		/// </summary>
		/// <param name="view">
		/// The view or UI task to add to the region.
		/// </param>
		void Add(object view);

		/// <summary>
		/// Remove the specified view or UI task from the region
		/// </summary>
		/// <param name="view">The view or UI task to remove</param>
		void Remove(object view);

		/// <summary>
		/// Marks the specified view or UI task as active
		/// </summary>
		/// <param name="view"></param>
		void Activate(object view);

		/// <summary>
		/// Marks the specified view or UI task as inactive
		/// </summary>
		/// <param name="view"></param>
		void Deactivate(object view);
	}
}