using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NginxStarterGUI
{
	/// <summary>
	/// 储存程序设置的类
	/// </summary>
	[Serializable]
	public class Settings
	{
		public string nginxPath { get; set; }
		public string nginxConfigPath { get; set; }
		public string phpPath { get; set; }
		public string phpConfigPath { get; set; }
		public bool? phpUseIniFile { get; set; }
		public int? phpPort { get; set; }
		public string phpHost { get; set; }

		public Settings()
		{
			nginxPath = string.Empty;
			nginxConfigPath = string.Empty;
			phpPath = string.Empty;
			phpConfigPath = string.Empty;
			phpUseIniFile = false;
			phpPort = null;
			phpHost = string.Empty;
		}
	}
}
