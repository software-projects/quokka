using System;
using System.Runtime.InteropServices;
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

		public static void SetWindowRedraw(IWin32Window window, bool redraw) {
			SendMessage(window, WM_SETREDRAW, redraw ? 1 : 0, 0);
		}

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

		#region Private methods

		private static int SendMessage(IWin32Window window, int msg, int wparam, int lparam) {
			if (window.Handle == IntPtr.Zero) {
				throw new ArgumentNullException("window");
			}
			return User32.SendMessage(window.Handle, msg, wparam, lparam);
		}

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

			[DllImport("user32", SetLastError=true)]
			public static extern IntPtr GetSystemMenu(IntPtr hwnd, int revert);

			[DllImport("user32", SetLastError=true)]
			public static extern int EnableMenuItem(IntPtr hmenu, int itemId, int enable);
		}

		#endregion
	}
}
