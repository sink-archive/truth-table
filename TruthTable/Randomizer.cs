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
			// get all fields and props
			var vars   = type.GetMembers().Where(m => m.MemberType is MemberTypes.Field or MemberTypes.Property).ToArray();
			var fields = vars.Where(m => m.MemberType == MemberTypes.Field).Select(m => (FieldInfo) m).ToArray();
			var props  = vars.Where(m => m.MemberType == MemberTypes.Property).Select(m => (PropertyInfo) m).ToArray();
			
			// randomise each field and prop
			foreach (var field in fields) field.SetValue(item, Randomize(field.FieldType, ref randInstance));
			foreach (var prop in props) prop.SetValue(item, Randomize(prop.PropertyType, ref randInstance));
		}

		public static object? Randomize(Type type, ref Random randInstance)
		{
			switch (type.FullName)
			{
				// for integer, just get the next random int
				case "System.Byte":
					return randInstance.Next() / (int.MaxValue / byte.MaxValue);
				case "System.SByte":
					return randInstance.Next() / (int.MaxValue / sbyte.MaxValue);
				case "System.Int16":
					return randInstance.Next() / (int.MaxValue / short.MaxValue);
				case "System.Int32":
					return randInstance.Next();
				case "System.Int64":
					return randInstance.Next() / (int.MaxValue / long.MaxValue);
				
				// for floats, do some funky math with some recursive calls & casting for other precision levels
				case "System.Half":
					return (Half) Randomize(typeof(double), ref randInstance)!;
				case "System.Single":
					return (float) Randomize(typeof(double), ref randInstance)!;
				case "System.Double":
					// no its not overcomplicated i want some actually interesting random numbers :P
					return (randInstance.Next() + randInstance.NextDouble()) * (randInstance.Next() + randInstance.NextDouble());
				case "System.Decimal":
					return (decimal) Randomize(typeof(double), ref randInstance)!;
				
				// get a random lowercase char - works because of implicit char => int conversion :)
				case "System.Char":
					return (char) randInstance.Next('a', 'z');
				
				// just makes 128 random chars
				case "System.String":
					const int stringLength = 128;
					var       chars        = new char [stringLength];
					
					for (var i = 0; i < stringLength; i++)
						chars[i] = (char) randInstance.Next('a', 'z');

					return chars.Aggregate(string.Empty, (current, next) => current + next);
				
				// gets 0 or 1 and checks if equal to 1 to generate a bool
				case "System.Boolean":
					return randInstance.Next(2) == 1;
				
				// else assume a class or struct and randomise each field and prop in it. !! MAY BREAK SOME THINGS !!
				default:
					if (!TryMakeNew(type, out var obj)) break;
					RandomizeClass(ref obj, ref randInstance);
					return obj;
			}

			// just here because otherwise it'll error even though the default: case catches all.
			throw new InvalidOperationException();
		}

		public static bool TryMakeNew(Type type, out object? obj)
		{
			obj = null;

			// Try all methods of generating values one after the other.
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