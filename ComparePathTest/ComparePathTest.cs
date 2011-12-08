using NginxStarterGUI.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ComparePathTest
{
    
    
    /// <summary>
    ///这是 ComparePathTest 的测试类，旨在
    ///包含所有 ComparePathTest 单元测试
    ///</summary>
	[TestClass()]
	public class ComparePathTest
	{


		private TestContext testContextInstance;
		private string pathA;
		private string pathB;
		private char separator;

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

		[TestInitialize()]
		public void MyTestInitialize()
		{
			pathA = "C:/temp1/temp2/temp3/temp4.exe"; // TODO: 初始化为适当的值
			pathB = "C:\\temp1\\temp2\\temz\\"; // TODO: 初始化为适当的值
			separator = '/'; // TODO: 初始化为适当的值
		}

		/// <summary>
		///Compare 的测试
		///</summary>
		[TestMethod()]
		[DeploymentItem("Nginx Starter GUI.exe")]
		public void CompareTest()
		{
			string expected = "C:/temp1/temp2"; // TODO: 初始化为适当的值
			string actual;
			actual = ComparePath_Accessor.Compare(pathA, pathB, separator);
			Assert.AreEqual<string>(expected, actual);
			expected = "C:\\temp1\\temp2";
			actual = ComparePath_Accessor.Compare(pathA, pathB);
			Assert.AreEqual<string>(expected, actual);
		}

		[TestMethod()]
		public void CompareTest2()
		{
			string expectedA = "temp3/temp4.exe";
			string expectedB = "temz/";
			string header = ComparePath_Accessor.Compare(pathA, pathB, separator);
			int headerLength = header.Length + 1;
			pathA = pathA.Substring(headerLength);
			pathB = pathB.Substring(headerLength);
			Assert.AreEqual<string>(expectedA, pathA);
			Assert.AreEqual<string>(expectedB, pathB);
		}

		[TestMethod()]
		public void CompareTest3()
		{
			pathB = string.Empty;
			string expected = string.Empty;
			string header = ComparePath_Accessor.Compare(pathA, pathB, separator);
			Assert.AreEqual<string>(expected, header);
		}
	}
}
