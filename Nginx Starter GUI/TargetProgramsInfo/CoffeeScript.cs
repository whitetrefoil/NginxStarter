using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using NginxStarterGUI.Classes;
using System.Globalization;
using System.Security;
using System.Security.Permissions;

namespace NginxStarterGUI.TargetProgramsInfo
{
	class CoffeeScript : INotifyPropertyChanged, IDisposable
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private Process process;
		private BackgroundWorker processWorker;
		public event EventHandler MessageUpdated;
		public event EventHandler ProcessExited;

		public string nodeJsPath { get; set; }
		public string coffeePath { get; set; }
		public string inputPath { get; set; }
		public string outputPath { get; set; }
		public bool isWatch { get; set; }
		public bool isCoffeeGlobal { get; set; }
		public bool isNodeInPath { get; set; }
		public bool isBare { get; set; }

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

		public const string OfdNodeJsFilter = "Nodejs默认执行文件|node.exe|所有执行文件|*.exe|所有文件|*.*";
		public const string OfdNodeJsTitle = "选择Nodejs执行文件";
		public const string OfdCoffeeFilter = "Coffee默认二进制文件|coffee|所有文件|*.*";
		public const string OfdCoffeeTitle = "选择Coffee二进制文件";
		public const string OfdInputFilter = "coffee-script 或 目录|*.coffee";
		public const string OfdInputTitle = "选择输入文件/目录";
		public const string OfdOutputFilter = "目录|*.folder";
		public const string OfdOutputTitle = "选择输出文件/目录";

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

		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = false)]
		public bool start()
		{
			this.process = new Process();
			ProcessStartInfo info = new ProcessStartInfo(); ;
			info.Arguments = string.Empty;
			info.WorkingDirectory = null;
			process.Exited += (sender, e) =>
			{
				if (ProcessExited != null)
					ProcessExited(null, e);
			};

			#region Set exe files path

			if (this.isNodeInPath)
				this.nodeJsPath = FindInPath.Find("node.exe", MainWindow.WorkingDirectory, false);
			if (this.isCoffeeGlobal)
			{
				coffeePath = FindInPath.Find("coffee", MainWindow.WorkingDirectory, true, true);
				if (coffeePath.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) || coffeePath.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase) || coffeePath.EndsWith(".com", StringComparison.OrdinalIgnoreCase))
				{
					HashSet<string> possibleCoffeeLocations = FindPathInfo.InBat("%~dp0\\.\\", "coffee", coffeePath);
					if (possibleCoffeeLocations != null)
					{
						foreach (string possibleCoffeeLocation in possibleCoffeeLocations)
						{
							string temp = possibleCoffeeLocation.Replace("%~dp0\\.\\", string.Empty);
							string tempFullPath = Path.GetDirectoryName(nodeJsPath) + Path.DirectorySeparatorChar + temp;
							if (File.Exists(tempFullPath))
							{
								coffeePath = tempFullPath;
								break;
							}
						}
					}
				};
			}
			info.FileName = this.nodeJsPath;
			info.Arguments += "\"" + PathConverter.ConvertWinToUnix(this.coffeePath) + "\"";

			#endregion

			#region Merge paths

			inputPath = !String.IsNullOrEmpty(inputPath) ? PathConverter.ConvertUnixToWin(inputPath) : ".";
			if (Directory.Exists(inputPath))
			{
				if (inputPath[inputPath.Length - 1] != '\\')
					inputPath += '\\';
			}
			if (Directory.Exists(outputPath))
			{
				if (outputPath[outputPath.Length - 1] != '\\')
					outputPath += '\\';
			}
			if (String.IsNullOrEmpty(outputPath))
			{
				info.WorkingDirectory = Path.GetDirectoryName(inputPath);
				inputPath = PathConverter.ConvertWinToUnix(Path.GetFileName(inputPath));
				outputPath = PathConverter.ConvertWinToUnix(Path.GetFileName(outputPath));
			}
			else
			{
				info.WorkingDirectory = ComparePath.Compare(inputPath, outputPath, '\\');
				int headerIndex = info.WorkingDirectory.Length;
				inputPath = PathConverter.ConvertWinToUnix(inputPath.Substring(headerIndex));
				outputPath = PathConverter.ConvertWinToUnix(outputPath.Substring(headerIndex));
			}
			if (String.IsNullOrEmpty(inputPath))
				inputPath = ".";
			if (String.IsNullOrEmpty(outputPath))
				outputPath = ".";

			#endregion

			#region Set arguments

			if (this.isBare)
				info.Arguments += " --bare";
			if (this.isWatch)
				info.Arguments += " --watch";
			info.Arguments += " --compile";
			if (!String.IsNullOrEmpty(outputPath))
				info.Arguments += " --output " + this.outputPath;
			info.Arguments += " " + this.inputPath;

			#endregion

			#region Set process properties

			info.UseShellExecute = false;
			info.CreateNoWindow = true;
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;
			info.RedirectStandardInput = true;

			processWorker = new BackgroundWorker();
			processWorker.WorkerSupportsCancellation = true;
			processWorker.WorkerReportsProgress = true;

			processWorker.DoWork += (sender, e) =>
				{
					process.StartInfo = info;
					process.ErrorDataReceived += (_sender, _e) =>
						processWorker.ReportProgress(0, _e.Data);
					process.OutputDataReceived += (_sender, _e) =>
						processWorker.ReportProgress(0, _e.Data);
					process.Start();
					process.BeginOutputReadLine();
					process.BeginErrorReadLine();
					process.WaitForExit();
				};
			processWorker.ProgressChanged += (sender, e) =>
			{
				Message += e.UserState + "\n";
				MessageUpdated(this, null);
			};
			processWorker.RunWorkerCompleted += (sender, e) =>
				{
					if (!process.HasExited)
					{
						process.Kill();
					}
				};

			processWorker.RunWorkerAsync(info);

			#endregion

			return true;
		}

		public bool stop()
		{
			if (process != null && !process.HasExited)
			{
				process.Kill();
			}
			if (process.HasExited)
				return true;
			else
				return false;
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

		~CoffeeScript()
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
				if (processWorker != null)
				{
					processWorker.Dispose();
					processWorker = null;
				}
			}
		}

		#endregion
	}
}
