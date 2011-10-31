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