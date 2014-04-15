#region License

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
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;


namespace Quokka.WinForms
{
	using System.ComponentModel;

	/// <summary>
	///		Window native function wrappers
	/// </summary>
	internal static class Win32
	{
		// win32 constants used
		private const int MF_GRAYED = 1;
		private const int MF_BYCOMMAND = 0;
		private const int SC_CLOSE = 0xf060;
		private const int WM_SETREDRAW = 0x000b;
	    public const int WM_SETTEXT = 0x000c;
		public const int BS_COMMANDLINK = 0x0000000E;
		public const int BCM_SETNOTE = 0x00001609;
		public const int BCM_SETSHIELD = 0x0000160C; 

		public static void SetWindowRedraw(IWin32Window window, bool redraw) {
			SendMessage(window, WM_SETREDRAW, redraw ? 1 : 0, 0);
		}

		[SecuritySafeCritical]
		public static void SetWindowCloseButtonEnabled(IWin32Window window, bool enable)
		{
			if (window == null)
				throw new ArgumentNullException("window");
			if (window.Handle == IntPtr.Zero)
				throw new ArgumentNullException("Window handle is null");

			IntPtr hmenu = User32.GetSystemMenu(window.Handle, 0);
			if (hmenu == IntPtr.Zero) {
				ThrowWin32Exception();
			}

			int flags = MF_BYCOMMAND;
			if (!enable) {
				flags |= MF_GRAYED;
			}

			User32.EnableMenuItem(hmenu, SC_CLOSE, flags);
		}

		[SecuritySafeCritical]
		public static int SendMessage(IWin32Window window, int msg, int wparam, int lparam)
		{
			if (window.Handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("window");
			}
			return User32.SendMessage(window.Handle, msg, wparam, lparam);
		}

		[SecuritySafeCritical]
		public static int SendMessage(IWin32Window window, int msg, int wparam, string lparam)
		{
			if (window.Handle == IntPtr.Zero) {
				throw new ArgumentNullException("window");
			}
			return User32.SendMessage(window.Handle, msg, wparam, lparam);
		}

		#region Private methods

		private static void ThrowWin32Exception()
		{
			int error = Marshal.GetLastWin32Error();
			throw new Win32Exception(error);
		}

		#endregion

		#region User32 P/Invoke functions

		private static class User32
		{
			[DllImport("user32", SetLastError=true)]
			public static extern int SendMessage(IntPtr hwnd, int msg, int wparam, int lparam);

			[DllImport("user32", CharSet = CharSet.Unicode, SetLastError=true)]
			public static extern int SendMessage(IntPtr hwnd, int msg, int wparam, string lparam);

			[DllImport("user32", SetLastError=true)]
			public static extern IntPtr GetSystemMenu(IntPtr hwnd, int revert);

			[DllImport("user32", SetLastError=true)]
			public static extern int EnableMenuItem(IntPtr hmenu, int itemId, int enable);
		}

		#endregion
	}
}

