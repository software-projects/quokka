using System.Drawing;

namespace Quokka.WinForms.Regions
{
	/// <summary>
	/// Information about the region that can be controlled by
	/// the contents of the region (ie view, presenter, task).
	/// </summary>
	/// <remarks>
	/// This interface has a few problems. (1) Extending is difficult. (2) Can be dependency injected into tasks, but not views.
	/// Just continue with it for now. More usage might present a better method (maybe even ISmartPartInfoProvider et all).
	/// </remarks>
	public interface IRegionInfo
	{
		/// <summary>
		/// Text associated with the region contents.
		/// </summary>
		/// <remarks>
		/// For example if the contents are in a tabbed window, the text would be
		/// the text of the tab.
		/// </remarks>
		string Text { get; set; }

		/// <summary>
		/// Image associated with the region contents.
		/// </summary>
		/// <remarks>
		/// <para>
		/// For example, if the contents are in a tabbed window, the image would be
		/// displayed on the tab. 
		/// </para>
		/// <para>
		/// Need to think about different image resolutions. Could have a SmallImage, MediumImage
		/// and LargeImage property, for example.
		/// </remarks>
		Image Image { get; set; }

		/// <summary>
		/// Do the contents support the ability for the user to close the window.
		/// </summary>
		bool CanClose { get; set; }
	}
}