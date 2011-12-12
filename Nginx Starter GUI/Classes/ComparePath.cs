using System;
using System.IO;

namespace NginxStarterGUI.Classes
{
	static class ComparePath
	{
		/// <summary>
		/// 比较两个路径寻找相同部分
		/// </summary>
		/// <param name="pathA"></param>
		/// <param name="pathB"></param>
		/// <param name="separator">输出路径的分隔符，默认为当前系统值</param>
		/// <returns>返回两者的共同上级路径，包括末尾的分隔符</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower")]
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
			if (!output.EndsWith(separator.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				int lastSeparatorIndex = output.LastIndexOf(separator);
				if (lastSeparatorIndex > 0)
					output = output.Remove(lastSeparatorIndex + 1);
				else
					output = string.Empty;
			}
			return output;
		}

		public static string Compare(string pathA, string pathB)
		{
			return Compare(pathA, pathB, Path.DirectorySeparatorChar);
		}
	}
}
