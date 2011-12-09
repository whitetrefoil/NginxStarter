using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NginxStarterGUI.Classes
{
	public static class PathConverter
	{
		public static string ConvertWinToUnix(string input)
		{
			return input.Replace('\\', '/');
		}

		public static string ConvertUnixToWin(string input)
		{
			return input.Replace('/', '\\');
		}
	}
}
