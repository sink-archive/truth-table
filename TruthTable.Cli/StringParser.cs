using System;
using System.Collections.Generic;
using System.Linq;

namespace TruthTable.Cli
{
	public static class StringParser
	{
		private static string[] SplitAndTrim(string source, string separator)
			=> source.Split(separator, StringSplitOptions.TrimEntries 
									 | StringSplitOptions.RemoveEmptyEntries);

		public static ParseResult Parse(string raw, out bool isLambda)
		{
			isLambda = false;

			if (TryParseLambda(raw, out var result)) isLambda = true;
			else if (!TryParseSimple(raw, out result))
				throw new ArgumentException("Not a valid function", nameof(raw));

			return result;
		}

		private static bool TryParseLambda(string raw, out ParseResult result)
		{
			result = null;
			
			var split      = SplitAndTrim(raw, "->");
			if (split.Length != 2) return false;

			if (!TryParseSimple(split[1], out result))
				return false;

			result.Parameters = SplitAndTrim(split[0], ",");
			return true;
		}

		private static bool TryParseSimple(string raw, out ParseResult result)
		{
			var processed = raw.Replace("!", " NOT ")                        // NOT
							   .Replace("||", " OR ").Replace("|", " OR ")   // OR
							   .Replace("&&", " AND ").Replace("&", " AND ") // AND
							   .Replace("^",  " XOR ")                       // XOR
							   .Replace("(",  " ( ").Replace(")", " ) ");    // Brackets to help with splitting later

			// correct casing for later switch statements
			processed = processed.Replace("or", "OR", true, null).Replace("and", "AND", true, null)
								 .Replace("xor",  "XOR",  true, null).Replace("nor", "NOR", true, null)
								 .Replace("nand", "NAND", true, null).Replace("not", "NOT", true, null);

			var split       = SplitAndTrim(processed, " ");
			var parsedNodes = ParseNodesRecursive(split);

			result = new()
			{
				Nodes = parsedNodes,
				Parameters = (from n in parsedNodes
							  where n.Type == NodeTypes.Parameter
							  select n.ParameterName).ToArray()
			};
			return true;
		}

		private static FunctionNode[] ParseNodesRecursive(string[] split)
		{
			var working = new List<FunctionNode>();
			for (var i = 0; i < split.Length; i++)
			{
				switch (split[i])
				{
					case "NOT":
						working.Add(new(Operations.NOT));
						break;
					case "OR":
						working.Add(new(Operations.OR));
						break;
					case "AND":
						working.Add(new(Operations.AND));
						break;
					case "XOR":
						working.Add(new(Operations.XOR));
						break;
					case "NOR":
						working.Add(new(Operations.NOR));
						break;
					case "NAND":
						working.Add(new(Operations.NAND));
						break;
					
					case "(":
						var bracketRange = BracketRange(split, i);
						i += bracketRange.Length + 1;
						working.Add(new(ParseNodesRecursive(bracketRange)));
						break;
					
					default:
						working.Add(new(split[i]));
						break;
				}
			}

			return working.ToArray();
		}

		private static string[] BracketRange(string[] items, int startIndex)
		{
			var working = new List<string>();
			for (var i = startIndex + 1; i < items.Length; i++)
			{
				if (items[i] == ")")
					return working.ToArray();
				
				working.Add(items[i]);
			}

			throw new Exception($"Unclosed bracket (opened at position {startIndex})");
		}
		
		public class ParseResult
		{
			public string[]       Parameters;
			public FunctionNode[] Nodes;
		}

		public class FunctionNode
		{
			public FunctionNode(string parameterName)
			{
				Type          = NodeTypes.Parameter;
				ParameterName = parameterName;
			}

			public FunctionNode(Operations operation)
			{
				Type      = NodeTypes.Operation;
				Operation = operation;
			}

			public FunctionNode(FunctionNode[] subNodes)
			{
				Type     = NodeTypes.Parenthesis;
				SubNodes = subNodes;
			}
			
			public NodeTypes      Type          { get; }
			public Operations?    Operation     { get; }
			public FunctionNode[] SubNodes      { get; }
			public string         ParameterName { get; }
		}

		public enum NodeTypes
		{
			Parameter,
			Operation,
			Parenthesis
		}
	}
	
	public enum Operations
	{
		NOT, OR, AND, XOR, NOR, NAND
	}
}