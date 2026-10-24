using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utils.NonAllocLINQ;

// Token: 0x020001D2 RID: 466
public static class Translations
{
	// Token: 0x060009A8 RID: 2472 RVA: 0x00026CEC File Offset: 0x00024EEC
	private static void DefineKey(string key)
	{
		string formatted = key.Replace("_", string.Empty).ToLowerInvariant();
		Translations.TypeToFilename[formatted] = key;
		Translations.Suffixes.ForEach(delegate(string x)
		{
			Translations.TypeToFilename[formatted + x] = key;
		});
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x00026D54 File Offset: 0x00024F54
	private static bool TryGenerate(Type enumType, out Translations.LoadedTranslation[] generated)
	{
		if (!Translations._pascalCaseGenerated)
		{
			Translations._pascalCaseGenerated = true;
			Dictionary<string, string[]> fallback = TranslationReader.Fallback;
			Action<string> action;
			if ((action = Translations.<>O.<0>__DefineKey) == null)
			{
				action = (Translations.<>O.<0>__DefineKey = new Action<string>(Translations.DefineKey));
			}
			fallback.ForEachKey(action);
		}
		string key;
		if (!Translations.TypeToFilename.TryGetValue(enumType.Name.ToLowerInvariant(), out key))
		{
			generated = new Translations.LoadedTranslation[0];
			return false;
		}
		string[] array = TranslationReader.Fallback[key];
		string[] locals;
		bool hasLocal = TranslationReader.Elements.TryGetValue(key, out locals);
		int num = array.Length;
		generated = new Translations.LoadedTranslation[num];
		for (int i = 0; i < num; i++)
		{
			generated[i] = new Translations.LoadedTranslation(array, locals, hasLocal, i);
		}
		Translations.AllTranslations[enumType] = generated;
		return true;
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x00026E0C File Offset: 0x0002500C
	public static bool TryGet(Type type, int index, out string str)
	{
		Translations.LoadedTranslation[] array;
		if (!Translations.AllTranslations.TryGetValue(type, out array) && !Translations.TryGenerate(type, out array))
		{
			str = null;
			return false;
		}
		if (index >= array.Length)
		{
			str = null;
			return false;
		}
		Translations.LoadedTranslation loadedTranslation = array[index];
		str = (loadedTranslation.HasLocal ? loadedTranslation.Localized : loadedTranslation.Fallback);
		return !string.IsNullOrWhiteSpace(str);
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x00026E68 File Offset: 0x00025068
	public static void ResetCache()
	{
		Translations.AllTranslations.Clear();
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x00026E74 File Offset: 0x00025074
	public static bool TryGet<T>(T val, out string tr) where T : Enum
	{
		Type typeFromHandle = typeof(T);
		int index = val.ToInt32(null);
		if (Translations.TryGet(typeFromHandle, index, out tr))
		{
			return true;
		}
		tr = val.ToString();
		Debug.LogWarning(string.Concat(new string[]
		{
			"Missing translation! ",
			typeFromHandle.Name,
			":",
			tr,
			"."
		}));
		return false;
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x00026EEC File Offset: 0x000250EC
	public static string Get<T>(T val) where T : Enum
	{
		string result;
		Translations.TryGet<T>(val, out result);
		return result;
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x00026F04 File Offset: 0x00025104
	public static string Get<T>(T val, string fallback) where T : Enum
	{
		string result;
		if (!Translations.TryGet<T>(val, out result))
		{
			return fallback;
		}
		return result;
	}

	// Token: 0x0400089E RID: 2206
	private static bool _pascalCaseGenerated;

	// Token: 0x0400089F RID: 2207
	private static readonly Dictionary<Type, Translations.LoadedTranslation[]> AllTranslations = new Dictionary<Type, Translations.LoadedTranslation[]>();

	// Token: 0x040008A0 RID: 2208
	private static readonly Dictionary<string, string> TypeToFilename = new Dictionary<string, string>();

	// Token: 0x040008A1 RID: 2209
	private static readonly string[] Suffixes = new string[]
	{
		"translation",
		"key"
	};

	// Token: 0x020001D3 RID: 467
	private class LoadedTranslation
	{
		// Token: 0x060009B0 RID: 2480 RVA: 0x00026F50 File Offset: 0x00025150
		public LoadedTranslation(string[] fallbacks, string[] locals, bool hasLocal, int i)
		{
			this.Fallback = fallbacks[i];
			if (hasLocal && i < locals.Length)
			{
				this.Localized = locals[i];
				this.HasLocal = !string.IsNullOrWhiteSpace(this.Localized);
				return;
			}
			this.Localized = null;
			this.HasLocal = false;
		}

		// Token: 0x040008A2 RID: 2210
		public readonly string Localized;

		// Token: 0x040008A3 RID: 2211
		public readonly string Fallback;

		// Token: 0x040008A4 RID: 2212
		public readonly bool HasLocal;
	}

	// Token: 0x020001D4 RID: 468
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x040008A5 RID: 2213
		public static Action<string> <0>__DefineKey;
	}
}
