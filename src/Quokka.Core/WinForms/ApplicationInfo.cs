﻿using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Quokka.Util;

namespace Quokka.WinForms
{
	/// <summary>
	/// Provides static information about the Windows Forms application.
	/// </summary>
	public static class ApplicationInfo
	{
		private static string _productName;
		private static string _companyName;
		private static string _copyrightText;
		private static string _windowTitle;
		private static string _messageBoxCaption;

		/// <summary>
		/// The product name. If not set, defaults to <see cref="Application.ProductName"/>.
		/// </summary>
		public static string ProductName
		{
			get { return Value(_productName, () => Application.ProductName); }

			set
			{
				value = Trim(value);
				_productName = value;
				RegistryUtil.ProductName = value;
			}
		}

		/// <summary>
		/// The company name. Defaults to <see cref="Application.CompanyName"/>.
		/// </summary>
		public static string CompanyName
		{
			get { return Value(_companyName, () => Application.CompanyName); }
			set { _companyName = Trim(value); }
		}

		public static string CopyrightText
		{
			get { return Value(_copyrightText, () => GetCopyrightText()); }
		}

		/// <summary>
		/// Icon for use in top-level <see cref="Form"/> windows.
		/// </summary>
		/// <remarks>
		/// The application is responsible for setting the <see cref="Form.Icon"/> property itself.
		/// </remarks>
		public static Icon Icon { get; set; }

		/// <summary>
		/// Window title for top-level windows. If not set defaults to <see cref="ProductName"/>.
		/// </summary>
		public static string WindowTitle
		{
			get { return Value(_windowTitle, () => ProductName); }
			set { _windowTitle = Trim(value); }
		}

		/// <summary>
		/// Caption text for message boxes.
		/// </summary>
		public static string MessageBoxCaption
		{
			get { return Value(_messageBoxCaption, () => WindowTitle); }
			set { _messageBoxCaption = Trim(value); }
		}

		private static string Value(string value, Func<string> defaultCallback)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return defaultCallback();
			}
			return value;
		}

		private static string Trim(string s)
		{
			if (s == null)
			{
				return string.Empty;
			}
			return s.Trim();
		}

		private static string GetCopyrightText()
		{
			var result = string.Empty;
			var assembly = Assembly.GetEntryAssembly();
			if (assembly != null)
			{
				var attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				foreach (AssemblyCopyrightAttribute attribute in attributes)
				{
					if (!string.IsNullOrWhiteSpace(attribute.Copyright))
					{
						result = attribute.Copyright;
					}
				}
			}
			return result;
		}
	}
}
