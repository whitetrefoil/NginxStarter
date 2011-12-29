using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NginxStarterGUI.Classes;
using System.IO;

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

		public const string OfdNodeJsFilter = "Nodejs默认执行文件|node.exe|所有执行文件|*.exe|所有文件|*.*";
		public const string OfdNodeJsTitle = "选择Nodejs执行文件";
		public const string OfdLesscFilter = "Lessc默认二进制文件|lessc|所有文件|*.*";
		public const string OfdLesscTitle = "选择Lessc二进制文件";
		public const string OfdInputFilter = "LESS-CSS 或 目录|*.less";
		public const string OfdInputTitle = "选择输入文件/目录";
		public const string OfdOutputFilter = "目录|*.folder";
		public const string OfdOutputTitle = "选择输出文件/目录";

		#endregion

		public override bool Start()
		{
			#region Set exe files path

			if (this.IsNodeInPath)
				this.NodeJsPath = FindInPath.Find("node.exe", MainWindow.WorkingDirectory, false);
			if (this.IsLesscGlobal)
			{
				LesscPath = FindInPath.Find("lessc", MainWindow.WorkingDirectory, true, true);
				if (LesscPath.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) || LesscPath.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase) || LesscPath.EndsWith(".com", StringComparison.OrdinalIgnoreCase))
				{
					HashSet<string> possibleLesscLocations = FindPathInfo.InBat("%~dp0\\.\\", "lessc", LesscPath);
					if (possibleLesscLocations != null)
					{
						foreach (string possibleLesscLocation in possibleLesscLocations)
						{
							string temp = possibleLesscLocation.Replace("%~dp0\\.\\", string.Empty);
							string tempFullPath = Path.GetDirectoryName(NodeJsPath) + Path.DirectorySeparatorChar + temp;
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
				workingDirectory = Path.GetDirectoryName(InputPath);
				InputPath = PathConverter.ConvertWinToUnix(Path.GetFileName(InputPath));
				OutputPath = PathConverter.ConvertWinToUnix(Path.GetFileName(OutputPath));
			}
			else
			{
				workingDirectory = ComparePath.Compare(InputPath, OutputPath, '\\');
				int headerIndex = workingDirectory.Length;
				InputPath = PathConverter.ConvertWinToUnix(InputPath.Substring(headerIndex));
				OutputPath = PathConverter.ConvertWinToUnix(OutputPath.Substring(headerIndex));
			}
			if (String.IsNullOrEmpty(InputPath))
				InputPath = ".";
			if (String.IsNullOrEmpty(OutputPath))
				OutputPath = ".";

			#endregion

			#region Set arguments

			if (!String.IsNullOrEmpty(this.InputPath))
				arguments += " " + this.InputPath;
			if (!String.IsNullOrEmpty(this.OutputPath))
				arguments += ":" + this.OutputPath;

			#endregion

			#region Set process properties

			setupInfo(fileName, arguments, workingDirectory);

			return run();

			#endregion
		}

		public override bool Stop()
		{
			return stop();
		}
	}
}
