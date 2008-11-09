﻿using System;
using System.Text;

namespace Quokka.Util
{
	public static class RegistryUtil
	{
		private static string _companyName;
		private static string _majorVersion;
		private static string _productName;

		public static string CompanyName
		{
			get { return _companyName; }
			set { _companyName = (value ?? "").Trim(); }
		}

		public static string ProductName
		{
			get { return _productName; }
			set { _productName = (value ?? "").Trim(); }
		}

		public static string MajorVersion
		{
			get { return _majorVersion; }
			set { _majorVersion = (value ?? "").Trim(); }
		}

		public static string KeyPath
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				AppendKeyPath(sb);
				return sb.ToString();
			}
		}

		public static string SubKeyPath(string subKeyName)
		{
			StringBuilder sb = new StringBuilder();
			AppendKeyPath(sb);
			if (!String.IsNullOrEmpty(subKeyName))
			{
				sb.Append('\\');
				sb.Append(subKeyName);
			}
			return sb.ToString();
		}

		public static string SubKeyPath(params string[] subKeyNames)
		{
			StringBuilder sb = new StringBuilder();
			AppendKeyPath(sb);
			foreach (string subKeyName in subKeyNames)
			{
				if (!String.IsNullOrEmpty(subKeyName))
				{
					sb.Append('\\');
					sb.Append(subKeyName);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Append the key path to a string builder
		/// </summary>
		/// <param name="sb">StringBuilder</param>
		/// <remarks>This method is useful for building up sub-key paths</remarks>
		public static void AppendKeyPath(StringBuilder sb)
		{
			if (String.IsNullOrEmpty(CompanyName) && String.IsNullOrEmpty(ProductName))
			{
				throw new QuokkaException("CompanyName and/or ProductName must be specified.");
			}

			sb.Append("Software");
			if (!String.IsNullOrEmpty(CompanyName))
			{
				sb.Append(@"\");
				sb.Append(CompanyName);
			}
			if (!String.IsNullOrEmpty(ProductName))
			{
				sb.Append(@"\");
				sb.Append(ProductName);
			}
			if (!String.IsNullOrEmpty(MajorVersion))
			{
				sb.Append(@"\");
				sb.Append(MajorVersion);
			}
		}
	}
}