using System;
using System.Security.Principal;
using System.Text;
using Quokka.Security;

namespace Dashboard.DomainModel
{
	public class User : IIdentity, IPrincipal
	{
		private bool _authenticated;

		public string Username { get; set; }
		public string FamilyName { get; set; }
		public string GivenName { get; set; }
		public string CryptPassword { get; set; }

		#region IIdentity members

		string IIdentity.Name
		{
			get { return Username; }
		}

		string IIdentity.AuthenticationType
		{
			get { return "Custom"; }
		}

		bool IIdentity.IsAuthenticated
		{
			get { return _authenticated; }
		}

		#endregion

		#region IPrincipal members

		public bool IsInRole(string role)
		{
			return true;
		}

		public IIdentity Identity
		{
			get { return this; }
		}

		#endregion

		public string FullName
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(GivenName);
				if (sb.Length > 0)
				{
					sb.Append(' ');
				}
				sb.Append(FamilyName);
				return sb.ToString();
			}
		}

		public bool Authenticate(string clearTextPassword)
		{
			string cryptPassword = (CryptPassword ?? String.Empty).Trim();

			if (MD5Crypt.IsMD5Crypt(cryptPassword))
			{
				// handle when password is MD5Crypt
				_authenticated = MD5Crypt.Verify(clearTextPassword, cryptPassword);
			}
			else
			{
				// handle when password is stored in clear text
				_authenticated = (clearTextPassword == cryptPassword);
			}

			return _authenticated;
		}


	}
}