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
	}
}