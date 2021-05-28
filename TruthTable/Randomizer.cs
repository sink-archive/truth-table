#nullable enable
using System;
using System.Linq;
using System.Reflection;

namespace TruthTable
{
	public static class Randomizer
	{
		public static void Randomize<T>(ref T item)
		{
			var rand = new Random();
			Randomize(ref item, ref rand);
		}
		public static void Randomize<T>(ref T item, ref Random randInstance)
		{
			var type   = typeof(T);
			var vars   = type.GetMembers().Where(m => m.MemberType is MemberTypes.Field or MemberTypes.Property).ToArray();
			var fields = vars.Where(m => m.MemberType == MemberTypes.Field).Select(m => (FieldInfo) m).ToArray();
			var props  = vars.Where(m => m.MemberType == MemberTypes.Property).Select(m => (PropertyInfo) m).ToArray();
			
			foreach (var field in fields) field.SetValue(item, RandomValue(field.FieldType, ref randInstance));
			foreach (var prop in props) prop.SetValue(item, RandomValue(prop.PropertyType, ref randInstance));
		}

		private static object? RandomValue(Type type, ref Random randInstance)
		{
			switch (type.FullName)
			{
				case "System.Int16":
				case "System.Int32":
				case "System.Int64":
					return randInstance.Next();
				case "System.Half":
				case "System.Single":
				case "System.Double":
				case "System.Decimal":
					return (randInstance.Next() + randInstance.NextDouble()) * (randInstance.Next() + randInstance.NextDouble());
				case "System.String":
					const int stringLength = 128;
					var       chars        = new char [stringLength];
					
					for (var i = 0; i < stringLength; i++)
						chars[i] = (char) randInstance.Next('a', 'z');

					return chars.Aggregate(string.Empty, (current, next) => current + next);
				case "System.Boolean":
					return randInstance.Next(1) == 1;
				default:
					// Assuming a class or struct
					if (TryMakeNew(type, out var obj)) break;
					Randomize(ref obj, ref randInstance);
					return obj;
			}

			throw new InvalidOperationException();
		}

		public static bool TryMakeNew(Type type, out object? obj)
		{
			obj = new object();
			var constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor == null)
				return true;
			obj = constructor.Invoke(Array.Empty<object>());
			return false;
		}
	}
}