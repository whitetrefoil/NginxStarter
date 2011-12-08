using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Serialization;
using Microsoft.Win32;
using NginxStarterGUI.TargetProgramsInfo;

namespace NginxStarterGUI
{

	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		private static Settings.Settings _settings;
		private static bool _inGreenMode = false;
		private static string _configFilePath;
		private static System.Diagnostics.ProcessStartInfo _phpInfo;
		private static System.Diagnostics.Process _php;
		private static NotifyIcon _notifyIcon;
		private static Nginx _nginx;
		private CoffeeScript coffeeScript;
		private Sass sass;

		public MainWindow()
		{
			_configFilePath = AppDomain.CurrentDomain.BaseDirectory + "Nginx Starter GUI.config.xml";
			_settings = readConfigFile();
			coffeeScript = new CoffeeScript();
			InitializeComponent();
			this.txtNPath.Text = _settings.nginx.path;
			this.txtNConfigPath.Text = _settings.nginx.configPath;
			this.txtPPath.Text = _settings.php.path;
			this.txtPConfigPath.Text = _settings.php.configPath;
			this.txtPHost.Text = _settings.php.host;
			this.txtPPort.Text = _settings.php.port.ToString();
			this.chkPUseIniFile.IsChecked = _settings.php.useIniFile;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.saveConfigFile();
			if (_nginx != null)
			{
				_nginx.quit();
			}
		}

