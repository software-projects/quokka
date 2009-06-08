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

		public SplashScreenApplication()
		{
			// Register for the application idle loop. This prevents the class from calling
			// a virtual method (CreateSplashScreen) inside its constructor.
			Application.Idle += Application_Idle;

			// Create the presenter and wire the events. By doing this in the constructor, it
			// allows the derived class to set properties on the presenter (copyright, company name, etc).
			Presenter = new SplashScreenPresenter();
			Presenter.SplashScreenDisplayed += Presenter_SplashScreenDisplayed;
			Presenter.SplashScreenClosed += Presenter_SplashScreenClosed;
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
			// null means display the default splash screen
			return null;
		}

		/// <summary>
		/// Gets run the first time that the application is idle -- displays the splash screen
		/// </summary>
		private void Application_Idle(object sender, EventArgs e)
		{
			// Unsubscribe to the Application.Idle event
			Application.Idle -= Application_Idle;

			Presenter.DisplaySplashScreen(CreateSplashScreen());
			MainForm = Presenter.SplashScreen;
		}

		private void Presenter_SplashScreenDisplayed(object sender, EventArgs e)
		{
			OnSplashScreenDisplayed();
			RaiseSplashScreenDisplayed(this, e);

			if (MainForm != Presenter.SplashScreen)
			{
				if (!MainForm.IsHandleCreated)
				{
					MainForm.Show();
				}
				Presenter.FadeAway();
			}
		}

		private void Presenter_SplashScreenClosed(object sender, EventArgs e)
		{
			OnSplashScreenClosed();
			RaiseSplashScreenClosed(this, e);

			// unsubscribe events so that the presenter will be garbage collected
			Presenter.SplashScreenDisplayed -= Presenter_SplashScreenDisplayed;
			Presenter.SplashScreenClosed -= Presenter_SplashScreenClosed;
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