using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Quokka.Diagnostics;

namespace Quokka.WinForms.Regions
{
	public class TabControlRegion : SingleActiveRegion
	{
		private readonly TabControl _control;

		public TabControlRegion(TabControl control)
		{
			Verify.ArgumentNotNull(control, "control", out _control);
			_control.SelectedIndexChanged += TabControl_SelectedIndexChanged;
		}

		void TabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			foreach (TabPage tabPage in _control.TabPages)
			{
				RegionItem item = (RegionItem) tabPage.Tag;
				item.IsActive = (tabPage == _control.SelectedTab);
			}
		}

		protected override Control CreateHostControl()
		{
			return new TabPage();
		}

		protected override void OnAdd(RegionItem item)
		{
			TabPage tabPage = (TabPage) item.HostControl;
			_control.TabPages.Add(tabPage);
			item.PropertyChanged += ItemPropertyChanged;
		}

		protected override void OnRemove(RegionItem item)
		{
			TabPage tabPage = (TabPage) item.HostControl;
			_control.TabPages.Remove(tabPage);
			tabPage.Dispose();
		}

		private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RegionItem item = (RegionItem)sender;
			TabPage tabPage = (TabPage)item.HostControl;
			if (tabPage.Text != item.Text)
			{
				tabPage.Text = item.Text;
			}
			if (item.IsActive && _control.SelectedTab != tabPage)
			{
				_control.SelectTab(tabPage);
			}
		}

		private class WorkspaceInfo : IRegionInfo
		{
			private readonly TabPage _tabPage;
			private readonly TabControlRegion _region;

			public WorkspaceInfo(TabControlRegion region, TabPage tabPage)
			{
				Verify.ArgumentNotNull(region, "region", out _region);
				Verify.ArgumentNotNull(tabPage, "tabPage", out _tabPage);
			}

			public string Text
			{
				get { return _tabPage.Text; }
				set { _tabPage.Text = value; }
			}

			public Image Image
			{
				get { return null; }
				set
				{
					/* do nothing */
				}
			}

			public bool CanClose
			{
				get { return false; }
				set
				{
					/* do nothing */
				}
			}

			public void Activate()
			{
				_region._control.SelectTab(_tabPage);
			}
		}
	}
}