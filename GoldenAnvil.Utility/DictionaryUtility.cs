﻿using System;
using System.Collections.Generic;

namespace GoldenAnvil.Utility
{
	public static class DictionaryUtility
	{
		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultCreator)
		{
			return dictionary.TryGetValue(key, out TValue value) ? value : defaultCreator();
		}

		public static TValue GetOrAddValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
			if (!dictionary.ContainsKey(key))
			{
				dictionary[key] = value;
				return value;
			}

			return dictionary[key];
		}

		public static TValue GetOrAddValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueCreator)
		{
			if (!dictionary.ContainsKey(key))
			{
				var value = valueCreator();
				dictionary[key] = value;
				return value;
			}

			return dictionary[key];
		}
	}
}
