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
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32;

namespace GUI
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>

    [Serializable]
    public class Settings : ISerializable
    {
        public string nginxPath { get; set; }
        public string nginxConfigPath { get; set; }
        public string phpPath { get; set; }
        public string phpConfigPath { get; set; }
        public bool? phpUseIniFile { get; set; }
        public short phpPort { get; set; }
        public string phpLocal { get; set; }
    }

	public partial class MainWindow : Window
	{
        private static Settings _settings;
        private static string _configFilePath;
		private static string _nginxPath;
        private static string _nginxConfigPath;
        private static string _phpPath;
        private static string _phpConfigPath;
        private static bool? _phpUseIniFile;
        private static short _phpPort;
        private static string _phpLocal;
        private static System.Diagnostics.ProcessStartInfo _nginxInfo;
		private static System.Diagnostics.Process _nginx;
        private static NotifyIcon _notifyIcon;
        private static RegistryKey _registryKey;

		public MainWindow()
		{
            readConfigFile();
			InitializeComponent();
            _registryKey = Registry.CurrentUser.CreateSubKey("Software\\WhiteTrefoil\\NginxStarterGUI", RegistryKeyPermissionCheck.ReadWriteSubTree);
            this.txtNPath.Text = _registryKey.GetValue("nginxpath", string.Empty).ToString();
            _nginxPath = _registryKey.GetValue("nginxpath", string.Empty).ToString();
            this.txtNConfigPath.Text = _registryKey.GetValue("nginxconfigpath", string.Empty).ToString();
            _nginxConfigPath = _registryKey.GetValue("nginxconfigpath", string.Empty).ToString();
        }

        private void readConfigFile()
        {
            _configFilePath = AppDomain.CurrentDomain.BaseDirectory + "Nginx Starter GUI.config.xml";
            try
            {
                FileStream file = File.Open(_configFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }
            catch
            {
                MessageBox.Show("读取配置文件失败且无法创建，请确认是否拥有在程序运行目录下的读、写、新建文件权限，或与系统管理员联系！", "读取配置文件出错", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void saveRegistry()
        {
            _registryKey.SetValue("nginxpath", _nginxPath);
            _registryKey.SetValue("nginxConfigPath", _nginxConfigPath);
            XmlSerializer s = new XmlSerializer(typeof(Settings));
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
        public string nginxBrowse()
        {
			OpenFileDialog ofd = new OpenFileDialog();
            if (_nginxPath != null || _nginxPath != string.Empty)
            {
                ofd.InitialDirectory = _nginxPath;
            }
            else
            {
                ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
			ofd.Filter = "Nginx默认执行文件|nginx.exe|所有执行文件|*.exe|所有文件|*.*";
			if (ofd.ShowDialog() == true)
			{
				txtNPath.Text = ofd.FileName;
                return ofd.FileName;
			}
            else
            {
                return string.Empty;
            }
		}
        public string nginxConfigBrowse()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (_nginxConfigPath != null || _nginxConfigPath != string.Empty)
            {
                ofd.InitialDirectory = _nginxConfigPath;
            }
            else
            {
                ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\conf";
            }
            ofd.Filter = "Nginx默认配置文件|nginx.conf|所有配置文件|*.conf|所有文件|*.*";
            if (ofd.ShowDialog() == true)
            {
                txtNConfigPath.Text = ofd.FileName;
                return ofd.FileName;
            }
            else
            {
                return string.Empty;
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

        private void btnNConfigBrowse_Click(object sender, RoutedEventArgs e)
        {
            this.nginxConfigBrowse();
        }
	}
}
