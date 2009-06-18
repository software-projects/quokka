using System;
using System.Security.Cryptography;
using System.Text;

namespace Quokka.Security
{
	//
	// Oct-07 - Martin Fernández translation for all the linux fans and detractors...
	//  
	/*
	#########################################################
	# md5crypt.py
	#
	# 0423.2000 by michal wallace http://www.sabren.com/
	# based on perl's Crypt::PasswdMD5 by Luis Munoz (lem@cantv.net)
	# based on /usr/src/libcrypt/crypt.c from FreeBSD 2.2.5-RELEASE
	#
	# MANY THANKS TO
	#
	#  Carey Evans - http://home.clear.net.nz/pages/c.evans/
	#  Dennis Marti - http://users.starpower.net/marti1/
	#
	#  For the patches that got this thing working!
	#
	#########################################################
	md5crypt.py - Provides interoperable MD5-based crypt() function

	SYNOPSIS

		import md5crypt.py

		cryptedpassword = md5crypt.md5crypt(password, salt);

	DESCRIPTION

	unix_md5_crypt() provides a crypt()-compatible interface to the
	rather new MD5-based crypt() function found in modern operating systems.
	It's based on the implementation found on FreeBSD 2.2.[56]-RELEASE and
	contains the following license in it:

	 "THE BEER-WARE LICENSE" (Revision 42):
	 <phk@login.dknet.dk> wrote this file.  As long as you retain this notice you
	 can do whatever you want with this stuff. If we meet some day, and you think
	 this stuff is worth it, you can buy me a beer in return.   Poul-Henning Kamp

	apache_md5_crypt() provides a function compatible with Apache's
	.htpasswd files. This was contributed by Bryan Hart <bryan@eai.com>.
	*/

	public class MD5Crypt
	{
		// Password hash magic
		private const string Magic = "$1$";

		// Characters for base64 encoding
		private const string Itoa64 = "./0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

		/// <summary>
		/// A function to concatenate bytes[]
		/// </summary>
		/// <param name="array1">First array</param>
		/// <param name="array2">Second array</param>
		/// <returns>New adition array</returns>
		private static byte[] Concat(byte[] array1, byte[] array2)
		{
			byte[] concat = new byte[array1.Length + array2.Length];
			Buffer.BlockCopy(array1, 0, concat, 0, array1.Length);
			Buffer.BlockCopy(array2, 0, concat, array1.Length, array2.Length);
			return concat;
		}

		/// <summary>
		/// Another function to concatenate bytes[]
		/// </summary>
		/// <param name="array1">First array</param>
		/// <param name="array2">Second array</param>
		/// <param name="max">Number of bytes in the second array to copy</param>
		/// <returns>New adition array</returns>
		private static byte[] PartialConcat(byte[] array1, byte[] array2, int max)
		{
			byte[] concat = new byte[array1.Length + max];
			Buffer.BlockCopy(array1, 0, concat, 0, array1.Length);
			Buffer.BlockCopy(array2, 0, concat, array1.Length, max);
			return concat;
		}

		/// <summary>
		/// Base64-Encode integer value
		/// </summary>
		/// <param name="value"> The value to encode</param>
		/// <param name="length"> Desired length of the result</param>
		/// <returns>@return Base64 encoded value</returns>
		private static string ToBase64(int value, int length)
		{
			StringBuilder result = new StringBuilder();
			while (--length >= 0)
			{
				result.Append(Itoa64.Substring(value & 0x3f, 1));
				value >>= 6;
			}
			return (result.ToString());
		}

		/// <summary>
		/// Unix-like Crypt-MD5 function
		/// </summary>
		/// <param name="password">The user password</param>
		/// <param name="salt">The salt or the pepper of the password</param>
		/// <returns>a human readable string</returns>
		public static string Crypt(string password, string salt)
		{
			Diagnostics.Verify.ArgumentNotNull(password, "password");
			Diagnostics.Verify.ArgumentNotNull(salt, "salt");
			int saltEnd;
			int len;
			int i;

			HashAlgorithm hashAlgorithm = HashAlgorithm.Create("MD5");

			// Skip magic if it exists
			if (salt.StartsWith(Magic))
			{
				salt = salt.Substring(Magic.Length);
			}

			// Remove password hash if present
			if ((saltEnd = salt.LastIndexOf('$')) != -1)
			{
				salt = salt.Substring(0, saltEnd);
			}

			// Shorten salt to 8 characters if it is longer
			if (salt.Length > 8)
			{
				salt = salt.Substring(0, 8);
			}

			byte[] ctx = Encoding.ASCII.GetBytes((password + Magic + salt));
			byte[] final = hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes((password + salt + password)));

