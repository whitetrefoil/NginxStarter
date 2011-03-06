﻿using System;
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

namespace NginxStarterGUI
{

	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		private static Settings _settings;
		private static bool _inGreenMode = false;
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
			_configFilePath = AppDomain.CurrentDomain.BaseDirectory + "Nginx Starter GUI.config.xml";
			_settings = readConfigFile();
			InitializeComponent();
			_registryKey = Registry.CurrentUser.CreateSubKey("Software\\WhiteTrefoil\\NginxStarterGUI", RegistryKeyPermissionCheck.ReadWriteSubTree);
			this.txtNPath.Text = _registryKey.GetValue("nginxpath", string.Empty).ToString();
			_nginxPath = _registryKey.GetValue("nginxpath", string.Empty).ToString();
			this.txtNConfigPath.Text = _registryKey.GetValue("nginxconfigpath", string.Empty).ToString();
			_nginxConfigPath = _registryKey.GetValue("nginxconfigpath", string.Empty).ToString();
		}

		/// <summary>
		/// 从指定文件路径读取程序设置
		/// </summary>
		/// <param name="configFilePath"></param>
		/// <returns>返回程序设置类</returns>
		private Settings readConfigFile(string configFilePath)
		{
			try
			{
				using (FileStream fs = File.Open(configFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					XmlSerializer formatter = new XmlSerializer(typeof(Settings));
					Settings settings = (Settings)formatter.Deserialize(fs);
					fs.Close();
					return settings;
				}
			}
			catch (FileNotFoundException)
			{
				createConfigFile(configFilePath);
				return null;
			}
			catch (InvalidOperationException e)
			{
				MessageBoxResult mb = MessageBox.Show(
					"设置文件格式错误，是否重建？\n选“是”将备份原有文件后新建设置文件，如果已有备份文件，将会将其替换，选“否”将继续运行，但是将不会保存设置。\n错误详情：" + e.Message,
					"设置文件格式错误", MessageBoxButton.YesNo, MessageBoxImage.Warning
				);
				if (mb == MessageBoxResult.Yes)
				{
					createConfigFile(configFilePath);
					return null;
				}
				else
				{
					_inGreenMode = true;
					return null;
				}
			}
			catch (FileLoadException)
			{
				MessageBox.Show("设置文件无法读取，请检查您的权限！程序将继续运行，但是不会保存设置！", "读取设置文件出错", MessageBoxButton.OK, MessageBoxImage.Error);
				_inGreenMode = true;
				return null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// 从MainWindow类中设置的默认路径读取设置文件
		/// </summary>
		/// <returns>返回程序设置类</returns>
		private Settings readConfigFile()
		{
			return this.readConfigFile(_configFilePath);
		}

		/// <summary>
		/// 将指定程序设置，保存到指定设置文件
		/// </summary>
		/// <param name="settings">程序设置</param>
		/// <param name="configFilePath">设置文件路径</param>
		private void saveConfigFile(Settings settings, string configFilePath)
		{
			if (_inGreenMode == true)
				return;
			try
			{
				using (FileStream fs = File.Open(configFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
				{
					XmlSerializer formatter = new XmlSerializer(typeof(Settings));
					formatter.Serialize(fs, settings);
				}
			}
			catch(Exception e)
			{
				MessageBox.Show("保存设置文件失败，您本次的设置可能不会被保存，请确认是否拥有在程序运行目录下的读、写、新建文件权限，或与系统管理员联系！\n错误详情：" + e.Message, "保存设置文件出错", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// 将指定程序设置，保存到MainWindow类中的默认设置文件路径
		/// </summary>
		/// <param name="settings">程序设置</param>
		private void saveConfigFile(Settings settings)
		{
			this.saveConfigFile(settings, _configFilePath);
		}

		/// <summary>
		/// 将MainWindow类中的当前设置，保存到指定设置文件
		/// </summary>
		/// <param name="configFilePath">设置文件路径</param>
		private void saveConfigFile(string configFilePath)
		{
			this.saveConfigFile(_settings, configFilePath);
		}

		/// <summary>
		/// 将MainWindow类中的当前设置，保存到MainWindow类中的默认设置文件路径
		/// </summary>
		private void saveConfigFile()
		{
			this.saveConfigFile(_settings, _configFilePath);
		}

		private void createConfigFile(string configFilePath)
		{
			if (File.Exists(configFilePath))
			{
				if (File.Exists(configFilePath + ".bak"))
					File.Delete(configFilePath + ".bak");
				File.Move(configFilePath, configFilePath + ".bak");
			}
			File.Create(configFilePath).Close();
		}
		private void createConfigFile()
		{
			createConfigFile(_configFilePath);
		}

		private void saveRegistry()
		{
			_registryKey.SetValue("nginxpath", _nginxPath);
			_registryKey.SetValue("nginxConfigPath", _nginxConfigPath);
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
			this.saveConfigFile();
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
			ofd.Filter = "Nginx默认设置文件|nginx.conf|所有设置文件|*.conf|所有文件|*.*";
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
				if (_notifyIcon != null) _notifyIcon.Dispose();
			}
		}

		private void btnNConfigBrowse_Click(object sender, RoutedEventArgs e)
		{
			this.nginxConfigBrowse();
		}
	}
}
