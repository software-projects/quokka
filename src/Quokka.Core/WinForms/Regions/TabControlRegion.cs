#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

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