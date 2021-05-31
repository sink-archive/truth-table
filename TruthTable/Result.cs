#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TruthTable
{
	[DebuggerDisplay("{" + nameof(DebugDisplay) + "}")]
	public class Result
	{
		public Dictionary<string, object> Inputs = new();
		public object?                    Output;

		private string DebugDisplay => $"{string.Join(' ', Inputs.Select(p => $"[{p.Key}: {p.Value}]"))} -> {Output}";

		public override bool Equals(object? obj) => obj is Result r && Equals(r);

		protected bool Equals(Result other) => other.Inputs.SequenceEqual(Inputs) && other.Output == Output;

		public override int GetHashCode() => HashCode.Combine(Inputs, Output);
	}
}