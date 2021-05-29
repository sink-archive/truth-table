using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TruthTable.Tests
{
	public class TruthTesterTests
	{
		[Test]
		public void LogicalAndTest()
		{
			Func<bool, bool, bool> func = (a, b) => a && b;

			var actual = TruthTester.TestTruth(func, 999);

			var expected = new HashSet<(object[], object)>
			{
				(new object[] {false, false}, false),
				(new object[] {false, true},  false),
				(new object[] {true,  false}, false),
				(new object[] {true,  true},  true)
			};

			foreach (var item in actual) Assert.Contains(item, actual);
		}

		[Test]
		public void ComplexTest()
		{
			Func<int, string, bool, int> func = (a, b, c) => c ? b.Length : a;

			var actual = TruthTester.TestTruth(func, 5);

			foreach (var (args, result) in actual)
			{
				var a = (int) args[0];
				var b = (string) args[1];
				var c = (bool) args[2];
				Assert.AreEqual(func(a, b, c), (int) result);
			}
		}
	}
}