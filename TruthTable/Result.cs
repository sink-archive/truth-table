#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace TruthTable
{
	public class Result
	{
		public Dictionary<string, object> Inputs = new();
		public object?                    Output;

		public override bool Equals(object? obj) => obj is Result r && Equals(r);

		protected bool Equals(Result other) => other.Inputs.SequenceEqual(Inputs) && other.Output == Output;

		public override int GetHashCode() => HashCode.Combine(Inputs, Output);
	}
}