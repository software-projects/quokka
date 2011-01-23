namespace Dashboard.UI.TaskStates
{
	/// <summary>
	/// 	State during the login phase.
	/// </summary>
	public class LoginState
	{
		/// <summary>
		/// 	Username entered by the user at the login prompt
		/// </summary>
		/// <remarks>
		/// 	This value is set to <c>null</c> upon successful login.
		/// </remarks>
		public string UserName;

		/// <summary>
		/// 	Password entered by the user at the login prompt.
		/// </summary>
		/// <remarks>
		/// 	This value is set to <c>null</c> as soon as possible during the login task.
		/// </remarks>
		public string Password;
	}
}