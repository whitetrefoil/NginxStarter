using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NginxStarterGUI.Settings
{
	/// <summary>
	/// 储存程序设置的类
	/// </summary>
	[Serializable]
	public class Settings
	{
        public Nginx nginx;
        public Php php;

		public Settings()
		{
            nginx = new Nginx();
            php = new Php();
		}
	}

    public class Nginx
    {
        public string path { get; set; }
        public string configPath { get; set; }

        public Nginx()
        {
            path = string.Empty;
            configPath = string.Empty;
        }
    }

    public class Php
    {
        public string path { get; set; }
        public string configPath { get; set; }
        public bool? useIniFile { get; set; }
        public int? port { get; set; }
        public string host { get; set; }

        public Php()
        {
            path = string.Empty;
            configPath = string.Empty;
            useIniFile = false;
            port = null;
            host = string.Empty;
        }
    }
}
