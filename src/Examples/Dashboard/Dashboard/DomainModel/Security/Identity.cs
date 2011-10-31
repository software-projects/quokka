using System.Security.Principal;
using Quokka.Security;

namespace Dashboard.DomainModel.Security
{
	public class Identity : IIdentity
	{
		private string _name;
		private bool _authenticated;

		public string Name
		{
			get { return _name; }
		}

		public string AuthenticationType
		{
			get { return "CUSTOM"; }
		}

		public bool IsAuthenticated
		{
			get { return _authenticated; }
		}

		public bool Authenticate(User user, string clearTextPassword)
		{
			if (MD5Crypt.IsMD5Crypt(user.CryptPassword))
			{
				// handle when password is MD5Crypt
				_authenticated = MD5Crypt.Verify(clearTextPassword, user.CryptPassword);
			}
			else
			{
				// handle when password is stored in clear text
				_authenticated = (clearTextPassword == user.CryptPassword);
			}

			if (_authenticated)
			{
				_name = user.Username;
			}

			return _authenticated;
		}
	}
}