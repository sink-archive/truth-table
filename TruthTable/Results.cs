using System.Linq;

namespace TruthTable
{
	public class Results
	{
		public Result[] ResultArray;

		public string[] AllInputNames
			=> ResultArray.SelectMany(r => r.Inputs)
						  .Select(r => r.Key)
						  .Distinct()
						  .ToArray();

		public override bool Equals(object? obj) => obj is Results r && Equals(r);

		protected bool Equals(Results other) => other.ResultArray.SequenceEqual(ResultArray);

		public override int GetHashCode() => ResultArray != null ? ResultArray.GetHashCode() : 0;
	}
}