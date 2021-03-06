﻿#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
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
			get { return Value(_copyrightText, GetCopyrightText); }
			set { _copyrightText = Trim(value); }
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
			if (StringUtils.IsNullOrWhiteSpace(value))
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
					if (!StringUtils.IsNullOrWhiteSpace(attribute.Copyright))
					{
						result = attribute.Copyright;
					}
				}
			}
			return result;
		}
	}
}
