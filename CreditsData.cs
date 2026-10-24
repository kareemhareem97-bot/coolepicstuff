using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// Token: 0x020000B0 RID: 176
public static class CreditsData
{
	// Token: 0x06000392 RID: 914 RVA: 0x0000E478 File Offset: 0x0000C678
	static CreditsData()
	{
		try
		{
			CreditsData.SetCredits(File.ReadAllText(CreditsData.CachePath));
		}
		catch
		{
			CreditsData.LoadData(File.ReadAllText("CreditsCache.json"));
		}
	}

	// Token: 0x06000393 RID: 915 RVA: 0x0000E4D4 File Offset: 0x0000C6D4
	internal static void LoadData(string text)
	{
		try
		{
			CreditsData.SetCredits(text);
			File.WriteAllText(CreditsData.CachePath, text);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	// Token: 0x06000394 RID: 916 RVA: 0x0000E50C File Offset: 0x0000C70C
	private static void SetCredits(string text)
	{
		if (!text.StartsWith('{'))
		{
			throw new ArgumentException("Invalid credits data", "text");
		}
		CreditsList creditsList = JsonSerialize.FromJson<CreditsList>(text);
		List<CreditsCategory> list = new List<CreditsCategory>();
		TranslationManifest[] array = (from s in Directory.EnumerateFiles("Translations/", "manifest.json", SearchOption.AllDirectories)
		select JsonSerialize.FromFile<TranslationManifest>(s)).ToArray<TranslationManifest>();
		for (int i = 0; i < creditsList.credits.Length; i++)
		{
			CreditsListCategory creditsListCategory = creditsList.credits[i];
			List<CreditsEntry> list2 = new List<CreditsEntry>();
			for (int j = 0; j < creditsListCategory.members.Length; j++)
			{
				list2.Add(CreditsData.ProcessEntry(creditsListCategory.members[j]));
			}
			if (creditsListCategory.category.Equals("SPECIAL THANKS", StringComparison.OrdinalIgnoreCase))
			{
				list2.Add(new CreditsEntry(TranslationReader.Get("NewMainMenu", 87, "For playing our game!"), "<current nickname>", new Color32(byte.MaxValue, 215, 0, byte.MaxValue)));
				list.Add(new CreditsCategory
				{
					Header = creditsListCategory.category,
					Records = list2.ToArray()
				});
				list2.Clear();
				foreach (TranslationManifest translationManifest in array)
				{
					string text2 = translationManifest.Name;
					if (!text2.Equals("en", StringComparison.OrdinalIgnoreCase))
					{
						int num = translationManifest.Name.IndexOf('-');
						if (num > -1)
						{
							text2 = translationManifest.Name.Substring(num + 2);
						}
						foreach (string name in translationManifest.Authors)
						{
							list2.Add(new CreditsEntry(text2, name));
						}
						list2.Add(new CreditsEntry("", ""));
					}
				}
				list.Add(new CreditsCategory
				{
					Header = "COMMUNITY TRANSLATORS",
					Records = list2.ToArray()
				});
			}
			else
			{
				list.Add(new CreditsCategory
				{
					Header = creditsListCategory.category,
					Records = list2.ToArray()
				});
			}
		}
		CreditsData.Data = list.ToArray();
		CreditsData.RemoteLoaded = true;
	}

	// Token: 0x06000395 RID: 917 RVA: 0x0000E764 File Offset: 0x0000C964
	private static CreditsEntry ProcessEntry(CreditsListMember member)
	{
		if (string.IsNullOrEmpty(member.name))
		{
			return new CreditsEntry();
		}
		if (!string.IsNullOrEmpty(member.title))
		{
			return new CreditsEntry(member.title, member.name, CreditsData.HexToColor(member));
		}
		return new CreditsEntry(member.name);
	}

	// Token: 0x06000396 RID: 918 RVA: 0x0000E7BC File Offset: 0x0000C9BC
	private static Color32 HexToColor(CreditsListMember member)
	{
		if (string.IsNullOrEmpty(member.color))
		{
			return Color.white;
		}
		Color32 result;
		if (Misc.TryParseColor(member.color, out result))
		{
			return result;
		}
		Debug.LogError(string.Concat(new string[]
		{
			"Error during processing credits color (",
			member.name,
			" - ",
			member.title,
			" - ",
			member.color,
			")."
		}));
		return Color.white;
	}

	// Token: 0x04000338 RID: 824
	public static bool RemoteLoaded;

	// Token: 0x04000339 RID: 825
	internal const string CurrentNicknamePlaceholder = "<current nickname>";

	// Token: 0x0400033A RID: 826
	private static readonly string CachePath = FileManager.GetAppFolder(true, false, "") + "CreditsCache.json";

	// Token: 0x0400033B RID: 827
	internal static CreditsCategory[] Data;
}
