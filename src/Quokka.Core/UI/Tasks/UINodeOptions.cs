using System;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// Options that affect an <see cref="UINode"/> in a <see cref="UITask"/>
	/// </summary>
	/// <remarks>
	/// This enumeration may not last very long. It used to have more options, but now only
	/// has one. It might become a single, boolean property of the <see cref="UINode"/>
	/// (ie <c>StayOpen</c>). For now it will remain where it is, in case more options are
	/// added.
	/// </remarks>
	[Flags]
	public enum UINodeOptions
	{
		StayOpen = 1,
	}
}