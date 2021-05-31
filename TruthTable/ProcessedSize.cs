using System;
using System.Collections.Generic;

namespace TruthTable
{
	public class ProcessedSize
	{
		public int Size;
		public int LeftPadding;
		public int RightPadding;

		public ProcessedSize(int size, int leftPadding, int rightPadding)
		{
			Size         = size;
			LeftPadding  = leftPadding;
			RightPadding = rightPadding;
		}

		public static Dictionary<string, ProcessedSize> ProcessSizes(Dictionary<string, int> inputSizes)
		{
			var processedSizes = new Dictionary<string, ProcessedSize>();
			
			foreach (var inputSize in inputSizes)
			{
				var (name, size) = inputSize;

				processedSizes[name] = ProcessSize(name, size);
			}

			return processedSizes;
		}

		public static ProcessedSize ProcessSize(string name, int size)
		{
			var nameLength = name.Length;

			// if name is longer than result make it the new size
			size = Math.Max(size, nameLength);

			// do some maths for the padding, if text doesnt perfectly center itll be on the left slightly
			var nameMidpoint = nameLength / 2.0;
			var midpoint     = size       / 2.0;
			var leftPadding  = (int) Math.Floor(midpoint   - nameMidpoint);
			var rightPadding = (int) Math.Ceiling(midpoint - nameMidpoint);
			return new(size, leftPadding, rightPadding);
		}
	}
}