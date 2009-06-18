using System;
using System.Drawing;
using System.Windows.Forms;
using Quokka.Uip;

namespace Quokka.WinForms.Regions
{
	/// <summary>
	/// (Experimental) Represents a single 'pane' within a Region
	/// </summary>
	/// <remarks>
	/// <para>
	/// A region pane represents a single view (or task) and its surrounding trim. For example,
	/// in a tabbed region, the text and image can be specified for the tab page.
	/// </para>
	/// </remarks>
	public interface IRegionPane : IServiceProvider
	{
		object ViewObject { get; }

		string Text { get; set; }
		Image Image { get; set; }
		bool CanClose { get; set; }
		T GetService<T>();

		UipTask Task { get; }
		Control ClientControl { get; }
	}
}