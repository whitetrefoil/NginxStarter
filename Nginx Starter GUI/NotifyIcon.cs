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
        private static MainWindow mainWindow;

        public NotifyIcon()
        {
            InitializeComponent();
        }
        public NotifyIcon(MainWindow window)
            : this()
        {
            mainWindow = window;
        }

        public NotifyIcon(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
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
    }
}
