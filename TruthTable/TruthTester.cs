using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//[assembly:InternalsVisibleTo("TruthTable.Tests")]

namespace TruthTable
{
    public static class TruthTester
    {
		/// <summary>
		/// Tests the truth of a function
		/// </summary>
		/// <param name="toTest">The function to test</param>
		/// <param name="caseLimit">When generating test cases, what the maximum amount to generate is PER PARAMATER</param>
		public static (object[], object)[] TestTruth(Delegate toTest, int caseLimit)
		{
			// get parameters of the method
			var parameters = toTest.Method.GetParameters().ToDictionary(p => p, _ => Array.Empty<object>());
			// generate some random values for the params
			foreach (var param in parameters.Keys)
				parameters[param] = GenerateValues(param.ParameterType, caseLimit).Select(o => o).ToArray();

			// dictionary to array in preparation
			var possibleParameters = parameters.Select(p => p.Value).ToArray();
			// generate all possible combinations of parameter values
			var combos = ComboEntry(possibleParameters);

			// test 'em all!!!!
			var results = combos
						 .Select(combo => (combo,
										   toTest.Method.Invoke(toTest.Target, combo)))
						 .ToArray();

			return results;
		}

		private static object[][] ComboEntry(object[][] inputArrays)
		{
			// make some lists :)
			var recurseLevel = -1;
			return ComboRecurse(inputArrays, ref recurseLevel);
		}

		/// <summary>
		/// If you're calling this anywhere that isn't ComboEntry YOURE DOING SOMETHING WRONG, DO NOT USE!!!!
		/// </summary>
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
			// get the method we need
			var method = typeof(TruthTester)
						.GetMethods(BindingFlags.NonPublic
								  | BindingFlags.Public 
								  | BindingFlags.Static 
								  | BindingFlags.FlattenHierarchy)
						.First(m => m.Name == "GenerateValuesGeneric");

			// apply the type params
			method = method.MakeGenericMethod(type);
			
			// call it and get the correct type for the return value
			return (object[]) method.Invoke(null, new object[] {caseLimit});
		}

		// ReSharper disable once UnusedMember.Local
		private static object[] GenerateValuesGeneric<T>(int caseLimit)
		{
			// setup
			var randInstance = new Random();
			var working      = new List<object>();
			// generate values until we hit the limit or are done
			for (var i = 0; i < caseLimit; i++)
			{
				var item = GenerateValue<T>(caseLimit, ref randInstance, working, out var done);
				if (done) break;
				working.Add(item);
			}

			return working.ToArray();
		}

		private static T GenerateValue<T>(int      caseLimit, ref Random randInstance, IEnumerable<object> previousItems,
										  out bool done)
		{
			done = false;
			
			// ask for a new instance of the object, if fails return default.
			if (!Randomizer.TryMakeNew(typeof(T), out var randomItem))
				return default;
			var count = 0;
			
			// get random values until we hit caseLimit with no duplicates
			// ReSharper disable once PossibleMultipleEnumeration
			while (previousItems.Contains((T) randomItem) || count == 0)
			{
				if (count >= caseLimit)
				{
					done = true;
					break;
				}

				randomItem = Randomizer.Randomize(typeof(T), ref randInstance);
				count++;
			}

			return (T) randomItem;
		}
	}
}
