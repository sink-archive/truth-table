using System;
using System.Linq;
using NUnit.Framework;

namespace TruthTable.Tests
{
	/// <summary>
	/// These arent really tests - just easy ways for me to get (a) my code running, (b) a debugger in there
	/// </summary>
	public class Debugging
	{
		[SetUp]
		public void Setup() { }

		[Test]
		public void Test1()
		{
			var arrays = new[]
			{
				new object[]
				{
					1,2,3
				},
				new object[]
				{
					'a','b','c'
				},
				new object[]
				{
					'A','B','C'
				}
			};
			var recurseLevel = -1;
			// this didnt error but then i made it not internal
			//TruthTester.ComboRecurse(arrays, ref recurseLevel);

			Assert.Pass();
		}

		[Test]
		public void Test2()
		{
			// complex enough to hopefully test a little :)
			//    /-parameter types  /-return type
			Func<bool, string, int, int> TestFunc =
				(testBool, testString, testInt) => testBool ? testString.Length : testInt * 2;

			var results = TruthTester.TestTruth(TestFunc, 5);
		}
	}
}