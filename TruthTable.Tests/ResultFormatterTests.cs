using NUnit.Framework;

namespace TruthTable.Tests
{
	public class ResultFormatterTests
	{
		[Test]
		public void SimpleTest()
		{
			var results = new Results
			{
				ResultArray = new []
				{
					new Result
					{
						Inputs = new()
						{
							{"a", true},
							{"b", true}
						},
						Output = false
					},
					new Result
					{
						Inputs = new()
						{
							{"a", true},
							{"b", false}
						},
						Output = true
					},
					new Result
					{
						Inputs = new()
						{
							{"a", false},
							{"b", true}
						},
						Output = true
					},
					new Result
					{
						Inputs = new()
						{
							{"a", false},
							{"b", false}
						},
						Output = false
					}
				}
			};

			var actual = ResultFormatter.Format(results);

			const string expected = @"|   a   |   b   | result |
|-------|-------|--------|
| True  | True  | False  |
| True  | False | True   |
| False | True  | True   |
| False | False | False  |
|-------|-------|--------|";
			
			Assert.AreEqual(expected, actual);
		}
	}
}