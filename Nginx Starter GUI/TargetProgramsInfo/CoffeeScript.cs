using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using NginxStarterGUI.Classes;

namespace NginxStarterGUI.TargetProgramsInfo
{
	class CoffeeScript
	{
		private ProcessStartInfo processStartInfo;
		private Process process;
		public string nodeJsPath { private get; set; }
		public string coffeePath { private get; set; }
		public string inputPath { private get; set; }
		public string outputPath { private get; set; }
		public bool isWatch { private get; set; }
		public bool isCoffeeGlobal { private get; set; }
		public string message { private set; get; }

		public const string _ofdNodeJsFilter = "Nodejs默认执行文件|node.exe|所有执行文件|*.exe|所有文件|*.*";
		public const string _ofdNodeJsTitle = "选择Nodejs执行文件";
		public const string _ofdCoffeeFilter = "Coffee默认二进制文件|coffee|所有文件|*.*";
		public const string _ofdCoffeeTitle = "选择Coffee二进制文件";
		public const string _ofdInputFilter = "";
		public const string _ofdInputTitle = "选择输入文件/目录";
		public const string _ofdOutputFilter = "";
		public const string _ofdOutputTitle = "选择输出文件/目录";

		public bool testStart()
		{
			this.nodeJsPath = "C:\\Program Files (x86)\\nodejs\\node.exe";
			this.coffeePath = "C:\\Program Files (x86)\\nodejs\\node_modules\\coffee-script\\bin\\coffee";
			this.inputPath = "C:\\temp";
			this.outputPath = "C:\\temp";
			this.isCoffeeGlobal = true;
			this.isWatch = false;
			return this.start();
		}
		public bool start()
		{
			try
			{
				if (this.isCoffeeGlobal)
					this.coffeePath = FindInPath.Find("coffee");
				this.processStartInfo = new ProcessStartInfo();
				if (this.isCoffeeGlobal)
				    this.processStartInfo.FileName = FindInPath.Find("coffee");
				//else
				//{
				//    this.processStartInfo.FileName = this.nodeJsPath;
				//    this.processStartInfo.Arguments += this.coffeePath + " ";
				//}
				//if (this.outputPath != string.Empty)
				//    this.processStartInfo.Arguments += "-o " + this.outputPath + " ";
				//if (this.isWatch)
				//    this.processStartInfo.Arguments += "-w ";
				//this.processStartInfo.Arguments += "-c " + this.inputPath;
				this.processStartInfo.FileName = "coffee";
				this.processStartInfo.Arguments = "-o c:\\temp -c C:\\temp";
				this.process = Process.Start(this.processStartInfo);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool watch()
		{
			return true;
		}

		public bool stop()
		{
			return true;
		}
	}
}
