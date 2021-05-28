using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

//[assembly:InternalsVisibleTo("TruthTable.Tests")]

namespace TruthTable
{
    public class TruthTester
    {
		/// <summary>
		/// Tests the truth of a function
		/// </summary>
		/// <param name="toTest">The function to test</param>
		/// <param name="caseLimit">When generating test cases, what the maximum amount to generate is PER PARAMATER</param>
		public void TestTruth(Delegate toTest, int caseLimit)
		{
			var parameters = toTest.Method.GetParameters().ToDictionary(p => p, _ => Array.Empty<object>());
			foreach (var param in parameters.Keys)
				parameters[param] = GenerateValues(param.ParameterType, caseLimit);
			
			
		}

		private static object[][] MakeCombos(object[][] inputArrays)
		{
			// make some lists :)

			var recurseLevel = -1;

			return ComboRecurse(inputArrays, ref recurseLevel);
		}

		private static object[][] ComboRecurse(object[][] inputArrays, ref int recurseLevel)
		{
			// keep track of recursion
			recurseLevel++;
			
			// if we're at the bottom of the recurse, just return the elements of the array in a suitable way for later
			if (recurseLevel >= inputArrays.Length - 1)
			{
				recurseLevel--;
				return inputArrays[recurseLevel + 1].Select(o => new [] { o }).ToArray();
			}

			// get a working list to use temporarily
			var working = new List<List<object>>();
			
			// recurse down!
			var next = ComboRecurse(inputArrays, ref recurseLevel);
			
			// generate the possibilities
			// for each item in the current array
			foreach (var item in inputArrays[recurseLevel])
			{
				// add this item, and each of the arrays from next (appended) to working
				foreach (var nextArr in next)
				{
					var list = new List<object> {item};
					list.AddRange(nextArr);
					working.Add(list);
				}
			}

			// go up, either to upper recurse level or exiting the recursing entirely
			recurseLevel--;
			return working.Select(l => l.ToArray()).ToArray();
		}

		private static object[] GenerateValues(Type type, int caseLimit)
		{
			var method = typeof(TruthTester).GetMethods().First(m => m.Name == "GenerateValues" && m.ContainsGenericParameters);

			method.MakeGenericMethod(type);
			return (object[]) method.Invoke(null, new object[] {caseLimit});
		}

		private static T[] GenerateValues<T>(int caseLimit)
		{
			var randInstance = new Random();
			var working      = new List<T>();
			for (var i = 0; i < caseLimit; i++)
			{
				working.Add(GenerateValue(caseLimit, ref randInstance, working, out var done));
				if (done) break;
			}

			return working.ToArray();
		}

		private static T GenerateValue<T>(int      caseLimit, ref Random randInstance, IEnumerable<T> previousItems,
										  out bool done)
		{
			done = false;
			if (!Randomizer.TryMakeNew(typeof(T), out var randomItem))
				return default;
			var count = 0;
			// ReSharper disable once PossibleMultipleEnumeration
			while (previousItems.Contains((T) randomItem) || count == 0)
			{
				if (count >= caseLimit)
				{
					done = true;
					break;
				}

				Randomizer.Randomize(ref randomItem, ref randInstance);
				count++;
			}

			return (T) randomItem;
		}
	}
}
