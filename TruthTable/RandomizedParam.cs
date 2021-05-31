using System.Reflection;

namespace TruthTable
{
	public class RandomizedParam
	{
		public ParameterInfo Param;
		public string        ParamName => Param.Name;
		public object[]      RandomValues;
	}
}