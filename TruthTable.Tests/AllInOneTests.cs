using System;
using NUnit.Framework;

namespace TruthTable.Tests
{
	public class AllInOneTests
	{
		[Test]
		public void LogicalNandXorTest()
		{
			Func<bool, bool, bool, bool> func = (a, b, c) => !(a && b) ^ c;

			const string expected = @"|   a   |   b   |   c   | result |
|-------|-------|-------|--------|
| False | False | False | True   |
| False | False | True  | False  |
| False | True  | False | True   |
| False | True  | True  | False  |
| True  | False | False | True   |
| True  | False | True  | False  |
| True  | True  | False | False  |
| True  | True  | True  | True   |
|-------|-------|-------|--------|";

			var actual = func.GetFormattedResults();
			
			Assert.AreEqual(expected, actual);
		}
	}
}