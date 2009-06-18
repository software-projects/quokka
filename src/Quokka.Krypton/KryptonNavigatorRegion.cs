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

		protected override Control CreateHostControl()
		{
			return new KryptonPage();
		}

		protected override void OnAdd(RegionItem item)
		{
			KryptonPage tabPage = (KryptonPage) item.HostControl;
			_navigator.Pages.Add(tabPage);
			item.PropertyChanged += Item_PropertyChanged;
		}

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RegionItem item = (RegionItem) sender;
			KryptonPage tabPage = (KryptonPage) item.HostControl;

			if (MatchPropertyName("Text", e))
			{
				tabPage.Text = item.Text;
			}
			if (MatchPropertyName("Image", e))
			{
				tabPage.ImageSmall = item.Image;
			}

			if (MatchPropertyName("IsActive", e))
			{
				if (item.IsActive)
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

		private static void Navigator_CloseAction(object sender, CloseActionEventArgs e)
		{
			// TODO:
		}

		private void Navigator_Selected(object sender, KryptonPageEventArgs e)
		{
			KryptonPage tabPage = e.Item;
			RegionItem item = (RegionItem) tabPage.Tag;

			foreach (KryptonPage page in _navigator.Pages)
			{
				RegionItem it = (RegionItem) page.Tag;
				it.IsActive = (page == _navigator.SelectedPage);
			}

			_navigator.Button.CloseButtonDisplay = item.CanClose ? ButtonDisplay.Logic : ButtonDisplay.ShowDisabled;
		}
	}
}