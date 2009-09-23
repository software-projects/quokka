using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Common.Logging;
using Microsoft.Practices.ServiceLocation;
using Quokka.Diagnostics;
using Quokka.UI.Regions;

namespace Quokka.WinForms.Regions
{
	public abstract class Region : IRegion
	{
		private static readonly ILog _log = LogManager.GetCurrentClassLogger();
		private readonly ViewsCollection _views = new ViewsCollection();
		private readonly ViewsCollection _activeViews = new ViewsCollection();
		private readonly List<RegionItem> _regionItems = new List<RegionItem>();

		/// <summary>
		/// This event is raised when the underlying region control is closed. It signals to the region manager
		/// that the region is no longer functional.
		/// </summary>
		public event EventHandler RegionClosed;

		public IViewsCollection Views
		{
			get { return _views; }
		}

		public IViewsCollection ActiveViews
		{
			get { return _activeViews; }
		}

		public IRegionManager RegionManager { get; internal set; }

		public string Name { get; set; }

		public void Add(object view)
		{
			Verify.ArgumentNotNull(view, "view");
			object localView = view; // resharper wants a local copy because it is used in a lambda expression
			RegionItem item = _regionItems.FirstOrDefault(x => x.Item == localView);
			if (item != null)
			{
				throw new InvalidOperationException("View has already been added to the region");
			}

			Type type = view as Type;
			if (type != null)
			{
				view = ServiceLocator.Current.GetInstance(type);
			}

			// Initialise the region item and add it to the appropriate collections. If
			// the subclass fails its initialization for any reason, we will clean up.
			Control hostControl;
			try
			{
				hostControl = CreateHostControl();
			}
			catch (Exception ex)
			{
				_log.WarnFormat("Failed to add view to region", ex);
				throw;
			}
			item = new RegionItem(this, view, hostControl);
			item.PropertyChanged += Item_PropertyChanged;
			_regionItems.Add(item);
			_views.Add(item.Item);

			if (item.Task != null)
			{
				item.Task.Start(item.ViewManager);
			}

			try
			{
				OnAdd(item);
			}
			catch (Exception ex)
			{
				_log.WarnFormat("Failed to add view to region", ex);
				Cleanup(item);
				throw;
			}
		}

		public void Remove(object view)
		{
			RegionItem item = GetRegionItem(view, false);
			if (item != null)
			{
				// TODO: not sure if this is a good idea to deactivate before removing.
				// Maybe the subclass should be in a position to decide.
				if (item.IsActive)
				{
					Deactivate(item.Item);
				}

				Cleanup(item);
				OnRemove(item);
			}
		}

		public virtual void Activate(object view)
		{
			RegionItem item = GetRegionItem(view, true);

			if (!item.IsActive)
			{
				item.IsActive = true;
			}
		}

		public virtual void Deactivate(object view)
		{
			RegionItem item = GetRegionItem(view, true);
			if (item.IsActive)
			{
				item.IsActive = false;
			}
		}

		protected abstract Control CreateHostControl();
		protected abstract void OnAdd(RegionItem item);
		protected abstract void OnRemove(RegionItem item);

		/// <summary>
		/// Called by the derived class to inform that a region item has closed.
		/// </summary>
		/// <param name="item"></param>
		protected void RegionItemClosed(RegionItem item)
		{
			Cleanup(item);
		}

		protected void RaiseRegionClosed()
		{
			if (RegionClosed != null)
			{
				RegionClosed(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Cleanup a region item before discarding it forever
		/// </summary>
		/// <param name="item">Item to cleanup</param>
		/// <remarks>
		/// The item is being cleaned up either because it is no longer required, or because
		/// the subclass failed to initialize properly. Careful, the item might not be in a
		/// valid state.
		/// </remarks>
		private void Cleanup(RegionItem item)
		{
			_regionItems.Remove(item);
			_views.Remove(item.Item);
			_activeViews.Remove(item.Item);
			item.PropertyChanged -= Item_PropertyChanged;
		}

		private RegionItem GetRegionItem(object view, bool throwIfNotFound)
		{
			if (view == null)
			{
				throw new ArgumentNullException("view");
			}

			RegionItem regionItem = _regionItems.FirstOrDefault(x => x.Item == view);
			if (regionItem == null && throwIfNotFound)
			{
				throw new ArgumentException("View is not in this region", "view");
			}

			return regionItem;
		}

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender == null)
			{
				_log.Warn("Sender is null for PropertyChanged event");
				return;
			}
			RegionItem item = sender as RegionItem;
			if (item == null)
			{
				_log.WarnFormat("Expected object of type {0}, actual = {1}", typeof(RegionItem), sender.GetType());
				return;
			}

			if (e.PropertyName == "IsActive" || String.IsNullOrEmpty(e.PropertyName))
			{
				if (item.IsActive)
				{
					if (!_activeViews.Contains(item.Item))
					{
						_activeViews.Add(item.Item);
					}
				}
				else
				{
					_activeViews.Remove(item.Item);
				}
			}
		}
	}
}