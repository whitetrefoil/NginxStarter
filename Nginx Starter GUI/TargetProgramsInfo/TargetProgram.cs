using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Permissions;
using NginxStarterGUI.Classes;

namespace NginxStarterGUI.TargetProgramsInfo
{
	class TargetProgram : INotifyPropertyChanged, IDisposable
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private Process process;
		private ProcessStartInfo info;
		private BackgroundWorker bw;
		public event EventHandler MessageUpdated;
		public event EventHandler ProcessExited;
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

		/// <summary>
		/// 本程序所支持的程序的抽象类
		/// </summary>
		public TargetProgram()
		{
			setupProcess();
			setupBw();
		}

		/// <summary>
		/// 设置固定的进程相关的基本信息，构造函数会调用它，一般不需要手动调用
		/// </summary>
		private void setupProcess()
		{
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
					bw.ReportProgress(0, _e.Data);
				process.OutputDataReceived += (_sender, _e) =>
					bw.ReportProgress(0, _e.Data);
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
				if (!process.HasExited)
				{
					process.Kill();
				}
			};
		}

		/// <summary>
		/// 设置进程文件名、参数等信息
		/// </summary>
		/// <param name="fileName">进程文件名</param>
		/// <param name="args">完全格式化好的参数</param>
		/// <param name="workingDirectory">工作目录</param>
		protected void setupInfo(string fileName, string args, string workingDirectory)
		{
			info.FileName = fileName;
			info.Arguments = args;
			info.WorkingDirectory = workingDirectory;
		}

		/// <summary>
		/// 启动进程
		/// </summary>
		/// <returns>返回一个布尔值表示是否启动成功，目前此判断还未完成，必定返回true</returns>
		protected bool run()
		{
			bw.RunWorkerAsync(info);
			return true;
		}

		/// <summary>
		/// 启动进程，如果传入参数，将调用setupInfo()设置参数
		/// </summary>
		/// <param name="fileName">进程文件名</param>
		/// <param name="args">完全格式化好的参数</param>
		/// <param name="workingDirectory">工作目录</param>
		/// <returns>返回一个布尔值表示是否启动成功，目前此判断还未完成，必定返回true</returns>
		protected bool run(string fileName, string args, string workingDirectory)
		{
			setupInfo(fileName, args, workingDirectory);
			return run();
		}

		/// <summary>
		/// 结束进程，!!!同时释放本实例
		/// </summary>
		/// <returns>返回一个布尔值表示是否结束进程成功</returns>
		protected bool stop()
		{
			if (process != null && !process.HasExited)
			{
				process.Kill();
			}
			if (process.HasExited)
			{
				Dispose();
				return true;
			}
			else
				return false;
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
