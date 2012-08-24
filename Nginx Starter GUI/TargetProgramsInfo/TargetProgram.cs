using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using NginxStarterGUI.Classes;
using System.Security.Permissions;

namespace NginxStarterGUI.TargetProgramsInfo
{
	abstract class TargetProgram : INotifyPropertyChanged, IDisposable
	{
		#region 设置字段和属性

		public event PropertyChangedEventHandler PropertyChanged;
		private Process process;
		private ProcessStartInfo info;
		private BackgroundWorker bw;
		public event EventHandler MessageUpdated;
		public event EventHandler ProcessExited;
		protected string fileName;
		protected string arguments;
		protected string workingDirectory;
		public string AddParams { get; set; }

		private string message;
		public virtual string Message
		{
			get { return message; }
			set
			{
				message = value;
				OnPropertyChanged("Message");
			}
		}
		protected event DataReceivedEventHandler StdoutReceived;
		protected event DataReceivedEventHandler StderrReceived;
		protected bool IsStdoutDisplayed;
		protected bool IsStderrDisplayed;

		#endregion

		/// <summary>
		/// 本程序所支持的程序的抽象类
		/// </summary>
		public TargetProgram()
		{
			this.info = new ProcessStartInfo();
			this.IsStdoutDisplayed = true;
			this.IsStderrDisplayed = true;
			setupBw();
		}

		/// <summary>
		/// 设置固定的进程相关的基本信息，构造函数会调用它，一般不需要手动调用
		/// </summary>
		private void setupProcess()
		{
			this.process = new Process();

			process.Exited += (sender, e) =>
			{
				if (ProcessExited != null)
					ProcessExited(null, e);
			};

			info.UseShellExecute = false;
			info.CreateNoWindow = true;
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;
			info.RedirectStandardInput = true;

		}

		/// <summary>
		/// 设置BackgroundWorker的基本信息，构造函数会调用它，一般不需要手动调用
		/// </summary>
		private void setupBw()
		{
			bw = new BackgroundWorker();
			bw.WorkerSupportsCancellation = true;
			bw.WorkerReportsProgress = true;

			bw.DoWork += (sender, e) =>
			{
				process.StartInfo = info;
				process.ErrorDataReceived += (_sender, _e) =>
				{
					if (this.IsStderrDisplayed)
						bw.ReportProgress(0, _e.Data);
					if (this.StderrReceived != null)
						this.StderrReceived(this, _e);
				};
				process.OutputDataReceived += (_sender, _e) =>
				{
					if (this.IsStdoutDisplayed)
						bw.ReportProgress(0, _e.Data);
					if (this.StdoutReceived != null)
						this.StdoutReceived(this, _e);
				};
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit();
			};
			bw.ProgressChanged += (sender, e) =>
			{
				Message += e.UserState + "\n";
				MessageUpdated(this, null);
			};
			bw.RunWorkerCompleted += (sender, e) =>
			{
				this.stop();
				//if (!process.HasExited)
				//{
				//    process.Kill();
				//}
			};
		}

		/// <summary>
		/// 设置进程文件名、参数等信息
		/// </summary>
		/// <param name="fn">进程文件名</param>
		/// <param name="args">完全格式化好的参数</param>
		/// <param name="wd">工作目录</param>
		protected void setupInfo(string fn, string args, string wd)
		{
			info.FileName = fn;
			info.Arguments = args;
			info.WorkingDirectory = wd;
		}

		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = false)]
		abstract public bool Start();
		/// <summary>
		/// 启动进程
		/// </summary>
		/// <returns>返回一个布尔值表示是否启动成功，目前此判断还未完成，必定返回true</returns>
		protected bool run()
		{
			setupProcess();
			bw.RunWorkerAsync(info);
			return true;
		}

		/// <summary>
		/// 启动进程，如果传入参数，将调用setupInfo()设置参数
		/// </summary>
		/// <param name="fn">进程文件名</param>
		/// <param name="args">完全格式化好的参数</param>
		/// <param name="wd">工作目录</param>
		/// <returns>返回一个布尔值表示是否启动成功，目前此判断还未完成，必定返回true</returns>
		protected bool run(string fn, string args, string wd)
		{
			setupInfo(fn, args, wd);
			return run();
		}

		/// <summary>
		/// 结束进程
		/// </summary>
		/// <returns>返回一个布尔值表示是否结束进程成功</returns>
		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = false)]
		protected bool stop()
		{
			if (process != null)
			{
				if (!process.HasExited) process.Kill();
				process = null;
			}
			return true;
		}

		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = false)]
		abstract public bool Stop();

		protected Dictionary<string, string> findJointRoot(string inputPath, string outputPath)
		{
			Dictionary<string, string> output = new Dictionary<string, string>();
			output.Add("same", "");
			output.Add("workingDirectory", "");
			output.Add("inputPath", "");
			output.Add("outputPath", "");

			if (String.IsNullOrEmpty(inputPath))
			{
				inputPath = ".";
			}
			else if (Directory.Exists(inputPath))
			{
				if (inputPath[inputPath.Length - 1] != '\\')
					inputPath += '\\';
			}

			if (String.IsNullOrEmpty(outputPath))
			{
				output["workingDirectory"] = inputPath;
			}
			else if (Directory.Exists(outputPath))
			{
				if (outputPath[outputPath.Length - 1] != '\\')
					outputPath += '\\';
			}

			return output;
		}

		private void OnPropertyChanged(string data)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(data));
			}
		}

		#region Dispose Region

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~TargetProgram()
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
				if (bw != null)
				{
					bw.Dispose();
					bw = null;
				}
			}
		}

		#endregion
	}
}
