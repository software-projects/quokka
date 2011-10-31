using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Common.Logging;
using Quokka.Diagnostics;
using Quokka.DynamicCodeGeneration;
using Quokka.ServiceLocation;
using Quokka.UI.Regions;
using Quokka.UI.Tasks;
using Quokka.Uip;

namespace Quokka.WinForms.Regions
{
	/// <summary>
	/// Contains information about an item added to a region.
	/// </summary>
	/// <remarks>
	/// Items added to a region can be either a Windows Forms
	/// <see cref="ClientControl"/>, or a Quokka UI task (<see cref="UipTask"/>,
	/// which itself hosts windows forms controls.
	/// </remarks>
	public class RegionItem : IRegionInfo, INotifyPropertyChanged
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();
		private readonly object _item;
		private readonly Control _clientControl;
		private readonly Control _hostControl;
		private readonly IUITask _task;
		private readonly ViewManager _viewManager;
		private readonly Region _region;
		private bool _isActive;
		private static readonly PropertyChangedEventArgs IsActiveChanged = new PropertyChangedEventArgs("IsActive");

		public event PropertyChangedEventHandler PropertyChanged;

		internal RegionItem(Region region, object item, Control hostControl)
		{
			Verify.ArgumentNotNull(region, "region", out _region);
			Verify.ArgumentNotNull(item, "item", out _item);
			Verify.ArgumentNotNull(hostControl, "hostControl", out _hostControl);
			_hostControl.Tag = this;
			_clientControl = item as Control;
			_task = item as IUITask;

			if (_task != null)
			{
				_viewManager = new ViewManager(_hostControl);
				_task.ServiceContainer.RegisterInstance<IRegion>(_region);
				_task.ServiceContainer.RegisterInstance<IRegionInfo>(this);
				_task.TaskComplete += Task_TaskComplete;
				UpdateRegionInfo(_task);
			}
			else if (_clientControl != null)
			{
				Form form = _clientControl as Form;
				if (form != null)
				{
					form.TopLevel = false;
					form.FormBorderStyle = FormBorderStyle.None;
				}

				UpdateRegionInfo(_clientControl);

				_hostControl.Controls.Add(_clientControl);
				_clientControl.Dock = DockStyle.Fill;
				_clientControl.Visible = true;
			}
			else
			{
				string message = String.Format("Cannot add object of type {0} to a Windows Forms region",
											   item.GetType().FullName);
				Log.Error(message);
				throw new ArgumentException(message);
			}
		}

		private void UpdateRegionInfo(object obj)
		{
			// Attempt to inject the IRegionInfo object into a control or a task, either by a property
			// named 'RegionInfo', or by a method called 'SetRegionInfo'.
			IRegionInfoAware regionInfoAware = ProxyFactory.CreateDuckProxy<IRegionInfoAware>(obj);
			if (regionInfoAware.IsRegionInfoSupported)
			{
				regionInfoAware.RegionInfo = this;
			}
			if (regionInfoAware.IsSetRegionInfoSupported)
			{
				regionInfoAware.SetRegionInfo(this);
			}
		}

		void Task_TaskComplete(object sender, EventArgs e)
		{
			// Clear any client controls in the view manager. They will be disposed elsewhere.
			ViewManager.Clear();
			_region.Remove(_item);
		}

		/// <summary>
		/// The item that was added to the region.
		/// </summary>
		public object Item
		{
			get { return _item; }
		}

		/// <summary>
		/// The Windows Form <see cref="Control"/> associated with the item.
		/// </summary>
		/// <remarks>
		/// If the item is a Windows Forms <see cref="Control"/>, then this property 
		/// is the same as the item. If the item is a <see cref="UipTask"/>, then this
		/// property is <see langword="null"/>.
		/// </remarks>
		public Control ClientControl
		{
			get { return _clientControl; }
		}

		/// <summary>
		/// The parent control that hosts the UI task or the client control
		/// </summary>
		public Control HostControl
		{
			get { return _hostControl; }
		}

		/// <summary>
		/// The view manager used by the UI task.
		/// </summary>
		public ViewManager ViewManager
		{
			get { return _viewManager; }
		}

		/// <summary>
		/// The UI task (if any) associated with the item 
		/// </summary>
		public IUITask Task
		{
			get { return _task; }
		}

		/// <summary>
		/// For use by the subclassed region for storing any relevant information.
		/// </summary>
		public object Tag { get; set; }

		/// <summary>
		/// Is the item active within its region.
		/// </summary>
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					OnPropertyChanged(IsActiveChanged);
				}
			}
		}

		protected void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, e);
			}
		}

		private string _text;
		private static readonly PropertyChangedEventArgs TextPropertyChanged = new PropertyChangedEventArgs("Text");

		public string Text
		{
			get { return _text; }
			set { if (value != _text)
			{
				_text = value;
				OnPropertyChanged(TextPropertyChanged);
			} }
		}

		private Image _image;
		private static readonly PropertyChangedEventArgs ImagePropertyChanged = new PropertyChangedEventArgs("Image");

		public Image Image
		{
			get { return _image; }
			set
			{
				if (value != _image)
				{
					_image = value;
					OnPropertyChanged(ImagePropertyChanged);
				}
			}
		}

		private bool _canClose;
		private static readonly PropertyChangedEventArgs CanClosePropertyChanged = new PropertyChangedEventArgs("CanClose");

		public bool CanClose
		{
			get { return _canClose; }
			set
			{
				if (_canClose != value)
				{
					_canClose = value;
					OnPropertyChanged(CanClosePropertyChanged);
				}
			}
		}
	}
}