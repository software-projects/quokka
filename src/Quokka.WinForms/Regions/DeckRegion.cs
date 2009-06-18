using System.ComponentModel;
using System.Windows.Forms;
using Quokka.Diagnostics;

namespace Quokka.WinForms.Regions
{
	/// <summary>
	/// A region that shows one view at a time inside an arbitrary Windows Forms <see cref="Control"/>
	/// </summary>
	public class DeckRegion : SingleActiveRegion
	{
		private readonly Control _control;

		public DeckRegion(Control control)
		{
			Verify.ArgumentNotNull(control, "control", out _control);
		}

		protected override Control CreateHostControl()
		{
			return new Panel();
		}

		protected override void OnAdd(RegionItem item)
		{
			_control.Controls.Add(item.HostControl);
			item.HostControl.Dock = DockStyle.Fill;
			item.HostControl.Visible = item.IsActive;
			item.PropertyChanged += ItemPropertyChanged;
		}

		private static void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RegionItem item = (RegionItem) sender;
			item.HostControl.Visible = item.IsActive;
		}

		protected override void OnRemove(RegionItem item)
		{
			_control.Controls.Remove(item.HostControl);
		}
	}
}