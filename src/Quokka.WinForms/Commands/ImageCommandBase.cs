using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Quokka.WinForms.Commands
{
	/// <summary>
	/// Default implementation of <see cref="IImageCommand"/>
	/// </summary>
	public abstract class ImageCommandBase : IImageCommand
	{
		private bool _checked;
		private CheckState _checkState;
		private bool _enabled;
		private string _text;
		private Image _imageLarge;
		private Image _imageSmall;
		private Color _imageTransparentColor;

		public event PropertyChangedEventHandler PropertyChanged;

		public bool Checked
		{
			get { return _checked; }
			set
			{
				if (_checked != value)
				{
					_checked = value;
					_checkState = _checked ? CheckState.Checked : CheckState.Unchecked;
					RaisePropertyChanged("Checked");
					RaisePropertyChanged("CheckState");
				}
			}
		}

		public CheckState CheckState
		{
			get { return _checkState; }
			set
			{
				if (_checkState != value)
				{
					_checkState = value;
					bool newChecked = (_checkState == CheckState.Checked || _checkState == CheckState.Indeterminate);
					bool checkedChanged = newChecked != _checked;
					if (checkedChanged)
					{
						_checked = newChecked;
					}
					RaisePropertyChanged("CheckState");
					if (checkedChanged)
					{
						RaisePropertyChanged("Checked");
					}
				}
			}
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					RaisePropertyChanged("Enabled");
				}
			}
		}

		public string Text
		{
			get { return _text; }
			set
			{
				if (_text != value)
				{
					_text = value;
					RaisePropertyChanged("Text");
				}
			}
		}

		public Image ImageLarge
		{
			get { return _imageLarge; }
			set
			{
				if (_imageLarge != value)
				{
					_imageLarge = value;
					RaisePropertyChanged("ImageLarge");
				}
			}
		}

		public Image ImageSmall
		{
			get { return _imageSmall; }
			set
			{
				if (_imageSmall != value)
				{
					_imageSmall = value;
					RaisePropertyChanged("ImageSmall");
				}
			}
		}

		public Color ImageTransparentColor
		{
			get { return _imageTransparentColor; }
			set
			{
				if (_imageTransparentColor != value)
				{
					_imageTransparentColor = value;
					RaisePropertyChanged("ImageTransparentColor");
				}
			}
		}

		public abstract void Execute();

		protected void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}