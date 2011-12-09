using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using NginxStarterGUI.Classes;
using System.Security;
using System.Security.Permissions;
using System;

namespace NginxStarterGUI.TargetProgramsInfo
{
	public class Sass : INotifyPropertyChanged, IDisposable
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private Process process;
		public string RubyPath { get; set; }
		public string SassPath { get; set; }
		public string InputPath { get; set; }
		public string OutputPath { get; set; }
		private bool isWatch;
		public bool IsInPath { get; set; }
		public bool IsUseLF { get; set; }
		public bool IsForce { get; set; }
		public bool IsNoCache { get; set; }

		private string message;
		public string Message
		{
			get { return message; }
			set
			{
				message = value;
				OnPropertyChanged("Message");
			}
		}

		public const string OfdRubyFilter = "Ruby默认执行文件|ruby.exe|所有执行文件|*.exe|所有文件|*.*";
		public const string OfdRubyTitle = "选择Ruby执行文件";
		public const string OfdCoffeeFilter = "SASS二进制文件|sass|所有文件|*.*";
		public const string OfdCoffeeTitle = "选择SASS二进制文件";
		public const string OfdInputFilter = "SASS 文件 或 目录|*.sass *.scss";
		public const string OfdInputTitle = "选择输入文件/目录";
		public const string OfdOutputFilter = "";
		public const string OfdOutputTitle = "选择输出文件/目录";

		public Sass()
		{
			message = "asdf";
		}

		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = false)]
		public void SetTestData()
		{
			this.RubyPath = "ruby.exe";
			this.SassPath = "sass";
			this.InputPath = "C:\\temp";
			this.OutputPath = "C:\\temp";
			this.isWatch = false;
			this.IsInPath = false;
			this.IsUseLF = false;
			this.IsForce = true;
			this.IsNoCache = true;

			this.Start();
		}

		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted=false)]
		public void Start()
		{
			this.process = new Process();
			ProcessStartInfo info = new ProcessStartInfo();
			if (this.IsInPath)
			{
				this.RubyPath = FindInPath.Find("ruby.exe", MainWindow.WorkingDirectory, false);
				this.SassPath = FindInPath.Find("sass", MainWindow.WorkingDirectory, true, true);
			}
			this.InputPath = this.InputPath.Replace(Path.DirectorySeparatorChar, '/');
			this.OutputPath = this.OutputPath.Replace(Path.DirectorySeparatorChar, '/');
			info.FileName = this.RubyPath;
			info.Arguments = string.Empty;
			if (this.IsUseLF)
				info.Arguments += " --unix-newlines";
			if (this.IsForce)
				info.Arguments += " --force";
			if (this.IsNoCache)
				info.Arguments += " --no-cache";
			if (this.isWatch)
				info.Arguments += " --watch " + this.InputPath + ":" + this.OutputPath;
			else
				info.Arguments += " --update " + this.InputPath + ":" + this.OutputPath;
		}

		private void OnPropertyChanged(string info)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(info));
			}
		}

		#region Dispose Region

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~Sass()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				if (process != null)
				{
					process.Dispose();
					process = null;
				}
			}
		}

		#endregion
	}

	public class SassCodeStyle : List<string>
	{
		public SassCodeStyle()
			: base()
		{
			Add("nested");
			Add("compact");
			Add("compressed");
			Add("expanded");
		}
	}
}
