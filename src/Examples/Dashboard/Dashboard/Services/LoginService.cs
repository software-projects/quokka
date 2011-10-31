using System.Threading;
using Dashboard.DomainModel;
using Dashboard.Services.Interfaces;
using Quokka.Security;
using Quokka.ServiceLocation;

namespace Dashboard.Services
{
	[PerRequest(typeof(ILoginService))]
	public class LoginService : ILoginService
	{
		public User AttemptLogin(string username, string password)
		{
			Thread.Sleep(1000);
			if (username == "admin" && password == "test")
			{
				User user = new User
				            	{
				            		Username = "admin",
				            		FamilyName = "Administrator",
				            		CryptPassword = MD5Crypt.Crypt(password)
				            	};
				user.Authenticate(password);
				return user;
			}

			return null;
		}
	}
}