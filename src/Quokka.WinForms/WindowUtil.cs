using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace Quokka.WinForms
{
	/// <summary>
	///		Window native function wrappers
	/// </summary>
	internal static class WindowUtil
	{
		// windows messages
		private const int WM_SETREDRAW = 0x000b;

		#region SendMessage

		[DllImport("user32")]
		private static extern int SendMessage(IntPtr hwnd, int msg, int wparam, int lparam);

		private static int SendMessage(IWin32Window window, int msg, int wparam, int lparam) {
			if (window.Handle == IntPtr.Zero) {
				throw new ArgumentNullException("window");
			}
			return SendMessage(window.Handle, msg, wparam, lparam);
		}

		#endregion

		public static void SetWindowRedraw(IWin32Window window, bool redraw) {
			SendMessage(window.Handle, WM_SETREDRAW, redraw ? 1 : 0, 0);
		}
	}
}

