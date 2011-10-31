using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Quokka.Stomp
{
	/// <summary>
	/// Collection for headers in a STOMP frame.
	/// </summary>
	/// <remarks>
	/// Current implementation is identical to <see cref="NameValueCollection"/>. Future implementations
	/// may contain overrides and additional functionality.
	/// </remarks>
	public class StompHeaderCollection : NameValueCollection
	{
	}
}
