using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace Quokka.UI.WebBrowsers
{
	public static class PluggableProtocol
	{
		private static bool _isRegistered;
		private static IClassFactory _classFactory;
		private static readonly EmbeddedResourceMap EmbeddedResourceMap = new EmbeddedResourceMap();
		private const string SchemeName = "quokka";

		public static void Register(params Assembly[] assemblies)
		{
			foreach (var assembly in assemblies)
			{
				EmbeddedResourceMap.AddAssembly(assembly);
			}

			if (!_isRegistered)
			{

				var internetSession = GetInternetSession();
				var factory = new QuokkaProtocolHandlerFactory(EmbeddedResourceMap);
				var guid = new Guid(QuokkaProtocolHandler.Guid);
				var hr = internetSession.RegisterNameSpace(factory, ref guid, SchemeName, 0, null, 0);
				if (hr != 0)
				{
					Marshal.ThrowExceptionForHR(hr);
				}
				_isRegistered = true;
				_classFactory = factory;
			}
		}

		public static void Unregister()
		{
			if (_isRegistered)
			{
				var internetSession = GetInternetSession();
				internetSession.UnregisterNameSpace(_classFactory, SchemeName);
			}
		}

		[SecuritySafeCritical]
		private static IInternetSession GetInternetSession()
		{
			IInternetSession internetSession = null;
			var hr = CoInternetGetSession(0, ref internetSession, 0);
			if (hr != 0)
			{
				Marshal.ThrowExceptionForHR(hr);
			}
			return internetSession;
		}


		[DllImport("urlmon.dll")]
		private static extern int CoInternetGetSession(UInt32 dwSessionMode /* = 0 */,
		                                               ref IInternetSession ppIInternetSession,
		                                               UInt32 dwReserved /* = 0 */);

	}
}
