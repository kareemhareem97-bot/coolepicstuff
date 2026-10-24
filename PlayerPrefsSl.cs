using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NorthwoodLib.Pools;

// Token: 0x0200021E RID: 542
public static class PlayerPrefsSl
{
	// Token: 0x14000028 RID: 40
	// (add) Token: 0x06000B79 RID: 2937 RVA: 0x0002D964 File Offset: 0x0002BB64
	// (remove) Token: 0x06000B7A RID: 2938 RVA: 0x0002D998 File Offset: 0x0002BB98
	public static event Action<string, string> SettingChanged;

	// Token: 0x14000029 RID: 41
	// (add) Token: 0x06000B7B RID: 2939 RVA: 0x0002D9CC File Offset: 0x0002BBCC
	// (remove) Token: 0x06000B7C RID: 2940 RVA: 0x0002DA00 File Offset: 0x0002BC00
	public static event Action<string> SettingRemoved;

	// Token: 0x1400002A RID: 42
	// (add) Token: 0x06000B7D RID: 2941 RVA: 0x0002DA34 File Offset: 0x0002BC34
	// (remove) Token: 0x06000B7E RID: 2942 RVA: 0x0002DA68 File Offset: 0x0002BC68
	public static event Action SettingsRefreshed;

	// Token: 0x06000B7F RID: 2943 RVA: 0x0002DA9B File Offset: 0x0002BC9B
	static PlayerPrefsSl()
	{
		PlayerPrefsSl._path = FileManager.GetAppFolder(true, false, "") + "registry.txt";
		PlayerPrefsSl.Refresh();
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x0002DAD4 File Offset: 0x0002BCD4
	private static string Prefix(string key, PlayerPrefsSl.DataType type)
	{
		byte b = (byte)type;
		return b.ToString("00") + key;
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x0002DAF8 File Offset: 0x0002BCF8
	public static void Refresh()
	{
		PlayerPrefsSl._registry.Clear();
		if (!File.Exists(PlayerPrefsSl._path))
		{
			File.Create(PlayerPrefsSl._path).Close();
			return;
		}
		using (StreamReader streamReader = new StreamReader(PlayerPrefsSl._path))
		{
			string text;
			while ((text = streamReader.ReadLine()) != null)
			{
				if (text.Contains("::-%(|::"))
				{
					int num = text.IndexOf("::-%(|::", StringComparison.Ordinal);
					PlayerPrefsSl._registry.Add(text.Substring(0, num), text.Substring(num + "::-%(|::".Length));
				}
			}
		}
		Action settingsRefreshed = PlayerPrefsSl.SettingsRefreshed;
		if (settingsRefreshed == null)
		{
			return;
		}
		settingsRefreshed();
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x0002DBAC File Offset: 0x0002BDAC
	private static string Serialize()
	{
		StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
		foreach (KeyValuePair<string, string> keyValuePair in PlayerPrefsSl._registry)
		{
			stringBuilder.Append(keyValuePair.Key);
			stringBuilder.Append("::-%(|::");
			stringBuilder.Append(keyValuePair.Value);
			stringBuilder.AppendLine();
		}
		string result = stringBuilder.ToString();
		StringBuilderPool.Shared.Return(stringBuilder);
		return result;
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x0002DC44 File Offset: 0x0002BE44
	private static void Save()
	{
		File.WriteAllText(PlayerPrefsSl._path, PlayerPrefsSl.Serialize(), PlayerPrefsSl.Encoding);
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x0002DC5A File Offset: 0x0002BE5A
	public static bool HasKey(string key, PlayerPrefsSl.DataType type)
	{
		return PlayerPrefsSl._registry.ContainsKey(PlayerPrefsSl.Prefix(key, type));
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x0002DC6D File Offset: 0x0002BE6D
	public static void DeleteKey(string key, PlayerPrefsSl.DataType type)
	{
		PlayerPrefsSl._registry.Remove(PlayerPrefsSl.Prefix(key, type));
		PlayerPrefsSl.Save();
		Action<string> settingRemoved = PlayerPrefsSl.SettingRemoved;
		if (settingRemoved == null)
		{
			return;
		}
		settingRemoved(key);
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x0002DC96 File Offset: 0x0002BE96
	public static bool TryGetKey(string key, PlayerPrefsSl.DataType type, out string value)
	{
		return PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, type), out value);
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x0002DCAA File Offset: 0x0002BEAA
	public static void SetKey(string key, PlayerPrefsSl.DataType type, string value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, type), value);
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x0002DCB9 File Offset: 0x0002BEB9
	public static void DeleteAll()
	{
		PlayerPrefsSl._registry.Clear();
		File.WriteAllText(PlayerPrefsSl._path, "", PlayerPrefsSl.Encoding);
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x0002DCD9 File Offset: 0x0002BED9
	private static void WriteString(string key, string value)
	{
		PlayerPrefsSl._registry[key] = value;
		PlayerPrefsSl.Save();
		Action<string, string> settingChanged = PlayerPrefsSl.SettingChanged;
		if (settingChanged == null)
		{
			return;
		}
		settingChanged(key, value);
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x0002DCFD File Offset: 0x0002BEFD
	public static void Set(string key, bool value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Bool), value ? "true" : "false");
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x0002DD1A File Offset: 0x0002BF1A
	public static void Set(string key, byte value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Byte), value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x0002DD34 File Offset: 0x0002BF34
	public static void Set(string key, sbyte value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Sbyte), value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x0002DD4E File Offset: 0x0002BF4E
	public static void Set(string key, char value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Char), value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x0002DD68 File Offset: 0x0002BF68
	public static void Set(string key, decimal value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Decimal), value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x0002DD82 File Offset: 0x0002BF82
	public static void Set(string key, double value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Double), value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000B90 RID: 2960 RVA: 0x0002DD9C File Offset: 0x0002BF9C
	public static void Set(string key, float value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Float), value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000B91 RID: 2961 RVA: 0x0002DDB6 File Offset: 0x0002BFB6
	public static void Set(string key, int value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Int), value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000B92 RID: 2962 RVA: 0x0002DDD0 File Offset: 0x0002BFD0
	public static void Set(string key, uint value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Uint), value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000B93 RID: 2963 RVA: 0x0002DDEA File Offset: 0x0002BFEA
	public static void Set(string key, long value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Long), value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000B94 RID: 2964 RVA: 0x0002DE05 File Offset: 0x0002C005
	public static void Set(string key, ulong value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Ulong), value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000B95 RID: 2965 RVA: 0x0002DE20 File Offset: 0x0002C020
	public static void Set(string key, short value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Short), value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000B96 RID: 2966 RVA: 0x0002DE3B File Offset: 0x0002C03B
	public static void Set(string key, ushort value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Ushort), value.ToString(CultureInfo.InvariantCulture));
	}

	// Token: 0x06000B97 RID: 2967 RVA: 0x0002DE56 File Offset: 0x0002C056
	public static void Set(string key, string value)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.String), value);
	}

	// Token: 0x06000B98 RID: 2968 RVA: 0x0002DE66 File Offset: 0x0002C066
	public static void Set(string key, bool[] array)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.BoolArray), string.Join(";;`'.+=;;", from n in array
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000B99 RID: 2969 RVA: 0x0002DEA4 File Offset: 0x0002C0A4
	public static void Set(string key, IEnumerable<bool> ienumerable)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.BoolArray), string.Join(";;`'.+=;;", from n in ienumerable
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x0002DEE2 File Offset: 0x0002C0E2
	public static void Set(string key, byte[] array)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.ByteArray), string.Join(";;`'.+=;;", from n in array
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x0002DF20 File Offset: 0x0002C120
	public static void Set(string key, IEnumerable<byte> ienumerable)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.ByteArray), string.Join(";;`'.+=;;", from n in ienumerable
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x0002DF5E File Offset: 0x0002C15E
	public static void Set(string key, sbyte[] array)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.SbyteArray), string.Join(";;`'.+=;;", from n in array
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000B9D RID: 2973 RVA: 0x0002DF9C File Offset: 0x0002C19C
	public static void Set(string key, IEnumerable<sbyte> ienumerable)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.SbyteArray), string.Join(";;`'.+=;;", from n in ienumerable
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000B9E RID: 2974 RVA: 0x0002DFDA File Offset: 0x0002C1DA
	public static void Set(string key, char[] array)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.CharArray), string.Join(";;`'.+=;;", from n in array
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x0002E018 File Offset: 0x0002C218
	public static void Set(string key, IEnumerable<char> ienumerable)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.CharArray), string.Join(";;`'.+=;;", from n in ienumerable
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x0002E056 File Offset: 0x0002C256
	public static void Set(string key, decimal[] array)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.DecimalArray), string.Join(";;`'.+=;;", from n in array
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x0002E094 File Offset: 0x0002C294
	public static void Set(string key, IEnumerable<decimal> ienumerable)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.DecimalArray), string.Join(";;`'.+=;;", from n in ienumerable
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x0002E0D2 File Offset: 0x0002C2D2
	public static void Set(string key, double[] array)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.DoubleArray), string.Join(";;`'.+=;;", from n in array
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x0002E110 File Offset: 0x0002C310
	public static void Set(string key, IEnumerable<double> ienumerable)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.DoubleArray), string.Join(";;`'.+=;;", from n in ienumerable
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x0002E14E File Offset: 0x0002C34E
	public static void Set(string key, float[] array)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.FloatArray), string.Join(";;`'.+=;;", from n in array
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x0002E18C File Offset: 0x0002C38C
	public static void Set(string key, IEnumerable<float> ienumerable)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.FloatArray), string.Join(";;`'.+=;;", from n in ienumerable
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x0002E1CA File Offset: 0x0002C3CA
	public static void Set(string key, int[] array)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.IntArray), string.Join(";;`'.+=;;", from n in array
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x0002E208 File Offset: 0x0002C408
	public static void Set(string key, IEnumerable<int> ienumerable)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.IntArray), string.Join(";;`'.+=;;", from n in ienumerable
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x0002E246 File Offset: 0x0002C446
	public static void Set(string key, uint[] array)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.UintArray), string.Join(";;`'.+=;;", from n in array
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x0002E284 File Offset: 0x0002C484
	public static void Set(string key, IEnumerable<uint> ienumerable)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.UintArray), string.Join(";;`'.+=;;", from n in ienumerable
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x0002E2C2 File Offset: 0x0002C4C2
	public static void Set(string key, long[] array)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.LongArray), string.Join(";;`'.+=;;", from n in array
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x0002E300 File Offset: 0x0002C500
	public static void Set(string key, IEnumerable<long> ienumerable)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.LongArray), string.Join(";;`'.+=;;", from n in ienumerable
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x0002E33E File Offset: 0x0002C53E
	public static void Set(string key, ulong[] array)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.UlongArray), string.Join(";;`'.+=;;", from n in array
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x0002E37C File Offset: 0x0002C57C
	public static void Set(string key, IEnumerable<ulong> ienumerable)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.UlongArray), string.Join(";;`'.+=;;", from n in ienumerable
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x0002E3BA File Offset: 0x0002C5BA
	public static void Set(string key, short[] array)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.ShortArray), string.Join(";;`'.+=;;", from n in array
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x0002E3F8 File Offset: 0x0002C5F8
	public static void Set(string key, IEnumerable<short> ienumerable)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.ShortArray), string.Join(";;`'.+=;;", from n in ienumerable
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x0002E436 File Offset: 0x0002C636
	public static void Set(string key, ushort[] array)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.UshortArray), string.Join(";;`'.+=;;", from n in array
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x0002E474 File Offset: 0x0002C674
	public static void Set(string key, IEnumerable<ushort> ienumerable)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.UshortArray), string.Join(";;`'.+=;;", from n in ienumerable
		select n.ToString(CultureInfo.InvariantCulture)));
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x0002E4B2 File Offset: 0x0002C6B2
	public static void Set(string key, string[] array)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.StringArray), string.Join(";;`'.+=;;", array));
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x0002E4CC File Offset: 0x0002C6CC
	public static void Set(string key, IEnumerable<string> ienumerable)
	{
		PlayerPrefsSl.WriteString(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.StringArray), string.Join(";;`'.+=;;", ienumerable));
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x0002E4E8 File Offset: 0x0002C6E8
	public static bool Get(string key, bool defaultValue)
	{
		string value;
		bool result;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Bool), out value) || !bool.TryParse(value, out result))
		{
			return defaultValue;
		}
		return result;
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x0002E518 File Offset: 0x0002C718
	public static byte Get(string key, byte defaultValue)
	{
		string s;
		byte result;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Byte), out s) || !byte.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return defaultValue;
		}
		return result;
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x0002E554 File Offset: 0x0002C754
	public static sbyte Get(string key, sbyte defaultValue)
	{
		string s;
		sbyte result;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Sbyte), out s) || !sbyte.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return defaultValue;
		}
		return result;
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x0002E590 File Offset: 0x0002C790
	public static char Get(string key, char defaultValue)
	{
		string s;
		char result;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Char), out s) || !char.TryParse(s, out result))
		{
			return defaultValue;
		}
		return result;
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x0002E5C0 File Offset: 0x0002C7C0
	public static decimal Get(string key, decimal defaultValue)
	{
		string s;
		decimal result;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Decimal), out s) || !decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return defaultValue;
		}
		return result;
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x0002E5FC File Offset: 0x0002C7FC
	public static double Get(string key, double defaultValue)
	{
		string s;
		double result;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Double), out s) || !double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return defaultValue;
		}
		return result;
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x0002E638 File Offset: 0x0002C838
	public static float Get(string key, float defaultValue)
	{
		string s;
		float result;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Float), out s) || !float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return defaultValue;
		}
		return result;
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x0002E674 File Offset: 0x0002C874
	public static int Get(string key, int defaultValue)
	{
		string s;
		int result;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Int), out s) || !int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return defaultValue;
		}
		return result;
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x0002E6B0 File Offset: 0x0002C8B0
	public static uint Get(string key, uint defaultValue)
	{
		string s;
		uint result;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Uint), out s) || !uint.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return defaultValue;
		}
		return result;
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x0002E6EC File Offset: 0x0002C8EC
	public static long Get(string key, long defaultValue)
	{
		string s;
		long result;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Long), out s) || !long.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return defaultValue;
		}
		return result;
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x0002E728 File Offset: 0x0002C928
	public static ulong Get(string key, ulong defaultValue)
	{
		string s;
		ulong result;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Ulong), out s) || !ulong.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return defaultValue;
		}
		return result;
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x0002E764 File Offset: 0x0002C964
	public static short Get(string key, short defaultValue)
	{
		string s;
		short result;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Short), out s) || !short.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return defaultValue;
		}
		return result;
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x0002E7A0 File Offset: 0x0002C9A0
	public static ushort Get(string key, ushort defaultValue)
	{
		string s;
		ushort result;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.Int), out s) || !ushort.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return defaultValue;
		}
		return result;
	}

	// Token: 0x06000BC1 RID: 3009 RVA: 0x0002E7DC File Offset: 0x0002C9DC
	public static string Get(string key, string defaultValue)
	{
		string result;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.String), out result))
		{
			return defaultValue;
		}
		return result;
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x0002E804 File Offset: 0x0002CA04
	public static bool[] Get(string key, bool[] defaultValue)
	{
		string text;
		if (PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.BoolArray), out text))
		{
			string[] array = text.Split(new string[]
			{
				";;`'.+=;;"
			}, StringSplitOptions.None);
			bool[] array2 = new bool[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!bool.TryParse(array[i], out array2[i]))
				{
					return defaultValue;
				}
			}
			return array2;
		}
		return defaultValue;
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x0002E86C File Offset: 0x0002CA6C
	public static byte[] Get(string key, byte[] defaultValue)
	{
		string text;
		if (PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.ByteArray), out text))
		{
			string[] array = text.Split(new string[]
			{
				";;`'.+=;;"
			}, StringSplitOptions.None);
			byte[] array2 = new byte[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!byte.TryParse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture, out array2[i]))
				{
					return defaultValue;
				}
			}
			return array2;
		}
		return defaultValue;
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x0002E8DC File Offset: 0x0002CADC
	public static sbyte[] Get(string key, sbyte[] defaultValue)
	{
		string text;
		if (PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.SbyteArray), out text))
		{
			string[] array = text.Split(new string[]
			{
				";;`'.+=;;"
			}, StringSplitOptions.None);
			sbyte[] array2 = new sbyte[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!sbyte.TryParse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture, out array2[i]))
				{
					return defaultValue;
				}
			}
			return array2;
		}
		return defaultValue;
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x0002E94C File Offset: 0x0002CB4C
	public static char[] Get(string key, char[] defaultValue)
	{
		string text;
		if (PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.CharArray), out text))
		{
			string[] array = text.Split(new string[]
			{
				";;`'.+=;;"
			}, StringSplitOptions.None);
			char[] array2 = new char[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!char.TryParse(array[i], out array2[i]))
				{
					return defaultValue;
				}
			}
			return array2;
		}
		return defaultValue;
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x0002E9B4 File Offset: 0x0002CBB4
	public static decimal[] Get(string key, decimal[] defaultValue)
	{
		string text;
		if (PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.DecimalArray), out text))
		{
			string[] array = text.Split(new string[]
			{
				";;`'.+=;;"
			}, StringSplitOptions.None);
			decimal[] array2 = new decimal[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!decimal.TryParse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture, out array2[i]))
				{
					return defaultValue;
				}
			}
			return array2;
		}
		return defaultValue;
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x0002EA24 File Offset: 0x0002CC24
	public static double[] Get(string key, double[] defaultValue)
	{
		string text;
		if (PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.DoubleArray), out text))
		{
			string[] array = text.Split(new string[]
			{
				";;`'.+=;;"
			}, StringSplitOptions.None);
			double[] array2 = new double[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!double.TryParse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture, out array2[i]))
				{
					return defaultValue;
				}
			}
			return array2;
		}
		return defaultValue;
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x0002EA94 File Offset: 0x0002CC94
	public static float[] Get(string key, float[] defaultValue)
	{
		string text;
		if (PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.FloatArray), out text))
		{
			string[] array = text.Split(new string[]
			{
				";;`'.+=;;"
			}, StringSplitOptions.None);
			float[] array2 = new float[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!float.TryParse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture, out array2[i]))
				{
					return defaultValue;
				}
			}
			return array2;
		}
		return defaultValue;
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x0002EB04 File Offset: 0x0002CD04
	public static int[] Get(string key, int[] defaultValue)
	{
		string text;
		if (PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.IntArray), out text))
		{
			string[] array = text.Split(new string[]
			{
				";;`'.+=;;"
			}, StringSplitOptions.None);
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!int.TryParse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture, out array2[i]))
				{
					return defaultValue;
				}
			}
			return array2;
		}
		return defaultValue;
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x0002EB74 File Offset: 0x0002CD74
	public static uint[] Get(string key, uint[] defaultValue)
	{
		string text;
		if (PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.UintArray), out text))
		{
			string[] array = text.Split(new string[]
			{
				";;`'.+=;;"
			}, StringSplitOptions.None);
			uint[] array2 = new uint[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!uint.TryParse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture, out array2[i]))
				{
					return defaultValue;
				}
			}
			return array2;
		}
		return defaultValue;
	}

	// Token: 0x06000BCB RID: 3019 RVA: 0x0002EBE4 File Offset: 0x0002CDE4
	public static long[] Get(string key, long[] defaultValue)
	{
		string text;
		if (PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.LongArray), out text))
		{
			string[] array = text.Split(new string[]
			{
				";;`'.+=;;"
			}, StringSplitOptions.None);
			long[] array2 = new long[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!long.TryParse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture, out array2[i]))
				{
					return defaultValue;
				}
			}
			return array2;
		}
		return defaultValue;
	}

	// Token: 0x06000BCC RID: 3020 RVA: 0x0002EC54 File Offset: 0x0002CE54
	public static ulong[] Get(string key, ulong[] defaultValue)
	{
		string text;
		if (PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.UlongArray), out text))
		{
			string[] array = text.Split(new string[]
			{
				";;`'.+=;;"
			}, StringSplitOptions.None);
			ulong[] array2 = new ulong[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!ulong.TryParse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture, out array2[i]))
				{
					return defaultValue;
				}
			}
			return array2;
		}
		return defaultValue;
	}

	// Token: 0x06000BCD RID: 3021 RVA: 0x0002ECC4 File Offset: 0x0002CEC4
	public static short[] Get(string key, short[] defaultValue)
	{
		string text;
		if (PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.ShortArray), out text))
		{
			string[] array = text.Split(new string[]
			{
				";;`'.+=;;"
			}, StringSplitOptions.None);
			short[] array2 = new short[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!short.TryParse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture, out array2[i]))
				{
					return defaultValue;
				}
			}
			return array2;
		}
		return defaultValue;
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x0002ED34 File Offset: 0x0002CF34
	public static ushort[] Get(string key, ushort[] defaultValue)
	{
		string text;
		if (PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.UshortArray), out text))
		{
			string[] array = text.Split(new string[]
			{
				";;`'.+=;;"
			}, StringSplitOptions.None);
			ushort[] array2 = new ushort[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!ushort.TryParse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture, out array2[i]))
				{
					return defaultValue;
				}
			}
			return array2;
		}
		return defaultValue;
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x0002EDA4 File Offset: 0x0002CFA4
	public static string[] Get(string key, string[] defaultValue)
	{
		string text;
		if (!PlayerPrefsSl._registry.TryGetValue(PlayerPrefsSl.Prefix(key, PlayerPrefsSl.DataType.StringArray), out text))
		{
			return defaultValue;
		}
		return text.Split(new string[]
		{
			";;`'.+=;;"
		}, StringSplitOptions.None);
	}

	// Token: 0x040009CF RID: 2511
	private const string ArraySeparator = ";;`'.+=;;";

	// Token: 0x040009D0 RID: 2512
	private const string KeySeparator = "::-%(|::";

	// Token: 0x040009D1 RID: 2513
	private static readonly Dictionary<string, string> _registry = new Dictionary<string, string>();

	// Token: 0x040009D2 RID: 2514
	private static readonly string _path;

	// Token: 0x040009D3 RID: 2515
	private static readonly UTF8Encoding Encoding = new UTF8Encoding(false);

	// Token: 0x0200021F RID: 543
	public enum DataType : byte
	{
		// Token: 0x040009D8 RID: 2520
		Bool,
		// Token: 0x040009D9 RID: 2521
		Byte,
		// Token: 0x040009DA RID: 2522
		Sbyte,
		// Token: 0x040009DB RID: 2523
		Char,
		// Token: 0x040009DC RID: 2524
		Decimal,
		// Token: 0x040009DD RID: 2525
		Double,
		// Token: 0x040009DE RID: 2526
		Float,
		// Token: 0x040009DF RID: 2527
		Int,
		// Token: 0x040009E0 RID: 2528
		Uint,
		// Token: 0x040009E1 RID: 2529
		Long,
		// Token: 0x040009E2 RID: 2530
		Ulong,
		// Token: 0x040009E3 RID: 2531
		Short,
		// Token: 0x040009E4 RID: 2532
		Ushort,
		// Token: 0x040009E5 RID: 2533
		String,
		// Token: 0x040009E6 RID: 2534
		BoolArray,
		// Token: 0x040009E7 RID: 2535
		ByteArray,
		// Token: 0x040009E8 RID: 2536
		SbyteArray,
		// Token: 0x040009E9 RID: 2537
		CharArray,
		// Token: 0x040009EA RID: 2538
		DecimalArray,
		// Token: 0x040009EB RID: 2539
		DoubleArray,
		// Token: 0x040009EC RID: 2540
		FloatArray,
		// Token: 0x040009ED RID: 2541
		IntArray,
		// Token: 0x040009EE RID: 2542
		UintArray,
		// Token: 0x040009EF RID: 2543
		LongArray,
		// Token: 0x040009F0 RID: 2544
		UlongArray,
		// Token: 0x040009F1 RID: 2545
		ShortArray,
		// Token: 0x040009F2 RID: 2546
		UshortArray,
		// Token: 0x040009F3 RID: 2547
		StringArray
	}
}
