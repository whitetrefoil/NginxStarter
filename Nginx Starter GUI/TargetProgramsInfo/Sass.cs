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
		public event EventHandler MessageUpdated;
		public event EventHandler ProcessExited;

		public string RubyPath { get; set; }
		public string SassPath { get; set; }
		public string InputPath { get; set; }
		public string OutputPath { get; set; }
		public bool IsWatch { get; set; }
		public bool IsRubyInPath { get; set; }
		public bool IsUseLF { get; set; }
		public bool IsForce { get; set; }
		public bool IsNoCache { get; set; }
		private bool IsScss { get; set; }
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

		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = false)]
		public bool Start()
		{
			process = new Process();
			ProcessStartInfo info = new ProcessStartInfo();
			info.Arguments = string.Empty;
			info.WorkingDirectory = null;
			process.Exited += (sender, e) =>
				{
					if (ProcessExited != null)
						ProcessExited(null, e);
				};

			#region Set EXE file path

			if (this.IsRubyInPath)
			{
				this.RubyPath = FindInPath.Find("ruby.exe", MainWindow.WorkingDirectory, false);
				this.SassPath = FindInPath.Find("sass", MainWindow.WorkingDirectory, true, true);
				if (SassPath.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) || SassPath.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase) || SassPath.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase))
				{
					HashSet<string> possibleSassLocations = FindPathInfo.InBat("\"ruby.exe\"", "sass\"", SassPath);
					if (possibleSassLocations != null)
					{
						foreach (string possibleSassLocation in possibleSassLocations)
						{
							string temp = possibleSassLocation.Replace("\"", "").Replace("ruby.exe ", "");
							if (File.Exists(temp))
							{
								SassPath = temp;
								break;
							}
						}
					}
				}
			}
			info.FileName = RubyPath;
			info.Arguments = "\"" + PathConverter.ConvertWinToUnix(SassPath) + "\"";

			#endregion

			#region Merge paths

			InputPath = !String.IsNullOrEmpty(InputPath) ? PathConverter.ConvertUnixToWin(InputPath) : ".";
			if (Directory.Exists(InputPath))
			{
				if (InputPath[InputPath.Length - 1] != '\\')
					InputPath += '\\';
			}
			if (Directory.Exists(OutputPath))
			{
				if (OutputPath[OutputPath.Length - 1] != '\\')
					OutputPath += '\\';
			}
			if (String.IsNullOrEmpty(OutputPath))
			{
				info.WorkingDirectory = Path.GetDirectoryName(InputPath);
				InputPath = PathConverter.ConvertWinToUnix(Path.GetFileName(InputPath));
				OutputPath = PathConverter.ConvertWinToUnix(Path.GetFileName(OutputPath));
			}
			else
			{
				info.WorkingDirectory = ComparePath.Compare(InputPath, OutputPath, '\\');
				int headerIndex = info.WorkingDirectory.Length;
				InputPath = PathConverter.ConvertWinToUnix(InputPath.Substring(headerIndex));
				OutputPath = PathConverter.ConvertWinToUnix(OutputPath.Substring(headerIndex));
			}
			if (String.IsNullOrEmpty(InputPath))
				InputPath = ".";
			if (String.IsNullOrEmpty(OutputPath))
				OutputPath = ".";

			#endregion

			#region Set arguments

			info.FileName = this.RubyPath;
			if (!String.IsNullOrEmpty(CodeStyle))
				info.Arguments += " --style " + CodeStyle;
			if (this.IsUseLF)
				info.Arguments += " --unix-newlines";
			if (!this.IsWatch && this.IsForce)
				info.Arguments += " --force";
			if (this.IsNoCache)
				info.Arguments += " --no-cache";
			if (this.IsWatch)
				info.Arguments += " --watch " + this.InputPath + ":" + this.OutputPath;
			else
				info.Arguments += " --update " + this.InputPath + ":" + this.OutputPath;

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

		public bool Stop()
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
