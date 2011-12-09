using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NginxStarterGUI.Classes
{
	public static class FindPathInfo
	{
		public static HashSet<string> InBat(string startWith, string endWith, string path)
		{
			FileStream fs = null;
			StreamReader sr = null;
			try
			{
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
						if (line.IndexOf(endWith) >= 0)
						{
							string temp = line.Remove(line.LastIndexOf(endWith) + endWith.Length);
							temp = temp.Substring(temp.IndexOf(startWith));
							newLines.Add(temp);
						}
					}
					sr.Close();
					fs.Close();

					//temp
					return newLines;
				}
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
