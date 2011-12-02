using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace NginxStarterGUI.TargetProgramsInfo
{
	class Nginx
	{
		private ProcessStartInfo nginxInfo;
		private Process nginxProcess;
		public string exePath { private get; set; }
		public string configPath { private get; set; }
		public static const string _ofdExeFilter = "Nginx默认执行文件|nginx.exe|所有执行文件|*.exe|所有文件|*.*";
		public static const string _ofdExeTitle = "选择Nginx执行文件";
		public static const string _ofdConfigFileFilter = "Nginx默认设置文件|nginx.conf|所有设置文件|*.conf|所有文件|*.*";
		public static const string _ofdConfigFileTitle = "选择Nginx主配置文件";

		/// <summary>
		/// 创建一个nginx
		/// </summary>
		/// <param name="exePath">exe文件路径</param>
		/// <param name="configPath">配置文件路径</param>
		public Nginx(string exePath, string configPath)
		{
			this.exePath = exePath;
			this.configPath = configPath;
		}

		/// <summary>
		/// 创建一个Nginx
		/// </summary>
		/// <param name="exePath">exe文件路径</param>
		public Nginx(string exePath)
			: this(exePath, string.Empty)
		{
		}

		/// <summary>
		/// 启动Nginx
		/// </summary>
		/// <returns>返回一个bool值表示启动成功与否</returns>
		public bool start()
		{
			this.nginxInfo = new ProcessStartInfo();
			this.nginxInfo.Arguments = string.Empty;
			if (this.configPath != string.Empty)
				this.nginxInfo.Arguments += " -c " + this.configPath;
			this.nginxInfo.FileName = this.exePath;
			this.nginxInfo.WorkingDirectory = this.exePath.Substring(0, this.exePath.LastIndexOf('\\'));
			this.nginxInfo.CreateNoWindow = true;
			this.nginxInfo.UseShellExecute = false;
			try
			{
				this.nginxProcess = Process.Start(this.nginxInfo);
				return true;
			}
			catch
			{
				MessageBox.Show("启动失败！");
				return false;
			}
		}

		/// <summary>
		/// 停止Nginx
		/// </summary>
		/// <returns>返回一个bool值表示停止成功与否</returns>
		public bool stop()
		{
			try
			{
				this.nginxInfo.Arguments = "-s stop";
				Process.Start(this.nginxInfo);
				return true;
			}
			catch
			{
				MessageBox.Show("停止失败！");
				return false;
			}
		}

		/// <summary>
		/// 强制关闭所有nginx进程，
		/// </summary>
		/// <returns>返回一个bool值，该值只表示执行代码期间是否遇到错误，并不代表是否成功</returns>
		public bool quit()
		{
			try
			{
				this.nginxInfo.Arguments = "-s quit";
				Process.Start(this.nginxInfo);
				Process[] nginxs = Process.GetProcessesByName("nginx.exe");
				foreach (Process nginx in nginxs)
				{
					nginx.Kill();
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 调用Nginx自身reload参数重载配置文件
		/// </summary>
		public void reload()
		{
			try
			{
				this.nginxInfo.Arguments = "-s reload";
				Process.Start(this.nginxInfo);
			}
			catch
			{ }
		}

		/// <summary>
		/// 调用Nginx自身restart参数重启Nginx
		/// </summary>
		public void restart()
		{
			try
			{
				this.nginxInfo.Arguments = "-s restart";
				Process.Start(this.nginxInfo);
			}
			catch
			{ }
		}
	}
}
