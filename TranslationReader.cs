using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GameCore;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020001D1 RID: 465
public static class TranslationReader
{
	// Token: 0x17000168 RID: 360
	// (get) Token: 0x0600098A RID: 2442 RVA: 0x000265F9 File Offset: 0x000247F9
	// (set) Token: 0x0600098B RID: 2443 RVA: 0x00026600 File Offset: 0x00024800
	public static string TranslationPath
	{
		get
		{
			return TranslationReader._translationPath;
		}
		private set
		{
			TranslationReader._translationPath = Path.GetFullPath(value);
			TranslationReader.TranslationDirectoryName = Path.GetFileName(TranslationReader._translationPath);
		}
	}

	// Token: 0x14000025 RID: 37
	// (add) Token: 0x0600098C RID: 2444 RVA: 0x0002661C File Offset: 0x0002481C
	// (remove) Token: 0x0600098D RID: 2445 RVA: 0x00026650 File Offset: 0x00024850
	public static event Action OnTranslationsRefreshed;

	// Token: 0x17000169 RID: 361
	// (get) Token: 0x0600098E RID: 2446 RVA: 0x00026683 File Offset: 0x00024883
	// (set) Token: 0x0600098F RID: 2447 RVA: 0x0002668A File Offset: 0x0002488A
	public static bool EverLoaded { get; private set; }

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x06000990 RID: 2448 RVA: 0x00026692 File Offset: 0x00024892
	// (set) Token: 0x06000991 RID: 2449 RVA: 0x00026699 File Offset: 0x00024899
	public static string TranslationDirectoryName { get; private set; }

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x06000992 RID: 2450 RVA: 0x000266A1 File Offset: 0x000248A1
	// (set) Token: 0x06000993 RID: 2451 RVA: 0x000266A8 File Offset: 0x000248A8
	public static TranslationManifest TranslationManifest { get; private set; }

	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06000994 RID: 2452 RVA: 0x000266B0 File Offset: 0x000248B0
	// (set) Token: 0x06000995 RID: 2453 RVA: 0x000266B7 File Offset: 0x000248B7
	public static CultureInfo TranslationCulture { get; private set; }

