using System;
using System.Threading;
using System.Windows.Forms;
using Quokka.WinForms;

namespace Quokka.Server.Internal
{
	public partial class HostingForm : Form
	{
// ReSharper disable NotAccessedField.Local
		private readonly DisplaySettings _displaySettings;
// ReSharper restore NotAccessedField.Local
		private bool _closeRequested;
		public ServerHost ServerHost { get; set; }

		public HostingForm()
		{
			InitializeComponent();
			_displaySettings = new DisplaySettings(this);
		}

		public void ShowLog()
		{
			startingLabel = null;
			Controls.Clear();
			var logView = new LogView {Dock = DockStyle.Fill, Visible = true, Name = "logView" };
			logView.StopRequested += StopRequested;
			Controls.Add(logView);
		}

		private void FormLoad(object sender, EventArgs e)
		{
			Text = Application.ProductName;
			timer.Enabled = true;
		}

		private void TimerTick(object sender, EventArgs e)
		{
			timer.Enabled = false;
			Control errorControl = null;
			try
			{
				if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
				{
					throw new CannotStartException("Main thread is not an STA thread. Please ensure that your program's Main() method is decorated with [STAThread]");
				}
				if (ServerHost == null)
				{
					throw new NullReferenceException("ServerHost should not be null");
				}
				ServerHost.Stopped += Stopped;
				ServerHost.InternalRun();
			}
			catch (Exception ex)
			{
				errorControl = new CannotStartError(ex);
			}

			if (errorControl != null)
			{
				Controls.Clear();
				errorControl.Dock = DockStyle.Fill;
				errorControl.Name = "errorControl";
				errorControl.Visible = true;
				Controls.Add(errorControl);
			}
		}

		private void HostingFormFormClosing(object sender, FormClosingEventArgs e)
		{
			_closeRequested = true;
			if (ServerHost.IsRunning)
			{
				// First thing we do is cancel the 
				e.Cancel = true;
				ServerHost.RequestStop();
			}
		}

		private void Stopped(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new Action(() => Stopped(sender, e)));
				return;
			}

			if (_closeRequested)
			{
				Close();
			}
			else
			{
				Text += " (stopped)";
			}
		}

		private void StopRequested(object sender, EventArgs e)
		{
			if (ServerHost.IsRunning)
			{
				ServerHost.RequestStop();
			}
		}
	}
}
