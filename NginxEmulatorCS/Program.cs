using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NginxEmulatorCS
{
	class Program
	{
		static void Main(string[] args)
		{
			string strTest = "test:";
			foreach (string arg in args) strTest += arg;
			Console.WriteLine(strTest);
			Console.ReadLine();
		}
	}
}
