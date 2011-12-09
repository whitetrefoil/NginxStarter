using System;
using System.IO;

namespace NginxStarterGUI.Classes
{
	static class ComparePath
	{
		public static string Compare(string pathA, string pathB, char separator)
		{
			pathA = pathA.Replace('/', separator);
			pathA = pathA.Replace('\\', separator);
			pathB = pathB.Replace('/', separator);
			pathB = pathB.Replace('\\', separator);

			int minLength = Math.Min(pathA.Length, pathB.Length);

			string output = string.Empty;

			for (int i = 0; i < minLength; i++)
			{
				if (pathA.ToLower()[i] == pathB.ToLower()[i])
					output += pathA[i];
				else
					break;
			}

			int lastSeparatorIndex = output.LastIndexOf(separator);
			if (lastSeparatorIndex > 0)
				output = output.Remove(lastSeparatorIndex + 1);
			else
				output = string.Empty;

			return output;
		}

		public static string Compare(string pathA, string pathB)
		{
			return Compare(pathA, pathB, Path.DirectorySeparatorChar);
		}
	}
}
