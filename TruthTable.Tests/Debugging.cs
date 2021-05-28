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
	}
}