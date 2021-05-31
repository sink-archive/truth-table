using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruthTable
{
	public static class ResultFormatter
	{
		public static string Format(Results results)
		{
			// get the max sizes of each input
			var inputMaxSizes = new Dictionary<string, int>();
			foreach (var result in results.ResultArray)
			{
				foreach (var (key, value) in result.Inputs)
					inputMaxSizes[key] = Math.Max(inputMaxSizes.GetValueOrDefault(key), value.ToString()!.Length);
			}
			
			// get the max size of the output
			var outputMaxSize = results.ResultArray.Select(r => r.Output.ToString()!.Length).Max();

			// process all inputs
			var processedSizes = ProcessedSize.ProcessSizes(inputMaxSizes);

			// Create the header
			var final = new StringBuilder();
			final.AppendLine(FormatHeader(processedSizes, outputMaxSize));

			// Create the actual rows
			foreach (var result in results.ResultArray)
				final.AppendLine(FormatRow(result.Inputs
												 .Select(p => new KeyValuePair<object, int>(p.Value, inputMaxSizes[p.Key]))
												 .ToArray(),
										   (result.Output, outputMaxSize)));

			// Add a nicer bottom to the table
			final.Append(FormatSpacerBar(processedSizes
											.Select(p => p.Value.Size)
											.Append(outputMaxSize)
											.ToArray()));
			
			return final.ToString();
		}

		private static string FormatHeader(Dictionary<string, ProcessedSize> inputSizes, int outputSize)
		{
			var sb = new StringBuilder();
			
			// add params
			foreach (var (name, size) in inputSizes)
			{
				sb.Append("| ");
				for (var i = 0; i < size.LeftPadding; i++) sb.Append(' ');
				sb.Append(name);
				for (var i = 0; i <= size.RightPadding; i++) sb.Append(' ');
			}


			// add output
			var processedOutputSize = ProcessedSize.ProcessSize("result", outputSize);
			
			sb.Append("| ");
			for (var i = 0; i < processedOutputSize.LeftPadding; i++) sb.Append(' ');
			sb.Append("result");
			for (var i = 0; i <= processedOutputSize.RightPadding; i++) sb.Append(' ');

			sb.Append("|\n");
			
			// Spacer row markdown-style
			sb.Append(FormatSpacerBar(inputSizes.Select(p => p.Value.Size).Append(outputSize).ToArray()));

			return sb.ToString();
		}

		private static string FormatSpacerBar(IEnumerable<int> inputSizes)
		{
			var sb = new StringBuilder();
			
			foreach (var size in inputSizes)
			{
				sb.Append("|-");
				for (var i = 0; i <= size; i++) sb.Append('-');
			}

			sb.Append("-|");

			return sb.ToString();
		}

		private static string FormatRow(IEnumerable<KeyValuePair<object, int>> inputs, (object, int) output)
		{
			var sb = new StringBuilder();
			
			foreach (var (obj, size) in inputs)
			{
				var processed = ProcessedSize.ProcessSize(obj.ToString(), size);
				
				sb.Append("| ");
				for (var i = 0; i < processed.LeftPadding; i++) sb.Append(' ');
				sb.Append(obj);
				for (var i = 0; i <= processed.RightPadding; i++) sb.Append(' ');
			}

			var (outputObj, outputSize) = output;
			var processedOutputSize = ProcessedSize.ProcessSize(outputObj.ToString(), outputSize);
			
			sb.Append("| ");
			for (var i = 0; i < processedOutputSize.LeftPadding; i++) sb.Append(' ');
			sb.Append(outputObj);
			for (var i = 0; i <= processedOutputSize.RightPadding; i++) sb.Append(' ');

			sb.Append(" |");

			return sb.ToString();
		}
	}
}