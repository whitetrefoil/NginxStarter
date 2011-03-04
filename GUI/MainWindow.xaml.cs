using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using Microsoft.Win32;

namespace GUI
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		private static string _nginxPath;
		private static System.Diagnostics.ProcessStartInfo _nginxInfo;
		private static System.Diagnostics.Process _nginx;
        private static NotifyIcon _notifyIcon;
        private static RegistryKey _registryKey;

		public MainWindow()
		{
			InitializeComponent();
            _registryKey = Registry.CurrentUser.CreateSubKey("Software\\WhiteTrefoil\\NginxStarterGUI", RegistryKeyPermissionCheck.ReadWriteSubTree);
            if (_registryKey.GetValue("nginxpath", string.Empty).ToString() != string.Empty)
            {
                this.txtNPath.Text = _registryKey.GetValue("nginxpath", string.Empty).ToString();
                _nginxPath = _registryKey.GetValue("nginxpath", string.Empty).ToString();
            }
        }


		public bool nginxStart()
		{
			_nginxInfo = new System.Diagnostics.ProcessStartInfo();
			_nginxInfo.Arguments = string.Empty;
			_nginxInfo.FileName = _nginxPath;
			_nginxInfo.WorkingDirectory = _nginxPath.Substring(0, _nginxPath.LastIndexOf('\\'));
			_nginxInfo.CreateNoWindow = true;
			_nginxInfo.UseShellExecute = false;
			try
			{
				_nginx = System.Diagnostics.Process.Start(_nginxInfo);
                _registryKey.SetValue("nginxpath", _nginxPath);
                return true;
			}
			catch
			{
				MessageBox.Show("启动失败！");
				return false;
			}
		}
		public bool nginxStop()
		{
			try
			{
				_nginxInfo.Arguments = "-s stop";
				System.Diagnostics.Process.Start(_nginxInfo);
                _registryKey.SetValue("nginxpath", _nginxPath);
                return true;
			}
			catch
			{
				return false;
			}
		}
		public bool nginxQuit()
		{
			try
			{
				_nginxInfo.Arguments = "-s quit";
				System.Diagnostics.Process.Start(_nginxInfo);
                _registryKey.SetValue("nginxpath", _nginxPath);
                return true;
			}
			catch
			{
				return false;
			}
		}
        public void nginxReload()
		{
			_nginxInfo.Arguments = "-s reload";
            _registryKey.SetValue("nginxpath", _nginxPath);
        }
        public void nginxRestart()
		{
			_nginxInfo.Arguments = "-s restart";
            _registryKey.SetValue("nginxpath", _nginxPath);
        }
		

		private void btnNStart_Click(object sender, RoutedEventArgs e)
		{
			if (txtNPath.Text == String.Empty)
			{
                this.btnNBrowse_Click(sender, e);
			}
			else
			{
				_nginxPath = txtNPath.Text;
				if (nginxStart())
				{
					buttonEnabledChange(true);
				}
			}
		}
		private void buttonEnabledChange(bool isStarted)
		{/*
			if (isStarted)
			{
				btnNStart.IsEnabled = false;
				btnNReload.IsEnabled = true;
				btnNRestart.IsEnabled = true;
				btnNQuit.IsEnabled = true;
				btnNStop.IsEnabled = true;
			}
			else
			{
				btnNStart.IsEnabled = true;
				btnNReload.IsEnabled = false;
				btnNRestart.IsEnabled = false;
				btnNQuit.IsEnabled = false;
				btnNStop.IsEnabled = false;
			}*/
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            _registryKey.Close();
			if (_nginx != null)
			{
				_nginx.Close();
				try
				{
					_nginx.Kill();
				}
				catch
				{
				}
			}
		}
        public void nginxBrowse()
        {
			OpenFileDialog ofd = new OpenFileDialog();
            if (_nginxPath != string.Empty)
            {
                ofd.InitialDirectory = _nginxPath;
            }
            else
            {
                ofd.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            }
			ofd.Filter = "Nginx默认执行文件|nginx.exe|所有执行文件|*.exe|所有文件|*.*";
			if (ofd.ShowDialog() == true)
			{
				txtNPath.Text = ofd.FileName;
			}
		}
        private void btnNBrowse_Click(object sender, RoutedEventArgs e)
        {
            this.nginxBrowse();
        }
		private void btnNReload_Click(object sender, RoutedEventArgs e)
		{
			this.nginxReload();
		}

		private void btnNRestart_Click(object sender, RoutedEventArgs e)
		{
			this.nginxRestart();
		}

		private void btnNQuit_Click(object sender, RoutedEventArgs e)
		{
			if (this.nginxQuit()) buttonEnabledChange(false);
		}

		private void btnNStop_Click(object sender, RoutedEventArgs e)
		{
			if (this.nginxStop()) buttonEnabledChange(false);
		}

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                _notifyIcon = new NotifyIcon(this);
                this.ShowInTaskbar = false;
            }
            else
            {
                this.ShowInTaskbar = true;
                if(_notifyIcon != null) _notifyIcon.Dispose();
            }
        }
	}
}
