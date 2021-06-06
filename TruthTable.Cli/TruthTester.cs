using System;
using System.Collections.Generic;
using System.Linq;

namespace TruthTable.Cli
{
	// Constructing a DynamicMethod on-the-fly turned out to just be too complex and not worth it, so heres a custom version of TruthTester.TestTruth()
	public static class TruthTester
	{
		public static Results TestTruth(Delegate toTest, string[] @params, int caseLimit = 3)
		{
			// get parameters of the method
			var parameters = @params.ToDictionary(p => p, p => new RandomizedParam{ParamName = p, Type = typeof(bool)});
			// generate some random values for the params
			foreach (var (param, type) in parameters)
				parameters[param].RandomValues = TruthTable.TruthTester.GenerateValues(type.Type, caseLimit).Select(o => o).ToArray();
			
			// generate all possible combinations of parameter values
			var combos = ComboEntry(parameters);

			// test 'em all!!!!
			// this is where query syntax (from x in y select z) beats API syntax y.Select(y => z),
			// mainly because you can define variables with let
			var results = (from c in combos
						   let paramValues = c.Select(p => p.Item2).ToArray()
						   let paramNames = c.Select(p => p.Item1).ToArray()
						   let toPass = c.ToDictionary(p => p.Item1, p => (bool)p.Item2)
						   let result = toTest.Method.Invoke(toTest.Target, new[] { toPass })
						   select new Result
						   {
							   Inputs = paramNames.Zip(paramValues).ToDictionary(t => t.First, t => t.Second),
							   Output = result
						   }).ToArray();

			return new Results { ResultArray = results };
		}
		
		private static (string, object)[][] ComboEntry(Dictionary<string, RandomizedParam> inputParams)
		{
			// make some lists :)
			var recurseLevel = -1;
			
			// convert to object[][]
			var paramInfos   = inputParams.Keys;
			var objectArrays = inputParams.Select(p => p.Value.RandomValues).ToArray();
			
			var combos = TruthTable.TruthTester.ComboRecurse(objectArrays, ref recurseLevel);
			
			// convert back
			return combos.Select(c => paramInfos.Zip(c).ToArray()).ToArray();
		}
	}

	public class RandomizedParam
	{
		public string   ParamName;
		public Type     Type;
		public object[] RandomValues;
	}
}