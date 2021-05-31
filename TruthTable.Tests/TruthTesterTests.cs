using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TruthTable.Tests
{
	public class TruthTesterTests
	{
		[Test]
		public void LogicalXorTest()
		{
			static KeyValuePair<string, object> p(string a, object b) => new(a, b);

			Func<bool, bool, bool> func = (a, b) => a ^ b;

			var actual = TruthTester.TestTruth(func, 999);

			var expectedParams = new[]
			{
				new[] { p("a", false), p("b", false) },
				new[] { p("a", false), p("b", true)  },
				new[] { p("a", true),  p("b", false) },
				new[] { p("a", true),  p("b", true)  }
			};

			var expectedResults = new object[] { false, true, true, false };

			// check that all results are in the array
			foreach (var item in actual.ResultArray)
			{
				var index = FindIndexSeq(expectedParams, item.Inputs);
				Assert.AreEqual(expectedParams[index], item.Inputs.ToArray());
				Assert.AreEqual(expectedResults[index], item.Output);
			}

			// check that names are correct because why not
			Assert.AreEqual(new [] {"a", "b"},
							actual.AllInputNames.OrderBy(s => s).ToArray());
		}

		private static int FindIndexSeq<T>(IEnumerable<IEnumerable<T>> collection, IEnumerable<T> item)
		{
			var array = collection as IEnumerable<T>[] ?? collection.ToArray();
			var itemArr = item as T[] ?? item.ToArray();
			for (var i = 0; i < array.Length; i++)
			{
				if (itemArr.SequenceEqual(array[i]))
					return i;
			}

			return -1;
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