using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using NginxStarterGUI.Classes;

namespace NginxStarterGUI.TargetProgramsInfo
{
	class CoffeeScript : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private Process process;
		private BackgroundWorker processWorker;
		public string nodeJsPath { private get; set; }
		public string coffeePath { private get; set; }
		public string inputPath { private get; set; }
		public string outputPath { private get; set; }
		public bool isWatch { private get; set; }
		public bool isCoffeeGlobal { private get; set; }
		public bool isNodeInPath { private get; set; }
		public bool isBare { private get; set; }
		private string message;
		public string Message
		{
			get
			{
				return message;
			}
			set
			{
				message = value;
				OnPropertyChanged("Message");
			}
		}

		public const string _ofdNodeJsFilter = "Nodejs默认执行文件|node.exe|所有执行文件|*.exe|所有文件|*.*";
		public const string _ofdNodeJsTitle = "选择Nodejs执行文件";
		public const string _ofdCoffeeFilter = "Coffee默认二进制文件|coffee|所有文件|*.*";
		public const string _ofdCoffeeTitle = "选择Coffee二进制文件";
		public const string _ofdInputFilter = "coffee-script 或 目录|*.coffee";
		public const string _ofdInputTitle = "选择输入文件/目录";
		public const string _ofdOutputFilter = "目录|*.folder";
		public const string _ofdOutputTitle = "选择输出文件/目录";

		public void setTestData()
		{
			this.nodeJsPath = "C:\\Program Files (x86)\\nodejs\\node.exe";
			this.coffeePath = "C:\\Program Files (x86)\\nodejs\\node_modules\\coffee-script\\bin\\coffee";
			this.inputPath = "C:\\temp";
			this.outputPath = "C:\\temp";
			this.isCoffeeGlobal = false;
			this.isNodeInPath = false;
			this.isWatch = false;
			this.isBare = true;
		}

		public bool start()
		{
			this.process = new Process();
			ProcessStartInfo info = new ProcessStartInfo(); ;
			info.Arguments = string.Empty;
			info.WorkingDirectory = null;

			// Set exe files path
			if (this.isCoffeeGlobal)
			{
				this.coffeePath = FindInPath.Find("coffee", MainWindow.WorkingDirectory, true, true);
				info.FileName = this.coffeePath;
			}
			else if (this.isNodeInPath)
			{
				this.nodeJsPath = FindInPath.Find("node.exe", MainWindow.WorkingDirectory, false);
				info.FileName = this.nodeJsPath;
				info.Arguments += PathConverter.ConvertWinToUnix(this.coffeePath);
			}
			else
			{
				info.FileName = this.nodeJsPath;
				info.Arguments += PathConverter.ConvertWinToUnix(this.coffeePath);
			}

			// Merge paths
			inputPath = PathConverter.ConvertUnixToWin(inputPath);
			outputPath = PathConverter.ConvertUnixToWin(outputPath);
			if (outputPath == string.Empty)
			{
				if (Directory.Exists(inputPath))
				{
					if (inputPath[inputPath.Length - 1] != '\\')
						inputPath += '\\';
				}
				info.WorkingDirectory = Path.GetDirectoryName(inputPath);
				inputPath = PathConverter.ConvertWinToUnix(Path.GetFileName(inputPath));
			}
			else
			{
				info.WorkingDirectory = ComparePath.Compare(inputPath, outputPath, '\\');
				int headerIndex = info.WorkingDirectory.Length;
				inputPath = PathConverter.ConvertWinToUnix(inputPath.Substring(headerIndex));
				outputPath = PathConverter.ConvertWinToUnix(outputPath.Substring(headerIndex));
			}
			//inputPath.
			// Set arguments
			if (this.isBare)
				info.Arguments += " --bare";
			if (this.isWatch)
				info.Arguments += " --watch";
			info.Arguments += " --compile";
			if (this.outputPath != string.Empty)
				info.Arguments += " --output " + this.outputPath;
			if (this.inputPath == string.Empty)
				inputPath = ".";
			info.Arguments += " " + this.inputPath;

			// Set process properties
			info.UseShellExecute = false;
			info.CreateNoWindow = true;
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;
			info.RedirectStandardInput = true;

			process.OutputDataReceived += (sender, e) =>
			{
				Message += e.Data + "\n";
			};
			process.ErrorDataReceived += (sender, e) =>
			{
				Message += e.Data + "\n";
			};

			processWorker = new BackgroundWorker();
			processWorker.WorkerSupportsCancellation = true;
			processWorker.WorkerReportsProgress = true;

			processWorker.DoWork += (sender, e) =>
				{
					BackgroundWorker bw = sender as BackgroundWorker;
					process.StartInfo = info;
					process.Start();
					process.BeginOutputReadLine();
					process.BeginErrorReadLine();
					process.WaitForExit();
				};
			processWorker.RunWorkerCompleted += (sender, e) =>
				{
					BackgroundWorker bw = sender as BackgroundWorker;
					if (!process.HasExited)
					{
						process.Kill();
					}
				};

			processWorker.RunWorkerAsync(info);
			return true;
		}

		public bool stop()
		{
			// Temp codes
			if (!isCoffeeGlobal)
			{
				process.Kill();
			}
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
	}
}
