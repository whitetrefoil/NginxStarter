using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows;

namespace NginxStarterGUI
{
	public partial class NotifyIcon : Component
	{
		private static MainWindow _mainWindow;
		private static bool _isNginxStarted;

		public NotifyIcon(MainWindow window, bool isNginxStarted = false)
		{
			_mainWindow = window;
			_isNginxStarted = isNginxStarted;
			InitializeComponent();
			if (_isNginxStarted)
				this.changeOptionsAfterNginxStarted();
			else
				this.changeOptionsAfterNginxStoped();
		}

		public NotifyIcon()
		{
			InitializeComponent();
		}

		public NotifyIcon(IContainer container)
		{
			if(container != null)
				container.Add(this);

			InitializeComponent();
		}

		private void notifyIcon1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			_mainWindow.WindowState = WindowState.Normal;
		}
		private void menuItemResume_Click(object sender, System.EventArgs e)
		{
			_mainWindow.WindowState = WindowState.Normal;
		}
		private void menuItemExit_Click(object sender, System.EventArgs e)
		{
			_mainWindow.Close();
			this.Dispose();
		}

		private void menuItemOStart_Click(object sender, EventArgs e)
		{
			_mainWindow.btnNStart_Click(null, null);
			changeOptionsAfterNginxStarted();
		}

		private void menuItemOReload_Click(object sender, EventArgs e)
		{
			_mainWindow.btnNReload_Click(null, null);
		}

		private void menuItemORestart_Click(object sender, EventArgs e)
		{
			_mainWindow.btnNRestart_Click(null, null);
		}

		private void menuItemOQuit_Click(object sender, EventArgs e)
		{
			_mainWindow.btnNQuit_Click(null, null);
			changeOptionsAfterNginxStoped();
		}

		private void menuItemOStop_Click(object sender, EventArgs e)
		{
			_mainWindow.btnNStop_Click(null, null);
			changeOptionsAfterNginxStoped();
		}

		private void menuItemOBrowse_Click(object sender, EventArgs e)
		{
			_mainWindow.btnNBrowse_Click(null, null);
		}

		private void changeOptionsAfterNginxStarted()
		{
			menuItemOReload.Enabled = true;
			menuItemORestart.Enabled = true;
			menuItemOQuit.Enabled = true;
			menuItemOStart.Enabled = false;
			menuItemOBrowse.Enabled = false;
		}

		private void changeOptionsAfterNginxStoped()
		{
			menuItemOReload.Enabled = false;
			menuItemORestart.Enabled = false;
			menuItemOQuit.Enabled = false;
			menuItemOStart.Enabled = true;
			menuItemOBrowse.Enabled = true;
		}
	}
}
