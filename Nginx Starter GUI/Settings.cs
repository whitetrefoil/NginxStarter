using System;
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
		public Nginx Nginx { get; set; }
		public Php Php { get; set; }
		public Coffee Coffee { get; set; }
		public Sass Sass { get; set; }
		public Less Less { get; set; }

		public Settings()
		{
			Nginx = new Nginx();
			Php = new Php();
			Coffee = new Coffee();
			Sass = new Sass();
			Less = new Less();
		}
	}

	public class Nginx : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public string Path { get; set; }
		public string ConfigPath { get; set; }
		public string AddParams { get; set; }
	}

	public class Php : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public string Path { get; set; }
		public string ConfigPath { get; set; }
		public bool? UseIniFile { get; set; }
		public int? Port { get; set; }
		public string Host { get; set; }
	}

	public class Coffee : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public string NodePath { get; set; }
		public string CoffeePath { get; set; }
		public string InputPath { get; set; }
		public string OutputPath { get; set; }
		public bool IsNodeInPath { get; set; }
		public bool IsCoffeeGlobal { get; set; }
		public bool IsBare { get; set; }
		public string AddParams { get; set; }
	}

	public class Less : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public string NodePath { get; set; }
		public string LessPath { get; set; }
		public string InputPath { get; set; }
		public string OutputPath { get; set; }
		public bool IsNodeInPath { get; set; }
		public bool IsLesscGlobal { get; set; }
        public bool IsMinified { get; set; }
		public string AddParams { get; set; }
	}

	public class Sass : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public string RubyPath { get; set; }
		public string SassPath { get; set; }
		public string InputPath { get; set; }
		public string OutputPath { get; set; }
		public bool IsRubyInPath { get; set; }
		public bool IsUseLF { get; set; }
		public bool IsForce { get; set; }
		public bool IsNoCache { get; set; }
		public string CodeStyle { get; set; }
		public string AddParams { get; set; }
	}
}
