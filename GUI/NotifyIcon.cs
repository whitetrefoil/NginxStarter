using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows;

namespace GUI
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

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void notifyIcon1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mainWindow.WindowState = System.Windows.WindowState.Normal;
        }
    }
}
