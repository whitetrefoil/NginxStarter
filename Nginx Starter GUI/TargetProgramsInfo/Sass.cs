using System;
using System.Diagnostics;
using System.Windows;
using NginxStarterGUI.Classes;
using System.IO;
using System.ComponentModel;

namespace NginxStarterGUI.TargetProgramsInfo
{
	public class Sass : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private Process process;
		public string rubyPath { private get; set; }
		public string sassPath { private get; set; }
		public string inputPath { private get; set; }
		public string outputPath { private get; set; }
		private bool isWatch;
		public bool isInPath { private get; set; }
		public bool isUseLF { private get; set; }
		public bool isForce { private get; set; }
		public bool isNoCache { private get; set; }

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

		public const string _ofdRubyFilter = "Ruby默认执行文件|ruby.exe|所有执行文件|*.exe|所有文件|*.*";
		public const string _ofdNodeJsTitle = "选择Ruby执行文件";
		public const string _ofdCoffeeFilter = "SASS二进制文件|sass|所有文件|*.*";
		public const string _ofdCoffeeTitle = "选择SASS二进制文件";
		public const string _ofdInputFilter = "";
		public const string _ofdInputTitle = "选择输入文件/目录";
		public const string _ofdOutputFilter = "";
		public const string _ofdOutputTitle = "选择输出文件/目录";

		public Sass()
		{
			message = "asdf";
		}

		public void setTestData()
		{
			this.rubyPath = "ruby.exe";
			this.sassPath = "sass";
			this.inputPath = "C:\\temp";
			this.outputPath = "C:\\temp";
			this.isWatch = false;
			this.isInPath = false;
			this.isUseLF = false;
			this.isForce = true;
			this.isNoCache = true;

			this.start();
		}

		public void start()
		{
			this.process = new Process();
			if (this.isInPath)
			{
				this.rubyPath = FindInPath.Find("ruby.exe", false);
				this.sassPath = FindInPath.Find("sass", true, true);
			}
			this.inputPath = this.inputPath.Replace(Path.DirectorySeparatorChar, '/');
			this.outputPath = this.outputPath.Replace(Path.DirectorySeparatorChar, '/');
			this.process.StartInfo.FileName = this.rubyPath;
			this.process.StartInfo.Arguments = string.Empty;
			if (this.isUseLF)
				this.process.StartInfo.Arguments += " --unix-newlines";
			if (this.isForce)
				this.process.StartInfo.Arguments += " --force";
			if (this.isNoCache)
				this.process.StartInfo.Arguments += " --no-cache";
			if (this.isWatch)
				this.process.StartInfo.Arguments += " --watch " + this.inputPath + ":" + this.outputPath;
			else
				this.process.StartInfo.Arguments += " --update " + this.inputPath + ":" + this.outputPath;


		}

		private void OnPropertyChanged(string info)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(info));
			}
		}
	}
}
