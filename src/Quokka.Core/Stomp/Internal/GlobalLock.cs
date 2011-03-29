namespace Quokka.Stomp.Internal
{
	/// <summary>
	/// This is here until we sort out the deadlock issues. Then it goes again.
	/// </summary>
	public static class GlobalLock
	{
		public static readonly object Instance = new object();
	}
}
