using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindInPath
{
    public static class FindInPath
    {
        public static string SystemPath = System.Environment.GetEnvironmentVariable("Path");

        public static List<string> GetSystemPaths()
        {
            List<string> SystemPaths = new List<string>();
            SystemPaths.AddRange((SystemPath + ";").Split(';'));
            return SystemPaths;
        }
    }
}
