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
	class Sass : TargetProgram
	{
		#region 设置字段和属性

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

		public const string OfdRubyFilter = "Ruby默认执行文件|ruby.exe|所有执行文件|*.exe|所有文件|*.*";
		public const string OfdRubyTitle = "选择Ruby执行文件";
		public const string OfdSassFilter = "SASS二进制文件|sass|所有文件|*.*";
		public const string OfdSassTitle = "选择SASS二进制文件";
		public const string OfdInputFilter = "SASS 文件 或 目录|*.sass;*.scss";
		public const string OfdInputTitle = "选择输入文件/目录";
		public const string OfdOutputFilter = "目录|*.folder";
		public const string OfdOutputTitle = "选择输出文件/目录";

		#endregion

		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = false)]
		public bool Start()
		{
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
			fileName = RubyPath;
			arguments = "\"" + PathConverter.ConvertWinToUnix(SassPath) + "\"";

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

			fileName = this.RubyPath;
			if (!String.IsNullOrEmpty(CodeStyle))
				arguments += " --style " + CodeStyle;
			if (this.IsUseLF)
				arguments += " --unix-newlines";
			if (!this.IsWatch && this.IsForce)
				arguments += " --force";
			if (this.IsNoCache)
				arguments += " --no-cache";
			if (this.IsWatch)
				arguments += " --watch " + this.InputPath + ":" + this.OutputPath;
			else
				arguments += " --update " + this.InputPath + ":" + this.OutputPath;

			#endregion

			#region Set process properties

			setupInfo(fileName, arguments, workingDirectory);

			return run();

			#endregion
		}

		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = false)]
		public bool Stop()
		{
			return stop();
		}

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
