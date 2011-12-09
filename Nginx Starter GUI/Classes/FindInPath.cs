using System;
using System.IO;
using System.Collections.Generic;

namespace NginxStarterGUI.Classes
{
	static class FindInPath
	{
		public static string SystemPath = System.Environment.GetEnvironmentVariable("Path");
		public static string SystemPathExt = System.Environment.GetEnvironmentVariable("Pathext");

		public static string Find(string targetName, string workingDirectory = null, bool isNeedToTestExt = true, bool isIncludeNoExt = false)
		{
			if (workingDirectory != null)
				Directory.SetCurrentDirectory(workingDirectory);
			else
				Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory.ToString());
			List<string> systemPaths = new List<string>(("." + Path.PathSeparator + SystemPath).Split(Path.PathSeparator));
			List<string> systemPathExts = SystemPathExt != string.Empty ?
				new List<string>(System.Environment.GetEnvironmentVariable("Pathext").Split(Path.PathSeparator)) :
				new List<string> { ".exe", ".cmd", ".bat" }; string targetPath = string.Empty;

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
				if (result != string.Empty)
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
