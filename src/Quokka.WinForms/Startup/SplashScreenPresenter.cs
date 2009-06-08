using System;
using System.Reflection;
using System.Windows.Forms;

// NOTE: Do not add any references to classes from other assemblies.
// This class needs to load as quickly as possible during program startup.

namespace Quokka.WinForms.Startup
{
	public class SplashScreenPresenter : IDisposable
	{
		// Fade in and out.
		private double _opacityIncrement = .05;
		private double _opacityDecrement = .1;
		private double _initialOpacity = 1.0;
		private const int _timerInterval = 50;
		private int _showingDelay = 150;
		private bool _fadingAway;

		// Status and progress bar
		private Timer _timer;
		private Form _splashScreen;

		#region Events

		public event EventHandler SplashScreenDisplayed;
		public event EventHandler SplashScreenClosed;

		#endregion

		#region Construction/disposal


		public void Dispose()
		{
			if (_timer != null)
				_timer.Dispose();
			if (_splashScreen != null)
				_splashScreen.Dispose();
		}

		#endregion

		#region Properties

		public double OpacityIncrement
		{
			get { return _opacityIncrement; }
			set
			{
				if (value <= 0.0 || value > 1.0)
				{
					throw new ArgumentException("Value should be in the range (0.0, 1.0)");
				}
				_opacityIncrement = value;
			}
		}

		public double OpacityDecrement
		{
			get { return _opacityDecrement; }
			set
			{
				if (value <= 0.0 || value > 1.0)
				{
					throw new ArgumentException("Value should be in the range (0.0, 1.0)");
				}
				_opacityDecrement = value;
			}
		}

		public double InitialOpacity
		{
			get { return _initialOpacity; }
			set
			{
				if (value < 0.0 || value > 1.0)
				{
					throw new ArgumentException("Value should be in the range [0.0, 1.0]");
				}
				_initialOpacity = value;
			}
		}

		public string ProductName { get; set; }
		public string CopyrightText { get; set; }
		public string CompanyName { get; set; }

		public Form SplashScreen
		{
			get { return _splashScreen; }
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Display a splash screen.
		/// </summary>
		/// <param name="splashScreen">
		/// The splash screen form to display. If <c>null</c>, then a default splash screen form is used.
		/// </param>
		public void DisplaySplashScreen(Form splashScreen)
		{
			if (splashScreen == null)
				splashScreen = new DefaultSplashScreen();
			_splashScreen = splashScreen;
			_splashScreen.Opacity = _initialOpacity;
			_splashScreen.TopMost = true;
			_splashScreen.ShowInTaskbar = false;
			_splashScreen.StartPosition = FormStartPosition.CenterScreen;

			_splashScreen.Load += SplashScreen_Load;
			_splashScreen.DoubleClick += SplashScreen_DoubleClick;
			_splashScreen.Closed += SplashScreen_Closed;
			_splashScreen.Show();
		}

		// Causes the splash screen to fade away
		public void FadeAway()
		{
			if (!_splashScreen.IsDisposed && !_fadingAway)
			{
				// Make it start going away.
				_splashScreen.Activate();
				_opacityIncrement = -_opacityDecrement/2;
				_fadingAway = true;
			}
		}

		#endregion

		#region Private methods

		private void RaiseDisplayed()
		{
			if (SplashScreenDisplayed != null)
			{
				SplashScreenDisplayed(this, EventArgs.Empty);
			}
		}

		private void RaiseSplashScreenClosed()
		{
			if (SplashScreenClosed != null)
			{
				SplashScreenClosed(this, EventArgs.Empty);
			}
		}

		private void UpdateView()
		{
			if (_timer == null)
			{
				_timer = new Timer {Interval = _timerInterval};
				_timer.Tick += Timer_Tick;
			}
			_timer.Start();

			ISplashScreenView view = _splashScreen as ISplashScreenView;
			if (view != null)
			{
				Assembly assembly = Assembly.GetEntryAssembly();
				Version version = assembly.GetName().Version;

				if (CompanyName == null)
				{
					CompanyName = Application.CompanyName;
				}
				if (ProductName == null)
				{
					ProductName = Application.ProductName;
				}
				if (CopyrightText == null)
				{
					var attributes = assembly.GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);
					foreach (AssemblyCopyrightAttribute attribute in attributes)
					{
						view.CopyrightText = attribute.Copyright;
					}
				}
				view.CompanyText = CompanyName;
				view.ProductText = ProductName;
				view.CopyrightText = CopyrightText;
				view.Version = version;
			}
		}

		#endregion

		#region Event handlers

		// Tick Event handler for the Timer control.  Handle fade in and fade out.
		private void Timer_Tick(object sender, EventArgs e)
		{
			if (_opacityIncrement > 0)
			{
				if (_splashScreen.Opacity < 1)
				{
					_splashScreen.Opacity += _opacityIncrement;
				}
			}
			else
			{
				if (_splashScreen.Opacity > 0)
					_splashScreen.Opacity += _opacityIncrement;
				else
				{
					_timer.Stop();
					_splashScreen.Close();
				}
			}

			if (_showingDelay > 0)
			{
				_showingDelay -= _timer.Interval;
				if (_showingDelay <= 0)
				{
					RaiseDisplayed();
				}
			}
		}

		// Close the form if they double click on it.
		private void SplashScreen_DoubleClick(object sender, EventArgs e)
		{
			FadeAway();
		}

		private void SplashScreen_Closed(object sender, EventArgs e)
		{
			RaiseSplashScreenClosed();

			// unsubscribe to events so that the form will be garbage collected
			_splashScreen.Load -= SplashScreen_Load;
			_splashScreen.DoubleClick -= SplashScreen_DoubleClick;
			_splashScreen.Closed -= SplashScreen_Closed;
			_splashScreen = null;
		}

		private void SplashScreen_Load(object sender, EventArgs e)
		{
			MethodInvoker m = UpdateView;
			_splashScreen.BeginInvoke(m);
		}

		#endregion
	}
}