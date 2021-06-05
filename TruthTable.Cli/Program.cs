using System;
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
				Console.WriteLine("Please enter a logic function: ");
				rawFunc = Console.ReadLine();
			}

			var parsed = StringParser.Parse(rawFunc, out var isLambda);

			var tree = TreeBuilder.GenerateTree(parsed.Nodes);
		}
	}
}