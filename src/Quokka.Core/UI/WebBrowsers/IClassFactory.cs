using System;
using System.Runtime.InteropServices;

namespace Quokka.UI.WebBrowsers
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00000001-0000-0000-C000-000000000046")]
	[ComVisible(true)]
	public interface IClassFactory
	{
		[PreserveSig]
		uint CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject);

		[PreserveSig]
		uint LockServer(bool fLock);
	}
}
