using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
//using NginxStarterGUI.Classes;

namespace NginxStarterGUI.Classes
{
	class IOPath
	{
		private string path;

		public IOPath(string path)
		{
			this.path = path;
		}

		public override string ToString()
		{
			return this.path;
		}

		#region 隐式转换规则

		public static implicit operator string(IOPath a)
		{
			return a.ToString();
		}

		public static implicit operator IOPath(string a)
		{
			return new IOPath(a);
		}

		#endregion

		/// <summary>
		/// 以特定的规则判断是否是目格式
		/// </summary>
		/// <returns></returns>
		public bool IsDirectory()
		{
			if (String.IsNullOrEmpty(this.path))
				return false;
			else if (this.IsExistedFile())
				return false;
			else if (this.IsExistedDirectory())
				return true;
			else if (this.path.EndsWith("\\") || this.path.EndsWith("/"))
				return true;
			else
				return false;
		}

		/// <summary>
		/// 以特定的规则判断是否是文件名格式
		/// </summary>
		/// <returns></returns>
		public bool IsFile()
		{
			return !this.IsDirectory();
		}

		/// <summary>
		/// 判断是否是存在的目录
		/// </summary>
		/// <returns></returns>
		public bool IsExistedDirectory()
		{
			return Directory.Exists(this.path);
		}

		/// <summary>
		/// 判断是否是存在的文件
		/// </summary>
		/// <returns></returns>
		public bool IsExistedFile()
		{
			return File.Exists(this.path);
		}

		public string LastDirectory()
		{
			if (String.IsNullOrEmpty(this.path))
				return null;
			else if (this.IsDirectory())
				return this.path;
			else
			{
				string winPath = PathConverter.ConvertUnixToWin(this.path);
				int index = winPath.LastIndexOf('\\');
				
				if (index < 0)
					return null;
				else
				{
					return this.path.Remove(index + 1);
				}
			}
		}
	}
}
