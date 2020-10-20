using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Engine.Serialization
{
	public static class HumanReadableConverter
	{
		public static Dictionary<Type, IHumanReadableConverter> m_humanReadableConvertersByType = new Dictionary<Type, IHumanReadableConverter>();

		public static HashSet<Assembly> m_scannedAssemblies = new HashSet<Assembly>();

		public static string ConvertToString(object value)
		{
			Type type = value.GetType();
			try
			{
				return GetConverter(type, throwIfNotFound: true).ConvertToString(value);
			}
			catch (Exception innerException)
			{
				throw new InvalidOperationException($"Cannot convert value of type \"{type.FullName}\" to string.", innerException);
			}
		}

		public static bool TryConvertFromString(Type type, string data, out object result)
		{
			try
			{
				result = GetConverter(type, throwIfNotFound: true).ConvertFromString(type, data);
				return true;
			}
			catch (Exception)
			{
				result = null;
				return false;
			}
		}

		public static bool TryConvertFromString<T>(string data, out T result)
		{
			if (TryConvertFromString(typeof(T), data, out object result2))
			{
				result = (T)result2;
				return true;
			}
			result = default(T);
			return false;
		}

		public static object ConvertFromString(Type type, string data)
		{
			try
			{
				return GetConverter(type, throwIfNotFound: true).ConvertFromString(type, data);
			}
			catch (Exception innerException)
			{
				throw new InvalidOperationException($"Cannot convert string \"{data}\" to value of type \"{type.FullName}\".", innerException);
			}
		}

		public static T ConvertFromString<T>(string data)
		{
			return (T)ConvertFromString(typeof(T), data);
		}

		public static bool IsTypeSupported(Type type)
		{
			return GetConverter(type, throwIfNotFound: false) != null;
		}

		public static string ValuesListToString<T>(char separator, params T[] values)
		{
			string[] array = new string[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				array[i] = ConvertToString(values[i]);
			}
			return string.Join(separator.ToString(), array);
		}

		public static T[] ValuesListFromString<T>(char separator, string data)
		{
			if (!string.IsNullOrEmpty(data))
			{
				string[] array = data.Split(separator, StringSplitOptions.None);
				T[] array2 = new T[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = ConvertFromString<T>(array[i]);
				}
				return array2;
			}
			return new T[0];
		}

		public static IHumanReadableConverter GetConverter(Type type, bool throwIfNotFound)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			lock (m_humanReadableConvertersByType)
			{
				if (!m_humanReadableConvertersByType.TryGetValue(type, out IHumanReadableConverter value))
				{
					ScanAssembliesForConverters();
					if (!m_humanReadableConvertersByType.TryGetValue(type, out value))
					{
						if (value == null)
						{
							foreach (KeyValuePair<Type, IHumanReadableConverter> item in m_humanReadableConvertersByType)
							{
								if (type.GetTypeInfo().IsSubclassOf(item.Key))
								{
									value = item.Value;
									break;
								}
							}
						}
						m_humanReadableConvertersByType.Add(type, value);
					}
				}
				if (value != null)
				{
					return value;
				}
				if (throwIfNotFound)
				{
					throw new InvalidOperationException($"IHumanReadableConverter for type \"{type.FullName}\" not found in any loaded assembly.");
				}
				return null;
			}
		}

		public static void ScanAssembliesForConverters()
		{
			foreach (Assembly item in TypeCache.LoadedAssemblies.Where((Assembly a) => !TypeCache.IsKnownSystemAssembly(a)))
			{
				if (!m_scannedAssemblies.Contains(item))
				{
					foreach (TypeInfo definedType in item.DefinedTypes)
					{
						HumanReadableConverterAttribute customAttribute = definedType.GetCustomAttribute<HumanReadableConverterAttribute>();
						if (customAttribute != null && !m_humanReadableConvertersByType.ContainsKey(customAttribute.Type))
						{
							IHumanReadableConverter value = (IHumanReadableConverter)Activator.CreateInstance(definedType.AsType());
							m_humanReadableConvertersByType.Add(customAttribute.Type, value);
						}
					}
					m_scannedAssemblies.Add(item);
				}
			}
		}
	}
}
