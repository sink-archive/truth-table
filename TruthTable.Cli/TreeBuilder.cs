using System;
using System.Collections.Generic;
using System.Linq;

namespace TruthTable.Cli
{
	public static class TreeBuilder
	{
		public static TreeNode GenerateTree(StringParser.FunctionNode[] flatNodes)
		{
			// create a flat list of tree nodes
			var flatTreeNodes = FunctionNodesToTreeNodes(flatNodes);

			return GroupIntoHierarchyRecursive(flatTreeNodes);
		}

		private static TreeNode GroupIntoHierarchyRecursive(WorkingTreeNode[] flatNodes)
		{
			// operator precedence:
			// NOT
			// NAND
			// AND
			// NOR
			// XOR
			// OR
			
			// create working and recurse
			var working = flatNodes.Select(node => node.Node ?? GroupIntoHierarchyRecursive(node.Nodes.ToArray()))
								   .ToList();

			// get NOTs
			var notActive = false;
			for (var i = 0; i < working.Count; i++)
			{
				var node = working[i];
				if (notActive)
				{
					working[i] = new(Operations.NOT, node);
					working.RemoveAt(i - 1); // remove the NOT
					notActive  = false;
				}

				if (node.Operation == Operations.NOT) notActive = true;
			}

			// get all others
			go(Operations.NAND);
			go(Operations.AND);
			go(Operations.NOR);
			go(Operations.XOR);
			go(Operations.OR);

			return working[0];


			// ReSharper disable once InconsistentNaming
			void go(Operations op) => GroupOperation(working, op);
		}

		private static void GroupOperation(List<TreeNode> working, Operations operation)
		{
			TreeNode lastParam  = null;
			var      opActive = false;
			for (var i = 0; i < working.Count; i++)
			{
				var node = working[i];
				if (opActive)
				{
					working[i] = new(operation, lastParam, node);
					working.RemoveAt(i - 1); // remove the left operand
					working.RemoveAt(i - 2); // remove the operator

					opActive = false;
					continue;
				}

				if (node.Operation == operation)
					opActive   = true;
				else lastParam = node;
			}
		}

		private static WorkingTreeNode[] FunctionNodesToTreeNodes(StringParser.FunctionNode[] flatNodes)
		{
			var working = new List<WorkingTreeNode>();
			foreach (var node in flatNodes)
			{
				switch (node.Type)
				{
					case StringParser.NodeTypes.Parameter:
						working.Add(new(new TreeNode(node.ParameterName)));
						break;
					case StringParser.NodeTypes.Operation:
						// ReSharper disable once PossibleInvalidOperationException
						working.Add(new(new TreeNode(node.Operation.Value, null)));
						break;
					case StringParser.NodeTypes.Parenthesis:
						working.Add(new(FunctionNodesToTreeNodes(node.SubNodes)));
						break;
				}
			}

			return working.ToArray();
		}

		private class WorkingTreeNode
		{
			public WorkingTreeNode(TreeNode node)
			{
				Node = node;
			}

			public WorkingTreeNode(IEnumerable<WorkingTreeNode> nodes)
			{
				Nodes = nodes.ToList();
			}

			public TreeNode              Node;
			public List<WorkingTreeNode> Nodes = new();
		}

		public class TreeNode
		{
			public TreeNode(Operations op, TreeNode left, TreeNode right = null)
			{
				LeftNode  = left;
				RightNode = right;
				Operation = op;
			}

			public TreeNode(string paramName)
			{
				_paramName = paramName;
			}
			
			public  TreeNode    LeftNode;
			public  TreeNode    RightNode;
			public  Operations? Operation;
			private string      _paramName;

			public void ToLiteralNode(string paramName) => _paramName = paramName;

			public bool GetValue(Dictionary<string, bool?> @params)
			{
				// ReSharper disable once PossibleInvalidOperationException
				return @params.TryGetValue(_paramName, out var result)
					? result.Value
					: Operation switch
					{
						Operations.NOT  => !LeftNode.GetValue(@params),
						Operations.OR   => LeftNode.GetValue(@params) || RightNode.GetValue(@params),
						Operations.AND  => LeftNode.GetValue(@params) && RightNode.GetValue(@params),
						Operations.XOR  => LeftNode.GetValue(@params) ^ RightNode.GetValue(@params),
						Operations.NOR  => !(LeftNode.GetValue(@params) || RightNode.GetValue(@params)),
						Operations.NAND => !(LeftNode.GetValue(@params) && RightNode.GetValue(@params)),
						_               => throw new ArgumentOutOfRangeException()
					};
			}
		}
	}
}