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