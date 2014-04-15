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
			_control.Disposed += delegate { RaiseRegionClosed(); };
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