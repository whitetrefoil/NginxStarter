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
        public string path;
        public string configPath;

        public Nginx()
        {
            path = string.Empty;
            configPath = string.Empty;
        }
    }

    public class Php
    {
        public string path;
        public string configPath;
        public bool? useIniFile;
        public int? port;
        public string host;

        public Php()
        {
            path = string.Empty;
            configPath = string.Empty;
            useIniFile = false;
            port = null;
            host = string.Empty;
        }
    }

    public class Coffee
    {
        public string nodePath;
        public string coffeePath;
        public string inputPath;
        public string outputPath;
        public bool isNodeInPath;
        public bool isCoffeeGlobal;

        public Coffee()
        {
            nodePath = string.Empty;
            coffeePath = string.Empty;
            inputPath = string.Empty;
            outputPath = string.Empty;
            isNodeInPath = false;
            isCoffeeGlobal = false;
        }
    }
}