		/// <summary>
		/// 从指定文件路径读取程序设置
		/// </summary>
		/// <param name="configFilePath"></param>
		/// <returns>返回程序设置类</returns>
		private Settings.Settings readConfigFile(string configFilePath)
		{
			FileStream fs = null;
			try
			{
				using (fs = File.Open(configFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					XmlSerializer formatter = new XmlSerializer(typeof(Settings.Settings));
					Settings.Settings settings = (Settings.Settings)formatter.Deserialize(fs);
					fs.Close();
					if (settings == null)
						throw new InvalidOperationException("设置文件里什么内容也没有保存");
					return settings;
				}
			}
			catch (FileNotFoundException)
			{
				return new Settings.Settings();
			}
			catch (InvalidOperationException e)
			{
				MessageBoxResult mb = MessageBox.Show(
					"设置文件格式错误，是否重建？\n选“是”将备份原有文件后新建设置文件，如果已有备份文件，将会将其替换，选“否”将继续运行，但是将不会保存设置。\n错误详情：" + e.Message,
					"设置文件格式错误", MessageBoxButton.YesNo, MessageBoxImage.Warning
				);
				if (mb == MessageBoxResult.Yes)
				{
					backupConfigFile(configFilePath);
					return new Settings.Settings();
				}
				else
				{
					_inGreenMode = true;
					return new Settings.Settings();
				}
			}
			catch (FileLoadException)
			{
				MessageBox.Show("设置文件无法读取，请检查您的权限！程序将继续运行，但是不会保存设置！", "读取设置文件出错", MessageBoxButton.OK, MessageBoxImage.Error);
				_inGreenMode = true;
				return new Settings.Settings();
			}
			catch
			{
				return new Settings.Settings();
			}
			finally
			{
				if (fs != null)
					fs.Dispose();
			}
		}

		/// <summary>
		/// 从MainWindow类中设置的默认路径读取设置文件
		/// </summary>
		/// <returns>返回程序设置类</returns>
		private Settings.Settings readConfigFile()
		{
			return this.readConfigFile(_configFilePath);
		}

		/// <summary>
		/// 将指定程序设置，保存到指定设置文件
		/// </summary>
		/// <param name="settings">程序设置</param>
		/// <param name="configFilePath">设置文件路径</param>
		private void saveConfigFile(Settings.Settings settings, string configFilePath)
		{
			if (_inGreenMode == true)
				return;
			try
			{
				using (FileStream fs = File.Open(configFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
				{
					XmlSerializer formatter = new XmlSerializer(typeof(Settings.Settings));
					formatter.Serialize(fs, settings);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show("保存设置文件失败，您本次的设置可能不会被保存，请确认是否拥有在程序运行目录下的读、写、新建文件权限，或与系统管理员联系！\n错误详情：" + e.Message, "保存设置文件出错", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// 将指定程序设置，保存到MainWindow类中的默认设置文件路径
		/// </summary>
		/// <param name="settings">程序设置</param>
		private void saveConfigFile(Settings.Settings settings)
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

		private void backupConfigFile(string configFilePath)
		{
			if (File.Exists(configFilePath + ".bak"))
				File.Delete(configFilePath + ".bak");
			File.Move(configFilePath, configFilePath + ".bak");
		}

		private void backupConfigFile()
		{
			backupConfigFile(_configFilePath);
		}

		/// <summary>
		/// 点击Nginx启动按钮
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void btnNStart_Click(object sender, RoutedEventArgs e)
		{
			_nginx = new Nginx(txtNPath.Text, txtNConfigPath.Text);

			if (txtNPath.Text == String.Empty)
			{
				if (this.btnNBrowse_Fxxk())
					this.btnNStart_Click(sender, e);
			}
			else
			{
				if (_nginx.start())
				{
					_settings.nginx.path = txtNPath.Text;
					_settings.nginx.configPath = txtNConfigPath.Text;
					this.changeButtonsStatusAfterNginxStarted();
				}
			}
		}

		/// <summary>
		/// Nginx启动后激活一些按钮，禁止另一些按钮
		/// </summary>
		private void changeButtonsStatusAfterNginxStarted()
		{
			btnNStart.IsEnabled = false;
			btnNBrowse.IsEnabled = false;
			btnNConfigBrowse.IsEnabled = false;
			txtNPath.IsEnabled = false;
			txtNConfigPath.IsEnabled = false;
			btnNReload.IsEnabled = true;
			btnNRestart.IsEnabled = true;
			btnNQuit.IsEnabled = true;
		}

		/// <summary>
		/// Nginx退出后激活一些按钮，禁止另一些按钮
		/// </summary>
		private void changeButtonsStatusAfterNginxStoped()
		{
			btnNStart.IsEnabled = true;
			btnNBrowse.IsEnabled = true;
			btnNConfigBrowse.IsEnabled = true;
			txtNPath.IsEnabled = true;
			txtNConfigPath.IsEnabled = true;
			btnNReload.IsEnabled = false;
			btnNRestart.IsEnabled = false;
			btnNQuit.IsEnabled = false;
		}

		/// <summary>
		/// 点击NginxExe文件浏览按钮
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void btnNBrowse_Click(object sender, RoutedEventArgs e)
		{
			this.btnNBrowse_Fxxk();
		}

		/// <summary>
		/// 你妹的必须返回void！
		/// </summary>
		/// <returns>返回一个bool值表示是否选取文件成功</returns>
		private bool btnNBrowse_Fxxk()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (_settings.nginx.path != null && _settings.nginx.path != string.Empty)
			{
				ofd.InitialDirectory = _settings.nginx.path;
			}
			else
			{
				ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			}
			ofd.Filter = Nginx._ofdExeFilter;
			ofd.Title = Nginx._ofdExeTitle;
			if (ofd.ShowDialog() == true)
			{
				txtNPath.Text = ofd.FileName;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 点击Nginx配置文件浏览按钮
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <returns>返回一个bool值表示是否选取文件成功</returns>
		public void btnNConfigBrowse_Click(object sender, RoutedEventArgs e)
		{
			this.btnNConfigBrowse_Fxxk();
		}

		/// <summary>
		/// 你妹的必须返回void！
		/// </summary>
		/// <returns>返回一个bool值表示是否选取文件成功</returns>
		private bool btnNConfigBrowse_Fxxk()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (_settings.nginx.configPath != null || _settings.nginx.configPath != string.Empty)
			{
				ofd.InitialDirectory = _settings.nginx.configPath;
			}
			else
			{
				ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\conf";
			}
			ofd.Filter = Nginx._ofdConfigFileFilter;
			ofd.Title = Nginx._ofdConfigFileTitle;
			if (ofd.ShowDialog() == true)
			{
				txtNConfigPath.Text = ofd.FileName;
				return true;
			}
			else
			{
				return false;
			}
		}

		public void btnNReload_Click(object sender, RoutedEventArgs e)
		{
			_nginx.reload();
		}

		public void btnNRestart_Click(object sender, RoutedEventArgs e)
		{
			_nginx.restart();
		}

		public void btnNQuit_Click(object sender, RoutedEventArgs e)
		{
			_nginx.quit();
			changeButtonsStatusAfterNginxStoped();
		}

		public void btnNStop_Click(object sender, RoutedEventArgs e)
		{
			if (_nginx == null)
				Nginx._stop();
			else
				_nginx.stop();
			changeButtonsStatusAfterNginxStoped();
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

		public bool phpStart()
		{
			int port = 0;
			if (this.txtPPort.Text != string.Empty)
			{
				try
				{
					port = Convert.ToUInt16(this.txtPPath.Text);
				}
				catch
				{
					MessageBox.Show("端口号请填入一个整数！", "端口号错误", MessageBoxButton.OK, MessageBoxImage.Error);
					return false;
				}
			}
			_phpInfo = new System.Diagnostics.ProcessStartInfo();
			_phpInfo.Arguments = string.Empty;
			if (this.txtPConfigPath.Text != string.Empty)
				_phpInfo.Arguments += " -c " + this.txtPConfigPath;
			if (port != 0)
				if (this.txtPHost.Text != string.Empty)
					_phpInfo.Arguments += " -b " + this.txtPHost.Text + ":" + port.ToString();
				else
					_phpInfo.Arguments += " -b " + port.ToString();
			if (this.chkPUseIniFile.IsChecked != null && this.chkPUseIniFile.IsChecked == true)
				_phpInfo.Arguments += " -n";
			_phpInfo.FileName = this.txtPPath.Text;
			_phpInfo.WorkingDirectory = this.txtPPath.Text.Substring(0, _settings.php.path.LastIndexOf('\\'));
			_phpInfo.CreateNoWindow = true;
			_phpInfo.UseShellExecute = false;
			try
			{
				_php = System.Diagnostics.Process.Start(_phpInfo);
				_settings.php.path = this.txtPPath.Text;
				_settings.php.configPath = this.txtPConfigPath.Text;
				_settings.php.host = this.txtPHost.Text;
				_settings.php.port = port;
				_settings.php.useIniFile = this.chkPUseIniFile.IsChecked;
				return true;
			}
			catch
			{
				MessageBox.Show("启动失败！");
				return false;
			}
		}

		public bool phpRestart()
		{
			if (phpStop())
				return phpStart();
			else
				return false;
		}

		public bool phpStop()
		{
			try
			{
				_php.Kill();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public string phpBrowse()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (_settings.php.path != null || _settings.php.path != string.Empty)
			{
				ofd.InitialDirectory = _settings.php.path;
			}
			else
			{
				ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			}
			ofd.Filter = "PHP-CGI默认执行文件|php-cgi.exe|所有执行文件|*.exe|所有文件|*.*";
			ofd.Title = "选择PHP-CGI执行文件";
			if (ofd.ShowDialog() == true)
			{
				txtPPath.Text = ofd.FileName;
				return ofd.FileName;
			}
			else
			{
				return string.Empty;
			}
		}

		public string phpConfigBrowse()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (_settings.php.configPath != null || _settings.php.configPath != string.Empty)
			{
				ofd.InitialDirectory = _settings.php.configPath;
			}
			else
			{
				ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			}
			ofd.Filter = "PHP默认设置文件|php.ini|所有设置文件|*.ini|所有文件|*.*";
			ofd.Title = "选择PHP配置文件（php.ini）";
			if (ofd.ShowDialog() == true)
			{
				this.txtPConfigPath.Text = ofd.FileName;
				return ofd.FileName;
			}
			else
			{
				return string.Empty;
			}
		}

		private void btnPStart_Click(object sender, RoutedEventArgs e)
		{
			if (txtPPath.Text == String.Empty)
			{
				if (this.phpBrowse() != string.Empty)
					this.btnPStart_Click(sender, e);
			}
			else
			{
				_settings.php.path = txtPPath.Text;
				this.phpStart();
			}
		}

		private void btnPRestart_Click(object sender, RoutedEventArgs e)
		{
			this.phpRestart();
		}

		private void btnPStop_Click(object sender, RoutedEventArgs e)
		{
			this.phpStop();
		}

		private void btnPConfigBrowse_Click(object sender, RoutedEventArgs e)
		{
			this.phpConfigBrowse();
		}

		private void btnPBrowse_Click(object sender, RoutedEventArgs e)
		{
			this.phpBrowse();
		}

		private void btnCNodePathBrowse_Click(object sender, RoutedEventArgs e)
		{
			this.btnCNodePathBrowse_Fxxk();
		}

		private bool btnCNodePathBrowse_Fxxk()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (_settings.coffee.nodePath != null && _settings.coffee.nodePath != string.Empty)
			{
				ofd.InitialDirectory = _settings.coffee.nodePath;
			}
			else
			{
				ofd.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
			}
			ofd.Filter = CoffeeScript._ofdNodeJsFilter;
			ofd.Title = CoffeeScript._ofdNodeJsTitle;
			if (ofd.ShowDialog() == true)
			{
				txtCNodePath.Text = ofd.FileName;
				return true;
			}
			else
			{
				return false;
			}
		}

		private void btnCCoffeePathBrowse_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btnCInputPathBrowse_Click(object sender, RoutedEventArgs e)
		{

		}

		private void txtCOutputPathBrowse_Click(object sender, RoutedEventArgs e)
		{

		}

		private void setCoffee(bool isWatch = false)
		{
			coffeeScript.nodeJsPath = txtCNodePath.Text;
			coffeeScript.coffeePath = txtCCoffeePath.Text;
			coffeeScript.inputPath = txtCInputPath.Text;
			coffeeScript.outputPath = txtCOutputPath.Text;
			coffeeScript.isNodeInPath = chkCNodeInPath.IsChecked == true;
			coffeeScript.isCoffeeGlobal = chkCCoffeeInGlobal.IsChecked == true;
			coffeeScript.isBare = chkCBare.IsChecked == true;
			coffeeScript.isWatch = isWatch;
		}
		private void btnCStart_Click(object sender, RoutedEventArgs e)
		{
			if (coffeeScript == null)
				coffeeScript = new CoffeeScript();
			setCoffee();
			Binding coffeeMainBinding = new Binding("Message");
			coffeeMainBinding.Source = coffeeScript;
			txtCMain.SetBinding(TextBlock.TextProperty, coffeeMainBinding);
			coffeeScript.start();
		}

		private void btnCWatch_Click(object sender, RoutedEventArgs e)
		{
			if (coffeeScript == null)
				coffeeScript = new CoffeeScript();
			setCoffee(true);
			Binding coffeeMainBinding = new Binding("Message");
			coffeeMainBinding.Source = coffeeScript;
			txtCMain.SetBinding(TextBlock.TextProperty, coffeeMainBinding);
			coffeeScript.start();
			coffeeStartedWatch();
		}

		private void btnCStop_Click(object sender, RoutedEventArgs e)
		{
			if (coffeeScript.stop())
				this.coffeeStopedWatch();
		}

		private void coffeeStartedWatch()
		{
			divCAddOptions.IsEnabled = false;
			divCPaths.IsEnabled = false;
			btnCStart.IsEnabled = false;
			btnCWatch.IsEnabled = false;
			btnCStop.IsEnabled = true;
		}

		private void coffeeStopedWatch()
		{
			divCAddOptions.IsEnabled = true;
			divCPaths.IsEnabled = true;
			btnCStart.IsEnabled = true;
			btnCWatch.IsEnabled = true;
			btnCStop.IsEnabled = false;
		}

		private void chkCNodeInPath_Unchecked(object sender, RoutedEventArgs e)
		{
			chkCCoffeeInGlobal.IsChecked = false;
		}

		private void btnSStart_Click(object sender, RoutedEventArgs e)
		{
			if (sass == null)
				sass = new Sass();

			Binding test = new Binding("Message");
			test.Source = sass;
			txtSMain.SetBinding(TextBlock.TextProperty, test);

		}
	}
}
