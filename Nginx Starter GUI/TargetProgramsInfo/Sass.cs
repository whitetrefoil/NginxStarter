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
		private BackgroundWorker processWorker;
		public string RubyPath { get; set; }
		public string SassPath { get; set; }
		public string InputPath { get; set; }
		public string OutputPath { get; set; }
		public bool IsWatch { get; set; }
		public bool IsRubyInPath { get; set; }
		public bool IsUseLF { get; set; }
		public bool IsForce { get; set; }
		public bool IsNoCache { get; set; }
		public string CodeStyle { get; set; }

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
		public const string OfdSassFilter = "SASS二进制文件|sass|所有文件|*.*";
		public const string OfdSassTitle = "选择SASS二进制文件";
		public const string OfdInputFilter = "SASS 文件 或 目录|*.sass;*.scss";
		public const string OfdInputTitle = "选择输入文件/目录";
		public const string OfdOutputFilter = "目录|*.folder";
		public const string OfdOutputTitle = "选择输出文件/目录";

		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted=false)]
		public bool Start()
		{
			this.process = new Process();
			ProcessStartInfo info = new ProcessStartInfo();
			if (this.IsRubyInPath)
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

		public bool Stop()
		{
			return true;
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
