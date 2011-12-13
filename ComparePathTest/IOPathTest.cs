using NginxStarterGUI.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ComparePathTest
{
    
    
    /// <summary>
    ///这是 IOPathTest 的测试类，旨在
    ///包含所有 IOPathTest 单元测试
    ///</summary>
	[TestClass()]
	public class IOPathTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///获取或设置测试上下文，上下文提供
		///有关当前测试运行及其功能的信息。
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region 附加测试特性
		// 
		//编写测试时，还可使用以下特性:
		//
		//使用 ClassInitialize 在运行类中的第一个测试前先运行代码
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//使用 ClassCleanup 在运行完类中的所有测试后再运行代码
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//使用 TestInitialize 在运行每个测试前先运行代码
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//使用 TestCleanup 在运行完每个测试后运行代码
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///IOPath 隐式转换 的测试
		///</summary>
		[TestMethod()]
		public void IOPathImplicitTest()
		{
			string path = "test";
			IOPath target = new IOPath("test");
			string temp = target;
			Assert.AreEqual(temp, path);
		}

		/// <summary>
		///IOPath 反向隐式转换 的测试
		///</summary>
		[TestMethod()]
		public void IOPathImplicitBackTest()
		{
			IOPath ioPath = new IOPath("test");
			string temp = "test";
			IOPath target = temp;
			Assert.AreEqual(target, ioPath);
		}

		/// <summary>
		///ToString 的测试
		///</summary>
		[TestMethod()]
		public void ToStringTest()
		{
			string path = "test"; // TODO: 初始化为适当的值
			IOPath target = new IOPath(path); // TODO: 初始化为适当的值
			string expected = "test"; // TODO: 初始化为适当的值
			string actual;
			actual = target.ToString();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///IsExistedDirectory 的测试
		///</summary>
		[TestMethod()]
		public void IsExistedDirectoryTest1()
		{
			string path = "C:\\temp";
			IOPath target = new IOPath(path);
			bool expected = true;
			bool actual;
			actual = target.IsExistedDirectory();
			Assert.AreEqual(expected, actual);
		}

		[TestMethod()]
		public void IsExistedDirectoryTest2()
		{
			string path = "C:\\temp\\scardsvr.exe";
			IOPath target = new IOPath(path);
			bool expected = false;
			bool actual;
			actual = target.IsExistedDirectory();
			Assert.AreEqual(expected, actual);
		}

		[TestMethod()]
		public void IsExistedDirectoryTest3()
		{
			string path = "";
			IOPath target = new IOPath(path);
			bool expected = false;
			bool actual;
			actual = target.IsExistedDirectory();
			Assert.AreEqual(expected, actual);
		}

		[TestMethod()]
		public void IsExistedDirectoryTest4()
		{
			string path = null;
			IOPath target = new IOPath(path);
			bool expected = false;
			bool actual;
			actual = target.IsExistedDirectory();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///LastDirectory 的测试
		///</summary>
		[TestMethod()]
		public void LastDirectoryTest()
		{
			string path = "C:\\temp\\test.exe"; // TODO: 初始化为适当的值
			IOPath target = new IOPath(path); // TODO: 初始化为适当的值
			string expected = "C:\\temp\\"; // TODO: 初始化为适当的值
			string actual;
			actual = target.LastDirectory();
			Assert.AreEqual(expected, actual);
		}

		[TestMethod()]
		public void LastDirectoryTestL()
		{
			string path = "C:/temp/test.exe"; // TODO: 初始化为适当的值
			IOPath target = new IOPath(path); // TODO: 初始化为适当的值
			string expected = "C:/temp/"; // TODO: 初始化为适当的值
			string actual;
			actual = target.LastDirectory();
			Assert.AreEqual(expected, actual);
		}
	}
}
