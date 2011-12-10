using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NginxStarterGUI.Classes
{
	public static class FindPathInfo
	{
		/// <summary>
		/// 不严谨的Bat文件分析函数，用于协助其他代码。这个方法会将bat文件按行分割，在每一行中寻找以第一个startWith开头，以最后一个endWith结尾的字符串
		/// </summary>
		/// <param name="startWith">寻找每一行中的第一个该字符串</param>
		/// <param name="endWith">寻找每一行中的最后一个该字符串</param>
		/// <param name="path">要分析的文件的路径</param>
		/// <returns>返回一个HashSet&lt;string&gt;，其中的每一个值都是唯一的</returns>
		public static HashSet<string> InBat(string startWith, string endWith, string path)
		{
			FileStream fs = null;
			StreamReader sr = null;
			try
			{
				if (String.IsNullOrEmpty(startWith) || String.IsNullOrEmpty(endWith) || String.IsNullOrEmpty(path))
					throw new ArgumentException("Null或空字符串被传入");
				using (fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					sr = new StreamReader(fs, true);
					string fileContent = sr.ReadToEnd();
					fileContent = fileContent.Replace("\r\n", "\n");
					fileContent = fileContent.Replace("\r", "\n");
					string[] lines = fileContent.Split('\n');
					HashSet<string> newLines = new HashSet<string>();
					foreach (string line in lines)
					{
						if (line.IndexOf(endWith, StringComparison.OrdinalIgnoreCase) >= 0)
						{
							string temp = line.Remove(line.LastIndexOf(endWith, StringComparison.OrdinalIgnoreCase) + endWith.Length);
							if (line.IndexOf(startWith, StringComparison.OrdinalIgnoreCase) >= 0)
							{
								temp = temp.Substring(temp.IndexOf(startWith, StringComparison.OrdinalIgnoreCase));
								newLines.Add(temp);
							}
						}
					}
					return newLines;
				}
			}
			catch
			{
				return null;
			}
			finally
			{
				if (fs != null)
					fs.Dispose();
				if (sr != null)
					sr.Dispose();
			}
		}
	}
}
