using System;
using System.Linq;
using NUnit.Framework;

namespace TruthTable.Tests
{
	public class TruthTesterTests
	{
		[Test]
		public void LogicalXorTest()
		{
			Func<bool, bool, bool> func = (a, b) => a ^ b;

			var actual = TruthTester.TestTruth(func, 999);

			var expected = new[]
			{
				new Result
				{
					Inputs = new()
					{
						{ "a", false }, { "b", false }
					},
					Output = false
				},
				new Result
				{
					Inputs = new()
					{
						{ "a", false }, { "b", true }
					},
					Output = true
				},
				new Result
				{
					Inputs = new()
					{
						{ "a", true }, { "b", false }
					},
					Output = true
				},
				new Result
				{
					Inputs = new()
					{
						{ "a", false }, { "b", false }
					},
					Output = false
				}
			};

			// check that all results are in the array
			foreach (var item in actual.ResultArray) Assert.Contains(item, expected);
			// check that all in array are in the results
			foreach (var item in expected) Assert.Contains(item, actual.ResultArray);
			
			// check that names are correct because why not
			Assert.AreEqual(new [] {"a", "b"},
							actual.AllInputNames.OrderBy(s => s).ToArray());
		}

		[Test]
		public void ComplexTest()
		{
			Func<int, string, bool, int> func = (a, b, c) => c ? b.Length : a;

			var actual = TruthTester.TestTruth(func, 5);

			foreach (var result in actual.ResultArray)
			{
				var (args, output) = (result.Inputs.ToArray(), result.Output);
				// check names
				Assert.AreEqual("a", args[0].Key);
				Assert.AreEqual("b", args[1].Key);
				Assert.AreEqual("c", args[2].Key);
				// check result type
				Assert.AreEqual(typeof(int), output.GetType());
				// check output
				var a = (int) args[0].Value;
				var b = (string) args[1].Value;
				var c = (bool) args[2].Value;
				Assert.AreEqual(func(a, b, c), (int) output);
			}
		}
	}
}