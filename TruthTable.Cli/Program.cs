using System;
using System.Collections.Generic;
using System.Linq;

namespace TruthTable.Cli
{
	class Program
	{
		static void Main(string[] args)
		{
			var rawFunc = args.Any() ? args[0] : null;
			if (string.IsNullOrWhiteSpace(rawFunc))
			{
				Console.Write("Please enter a logic function: ");
				rawFunc = Console.ReadLine();
			}

			var parsed = StringParser.Parse(rawFunc, out var isLambda);

			Console.WriteLine($"Function type: {(isLambda ? "lambda" : "simple")}");
			Console.WriteLine($"Parameters: {string.Join(' ', parsed.Parameters)}");
			
			var tree = TreeBuilder.GenerateTree(parsed.Nodes);

			Func<Dictionary<string, bool>, bool> getValueInvoker = a => tree.GetValue(a);
			
			var results = TruthTester.TestTruth(getValueInvoker, parsed.Parameters, 999); // 999 makes sure we almost certainly get a true & false. Hacky but sure.
			
			Console.WriteLine(results.Format());
		}
	}
}