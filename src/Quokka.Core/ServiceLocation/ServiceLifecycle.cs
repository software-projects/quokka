namespace Quokka.ServiceLocation
{
	/// <summary>
	/// Describes the different lifecycles of objects created by
	/// the service container.
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	public enum ServiceLifecycle
	{
		/// <summary>
		/// An instance is created for each request to the container.
		/// </summary>
		PerRequest,

		/// <summary>
		/// An instance is created the first time that it is requested from the
		/// container, and any subsequent request will return the same instance.
		/// </summary>
		Singleton,
	}
}