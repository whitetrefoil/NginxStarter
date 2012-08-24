using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NginxStarterGUI.Classes;
using System.IO;
using System.Diagnostics;

namespace NginxStarterGUI.TargetProgramsInfo
{
    sealed class Less : TargetProgram
    {
		#region 设置字段和属性

		public string NodeJsPath { get; set; }
		public string LesscPath { get; set; }
		public string InputPath { get; set; }
		public string OutputPath { get; set; }
		public bool IsLesscGlobal { get; set; }
		public bool IsNodeInPath { get; set; }
		public bool IsMinified { get; set; }

		public const string OfdNodeJsFilter = "Nodejs默认执行文件|node.exe|所有执行文件|*.exe|所有文件|*.*";
		public const string OfdNodeJsTitle = "选择Nodejs执行文件";
		public const string OfdLesscFilter = "Lessc默认二进制文件|lessc|所有文件|*.*";
		public const string OfdLesscTitle = "选择Lessc二进制文件";
		public const string OfdInputFilter = "LESS-CSS 文件|*.less";
		public const string OfdInputTitle = "选择输入文件";

		private StreamWriter sw;
		private string dataReceived;

		#endregion

		public override bool Start()
		{
			#region Set exe files path

			if (this.IsNodeInPath)
				this.NodeJsPath = FindInPath.Find("node.exe", MainWindow.WorkingDirectory, false);
			if (this.IsLesscGlobal)
			{
				LesscPath = FindInPath.Find("lessc", MainWindow.WorkingDirectory, true, false);
				if (LesscPath.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) || LesscPath.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase) || LesscPath.EndsWith(".com", StringComparison.OrdinalIgnoreCase))
				{
					HashSet<string> possibleLesscLocations = FindPathInfo.InBat("%~dp0\\.\\", "lessc", LesscPath);
					if (possibleLesscLocations != null)
					{
						foreach (string possibleLesscLocation in possibleLesscLocations)
						{
							string temp = possibleLesscLocation.Replace("%~dp0\\.\\", string.Empty);
							string tempFullPath = Path.GetDirectoryName(LesscPath) + Path.DirectorySeparatorChar + temp;
							if (File.Exists(tempFullPath))
							{
								LesscPath = tempFullPath;
								break;
							}
						}
					}
				};
			}
			fileName = this.NodeJsPath;
			arguments = "\"" + PathConverter.ConvertWinToUnix(this.LesscPath) + "\"";

			#endregion

			#region Merge paths

			if (String.IsNullOrEmpty(this.InputPath))
				return false;
			this.workingDirectory = Path.GetDirectoryName(this.InputPath);
			this.InputPath = PathConverter.ConvertWinToUnix(InputPath.Substring(this.workingDirectory.Length + 1));
			if (String.IsNullOrEmpty(this.OutputPath))
			{
				this.OutputPath = this.InputPath.Remove(this.InputPath.LastIndexOf('.'));
				this.OutputPath += ".css";
			}

			#endregion

			#region Set arguments

			arguments += " " + this.AddParams + " ";
			if (this.IsMinified)
				arguments += "-x ";
			arguments += this.InputPath;

			#endregion

			#region Set process properties

			setupInfo(fileName, arguments, workingDirectory);

			this.IsStdoutDisplayed = false;

			this.StdoutReceived -= this.handleStdoutReceived;
			this.StdoutReceived += this.handleStdoutReceived;

			this.ProcessExited -= this.handleProcessExited;
			this.ProcessExited += this.handleProcessExited;

			this.dataReceived = null;
			return run();

			#endregion
		}

		void handleStdoutReceived(object sender, DataReceivedEventArgs e)
		{
			this.dataReceived += e.Data + "\r\n";
		}

		void handleProcessExited(object sender, EventArgs e)
		{
			try
			{
				string filePath = this.workingDirectory + '\\' + this.OutputPath;
				File.WriteAllText(filePath, this.dataReceived);
			}
			catch (Exception)
			{
				base.Message += "写文件失败！\n";
			}
		}

		public override bool Stop()
		{
			return stop();
		}
	}
}
