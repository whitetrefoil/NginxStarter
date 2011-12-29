using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Serialization;
using Microsoft.Win32;
using NginxStarterGUI.TargetProgramsInfo;

[assembly: CLSCompliant(true)]
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
		public static readonly string WorkingDirectory = Directory.GetCurrentDirectory();
		private static NotifyIcon notifyIcon;
		private Nginx nginx;
		private CoffeeScript coffeeScript;
		private Sass sass;

		public MainWindow()
		{
			InitializeComponent();
			_configFilePath = AppDomain.CurrentDomain.BaseDirectory + "Nginx Starter GUI.config.xml";
			_settings = readConfigFile();
			this.DataContext = _settings;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.saveConfigFile();
			if (nginx != null)
				nginx.quit();
		}

		private void Window_StateChanged(object sender, EventArgs e)
		{
			if (this.WindowState == WindowState.Minimized)
			{
				if (nginx != null)
					notifyIcon = new NotifyIcon(this, nginx.IsRunning);
				else
					notifyIcon = new NotifyIcon(this, false);
				this.ShowInTaskbar = false;
			}
			else
			{
				this.ShowInTaskbar = true;
				if (notifyIcon != null)
					notifyIcon.Dispose();
			}
		}

		#region Config File Region

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

		#endregion

		#region Nginx Region

		/// <summary>
		/// 点击Nginx启动按钮
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void btnNStart_Click(object sender, RoutedEventArgs e)
		{
			nginx = new Nginx(txtNPath.Text, txtNConfigPath.Text);

			if (txtNPath.Text == String.Empty)
			{
				if (this.btnNBrowse_Fxxk())
					this.btnNStart_Click(sender, e);
			}
			else
			{
				if (nginx.start())
				{
					_settings.Nginx.Path = txtNPath.Text;
					_settings.Nginx.ConfigPath = txtNConfigPath.Text;
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
			if (notifyIcon != null)
				notifyIcon.ChangeOptionsAfterNginxStarted();
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
			if (notifyIcon != null)
				notifyIcon.ChangeOptionsAfterNginxStoped();
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
			if (_settings.Nginx.Path != null && _settings.Nginx.Path != string.Empty)
			{
				ofd.InitialDirectory = _settings.Nginx.Path;
			}
			else
			{
				ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
			}
			ofd.Filter = Nginx.OfdExeFilter;
			ofd.Title = Nginx.OfdExeTitle;
			ofd.FileName = Nginx.OfdExeFileName;
			if (ofd.ShowDialog() == true)
			{
				txtNPath.Focus();
				txtNPath.Text = ofd.FileName;
				btnNBrowse.Focus();
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
			if (_settings.Nginx.ConfigPath != null || _settings.Nginx.ConfigPath != string.Empty)
			{
				ofd.InitialDirectory = _settings.Nginx.ConfigPath;
			}
			else
			{
				ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\conf";
			}
			ofd.Filter = Nginx.OfdConfigFileFilter;
			ofd.Title = Nginx.OfdConfigFileTitle;
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
			nginx.reload();
		}

		public void btnNRestart_Click(object sender, RoutedEventArgs e)
		{
			nginx.restart();
		}

		public void btnNQuit_Click(object sender, RoutedEventArgs e)
		{
			nginx.quit();
			changeButtonsStatusAfterNginxStoped();
		}

		public void btnNStop_Click(object sender, RoutedEventArgs e)
		{
			if (nginx == null)
				Nginx._stop();
			else
				nginx.stop();
			changeButtonsStatusAfterNginxStoped();
		}

		#endregion

		#region PHP Region

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
			_phpInfo.WorkingDirectory = this.txtPPath.Text.Substring(0, _settings.Php.Path.LastIndexOf('\\'));
			_phpInfo.CreateNoWindow = true;
			_phpInfo.UseShellExecute = false;
			try
			{
				_php = System.Diagnostics.Process.Start(_phpInfo);
				_settings.Php.Path = this.txtPPath.Text;
				_settings.Php.ConfigPath = this.txtPConfigPath.Text;
				_settings.Php.Host = this.txtPHost.Text;
				_settings.Php.Port = port;
				_settings.Php.UseIniFile = this.chkPUseIniFile.IsChecked;
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
			if (_settings.Php.Path != null || _settings.Php.Path != string.Empty)
			{
				ofd.InitialDirectory = _settings.Php.Path;
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
			if (_settings.Php.ConfigPath != null || _settings.Php.ConfigPath != string.Empty)
			{
				ofd.InitialDirectory = _settings.Php.ConfigPath;
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
				_settings.Php.Path = txtPPath.Text;
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

		#endregion

		#region Coffee-Script Region

		private void btnCNodePathBrowse_Click(object sender, RoutedEventArgs e)
		{
			this.btnCNodePathBrowse_Fxxk();
		}

		private bool btnCNodePathBrowse_Fxxk()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (_settings.Coffee.NodePath != null)
				ofd.InitialDirectory = _settings.Coffee.NodePath;
			ofd.Filter = CoffeeScript.OfdNodeJsFilter;
			ofd.Title = CoffeeScript.OfdNodeJsTitle;
			if (ofd.ShowDialog() == true)
			{
				txtCNodePath.Focus();
				txtCNodePath.Text = ofd.FileName;
				btnCNodePathBrowse.Focus();
				return true;
			}
			else
			{
				return false;
			}
		}

		private void btnCCoffeePathBrowse_Click(object sender, RoutedEventArgs e)
		{
			btnCCoffeePathBrowse_Fxxk();
		}

		private bool btnCCoffeePathBrowse_Fxxk()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (_settings.Coffee.NodePath != null)
				ofd.InitialDirectory = _settings.Coffee.NodePath;
			ofd.Filter = CoffeeScript.OfdCoffeeFilter;
			ofd.Title = CoffeeScript.OfdCoffeeTitle;
			if (ofd.ShowDialog() == true)
			{
				txtCCoffeePath.Focus();
				txtCCoffeePath.Text = ofd.FileName;
				btnCCoffeePathBrowse.Focus();
				return true;
			}
			else
			{
				return false;
			}
		}

		private void btnCInputPathBrowse_Click(object sender, RoutedEventArgs e)
		{
			btnCInputPathBrowse_Fxxk();
		}

		private bool btnCInputPathBrowse_Fxxk()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (_settings.Coffee.InputPath != null)
				ofd.InitialDirectory = _settings.Coffee.InputPath;
			ofd.Filter = CoffeeScript.OfdInputFilter;
			ofd.FileName = "文件名会被忽略";
			ofd.CheckFileExists = false;
			ofd.CheckPathExists = true;
			ofd.ValidateNames = false;
			ofd.AddExtension = false;
			if (ofd.ShowDialog() == true)
			{
				if (!ofd.FileName.EndsWith(".coffee", true, CultureInfo.CurrentCulture) || !File.Exists(ofd.FileName))
				{
					int lastSeparatorIndex = ofd.FileName.LastIndexOf('\\');
					ofd.FileName = ofd.FileName.Remove(lastSeparatorIndex);
				}
				txtCInputPath.Focus();
				txtCInputPath.Text = ofd.FileName;
				btnCInputPathBrowse.Focus();
				return true;
			}
			return false;
		}

		private void btnCOutputPathBrowse_Click(object sender, RoutedEventArgs e)
		{
			btnCOutputPathBrowse_Fxxk();
		}

		private bool btnCOutputPathBrowse_Fxxk()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (_settings.Coffee.OutputPath != null)
				ofd.InitialDirectory = _settings.Coffee.OutputPath;
			ofd.Filter = CoffeeScript.OfdOutputFilter;
			ofd.FileName = "文件名会被忽略";
			ofd.CheckFileExists = false;
			ofd.CheckPathExists = true;
			ofd.ValidateNames = false;
			ofd.AddExtension = false;
			if (ofd.ShowDialog() == true)
			{
				int lastSeparatorIndex = ofd.FileName.LastIndexOf('\\');
				ofd.FileName = ofd.FileName.Remove(lastSeparatorIndex);
				txtCOutputPath.Focus();
				txtCOutputPath.Text = ofd.FileName;
				btnCOutputPathBrowse.Focus();
				return true;
			}
			return false;
		}

		private void setCoffee(bool isWatch = false)
		{
			coffeeScript.NodeJsPath = txtCNodePath.Text;
			coffeeScript.CoffeePath = txtCCoffeePath.Text;
			coffeeScript.InputPath = txtCInputPath.Text;
			coffeeScript.OutputPath = txtCOutputPath.Text;
			coffeeScript.IsNodeInPath = chkCNodeInPath.IsChecked == true;
			coffeeScript.IsCoffeeGlobal = chkCCoffeeInGlobal.IsChecked == true;
			coffeeScript.IsBare = chkCBare.IsChecked == true;
			coffeeScript.IsWatch = isWatch;
		}

		private void setCoffeeEvents()
		{
			coffeeScript.MessageUpdated += (sender, e) =>
				slvCMain.ScrollToBottom();
			coffeeScript.ProcessExited += (sender, e) =>
				coffeeWatchStoped();
		}

		private void btnCStart_Click(object sender, RoutedEventArgs e)
		{
			if (coffeeScript == null)
				coffeeScript = new CoffeeScript();
			setCoffee();
			Binding coffeeMainBinding = new Binding("Message");
			coffeeMainBinding.Source = coffeeScript;
			txtCMain.SetBinding(TextBlock.TextProperty, coffeeMainBinding);
			setCoffeeEvents();
			if (coffeeScript.Start())
				coffeeWatchStarted();
		}

		private void btnCWatch_Click(object sender, RoutedEventArgs e)
		{
			if (coffeeScript == null)
				coffeeScript = new CoffeeScript();
			setCoffee(true);
			Binding coffeeMainBinding = new Binding("Message");
			coffeeMainBinding.Source = coffeeScript;
			txtCMain.SetBinding(TextBlock.TextProperty, coffeeMainBinding);
			setCoffeeEvents();
			if (coffeeScript.Start())
				coffeeWatchStarted();
		}

		private void btnCStop_Click(object sender, RoutedEventArgs e)
		{
			if (coffeeScript.Stop())
				this.coffeeWatchStoped();
		}

		private void coffeeWatchStarted()
		{
			divCAddOptions.IsEnabled = false;
			divCPaths.IsEnabled = false;
			btnCStart.IsEnabled = false;
			btnCWatch.IsEnabled = false;
			btnCStop.IsEnabled = true;
		}

		private void coffeeWatchStoped()
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

		private void txtCMain_TextInput(object sender, TextCompositionEventArgs e)
		{
			slvCMain.ScrollToBottom();
		}

		#endregion

		#region SASS Region

		private void setSass(bool isWatch)
		{
			sass.RubyPath = _settings.Sass.RubyPath;
			sass.SassPath = _settings.Sass.SassPath;
			sass.InputPath = _settings.Sass.InputPath;
			sass.OutputPath = _settings.Sass.OutputPath;
			sass.IsForce = _settings.Sass.IsForce;
			sass.IsRubyInPath = _settings.Sass.IsRubyInPath;
			sass.IsUseLF = _settings.Sass.IsUseLF;
			sass.IsNoCache = _settings.Sass.IsNoCache;
			sass.CodeStyle = _settings.Sass.CodeStyle;
			sass.IsWatch = isWatch;
		}

		private void setSassEvents()
		{
			sass.MessageUpdated += (sender, e) =>
				slvSMain.ScrollToBottom();
			sass.ProcessExited += (sender, e) =>
				sassWatchStoped();
		}

		private void btnSStart_Click(object sender, RoutedEventArgs e)
		{
			if (sass == null)
				sass = new Sass();
			setSass(false);
			Binding sassMainBinding = new Binding("Message");
			sassMainBinding.Source = sass;
			txtSMain.SetBinding(TextBlock.TextProperty, sassMainBinding);
			setSassEvents();
			if (sass.Start())
				sassWatchStarted();
		}

		private void btnSWatch_Click(object sender, RoutedEventArgs e)
		{
			if (sass == null)
				sass = new Sass();
			setSass(true);
			Binding sassMainBinding = new Binding("Message");
			sassMainBinding.Source = sass;
			txtSMain.SetBinding(TextBlock.TextProperty, sassMainBinding);
			setSassEvents();
			if (sass.Start())
				sassWatchStarted();
		}

		private void btnSStop_Click(object sender, RoutedEventArgs e)
		{
			if (sass.Stop())
				sassWatchStoped();
		}

		private void btnSRubyPathBrowse_Click(object sender, RoutedEventArgs e)
		{
			btnSRubyPathBrowse_Fxxk();
		}

		private bool btnSRubyPathBrowse_Fxxk()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (String.IsNullOrEmpty(_settings.Sass.RubyPath))
				ofd.InitialDirectory = _settings.Sass.RubyPath;
			ofd.Filter = Sass.OfdRubyFilter;
			ofd.Title = Sass.OfdRubyTitle;
			if (ofd.ShowDialog() == true)
			{
				txtSRubyPath.Focus();
				txtSRubyPath.Text = ofd.FileName;
				btnSRubyPathBrowse.Focus();
				return true;
			}
			else
			{
				return false;
			}
		}

		private void btnSSassPathBrowse_Click(object sender, RoutedEventArgs e)
		{
			btnSSassPathBrowse_Fxxk();
		}

		private bool btnSSassPathBrowse_Fxxk()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (String.IsNullOrEmpty(_settings.Sass.SassPath))
				ofd.InitialDirectory = _settings.Sass.SassPath;
			ofd.Filter = Sass.OfdSassFilter;
			ofd.Title = Sass.OfdSassTitle;
			if (ofd.ShowDialog() == true)
			{
				txtSSassPath.Focus();
				txtSSassPath.Text = ofd.FileName;
				btnSSassPathBrowse.Focus();
				return true;
			}
			else
				return false;
		}

		private void btnSInputPathBrowse_Click(object sender, RoutedEventArgs e)
		{
			btnSInputPathBrowse_Fxxk();
		}

		private bool btnSInputPathBrowse_Fxxk()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (_settings.Sass.InputPath != null)
				ofd.InitialDirectory = _settings.Sass.InputPath;
			ofd.Filter = Sass.OfdInputFilter;
			ofd.FileName = "选择一个明确的Sass文件或保留这段文字";
			ofd.CheckFileExists = false;
			ofd.CheckPathExists = true;
			ofd.ValidateNames = false;
			ofd.AddExtension = false;
			if (ofd.ShowDialog() == true)
			{
				if (!ofd.FileName.EndsWith(".sass", true, CultureInfo.CurrentCulture) ||
					!ofd.FileName.EndsWith(".scss", true, CultureInfo.CurrentCulture) ||
					!File.Exists(ofd.FileName))
				{
					int lastSeparatorIndex = ofd.FileName.LastIndexOf('\\');
					ofd.FileName = ofd.FileName.Remove(lastSeparatorIndex);
				}
				txtSInputPath.Focus();
				txtSInputPath.Text = ofd.FileName;
				btnSInputPathBrowse.Focus();
				return true;
			}
			return false;
		}

		private void btnSOutputPathBrowse_Click(object sender, RoutedEventArgs e)
		{
			btnSOutputPathBrowse_Fxxk();
		}

		private bool btnSOutputPathBrowse_Fxxk()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (_settings.Sass.OutputPath != null)
				ofd.InitialDirectory = _settings.Sass.OutputPath;
			ofd.Filter = Sass.OfdOutputFilter;
			ofd.FileName = "文件名会被忽略";
			ofd.CheckFileExists = false;
			ofd.CheckPathExists = true;
			ofd.ValidateNames = false;
			ofd.AddExtension = false;
			if (ofd.ShowDialog() == true)
			{
				if (!ofd.FileName.EndsWith(".sass", true, CultureInfo.CurrentCulture) ||
					!ofd.FileName.EndsWith(".scss", true, CultureInfo.CurrentCulture) ||
					!File.Exists(ofd.FileName))
				{
					int lastSeparatorIndex = ofd.FileName.LastIndexOf('\\');
					ofd.FileName = ofd.FileName.Remove(lastSeparatorIndex);
				}
				txtSOutputPath.Focus();
				txtSOutputPath.Text = ofd.FileName;
				btnSOutputPathBrowse.Focus();
				return true;
			}
			return false;
		}

		private void sassWatchStarted()
		{
			divSAddOptions.IsEnabled = false;
			divSPaths.IsEnabled = false;
			btnSStart.IsEnabled = false;
			btnSWatch.IsEnabled = false;
			btnSStop.IsEnabled = true;
		}

		private void sassWatchStoped()
		{
			divSAddOptions.IsEnabled = true;
			divSPaths.IsEnabled = true;
			btnSStart.IsEnabled = true;
			btnSWatch.IsEnabled = true;
			btnSStop.IsEnabled = false;
		}

		#endregion

		#region LESS Region
		
		#endregion
	}
}
