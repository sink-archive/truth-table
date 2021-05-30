#nullable enable
using System;
using System.Linq;
using System.Reflection;

namespace TruthTable
{
	public static class Randomizer
	{
		private static void RandomizeClass(ref object item, ref Random randInstance)
		{
			var type   = item.GetType();
			var vars   = type.GetMembers().Where(m => m.MemberType is MemberTypes.Field or MemberTypes.Property).ToArray();
			var fields = vars.Where(m => m.MemberType == MemberTypes.Field).Select(m => (FieldInfo) m).ToArray();
			var props  = vars.Where(m => m.MemberType == MemberTypes.Property).Select(m => (PropertyInfo) m).ToArray();
			
			foreach (var field in fields) field.SetValue(item, Randomize(field.FieldType, ref randInstance));
			foreach (var prop in props) prop.SetValue(item, Randomize(prop.PropertyType, ref randInstance));
		}

		public static object? Randomize(Type type, ref Random randInstance)
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
					// no its not overcomplicated i want some actually interesting random numbers :P
					return (randInstance.Next() + randInstance.NextDouble()) * (randInstance.Next() + randInstance.NextDouble());
				case "System.Char":
					return (char) randInstance.Next();
				case "System.String":
					const int stringLength = 128;
					var       chars        = new char [stringLength];
					
					for (var i = 0; i < stringLength; i++)
						chars[i] = (char) randInstance.Next('a', 'z');

					return chars.Aggregate(string.Empty, (current, next) => current + next);
				case "System.Boolean":
					return randInstance.Next(2) == 1;
				default:
					// Assuming a class or struct
					if (!TryMakeNew(type, out var obj)) break;
					RandomizeClass(ref obj, ref randInstance);
					return obj;
			}

			throw new InvalidOperationException();
		}

		public static bool TryMakeNew(Type type, out object? obj)
		{
			obj = null;

			return TrySpecialCases(type, out obj)
				|| TryConstructor(type, out obj)
				|| TryActivator(type, out obj);
		}

		private static bool TrySpecialCases(Type type, out object? obj)
		{
			obj = null;
			
			if (type == typeof(string))
			{
				obj = string.Empty;
				return true;
			}
			
			return false;
		}

		private static bool TryConstructor(Type type, out object? obj)
		{
			obj = null;

			try
			{
				obj = type.GetConstructors()
						  .First(c => c.GetParameters().Length == 0)
						  .Invoke(null);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		
		private static bool TryActivator(Type type, out object? obj)
		{
			obj = null;
			
			try
			{
				obj = Activator.CreateInstance(type);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}