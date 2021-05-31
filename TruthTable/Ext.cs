using System;

namespace TruthTable
{
	public static class Ext
	{
		public static Results GetResults(this Delegate func, int caseLimit = 3) => TruthTester.TestTruth(func, caseLimit);

		public static string Format(this Results results) => ResultFormatter.Format(results);

		public static string GetFormattedResults(this Delegate func, int caseLimit = 3)
			=> func.GetResults(caseLimit).Format();
	}
}