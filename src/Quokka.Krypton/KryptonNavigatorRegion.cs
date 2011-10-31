using System;
using System.ComponentModel;
using System.Windows.Forms;
using ComponentFactory.Krypton.Navigator;
using Quokka.Diagnostics;
using Quokka.WinForms.Regions;

namespace Quokka.Krypton
{
	public class KryptonNavigatorRegion : Region
	{
		private readonly KryptonNavigator _navigator;

		public KryptonNavigatorRegion(KryptonNavigator navigator)
		{
			Verify.ArgumentNotNull(navigator, "navigator", out _navigator);
			_navigator.Selected += Navigator_Selected;
			_navigator.CloseAction += Navigator_CloseAction;
			_navigator.Button.CloseButtonAction = CloseButtonAction.RemovePage;
		}

		void Navigator_CloseAction(object sender, CloseActionEventArgs e)
		{
			if (e.Action == CloseButtonAction.RemovePage 
				|| e.Action == CloseButtonAction.RemovePageAndDispose)
			{
				RegionItem item = e.Item.Tag as RegionItem;
				if (item == null)
				{
					return;
				}

				RegionItemClosed(item);
			}
		}

		// This class is in response to a bug in Windows Forms.
		// Deeply nested controls do not resize properly when their parents are resized.
		// See http://support.microsoft.com/kb/953934
		private class MyKryptonPage : KryptonPage
		{
			protected override void OnSizeChanged(EventArgs e)
			{
				if (Handle != null)
				{
					BeginInvoke((MethodInvoker) (() => base.OnSizeChanged(e)));
				}
			}
		}

		protected override Control CreateHostControl()
		{
			return new MyKryptonPage();
		}

		protected override void OnAdd(RegionItem item)
		{
			KryptonPage tabPage = (KryptonPage) item.HostControl;
			tabPage.Text = item.Text;
			tabPage.TextTitle = item.Text;
			tabPage.ImageSmall = item.Image;
			tabPage.Tag = item;
			item.PropertyChanged += Item_PropertyChanged;
			_navigator.Pages.Add(tabPage);
		}

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RegionItem item = (RegionItem) sender;
			KryptonPage tabPage = (KryptonPage) item.HostControl;

			if (MatchPropertyName("Text", e))
			{
				tabPage.Text = item.Text;
				tabPage.TextTitle = item.Text;
			}
			if (MatchPropertyName("Image", e))
			{
				tabPage.ImageSmall = item.Image;
			}

			if (MatchPropertyName("IsActive", e))
			{
				if (item.IsActive && _navigator.SelectedPage != tabPage)
				{
					_navigator.SelectedPage = tabPage;
				}
			}
		}

		private static bool MatchPropertyName(string propertyName, PropertyChangedEventArgs e)
		{
			if (e == null || String.IsNullOrEmpty(e.PropertyName))
			{
				return true;
			}
			return e.PropertyName == propertyName;
		}

		protected override void OnRemove(RegionItem item)
		{
			KryptonPage tabPage = (KryptonPage) item.HostControl;
			_navigator.Pages.Remove(tabPage);
		}

		private void Navigator_Selected(object sender, KryptonPageEventArgs e)
		{
			KryptonPage tabPage = e.Item;
			RegionItem item = (RegionItem) tabPage.Tag;

			// Note that we cannot assume that item is non-null.
			// There may be pages in the control that are not added via the region mechanism.

			foreach (KryptonPage page in _navigator.Pages)
			{
				RegionItem it = (RegionItem) page.Tag;
				if (it != null)
				{
					it.IsActive = (page == _navigator.SelectedPage);
				}
			}

			if (item != null)
			{
				_navigator.Button.CloseButtonDisplay = item.CanClose ? 
					ButtonDisplay.Logic : ButtonDisplay.ShowDisabled;
			}
		}
	}
}