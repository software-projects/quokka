using System;
using System.Windows.Forms;

namespace Quokka.WinForms.Startup
{
	public class SplashScreenApplication : ApplicationContext
	{
		private SplashScreenPresenter _presenter;

		public event EventHandler SplashScreenDisplayed;
		public event EventHandler SplashScreenClosed;

		public SplashScreenApplication()
		{
			_presenter = new SplashScreenPresenter();
			_presenter.SplashScreenDisplayed += Presenter_SplashScreenDisplayed;
			_presenter.SplashScreenClosed += Presenter_SplashScreenClosed;
			_presenter.DisplaySplashScreen();
			MainForm = _presenter.SplashScreen;
		}

		public SplashScreenApplication(Type formType) : this()
		{
			_presenter.FormType = formType;
		}

		public SplashScreenPresenter SplashScreenPresenter
		{
			get { return _presenter; }
		}

		protected virtual void OnSplashScreenDisplayed()
		{
		}

		protected virtual void OnSplashScreenClosed()
		{
		}

		private void Presenter_SplashScreenDisplayed(object sender, EventArgs e)
		{
			OnSplashScreenDisplayed();
			RaiseSplashScreenDisplayed(this, e);

			if (MainForm != _presenter.SplashScreen)
			{
				if (!MainForm.IsHandleCreated)
				{
					MainForm.Show();
				}
				_presenter.FadeAway();
			}
		}

		private void Presenter_SplashScreenClosed(object sender, EventArgs e)
		{
			OnSplashScreenClosed();
			RaiseSplashScreenClosed(this, e);

			// unsubscribe events so that the presenter will be garbage collected
			_presenter.SplashScreenDisplayed -= Presenter_SplashScreenDisplayed;
			_presenter.SplashScreenClosed -= Presenter_SplashScreenClosed;
			_presenter.Dispose();
			_presenter = null;
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