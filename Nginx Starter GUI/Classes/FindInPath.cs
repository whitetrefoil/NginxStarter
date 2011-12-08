using System;
using System.IO;
using System.Collections.Generic;

namespace NginxStarterGUI.Classes
{
	static class FindInPath
	{
		public static string SystemPath = System.Environment.GetEnvironmentVariable("Path");
		public static List<string> SystemPaths = new List<string>(("." + Path.PathSeparator + SystemPath).Split(Path.PathSeparator));
		public static string SystemPathExt = System.Environment.GetEnvironmentVariable("Pathext");
		public static List<string> SystemPathExts = SystemPathExt != string.Empty ?
			new List<string>(System.Environment.GetEnvironmentVariable("Pathext").Split(Path.PathSeparator)) :
			new List<string> {".exe", ".cmd", ".bat"};

		public static string Find(string targetName, bool isIncludeNoExt = false)
		{
			string targetPath = string.Empty;

			if (isIncludeNoExt)
			{
				SystemPathExts.Insert(0, string.Empty);
			}

			foreach (string pathExt in SystemPathExts)
			{
				string result = find(targetName + pathExt);
				if (result != string.Empty)
				{
					targetPath = result;
					break;
				}
			}

			return targetPath;
		}

		private static string find(string targetName)
		{
			string targetPath = string.Empty;

			foreach (string path in SystemPaths)
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
