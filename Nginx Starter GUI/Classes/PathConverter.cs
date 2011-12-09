using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NginxStarterGUI.Classes
{
	public static class PathConverter
	{
		/// <summary>
		/// 将Windows路径转换为Unix路径
		/// </summary>
		/// <param name="input">Windows格式路径</param>
		/// <returns>返回Unix格式路径</returns>
		public static string ConvertWinToUnix(string input)
		{
			return input.Replace('\\', '/');
		}

		/// <summary>
		/// 将Unix路径转换为Windows路径
		/// </summary>
		/// <param name="input">Unix格式路径</param>
		/// <returns>返回Windows格式路径</returns>
		public static string ConvertUnixToWin(string input)
		{
			return input.Replace('/', '\\');
		}
	}
}
