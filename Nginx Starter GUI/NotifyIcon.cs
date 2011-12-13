using System;
using System.ComponentModel;
using System.Windows;

namespace NginxStarterGUI
{
	public partial class NotifyIcon : Component
	{
		private MainWindow mainWindow;
		internal bool IsNginxStarted;

		public NotifyIcon(MainWindow window, bool isNginxStarted = false)
		{
			mainWindow = window;
			IsNginxStarted = isNginxStarted;
			InitializeComponent();
			if (isNginxStarted)
				this.ChangeOptionsAfterNginxStarted();
			else
				this.ChangeOptionsAfterNginxStoped();
		}

		private void notifyIcon1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			mainWindow.WindowState = WindowState.Normal;
		}
		private void menuItemResume_Click(object sender, System.EventArgs e)
		{
			mainWindow.WindowState = WindowState.Normal;
		}
		private void menuItemExit_Click(object sender, System.EventArgs e)
		{
			mainWindow.Close();
			this.Dispose();
		}

		private void menuItemOStart_Click(object sender, EventArgs e)
		{
			mainWindow.btnNStart_Click(null, null);
		}

		private void menuItemOReload_Click(object sender, EventArgs e)
		{
			mainWindow.btnNReload_Click(null, null);
		}

		private void menuItemORestart_Click(object sender, EventArgs e)
		{
			mainWindow.btnNRestart_Click(null, null);
		}

		private void menuItemOQuit_Click(object sender, EventArgs e)
		{
			mainWindow.btnNQuit_Click(null, null);
		}

		private void menuItemOStop_Click(object sender, EventArgs e)
		{
			mainWindow.btnNStop_Click(null, null);
		}

		private void menuItemOBrowse_Click(object sender, EventArgs e)
		{
			mainWindow.btnNBrowse_Click(null, null);
		}

		internal void ChangeOptionsAfterNginxStarted()
		{
			menuItemOReload.Enabled = true;
			menuItemORestart.Enabled = true;
			menuItemOQuit.Enabled = true;
			menuItemOStart.Enabled = false;
			menuItemOBrowse.Enabled = false;
		}

		internal void ChangeOptionsAfterNginxStoped()
		{
			menuItemOReload.Enabled = false;
			menuItemORestart.Enabled = false;
			menuItemOQuit.Enabled = false;
			menuItemOStart.Enabled = true;
			menuItemOBrowse.Enabled = true;
		}
	}
}
