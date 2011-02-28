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

		public MainWindow()
		{
			InitializeComponent();
		}

		private bool nginxStart()
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
				return true;
			}
			catch
			{
				MessageBox.Show("启动失败！");
				return false;
			}
		}
		private bool nginxStop()
		{
			try
			{
				_nginxInfo.Arguments = "-s stop";
				System.Diagnostics.Process.Start(_nginxInfo);
				return true;
			}
			catch
			{
				return false;
			}
		}
		private bool nginxQuit()
		{
			try
			{
				_nginxInfo.Arguments = "-s quit";
				System.Diagnostics.Process.Start(_nginxInfo);
				return true;
			}
			catch
			{
				return false;
			}
		}
		private void nginxReload()
		{
			_nginxInfo.Arguments = "-s reload";
		}
		private void nginxRestart()
		{
			_nginxInfo.Arguments = "-s restart";
		}
		

		private void btnNStart_Click(object sender, RoutedEventArgs e)
		{
			if (txtNPath.Text == String.Empty)
			{
				MessageBox.Show("请输入nginx.exe文件的路径！");
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
		{
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
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
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

		private void btnNBrowse_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
			ofd.Filter = "执行文件|*.exe|所有文件|*.*";
			if (ofd.ShowDialog() == true)
			{
				txtNPath.Text = ofd.FileName;
			}
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
	}
}
