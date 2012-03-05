using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NginxStarterGUI.Classes
{
	class Arg
	{
		public string ArgName { get; set; }
		public List<string> argContent { get; private set; }

		Arg(string argName, List<string> argContent)
		{

		}

		Arg(string argName, string argContent)
		{
			
		}
	}
	class Args
	{
		private List<Arg> args;

		Args()
		{

		}
	}
}
