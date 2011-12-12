using System;
using System.IO;
using System.Collections.Generic;

namespace NginxStarterGUI.Classes
{
	static class FindInPath
	{
		public static string SystemPath = System.Environment.GetEnvironmentVariable("Path");
		public static string SystemPathExt = System.Environment.GetEnvironmentVariable("Pathext");

		/// <summary>
		/// 在系统Path中寻找程序
		/// </summary>
		/// <param name="targetName">目标程序文件名</param>
		/// <param name="workingDirectory">程序工作目录（将优先在工作目录下寻找）</param>
		/// <param name="isNeedToTestExt">是否添加Pathext中的扩展名再逐一寻找</param>
		/// <param name="isIncludeNoExt">是否包括无扩展名的文件</param>
		/// <returns>返回第一个找到的文件路径</returns>
		public static string Find(string targetName, string workingDirectory = null, bool isNeedToTestExt = true, bool isIncludeNoExt = false)
		{
			if (workingDirectory != null)
				Directory.SetCurrentDirectory(workingDirectory);
			else
				Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory.ToString());
			List<string> systemPaths = new List<string>(("." + Path.PathSeparator + SystemPath).Split(Path.PathSeparator));
			List<string> systemPathExts = !String.IsNullOrEmpty(SystemPathExt) ?
				new List<string>(System.Environment.GetEnvironmentVariable("Pathext").Split(Path.PathSeparator)) :
				new List<string> { ".exe", ".cmd",".com", ".bat" };
			
			string targetPath = string.Empty;

			if (!isNeedToTestExt)
			{
				systemPathExts.Clear();
				systemPathExts.Add(string.Empty);
			}
			else if (isIncludeNoExt)
			{
				systemPathExts.Insert(0, string.Empty);
			}

			foreach (string pathExt in systemPathExts)
			{
				string result = find(targetName + pathExt, systemPaths);
				if (!String.IsNullOrEmpty(result))
				{
					targetPath = result;
					break;
				}
			}

			return targetPath;
		}

		private static string find(string targetName, List<string> systemPaths)
		{
			string targetPath = string.Empty;

			foreach (string path in systemPaths)
			{
				string tryThisPath = path + Path.DirectorySeparatorChar + targetName;
				if (File.Exists(tryThisPath))
				{
					targetPath = tryThisPath;
					break;
				}
			}

			return targetPath;
		}
	}
}
