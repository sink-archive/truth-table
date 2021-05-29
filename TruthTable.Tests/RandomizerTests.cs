using System;
using System.Linq;
using NUnit.Framework;

namespace TruthTable.Tests
{
	public class RandomizerTests
	{
		[Test]
		public void TryMakeNewTest()
		{
			Assert.IsTrue(Randomizer.TryMakeNew(typeof(MyClass), out var obj));
			Assert.AreEqual(new MyClass { MyInnerClass = null }, (MyClass) obj);
		}

		[Test]
		public void RandomStrings()
		{
			var rand          = new Random();
			var randomStrings = Enumerable.Range(1, 5)
										  .Select(_ => Randomizer.Randomize(typeof(string), ref rand))
										  .ToArray();

			foreach (var s in randomStrings)
			{
				Assert.AreEqual(typeof(string), s.GetType());
				Assert.Greater(((string) s).Length, 0);
			}
		}

		[Test]
		public void RandomClass()
		{
			var rand = new Random();
			var randomObjs = Enumerable.Range(1, 5)
									   .Select(_ => Randomizer.Randomize(typeof(MyClass), ref rand))
									   .ToArray();

			foreach (var obj in randomObjs)
			{
				Assert.AreEqual(typeof(MyClass), obj.GetType());
				var fields      = obj.GetType().GetFields();
				var fieldTypes = fields.Select(p => p.FieldType).ToArray();
				Assert.Contains(typeof(MyInnerClass), fieldTypes);
				Assert.Contains(typeof(double), fieldTypes);

				var innerFields     = fields.First(p => p.Name == "MyInnerClass").FieldType.GetFields();
				var innerFieldTypes = innerFields.Select(p => p.FieldType).ToArray();
				Assert.Contains(typeof(bool), innerFieldTypes);
				Assert.Contains(typeof(int), innerFieldTypes);
			}
		}

		private class MyClass
		{
			public MyInnerClass MyInnerClass;
			public double       theDouble;

			public override bool Equals(object? obj) => obj is MyClass mc && Equals(mc);
			protected bool Equals(MyClass other) => Equals(MyInnerClass, other.MyInnerClass) && theDouble.Equals(other.theDouble);
			public override int GetHashCode() => HashCode.Combine(MyInnerClass, theDouble);
		}

		private class MyInnerClass
		{
			public bool TheBool;
			public int  TheInt;
		}
	}
}