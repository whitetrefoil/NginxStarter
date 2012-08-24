using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Permissions;
using NginxStarterGUI.Classes;

namespace NginxStarterGUI.TargetProgramsInfo
{
	sealed class CoffeeScript : TargetProgram
	{
		#region 设置字段和属性

		public string NodeJsPath { get; set; }
		public string CoffeePath { get; set; }
		public string InputPath { get; set; }
		public string OutputPath { get; set; }
		public bool IsWatch { get; set; }
		public bool IsCoffeeGlobal { get; set; }
		public bool IsNodeInPath { get; set; }
		public bool IsBare { get; set; }

		public const string OfdNodeJsFilter = "Nodejs默认执行文件|node.exe|所有执行文件|*.exe|所有文件|*.*";
		public const string OfdNodeJsTitle = "选择Nodejs执行文件";
		public const string OfdCoffeeFilter = "Coffee默认二进制文件|coffee|所有文件|*.*";
		public const string OfdCoffeeTitle = "选择Coffee二进制文件";
		public const string OfdInputFilter = "coffee-script 或 目录|*.coffee";
		public const string OfdInputTitle = "选择输入文件/目录";
		public const string OfdOutputFilter = "目录|*.folder";
		public const string OfdOutputTitle = "选择输出文件/目录";

		#endregion

		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = false)]
		public override bool Start()
		{
			#region Set exe files path

			if (this.IsNodeInPath)
				this.NodeJsPath = FindInPath.Find("node.exe", MainWindow.WorkingDirectory, false);
			if (this.IsCoffeeGlobal)
			{
				CoffeePath = FindInPath.Find("coffee", MainWindow.WorkingDirectory, true, false);
				if (CoffeePath.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) || CoffeePath.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase) || CoffeePath.EndsWith(".com", StringComparison.OrdinalIgnoreCase))
				{
					HashSet<string> possibleCoffeeLocations = FindPathInfo.InBat("%~dp0\\.\\", "coffee", CoffeePath);
					if (possibleCoffeeLocations != null)
					{
						foreach (string possibleCoffeeLocation in possibleCoffeeLocations)
						{
							string temp = possibleCoffeeLocation.Replace("%~dp0\\.\\", string.Empty);
							string tempFullPath = Path.GetDirectoryName(CoffeePath) + Path.DirectorySeparatorChar + temp;
							if (File.Exists(tempFullPath))
							{
								CoffeePath = tempFullPath;
								break;
							}
						}
					}
				};
			}
			fileName = this.NodeJsPath;
			arguments = "\"" + PathConverter.ConvertWinToUnix(this.CoffeePath) + "\"";

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

			arguments += " " + this.AddParams;
			if (this.IsBare)
				arguments += " --bare";
			if (this.IsWatch)
				arguments += " --watch";
			arguments += " --compile";
			if (!String.IsNullOrEmpty(OutputPath))
				arguments += " --output " + this.OutputPath;
			arguments += " " + this.InputPath;

			#endregion

			#region Set process properties

			setupInfo(fileName, arguments, workingDirectory);

			return run();

			#endregion
		}

		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = false)]
		public override bool Stop()
		{
			return stop();
		}
	}
}
