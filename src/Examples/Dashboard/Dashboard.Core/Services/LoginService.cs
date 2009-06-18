using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Dashboard.DomainModel;
using Dashboard.Services.Interfaces;
using Quokka.Security;

namespace Dashboard.Services
{
	public class LoginService : ILoginService
	{
		public User AttemptLogin(string username, string password)
		{
			Thread.Sleep(1000);
			if (username == "admin" && password == "test")
			{
				User user = new User();
				user.Username = "admin";
				user.FamilyName = "Administrator";
				user.CryptPassword = MD5Crypt.Crypt(password);
				user.Authenticate(password);
				return user;
			}

			return null;
		}
	}
}