	// Token: 0x06000996 RID: 2454 RVA: 0x000266C0 File Offset: 0x000248C0
	static TranslationReader()
	{
		TranslationReader.LoadPositions();
		SceneManager.sceneLoaded += TranslationReader.OnSceneWasLoaded;
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x00026710 File Offset: 0x00024910
	private static void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
	{
		PlayerPrefsSl.Refresh();
		TranslationReader.Refresh();
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x0002671C File Offset: 0x0002491C
	public static void Refresh()
	{
		TranslationReader.TranslationPath = TranslationReader.GetTranslationPath();
		TranslationReader.TranslationManifest = TranslationReader.LoadTranslation(TranslationReader.TranslationPath, TranslationReader.Elements);
		TranslationReader.LoadTranslation(TranslationReader.CheckPath(Array.Empty<string>()), TranslationReader.Fallback);
		TranslationReader.TranslationCulture = null;
		foreach (string name in TranslationReader.TranslationManifest.InterfaceLocales ?? Array.Empty<string>())
		{
			try
			{
				TranslationReader.TranslationCulture = CultureInfo.GetCultureInfo(name);
				break;
			}
			catch
			{
				TranslationReader.TranslationCulture = null;
			}
		}
		if (TranslationReader.TranslationCulture == null)
		{
			TranslationReader.TranslationCulture = CultureInfo.CurrentCulture;
		}
		CultureInfo.CurrentCulture = TranslationReader.TranslationCulture;
		CultureInfo.CurrentUICulture = TranslationReader.TranslationCulture;
		TranslationReader.EverLoaded = true;
		Translations.ResetCache();
		Action onTranslationsRefreshed = TranslationReader.OnTranslationsRefreshed;
		if (onTranslationsRefreshed == null)
		{
			return;
		}
		onTranslationsRefreshed();
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x000267F0 File Offset: 0x000249F0
	private static string GetTranslationPath()
	{
		string text = TranslationReader.CheckPath(Array.Empty<string>());
		if (text == null)
		{
			throw new DirectoryNotFoundException();
		}
		return text;
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x00026808 File Offset: 0x00024A08
	private static void LoadPositions()
	{
		foreach (string path in Directory.GetFiles(TranslationReader.CheckPath(Array.Empty<string>())))
		{
			string[] array = File.ReadAllLines(path);
			Dictionary<int, Dictionary<string, int>> dictionary = new Dictionary<int, Dictionary<string, int>>();
			for (int j = 0; j < array.Length; j++)
			{
				Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
				MatchCollection matchCollection = TranslationReader._matchFormat.Matches(array[j]);
				for (int k = 0; k < matchCollection.Count; k++)
				{
					dictionary2.TryAdd(matchCollection[k].Value, k);
				}
				dictionary.Add(j, dictionary2);
			}
			TranslationReader._positions.Add(Path.GetFileNameWithoutExtension(path), dictionary);
		}
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x000268C0 File Offset: 0x00024AC0
	private static TranslationManifest LoadTranslation(string translationPath, Dictionary<string, string[]> dictionary)
	{
		dictionary.Clear();
		if (File.Exists(translationPath + "Legacy_Interfaces.txt") && File.Exists(translationPath + "Legancy_Interfaces.txt"))
		{
			File.Delete(translationPath + "Legancy_Interfaces.txt");
		}
		foreach (string path in Directory.EnumerateFiles(translationPath, "*.txt"))
		{
			string[] array = FileManager.ReadAllLines(path);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Replace("\\n", Environment.NewLine);
				foreach (object obj in TranslationReader._matchFormat.Matches(array[i]))
				{
					Match match = (Match)obj;
					Dictionary<int, Dictionary<string, int>> dictionary2;
					Dictionary<string, int> dictionary3;
					int num;
					if (TranslationReader._positions.TryGetValue(Path.GetFileNameWithoutExtension(path), out dictionary2) && dictionary2.TryGetValue(i, out dictionary3) && dictionary3.TryGetValue(match.Value, out num))
					{
						array[i] = array[i].Replace(match.Value, "{" + num.ToString() + "}");
					}
				}
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			dictionary[(fileNameWithoutExtension == "Legancy_Interfaces") ? "Legacy_Interfaces" : fileNameWithoutExtension] = array;
		}
		TranslationManifest result;
		try
		{
			result = JsonSerialize.FromFile<TranslationManifest>(Path.Combine(translationPath, "manifest.json"));
		}
		catch (FileNotFoundException)
		{
			result = new TranslationManifest(Path.GetFileName(translationPath), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());
		}
		return result;
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x00026AC0 File Offset: 0x00024CC0
	private static string CheckPath(params string[] suffixes)
	{
		if (suffixes == null || suffixes.Length == 0)
		{
			return TranslationReader.CheckDefaultLangPath();
		}
		foreach (string text in suffixes)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				string text2 = Path.Combine("Translations/", text);
				if (TranslationReader.CheckPathInternal(text2))
				{
					return text2;
				}
			}
		}
		return TranslationReader.CheckDefaultLangPath();
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x00026B14 File Offset: 0x00024D14
	private static string CheckDefaultLangPath()
	{
		string text = Path.Combine("Translations/", "en");
		if (!TranslationReader.CheckPathInternal(text))
		{
			return null;
		}
		return text;
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x00026B3C File Offset: 0x00024D3C
	private static bool CheckPathInternal(string path)
	{
		return Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any<string>();
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x00026B54 File Offset: 0x00024D54
	public static string[] GetKeys(string keyName)
	{
		if (keyName == "Legancy_Interfaces")
		{
			keyName = "Legacy_Interfaces";
		}
		string[] result;
		if (TranslationReader.Fallback.TryGetValue(keyName, out result))
		{
			return result;
		}
		Debug.LogWarning("Tried to get **FALLBACK** translation from nonexistent file " + keyName);
		return null;
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x00026B98 File Offset: 0x00024D98
	public static string[] GetFallbackKeys(string keyName)
	{
		if (keyName == "Legancy_Interfaces")
		{
			keyName = "Legacy_Interfaces";
		}
		string[] result;
		if (!TranslationReader.Fallback.TryGetValue(keyName, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x00026BCB File Offset: 0x00024DCB
	public static string Get(string keyName, int index, string defaultValue = "NO_TRANSLATION")
	{
		if (GameCore.Console.TranslationDebugMode)
		{
			return string.Format("{0}:{1}", keyName, index);
		}
		if (keyName == "Legancy_Interfaces")
		{
			keyName = "Legacy_Interfaces";
		}
		return TranslationReader.GetFallback(keyName, index, defaultValue) ?? defaultValue;
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x00026C08 File Offset: 0x00024E08
	public static bool TryGet(string keyName, int index, out string val)
	{
		string[] array;
		string text;
		if (TranslationReader.Fallback.TryGetValue(keyName, out array) && array.TryGet(index, out text) && !string.IsNullOrWhiteSpace(text))
		{
			val = text;
			return true;
		}
		val = TranslationReader.GetFallback(keyName, index, "NO_TRANSLATION");
		return false;
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x00026C4C File Offset: 0x00024E4C
	private static string GetFallback(string keyName, int index, string defaultvalue)
	{
		string[] array;
		string text;
		if (TranslationReader.Fallback.TryGetValue(keyName, out array) && array.TryGet(index, out text) && !string.IsNullOrWhiteSpace(text))
		{
			return text;
		}
		Debug.LogWarning(string.Format("Missing **FALLBACK** translation! {0}:{1}. Default value: {2}", keyName, index, defaultvalue.Replace("<", "(<)")));
		return null;
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x00026CA4 File Offset: 0x00024EA4
	public static string GetFormatted(string keyName, int index, string defaultvalue, object obj1)
	{
		return string.Format(TranslationReader.Get(keyName, index, defaultvalue), obj1);
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x00026CB4 File Offset: 0x00024EB4
	public static string GetFormatted(string keyName, int index, string defaultvalue, object obj1, object obj2)
	{
		return string.Format(TranslationReader.Get(keyName, index, defaultvalue), obj1, obj2);
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x00026CC6 File Offset: 0x00024EC6
	public static string GetFormatted(string keyName, int index, string defaultvalue, object obj1, object obj2, object obj3)
	{
		return string.Format(TranslationReader.Get(keyName, index, defaultvalue), obj1, obj2, obj3);
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x00026CDA File Offset: 0x00024EDA
	public static string GetFormatted(string keyName, int index, string defaultvalue, params object[] format)
	{
		return string.Format(TranslationReader.Get(keyName, index, defaultvalue), format);
	}

	// Token: 0x04000890 RID: 2192
	private static string _translationPath;

	// Token: 0x04000891 RID: 2193
	public static readonly Dictionary<string, string[]> Elements = new Dictionary<string, string[]>();

	// Token: 0x04000892 RID: 2194
	public static readonly Dictionary<string, string[]> Fallback = new Dictionary<string, string[]>();

	// Token: 0x04000893 RID: 2195
	private static readonly Dictionary<string, Dictionary<int, Dictionary<string, int>>> _positions = new Dictionary<string, Dictionary<int, Dictionary<string, int>>>();

	// Token: 0x04000894 RID: 2196
	private static readonly Regex _matchFormat = new Regex("\\{.*?\\}|\\[.*?\\]");

	// Token: 0x04000895 RID: 2197
	private static TMP_FontAsset[] defaultFallbacks;

	// Token: 0x04000896 RID: 2198
	public const string DefaultLanguage = "en";

	// Token: 0x04000897 RID: 2199
	public const string NoTranslation = "NO_TRANSLATION";

	// Token: 0x04000898 RID: 2200
	public const string TranslationDirectory = "Translations/";
}
