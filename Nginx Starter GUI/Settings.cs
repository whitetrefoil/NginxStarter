using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace NginxStarterGUI.Settings
{
	/// <summary>
	/// 储存程序设置的类
	/// </summary>
	[Serializable]
	public class Settings : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public Nginx nginx { get; set; }
		public Php php { get; set; }
		public Coffee coffee { get; set; }

		public Settings()
		{
			nginx = new Nginx();
			php = new Php();
			coffee = new Coffee();
		}
	}

	public class Nginx : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public string path { get; set; }
		public string configPath { get; set; }
	}

	public class Php : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public string path { get; set; }
		public string configPath { get; set; }
		public bool? useIniFile { get; set; }
		public int? port { get; set; }
		public string host { get; set; }
	}

	public class Coffee : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public string nodePath { get; set; }
		public string coffeePath { get; set; }
		public string inputPath { get; set; }
		public string outputPath { get; set; }
		public bool isNodeInPath { get; set; }
		public bool isCoffeeGlobal { get; set; }
		public bool isBare { get; set; }
	}
}
