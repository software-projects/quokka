using System.ComponentModel;

namespace Quokka.WinForms.Regions
{
	/// <summary>
	/// This interface is used internally, and is not meant to be called from your code.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IRegionInfoAware
	{
		bool IsRegionInfoSupported { get; }
		IRegionInfo RegionInfo { set; }

		bool IsSetRegionInfoSupported { get; }
		void SetRegionInfo(IRegionInfo regionInfo);
	}
}