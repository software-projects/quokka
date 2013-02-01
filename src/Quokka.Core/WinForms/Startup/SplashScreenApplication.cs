using System;
using System.Windows.Forms;

// NOTE: Do not add any references to classes from other assemblies.
// This class needs to load as quickly as possible during program startup.

namespace Quokka.WinForms.Startup
{
	/// <summary>
	/// Application context for a Windows Forms application that displays a splash screen at startup.
	/// </summary>
	public class SplashScreenApplication : ApplicationContext
	{
		public event EventHandler SplashScreenDisplayed;
		public event EventHandler SplashScreenClosed;
		public bool DisplaySplashScreen = true;

		public SplashScreenApplication()
		{
			// Register for the application idle loop. This prevents the class from calling
			// a virtual method (CreateSplashScreen) inside its constructor.
			Application.Idle += ApplicationOnIdle;

			// Create the presenter and wire the events. By doing this in the constructor, it
			// allows the derived class to set properties on the presenter (copyright, company name, etc).
			Presenter = new SplashScreenPresenter();
			Presenter.SplashScreenDisplayed += PresenterOnSplashScreenDisplayed;
			Presenter.SplashScreenClosed += PresenterOnSplashScreenClosed;
		}

		/// <summary>
		/// The splash screen presenter.
		/// </summary>
		/// <value>
		/// A <see cref="SplashScreenPresenter"/> object, which controls the display of the
		/// splash screen.
		/// </value>
		public SplashScreenPresenter Presenter { get; private set; }

		/// <summary>
		/// Override this method to perform application startup after the splash screen has
		/// been displayed. In particular, this method should assign a form to the <see cref="ApplicationContext.MainForm"/>
		/// property.
		/// </summary>
		protected virtual void OnSplashScreenDisplayed()
		{
		}

		/// <summary>
		/// Override this method to perform application startup after the splash screen has closed.
		/// </summary>
		/// <remarks>
		/// The splash screen is closed a short delay after the main form has been displayed.
		/// </remarks>
		protected virtual void OnSplashScreenClosed()
		{
		}

		/// <summary>
		/// Override this method to create a custom splash screen.
		/// </summary>
		/// <returns>
		/// A <see cref="Form"/> object to display as the splash screen, or <c>null</c> to display
		/// the default splash screen.
		/// </returns>
		protected virtual Form CreateSplashScreen()
		{
			// display the default splash screen
			return new DefaultSplashScreen(); ;
		}

		/// <summary>
		/// Gets run the first time that the application is idle -- displays the splash screen
		/// </summary>
		private void ApplicationOnIdle(object sender, EventArgs e)
		{
			// Unsubscribe to the Application.Idle event
			Application.Idle -= ApplicationOnIdle;

			Form splashScreen = null;
			if (DisplaySplashScreen)
			{
				splashScreen = CreateSplashScreen();
			}
			if (splashScreen == null)
			{
				// the application does not want a splash screen displayed, maybe it is in debug mode
				CreateMainForm();
				ShowMainForm();
			}
			else
			{
				Presenter.DisplaySplashScreen(CreateSplashScreen());
				MainForm = Presenter.SplashScreen;
			}
		}

		private void ShowMainForm()
		{
			if (MainForm == null)
			{
				MainForm = new ErrorForm("MainForm has not been assigned in the SplashScreenApplication.OnSplashScreenDisplayed method.");
			}

			try
			{
				if (!MainForm.IsHandleCreated)
				{
					MainForm.Show();
				}
			}
			catch (Exception ex)
			{
				MainForm = new ErrorForm(ex);
				MainForm.Show();
			}
		}

		private void PresenterOnSplashScreenDisplayed(object sender, EventArgs e)
		{
			CreateMainForm();
			RaiseSplashScreenDisplayed(this, e);

			if (MainForm == Presenter.SplashScreen)
			{
				MainForm = null;
			}

			ShowMainForm();
			Presenter.FadeAway();
		}

		private void CreateMainForm()
		{
			try
			{
				OnSplashScreenDisplayed();
			}
			catch (Exception ex)
			{
				MainForm = new ErrorForm(ex);
			}
		}

		private void PresenterOnSplashScreenClosed(object sender, EventArgs e)
		{
			OnSplashScreenClosed();
			RaiseSplashScreenClosed(this, e);

			// unsubscribe events so that the presenter will be garbage collected
			Presenter.SplashScreenDisplayed -= PresenterOnSplashScreenDisplayed;
			Presenter.SplashScreenClosed -= PresenterOnSplashScreenClosed;
			Presenter.Dispose();
			Presenter = null;
		}

		private void RaiseSplashScreenDisplayed(object sender, EventArgs e)
		{
			if (SplashScreenDisplayed != null)
			{
				SplashScreenDisplayed(sender, e);
			}
		}

		private void RaiseSplashScreenClosed(object sender, EventArgs e)
		{
			if (SplashScreenClosed != null)
			{
				SplashScreenClosed(sender, e);
			}
		}
	}
}