			// Add as many characters of ctx1 to ctx
			for (len = password.Length; len > 0; len -= 16)
			{
				ctx = len > 16 ? Concat(ctx, final) : PartialConcat(ctx, final, len);
			}

			// Then something really weird...
			byte[] passwordBytes = Encoding.ASCII.GetBytes(password);

			for (i = password.Length; i > 0; i >>= 1)
			{
				ctx = (i & 1) == 1 ? Concat(ctx, new byte[] {0}) : Concat(ctx, new[] {passwordBytes[0]});
			}

			final = hashAlgorithm.ComputeHash(ctx);

			// Do additional mutations
			byte[] saltBytes = Encoding.ASCII.GetBytes(salt);
			for (i = 0; i < 1000; i++)
			{
				byte[] ctx1 = new byte[] {};
				ctx1 = (i & 1) == 1 ? Concat(ctx1, passwordBytes) : Concat(ctx1, final);
				if (i%3 != 0)
				{
					ctx1 = Concat(ctx1, saltBytes);
				}
				if (i%7 != 0)
				{
					ctx1 = Concat(ctx1, passwordBytes);
				}
				ctx1 = (i & 1) != 0 ? Concat(ctx1, final) : Concat(ctx1, passwordBytes);
				final = hashAlgorithm.ComputeHash(ctx1);
			}
			StringBuilder result = new StringBuilder();
			// Add the password hash to the result string
			int value = ((final[0] & 0xff) << 16) | ((final[6] & 0xff) << 8)
			            | (final[12] & 0xff);
			result.Append(ToBase64(value, 4));
			value = ((final[1] & 0xff) << 16) | ((final[7] & 0xff) << 8)
			        | (final[13] & 0xff);
			result.Append(ToBase64(value, 4));
			value = ((final[2] & 0xff) << 16) | ((final[8] & 0xff) << 8)
			        | (final[14] & 0xff);
			result.Append(ToBase64(value, 4));
			value = ((final[3] & 0xff) << 16) | ((final[9] & 0xff) << 8)
			        | (final[15] & 0xff);
			result.Append(ToBase64(value, 4));
			value = ((final[4] & 0xff) << 16) | ((final[10] & 0xff) << 8)
			        | (final[5] & 0xff);
			result.Append(ToBase64(value, 4));
			value = final[11] & 0xff;
			result.Append(ToBase64(value, 2));

			// Return result string
			return Magic + salt + "$" + result;
		}

		public static bool IsMD5Crypt(string md5Crypt)
		{
			if (String.IsNullOrEmpty(md5Crypt))
				return false;
			return md5Crypt.StartsWith(Magic);
		}

		public static string GetSalt(string md5Crypt)
		{
			if (!IsMD5Crypt(md5Crypt))
			{
				return String.Empty;
			}

			string skipPrefix = md5Crypt.Substring(3);
			int index = skipPrefix.IndexOf('$');
			if (index < 0)
				return String.Empty;
			if (index > 8)
				index = 8;

			string salt = skipPrefix.Substring(0, index);
			return salt;
		}

		public static bool Verify(string clearText, string md5crypt)
		{
			string salt = GetSalt(md5crypt);
			if (salt.Length == 0)
				return false;

			string calcMd5Crypt = Crypt(clearText, salt);
			return (calcMd5Crypt == md5crypt);
		}

		public static string GenerateSalt()
		{
			const string chars = "./0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

			Random rand = new Random();

			StringBuilder sb = new StringBuilder();

			for (int index = 0; index < 8; ++index)
			{
				int randomIndex = rand.Next(chars.Length);
				sb.Append(chars[randomIndex]);
			}
			return sb.ToString();
		}

		public static string Crypt(string clearText)
		{
			return Crypt(clearText, GenerateSalt());
		}
	}
}