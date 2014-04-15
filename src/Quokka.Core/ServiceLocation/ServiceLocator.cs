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