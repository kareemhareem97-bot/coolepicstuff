using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CommandSystem;
using Footprinting;
using Mirror;
using NorthwoodLib;
using NorthwoodLib.Pools;
using UnityEngine;

// Token: 0x02000217 RID: 535
public static class Misc
{
	// Token: 0x06000B2F RID: 2863 RVA: 0x0002B71C File Offset: 0x0002991C
	static Misc()
	{
		foreach (ConsoleColor consoleColor in EnumUtils<ConsoleColor>.Values)
		{
			Misc.ConsoleColors.Add(consoleColor, ServerConsole.ConsoleColorToColor(consoleColor));
		}
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x0002BA7C File Offset: 0x00029C7C
	public static bool CheckPermission(this ICommandSender sender, PlayerPermissions[] perms)
	{
		CommandSender commandSender = sender as CommandSender;
		return commandSender != null && PermissionsHandler.IsPermitted(commandSender.Permissions, perms);
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x0002BAA4 File Offset: 0x00029CA4
	public static bool CheckPermission(this ICommandSender sender, PlayerPermissions perm)
	{
		CommandSender commandSender = sender as CommandSender;
		return commandSender != null && (commandSender.FullPermissions || PermissionsHandler.IsPermitted(commandSender.Permissions, perm));
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x0002BAD3 File Offset: 0x00029CD3
	public static bool CheckPermission(this ICommandSender sender, PlayerPermissions perm, out string response)
	{
		if (sender.CheckPermission(perm))
		{
			response = null;
			return true;
		}
		response = "You don't have permissions to execute this command.\nRequired permission: " + perm.ToString();
		return false;
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x0002BAFD File Offset: 0x00029CFD
	public static bool CheckPermission(this ICommandSender sender, PlayerPermissions[] perms, out string response)
	{
		if (sender.CheckPermission(perms))
		{
			response = null;
			return true;
		}
		response = "You don't have permissions to execute this command.\nYou need at least one of following permissions: " + string.Join<PlayerPermissions>(", ", perms);
		return false;
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x0002BB28 File Offset: 0x00029D28
	public static string LeadingZeroes(int integer, uint len, bool plusSign = false)
	{
		bool flag = integer < 0;
		if (flag)
		{
			integer *= -1;
		}
		string text = integer.ToString();
		while ((long)text.Length < (long)((ulong)len))
		{
			text = "0" + text;
		}
		return (flag ? "-" : (plusSign ? "+" : "")) + text;
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x0002BB82 File Offset: 0x00029D82
	public static string LoggedNameFromRefHub(this ReferenceHub me)
	{
		return me.nicknameSync.CombinedName + " (" + (me.authManager.UserId ?? "null") + ")";
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x0002BBB2 File Offset: 0x00029DB2
	public static string LoggedNameFromFootprint(this Footprint me)
	{
		if (me.Hub != null)
		{
			return me.Hub.LoggedNameFromRefHub();
		}
		return me.Nickname + " (" + me.LogUserID + ")";
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x0002BBEC File Offset: 0x00029DEC
	public static string SanitizeRichText(string content, string replaceOpenChar = "", string replaceCloseChar = "")
	{
		StringBuilder stringBuilder = StringBuilderPool.Shared.Rent(content);
		stringBuilder.Replace("<", replaceOpenChar);
		stringBuilder.Replace("\\u003c", replaceOpenChar);
		stringBuilder.Replace(">", replaceCloseChar);
		stringBuilder.Replace("\\u003e", replaceCloseChar);
		return StringBuilderPool.Shared.ToStringReturn(stringBuilder);
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x0002BC44 File Offset: 0x00029E44
	public static string CloseAllRichTextTags(string content)
	{
		if (string.IsNullOrEmpty(content))
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = StringBuilderPool.Shared.Rent(content);
		MatchCollection matchCollection = Misc.TagRegex.Matches(content);
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (object obj in matchCollection)
		{
			string text = ((Match)obj).Groups[1].Value.ToLowerInvariant();
			if (!dictionary.TryAdd(text, 1))
			{
				Dictionary<string, int> dictionary2 = dictionary;
				string key = text;
				int num = dictionary2[key];
				dictionary2[key] = num + 1;
			}
		}
		foreach (KeyValuePair<string, int> keyValuePair in from pair in dictionary
		where !pair.Key.StartsWith('/')
		select pair)
		{
			int num2;
			if (!dictionary.TryGetValue("/" + keyValuePair.Key, out num2))
			{
				num2 = keyValuePair.Value;
			}
			else
			{
				num2 = keyValuePair.Value - num2;
			}
			for (int i = 0; i < num2; i++)
			{
				stringBuilder.Append("</").Append(keyValuePair.Key).Append('>');
			}
		}
		return StringBuilderPool.Shared.ToStringReturn(stringBuilder);
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x0002BDC0 File Offset: 0x00029FC0
	public static int LevenshteinDistance(string s, string t)
	{
		int length = s.Length;
		int length2 = t.Length;
		int[,] array = new int[length + 1, length2 + 1];
		if (length == 0)
		{
			return length2;
		}
		if (length2 == 0)
		{
			return length;
		}
		int i = 0;
		while (i <= length)
		{
			array[i, 0] = i++;
		}
		int j = 0;
		while (j <= length2)
		{
			array[0, j] = j++;
		}
		for (int k = 1; k <= length; k++)
		{
			for (int l = 1; l <= length2; l++)
			{
				int num = (t[l - 1] == s[k - 1]) ? 0 : 1;
				array[k, l] = Math.Min(Math.Min(array[k - 1, l] + 1, array[k, l - 1] + 1), array[k - 1, l - 1] + num);
			}
		}
		return array[length, length2];
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x0002BEA8 File Offset: 0x0002A0A8
	public static string LongestCommonSubstring(string a, string b)
	{
		if (a == null || b == null)
		{
			return string.Empty;
		}
		int[,] array = new int[a.Length, b.Length];
		int num = 0;
		string result = "";
		for (int i = 0; i < a.Length; i++)
		{
			for (int j = 0; j < b.Length; j++)
			{
				if (a[i] == b[j])
				{
					array[i, j] = ((i == 0 || j == 0) ? 1 : (array[i - 1, j - 1] + 1));
					if (array[i, j] > num)
					{
						num = array[i, j];
						result = a.Substring(i - num + 1, num);
					}
				}
				else
				{
					array[i, j] = 0;
				}
			}
		}
		return result;
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x0002BF68 File Offset: 0x0002A168
	private static string LongestCommonSubstringOfAInB(string a, string b)
	{
		if (b.Length < a.Length)
		{
			string text = b;
			string text2 = a;
			a = text;
			b = text2;
		}
		for (int i = a.Length; i > 0; i--)
		{
			for (int j = a.Length - i; j <= a.Length - i; j++)
			{
				string text3 = a.Substring(j, i);
				if (b.Contains(text3))
				{
					return text3;
				}
			}
		}
		return string.Empty;
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x0002BFCF File Offset: 0x0002A1CF
	public static bool ValidatePastebin(string text)
	{
		return Misc._pbRgx.IsMatch(text);
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x0002BFDC File Offset: 0x0002A1DC
	public static bool ValidateIpOrHostname(string ipOrHost, out Misc.IPAddressType type, bool allowHostname = true, bool allowLocalhost = true)
	{
		if (ipOrHost == "localhost")
		{
			type = Misc.IPAddressType.Localhost;
			return allowLocalhost;
		}
		if (Misc._ipV4Rgx.IsMatch(ipOrHost))
		{
			type = Misc.IPAddressType.IPV4;
			return true;
		}
		if (Misc._ipV6Rgx.IsMatch(ipOrHost))
		{
			type = Misc.IPAddressType.IPV6;
			return true;
		}
		if (Misc._hostNameRgx.IsMatch(ipOrHost))
		{
			type = Misc.IPAddressType.Hostname;
			return allowHostname;
		}
		type = Misc.IPAddressType.Unknown;
		return false;
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x0002C038 File Offset: 0x0002A238
	public static long RelativeTimeToSeconds(string time, int defaultFactor = 1)
	{
		long num;
		if (long.TryParse(time, out num))
		{
			return num * (long)defaultFactor;
		}
		if (time.Length < 2)
		{
			throw new Exception(string.Format("{0} is not a valid time.", num));
		}
		if (!long.TryParse(time.Substring(0, time.Length - 1), out num))
		{
			throw new Exception(string.Format("{0} is not a valid time.", num));
		}
		char c = time[time.Length - 1];
		if (c <= 'Y')
		{
			if (c <= 'H')
			{
				if (c == 'D')
				{
					goto IL_C5;
				}
				if (c != 'H')
				{
					goto IL_E0;
				}
				goto IL_BC;
			}
			else
			{
				if (c == 'M')
				{
					return num * 2592000L;
				}
				if (c != 'S')
				{
					if (c != 'Y')
					{
						goto IL_E0;
					}
					goto IL_D7;
				}
			}
		}
		else if (c <= 'h')
		{
			if (c == 'd')
			{
				goto IL_C5;
			}
			if (c != 'h')
			{
				goto IL_E0;
			}
			goto IL_BC;
		}
		else
		{
			if (c == 'm')
			{
				return num * 60L;
			}
			if (c != 's')
			{
				if (c != 'y')
				{
					goto IL_E0;
				}
				goto IL_D7;
			}
		}
		return num;
		IL_BC:
		return num * 3600L;
		IL_C5:
		return num * 86400L;
		IL_D7:
		return num * 31536000L;
		IL_E0:
		throw new Exception(string.Format("{0} is not a valid time.", num));
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x0002C13C File Offset: 0x0002A33C
	public static List<int> ProcessRaPlayersList(string playerIds)
	{
		List<int> result;
		try
		{
			List<int> list = new List<int>();
			string[] source = playerIds.Split('.', StringSplitOptions.None);
			IEnumerable<string> source2 = from item in source
			where !string.IsNullOrEmpty(item)
			select item;
			Func<string, int> selector;
			if ((selector = Misc.<>O.<0>__Parse) == null)
			{
				selector = (Misc.<>O.<0>__Parse = new Func<string, int>(int.Parse));
			}
			list.AddRange(source2.Select(selector));
			result = list;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			result = null;
		}
		return result;
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x0002C1C0 File Offset: 0x0002A3C0
	public static string GetRuntimeVersion()
	{
		string result;
		try
		{
			result = RuntimeInformation.FrameworkDescription;
		}
		catch
		{
			result = "Not supported!";
		}
		return result;
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x0002C1F0 File Offset: 0x0002A3F0
	public static AudioType GetAudioType(string path)
	{
		string extension = Path.GetExtension(path);
		uint num = <PrivateImplementationDetails>.ComputeStringHash(extension);
		if (num <= 2194571213U)
		{
			if (num <= 575927418U)
			{
				if (num != 106392356U)
				{
					if (num != 575927418U)
					{
						return AudioType.UNKNOWN;
					}
					if (!(extension == ".mp2"))
					{
						return AudioType.UNKNOWN;
					}
				}
				else if (!(extension == ".mpeg"))
				{
					return AudioType.UNKNOWN;
				}
			}
			else if (num != 592705037U)
			{
				if (num != 2194571213U)
				{
					return AudioType.UNKNOWN;
				}
				if (!(extension == ".wav"))
				{
					return AudioType.UNKNOWN;
				}
				return AudioType.WAV;
			}
			else if (!(extension == ".mp3"))
			{
				return AudioType.UNKNOWN;
			}
			return AudioType.MPEG;
		}
		if (num <= 2685385760U)
		{
			if (num != 2561755776U)
			{
				if (num == 2685385760U)
				{
					if (extension == ".aac")
					{
						return AudioType.ACC;
					}
				}
			}
			else if (extension == ".ogg")
			{
				return AudioType.OGGVORBIS;
			}
		}
		else if (num != 2819025139U)
		{
			if (num == 3196928671U)
			{
				if (extension == ".mod")
				{
					return AudioType.MOD;
				}
			}
		}
		else if (extension == ".aiff")
		{
			return AudioType.AIFF;
		}
		return AudioType.UNKNOWN;
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x0002C30C File Offset: 0x0002A50C
	public static bool CultureInfoTryParse(string name, out CultureInfo info)
	{
		bool result;
		try
		{
			info = CultureInfo.GetCultureInfo(name);
			result = true;
		}
		catch
		{
			info = null;
			result = false;
		}
		return result;
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x0002C340 File Offset: 0x0002A540
	public static string ToHex(this Color color)
	{
		return color.ToHex();
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x0002C350 File Offset: 0x0002A550
	public static string ToHex(this Color32 color)
	{
		return string.Concat(new string[]
		{
			"#",
			color.r.ToString("X2"),
			color.g.ToString("X2"),
			color.b.ToString("X2"),
			color.a.ToString("X2")
		});
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x0002C3C0 File Offset: 0x0002A5C0
	public static bool TryParseColor(string input, out Color32 color)
	{
		if (string.IsNullOrEmpty(input))
		{
			color = Misc._defaultColor;
			return false;
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(input);
		if (num <= 1452231588U)
		{
			if (num <= 135788877U)
			{
				if (num <= 65090618U)
				{
					if (num != 18738364U)
					{
						if (num == 65090618U)
						{
							if (input == "fuchsia")
							{
								color = new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);
								return true;
							}
						}
					}
					else if (input == "green")
					{
						color = new Color32(0, 128, 0, byte.MaxValue);
						return true;
					}
				}
				else if (num != 96429129U)
				{
					if (num != 132336572U)
					{
						if (num == 135788877U)
						{
							if (input == "darkblue")
							{
								color = new Color32(0, 0, 139, byte.MaxValue);
								return true;
							}
						}
					}
					else if (input == "lime")
					{
						color = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
						return true;
					}
				}
				else if (input == "yellow")
				{
					color = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
					return true;
				}
			}
			else if (num <= 1110921109U)
			{
				if (num != 817772335U)
				{
					if (num != 1089765596U)
					{
						if (num == 1110921109U)
						{
							if (input == "aqua")
							{
								color = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
								return true;
							}
						}
					}
					else if (input == "red")
					{
						color = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);
						return true;
					}
				}
				else if (input == "brown")
				{
					color = new Color32(165, 42, 42, byte.MaxValue);
					return true;
				}
			}
			else if (num != 1169454059U)
			{
				if (num != 1231115066U)
				{
					if (num == 1452231588U)
					{
						if (input == "black")
						{
							color = new Color32(0, 0, 0, byte.MaxValue);
							return true;
						}
					}
				}
				else if (input == "cyan")
				{
					color = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
					return true;
				}
			}
			else if (input == "orange")
			{
				color = new Color32(byte.MaxValue, 165, 0, byte.MaxValue);
				return true;
			}
		}
		else
		{
			if (num > 2701128145U)
			{
				if (num <= 3042244896U)
				{
					if (num != 2751299231U)
					{
						if (num != 2995788198U)
						{
							if (num != 3042244896U)
							{
								goto IL_5C4;
							}
							if (!(input == "silver"))
							{
								goto IL_5C4;
							}
							color = new Color32(192, 192, 192, byte.MaxValue);
							return true;
						}
						else if (!(input == "grey"))
						{
							goto IL_5C4;
						}
					}
					else
					{
						if (!(input == "teal"))
						{
							goto IL_5C4;
						}
						color = new Color32(0, 128, 128, byte.MaxValue);
						return true;
					}
				}
				else if (num != 3130700698U)
				{
					if (num != 3278945851U)
					{
						if (num != 3724674918U)
						{
							goto IL_5C4;
						}
						if (!(input == "white"))
						{
							goto IL_5C4;
						}
						color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
						return true;
					}
					else
					{
						if (!(input == "lightblue"))
						{
							goto IL_5C4;
						}
						color = new Color32(173, 216, 230, byte.MaxValue);
						return true;
					}
				}
				else if (!(input == "gray"))
				{
					goto IL_5C4;
				}
				color = new Color32(128, 128, 128, byte.MaxValue);
				return true;
			}
			if (num <= 2197550541U)
			{
				if (num != 1676028392U)
				{
					if (num != 1848823029U)
					{
						if (num == 2197550541U)
						{
							if (input == "blue")
							{
								color = new Color32(0, 0, byte.MaxValue, byte.MaxValue);
								return true;
							}
						}
					}
					else if (input == "maroon")
					{
						color = new Color32(128, 0, 0, byte.MaxValue);
						return true;
					}
				}
				else if (input == "magenta")
				{
					color = new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);
					return true;
				}
			}
			else if (num != 2203898828U)
			{
				if (num != 2590900991U)
				{
					if (num == 2701128145U)
					{
						if (input == "navy")
						{
							color = new Color32(0, 0, 128, byte.MaxValue);
							return true;
						}
					}
				}
				else if (input == "purple")
				{
					color = new Color32(128, 0, 128, byte.MaxValue);
					return true;
				}
			}
			else if (input == "olive")
			{
				color = new Color32(128, 128, 0, byte.MaxValue);
				return true;
			}
		}
		IL_5C4:
		if (input.StartsWith("#"))
		{
			input = input.Substring(1);
		}
		if (input.Length < 6)
		{
			color = Misc._defaultColor;
			return false;
		}
		byte r;
		if (!byte.TryParse(input.Substring(0, 2), NumberStyles.HexNumber, null, out r))
		{
			color = Misc._defaultColor;
			return false;
		}
		byte g;
		if (!byte.TryParse(input.Substring(2, 2), NumberStyles.HexNumber, null, out g))
		{
			color = Misc._defaultColor;
			return false;
		}
		byte b;
		if (!byte.TryParse(input.Substring(4, 2), NumberStyles.HexNumber, null, out b))
		{
			color = Misc._defaultColor;
			return false;
		}
		byte a = byte.MaxValue;
		if (input.Length >= 8)
		{
			byte b2;
			if (!byte.TryParse(input.Substring(6, 2), NumberStyles.HexNumber, null, out b2))
			{
				color = Misc._defaultColor;
				return false;
			}
			a = b2;
		}
		color = new Color32(r, g, b, a);
		return true;
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x0002CA78 File Offset: 0x0002AC78
	public static ConsoleColor ClosestConsoleColor(Color color, bool excludeDark = true)
	{
		if (color == Color.green)
		{
			return ConsoleColor.Green;
		}
		if (color == Color.red)
		{
			return ConsoleColor.Red;
		}
		if (color == Color.gray || color == Color.white || color == Color.black)
		{
			return ConsoleColor.White;
		}
		if (color == Color.magenta)
		{
			return ConsoleColor.Magenta;
		}
		if (color == Color.yellow)
		{
			return ConsoleColor.Yellow;
		}
		if (color == Misc._raOrange)
		{
			return ConsoleColor.DarkYellow;
		}
		if (color == Misc._darkGreen)
		{
			return ConsoleColor.DarkGreen;
		}
		ConsoleColor consoleColor = ConsoleColor.White;
		double num = (double)color.r;
		double num2 = (double)color.g;
		double num3 = (double)color.b;
		double num4 = double.MaxValue;
		foreach (KeyValuePair<ConsoleColor, Color> keyValuePair in Misc.ConsoleColors)
		{
			Color value = keyValuePair.Value;
			double num5 = Math.Pow((double)value.r - num, 2.0) + Math.Pow((double)value.g - num2, 2.0) + Math.Pow((double)value.b - num3, 2.0);
			if (num5 == 0.0)
			{
				return keyValuePair.Key;
			}
			if (num5 < num4)
			{
				num4 = num5;
				consoleColor = keyValuePair.Key;
			}
		}
		if ((consoleColor == ConsoleColor.Black || consoleColor == ConsoleColor.Gray || consoleColor == ConsoleColor.DarkGray) && excludeDark)
		{
			return ConsoleColor.White;
		}
		return consoleColor;
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x0002CC14 File Offset: 0x0002AE14
	public static void WriteBoolByte(this NetworkWriter writer, bool bool1 = false, bool bool2 = false, bool bool3 = false, bool bool4 = false, bool bool5 = false, bool bool6 = false, bool bool7 = false, bool bool8 = false)
	{
		byte b = 0;
		if (bool1)
		{
			b |= 1;
		}
		if (bool2)
		{
			b |= 2;
		}
		if (bool3)
		{
			b |= 4;
		}
		if (bool4)
		{
			b |= 8;
		}
		if (bool5)
		{
			b |= 16;
		}
		if (bool6)
		{
			b |= 32;
		}
		if (bool7)
		{
			b |= 64;
		}
		if (bool8)
		{
			b |= 128;
		}
		writer.WriteByte(b);
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x0002CC78 File Offset: 0x0002AE78
	public static void ReadBoolByte(this NetworkReader reader, out bool bool1, out bool bool2, out bool bool3, out bool bool4, out bool bool5, out bool bool6, out bool bool7, out bool bool8)
	{
		byte b = reader.ReadByte();
		bool1 = ((b & 1) == 1);
		bool2 = ((b & 2) == 2);
		bool3 = ((b & 4) == 4);
		bool4 = ((b & 8) == 8);
		bool5 = ((b & 16) == 16);
		bool6 = ((b & 32) == 32);
		bool7 = ((b & 64) == 64);
		bool8 = ((b & 128) == 128);
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x0002CCE0 File Offset: 0x0002AEE0
	public static void ReadBoolByte(this NetworkReader reader, out bool bool1, out bool bool2, out bool bool3, out bool bool4)
	{
		byte b = reader.ReadByte();
		bool1 = ((b & 1) == 1);
		bool2 = ((b & 2) == 2);
		bool3 = ((b & 4) == 4);
		bool4 = ((b & 8) == 8);
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x0002CD18 File Offset: 0x0002AF18
	public static void WriteBoolArray(this NetworkWriter writer, bool[] array)
	{
		byte b = 0;
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			int num = i % 8;
			if (array[i])
			{
				b |= (byte)(1 << num);
			}
			if (num + 1 < 8)
			{
				flag = true;
			}
			else
			{
				writer.WriteByte(b);
				b = 0;
				flag = false;
			}
		}
		if (!flag)
		{
			return;
		}
		writer.WriteByte(b);
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x0002CD6C File Offset: 0x0002AF6C
	public static void ReadBoolArray(this NetworkReader reader, bool[] array)
	{
		byte b = 0;
		for (int i = 0; i < array.Length; i++)
		{
			int num = i % 8;
			if (num == 0)
			{
				b = reader.ReadByte();
			}
			byte b2 = (byte)(1 << num);
			array[i] = ((b2 & b) > 0);
		}
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x0002CDA8 File Offset: 0x0002AFA8
	public static byte BoolsToByte(bool bool1 = false, bool bool2 = false, bool bool3 = false, bool bool4 = false, bool bool5 = false, bool bool6 = false, bool bool7 = false, bool bool8 = false)
	{
		byte b = 0;
		if (bool1)
		{
			b |= 1;
		}
		if (bool2)
		{
			b |= 2;
		}
		if (bool3)
		{
			b |= 4;
		}
		if (bool4)
		{
			b |= 8;
		}
		if (bool5)
		{
			b |= 16;
		}
		if (bool6)
		{
			b |= 32;
		}
		if (bool7)
		{
			b |= 64;
		}
		if (bool8)
		{
			b |= 128;
		}
		return b;
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x0002CE04 File Offset: 0x0002B004
	public static void ByteToBools(byte b, out bool bool1, out bool bool2, out bool bool3, out bool bool4, out bool bool5, out bool bool6, out bool bool7, out bool bool8)
	{
		bool1 = ((b & 1) == 1);
		bool2 = ((b & 2) == 2);
		bool3 = ((b & 4) == 4);
		bool4 = ((b & 8) == 8);
		bool5 = ((b & 16) == 16);
		bool6 = ((b & 32) == 32);
		bool7 = ((b & 64) == 64);
		bool8 = ((b & 128) == 128);
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x0002CE64 File Offset: 0x0002B064
	public unsafe static int GetBytes(this Encoding encoding, string text, NativeMemory memory)
	{
		char* ptr = text;
		if (ptr != null)
		{
			ptr += RuntimeHelpers.OffsetToStringData / 2;
		}
		return encoding.GetBytes(ptr, text.Length, memory.ToPointer<byte>(), memory.Length);
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x0002CE9A File Offset: 0x0002B09A
	public static bool IsSafeCharacter(char c)
	{
		return c > '\u001f' && c < '\u007f';
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x0002CEA8 File Offset: 0x0002B0A8
	public static void ReplaceUnsafeCharacters(ref string text, char replaceCharacter = '?')
	{
		if (text == null)
		{
			text = string.Empty;
			return;
		}
		StringBuilder stringBuilder = StringBuilderPool.Shared.Rent(text.Length);
		for (int i = 0; i < text.Length; i++)
		{
			stringBuilder.Append(Misc.IsSafeCharacter(text[i]) ? text[i] : replaceCharacter);
		}
		text = stringBuilder.ToString();
		StringBuilderPool.Shared.Return(stringBuilder);
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x0002CF1C File Offset: 0x0002B11C
	public static System.Random CreateRandom()
	{
		byte[] array = new byte[4];
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(array);
		}
		return new System.Random(BitConverter.ToInt32(array, 0));
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x0002CF68 File Offset: 0x0002B168
	public static Vector3 NormalizeIgnoreY(this Vector3 v)
	{
		v.y = 0f;
		return v.normalized;
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x0002CF80 File Offset: 0x0002B180
	public static Vector3 NormalizeConstrained(this Vector3 v, Vector3 constraintDirection)
	{
		Vector3 vector = Vector3.Dot(v, constraintDirection) * constraintDirection;
		Vector3 vector2 = v - vector;
		float sqrMagnitude = vector2.sqrMagnitude;
		if (sqrMagnitude > 0f)
		{
			float d = Mathf.Sqrt(1f - vector.sqrMagnitude) / Mathf.Sqrt(sqrMagnitude);
			vector2 *= d;
		}
		return vector + vector2;
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x0002CFDC File Offset: 0x0002B1DC
	public static Vector3 ClampDot(this Vector3 vectorToClamp, Vector3 targetDirectionVector, float minimumDot)
	{
		float num = Vector3.Dot(vectorToClamp, targetDirectionVector);
		if (num >= minimumDot)
		{
			return vectorToClamp;
		}
		Vector3 vector = vectorToClamp - targetDirectionVector * num;
		float d = Mathf.Sqrt(1f - minimumDot * minimumDot);
		return targetDirectionVector * minimumDot + vector.normalized * d;
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x0002D02D File Offset: 0x0002B22D
	public static Vector3 Abs(this Vector3 v)
	{
		return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x0002D055 File Offset: 0x0002B255
	public static float SqrMagnitudeIgnoreY(this Vector3 v)
	{
		return (float)((double)v.x * (double)v.x + (double)v.z * (double)v.z);
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x0002D077 File Offset: 0x0002B277
	public static float MagnitudeIgnoreY(this Vector3 v)
	{
		return (float)Math.Sqrt((double)v.x * (double)v.x + (double)v.z * (double)v.z);
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x0002D09E File Offset: 0x0002B29E
	public static float MagnitudeOnlyY(this Vector3 v)
	{
		return Math.Abs(v.y);
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x0002D0AB File Offset: 0x0002B2AB
	public static string ToPreciseString(this Vector2 v)
	{
		return string.Format("[{0:F3}, {1:F3}]", v.x, v.y);
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x0002D0CD File Offset: 0x0002B2CD
	public static string ToPreciseString(this Vector3 v)
	{
		return string.Format("[{0:F3}, {1:F3}, {2:F3}]", v.x, v.y, v.z);
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x0002D0FC File Offset: 0x0002B2FC
	public static float AngleIgnoreY(Vector3 from, Vector3 to)
	{
		to.y = from.y;
		float num = (float)Math.Sqrt((double)from.SqrMagnitudeIgnoreY() * (double)to.SqrMagnitudeIgnoreY());
		if ((double)num >= 1.00000000362749E-15)
		{
			return (float)Math.Acos((double)Mathf.Clamp(Vector3.Dot(from, to) / num, -1f, 1f)) * 57.29578f;
		}
		return 0f;
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x0002D164 File Offset: 0x0002B364
	public static Vector2 RotateAroundZ(this Vector2 vector, float degrees)
	{
		float num = Mathf.Sin(degrees * 0.017453292f);
		float num2 = Mathf.Cos(degrees * 0.017453292f);
		float x = vector.x;
		float y = vector.y;
		vector.x = num2 * x - num * y;
		vector.y = num * x + num2 * y;
		return vector;
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x0002D1B6 File Offset: 0x0002B3B6
	public static bool SameSign(this float a, float b)
	{
		return a < 0f == b < 0f;
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x0002D1CA File Offset: 0x0002B3CA
	public static byte RoundToByte(float value)
	{
		return (byte)Mathf.Clamp(Mathf.RoundToInt(value), 0, 255);
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x0002D1DE File Offset: 0x0002B3DE
	public static ushort RoundToUShort(float value)
	{
		return (ushort)Mathf.Clamp(Mathf.RoundToInt(value), 0, 65535);
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x0002D1F2 File Offset: 0x0002B3F2
	public static short RoundToShort(float value)
	{
		return (short)Mathf.Clamp(Mathf.RoundToInt(value), -32768, 32767);
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x0002D20C File Offset: 0x0002B40C
	public static bool TryGetClosestLineSegment(Vector3 start, Vector3 end, Vector3 point, float minLength, float maxLength, out Vector3 newStart, out Vector3 newEnd, out Vector3 closestPointOnLine, out Vector3 normalizedDir)
	{
		Vector3 a = end - start;
		float magnitude = a.magnitude;
		if (magnitude < minLength)
		{
			newStart = start;
			newEnd = end;
			normalizedDir = Vector3.zero;
			closestPointOnLine = point;
			return false;
		}
		normalizedDir = a / magnitude;
		float num = Mathf.Clamp(Vector3.Dot(normalizedDir, end - point), 0f, magnitude);
		float num2 = magnitude - num;
		Vector3 vector = start + num2 * normalizedDir;
		float num3 = Mathf.Min(magnitude, maxLength) / 2f;
		float num4 = Mathf.Max(num3 - num2, 0f);
		float num5 = Mathf.Max(num3 - num, 0f);
		newStart = vector - normalizedDir * (num3 - num4 + num5);
		newEnd = vector + normalizedDir * (num3 + num4 - num5);
		closestPointOnLine = vector;
		return true;
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x0002D318 File Offset: 0x0002B518
	public static void GetClosestPointOnLine(Vector3 start, Vector3 end, Vector3 point, out Vector3 closestPointOnLine, out Vector3 normalizedDir)
	{
		Vector3 a = end - start;
		float magnitude = a.magnitude;
		if (Mathf.Approximately(magnitude, 0f))
		{
			closestPointOnLine = start;
			normalizedDir = Vector3.zero;
			return;
		}
		normalizedDir = a / magnitude;
		float num = Vector3.Dot(normalizedDir, point - start);
		if (num <= 0f)
		{
			closestPointOnLine = start;
			return;
		}
		if (num >= magnitude)
		{
			closestPointOnLine = end;
			return;
		}
		Vector3 b = normalizedDir * num;
		closestPointOnLine = start + b;
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x0002D3B0 File Offset: 0x0002B5B0
	public static bool TryCommandModeFromArgs(ref string[] newargs, out Misc.CommandOperationMode mode)
	{
		if (newargs != null && newargs.Length != 0)
		{
			string text = newargs[0].ToLowerInvariant();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 1303515621U)
			{
				if (num <= 873244444U)
				{
					if (num != 184981848U)
					{
						if (num != 873244444U)
						{
							goto IL_114;
						}
						if (!(text == "1"))
						{
							goto IL_114;
						}
					}
					else
					{
						if (!(text == "false"))
						{
							goto IL_114;
						}
						goto IL_10F;
					}
				}
				else if (num != 890022063U)
				{
					if (num != 1303515621U)
					{
						goto IL_114;
					}
					if (!(text == "true"))
					{
						goto IL_114;
					}
				}
				else
				{
					if (!(text == "0"))
					{
						goto IL_114;
					}
					goto IL_10F;
				}
			}
			else if (num <= 2872740362U)
			{
				if (num != 1630810064U)
				{
					if (num != 2872740362U)
					{
						goto IL_114;
					}
					if (!(text == "off"))
					{
						goto IL_114;
					}
					goto IL_10F;
				}
				else if (!(text == "on"))
				{
					goto IL_114;
				}
			}
			else if (num != 2945169614U)
			{
				if (num != 3454897251U)
				{
					goto IL_114;
				}
				if (!(text == "disable"))
				{
					goto IL_114;
				}
				goto IL_10F;
			}
			else if (!(text == "enable"))
			{
				goto IL_114;
			}
			mode = Misc.CommandOperationMode.Enable;
			return true;
			IL_10F:
			mode = Misc.CommandOperationMode.Disable;
			return true;
			IL_114:
			mode = Misc.CommandOperationMode.Toggle;
			return false;
		}
		mode = Misc.CommandOperationMode.Toggle;
		return true;
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x0002D4DC File Offset: 0x0002B6DC
	public static bool TryGetComponentInParent<T>(this Transform startTransform, out T comp)
	{
		int num;
		return startTransform.TryGetComponentInParent(out comp, out num);
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x0002D4F2 File Offset: 0x0002B6F2
	public static bool TryGetComponentInParent<T>(this Transform startTransform, out T comp, out int depth)
	{
		depth = 0;
		while (startTransform != null)
		{
			if (startTransform.TryGetComponent<T>(out comp))
			{
				return true;
			}
			depth++;
			startTransform = startTransform.parent;
		}
		comp = default(T);
		return false;
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x0002D524 File Offset: 0x0002B724
	public static void ForEachComponentInChildren<T>(this GameObject root, Action<T> action, bool includeInactive)
	{
		List<T> list = ListPool<T>.Shared.Rent();
		root.GetComponentsInChildren<T>(includeInactive, list);
		list.ForEach(action);
		ListPool<T>.Shared.Return(list);
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x0002D556 File Offset: 0x0002B756
	public static void ResetLocalPosition(this Transform tr)
	{
		tr.localPosition = Vector3.zero;
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x0002D563 File Offset: 0x0002B763
	public static void ResetLocalRotation(this Transform tr)
	{
		tr.localRotation = Quaternion.identity;
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x0002D570 File Offset: 0x0002B770
	public static void ResetLocalScale(this Transform tr)
	{
		tr.localScale = Vector3.one;
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x0002D57D File Offset: 0x0002B77D
	public static void ResetLocalPose(this Transform tr)
	{
		tr.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x0002D58F File Offset: 0x0002B78F
	public static void ResetTransform(this Transform tr)
	{
		tr.ResetLocalPose();
		tr.ResetLocalScale();
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x0002D5A0 File Offset: 0x0002B7A0
	public static string GetHierarchyPath(this Transform tr)
	{
		List<Transform> list = ListPool<Transform>.Shared.Rent();
		Transform parent = tr.parent;
		while (parent != null)
		{
			list.Add(parent);
			parent = parent.parent;
		}
		StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
		for (int i = list.Count - 1; i >= 0; i--)
		{
			stringBuilder.Append(list[i].name);
			stringBuilder.Append("/");
		}
		stringBuilder.Append(tr.name);
		ListPool<Transform>.Shared.Return(list);
		return StringBuilderPool.Shared.ToStringReturn(stringBuilder);
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x0002D638 File Offset: 0x0002B838
	public static float GetDuration(this AnimationCurve curve)
	{
		return curve[curve.length - 1].time;
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x0002D65B File Offset: 0x0002B85B
	public static string RemoveStacktraceZeroes(string stacktrace)
	{
		return stacktrace.Replace(" [0x00000] in <00000000000000000000000000000000>:0", "");
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x0002D670 File Offset: 0x0002B870
	public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
		if (!directoryInfo.Exists)
		{
			throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		if (Directory.Exists(destDirName))
		{
			Directory.Delete(destDirName, true);
		}
		Directory.CreateDirectory(destDirName);
		foreach (FileInfo fileInfo in directoryInfo.GetFiles())
		{
			string destFileName = Path.Combine(destDirName, fileInfo.Name);
			fileInfo.CopyTo(destFileName, true);
		}
		if (!copySubDirs)
		{
			return;
		}
		foreach (DirectoryInfo directoryInfo2 in directories)
		{
			string destDirName2 = Path.Combine(destDirName, directoryInfo2.Name);
			Misc.DirectoryCopy(directoryInfo2.FullName, destDirName2, true);
		}
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x0002D723 File Offset: 0x0002B923
	public static Color ConvertToGray(this Color oldColor)
	{
		float num = oldColor.r * 0.299f + oldColor.g * 0.587f + oldColor.b * 0.114f;
		return new Color(num, num, num, oldColor.a);
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x0002D758 File Offset: 0x0002B958
	public static string FormatAvailable(string format, params object[] args)
	{
		StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
		stringBuilder.Append(format);
		int num = 0;
		foreach (object obj in Misc.FormatBracketRegex.Matches(format))
		{
			Match match = (Match)obj;
			if (num <= args.Length - 1)
			{
				stringBuilder.Replace(match.Value, args[num].ToString());
			}
			num++;
		}
		return StringBuilderPool.Shared.ToStringReturn(stringBuilder);
	}

	// Token: 0x04000996 RID: 2454
	private static readonly Regex TagRegex = new Regex("<(\\/?(align|allcaps|alpha|b|color|cspace|font|font-weight|gradient|i|indent|line-height|line-indent|link|lowercase|margin|mark|mspace|nobr|noparse|page|pos|rotate|s|size|smallcaps|space|sprite|style|sub|sup|u|uppercase|voffset|width))[^<>]*>");

	// Token: 0x04000997 RID: 2455
	private static readonly Color _raOrange = new Color32(byte.MaxValue, 180, 0, byte.MaxValue);

	// Token: 0x04000998 RID: 2456
	private static readonly Color _darkGreen = new Color32(80, 150, 80, byte.MaxValue);

	// Token: 0x04000999 RID: 2457
	public static Encoding Utf8Encoding = new UTF8Encoding(false);

	// Token: 0x0400099A RID: 2458
	private static readonly Dictionary<ConsoleColor, Color> ConsoleColors = new Dictionary<ConsoleColor, Color>();

	// Token: 0x0400099B RID: 2459
	private static readonly Regex _pbRgx = new Regex("^[a-zA-Z0-9]{8}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	// Token: 0x0400099C RID: 2460
	private static readonly Regex _ipV4Rgx = new Regex("^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	// Token: 0x0400099D RID: 2461
	private static readonly Regex _ipV6Rgx = new Regex("^((([0-9a-f]{1,4}:){7}([0-9a-f]{1,4}|:))|(([0-9a-f]{1,4}:){6}(:[0-9a-f]{1,4}|((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3})|:))|(([0-9a-f]{1,4}:){5}(((:[0-9a-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3})|:))|(([0-9a-f]{1,4}:){4}(((:[0-9a-f]{1,4}){1,3})|((:[0-9a-f]{1,4})?:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(([0-9a-f]{1,4}:){3}(((:[0-9a-f]{1,4}){1,4})|((:[0-9a-f]{1,4}){0,2}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(([0-9a-f]{1,4}:){2}(((:[0-9a-f]{1,4}){1,5})|((:[0-9a-f]{1,4}){0,3}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(([0-9a-f]{1,4}:){1}(((:[0-9a-f]{1,4}){1,6})|((:[0-9a-f]{1,4}){0,4}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:))|(:(((:[0-9a-f]{1,4}){1,7})|((:[0-9a-f]{1,4}){0,5}:((25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)(\\.(25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]?\\d)){3}))|:)))$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	// Token: 0x0400099E RID: 2462
	private static readonly Regex _hostNameRgx = new Regex("^(([a-z0-9]|[a-z0-9][a-z0-9\\-]*[a-z0-9])\\.)*([a-z0-9]|[a-z0-9][a-z0-9\\-]*[a-z0-9])$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	// Token: 0x0400099F RID: 2463
	private static readonly Regex _stpRgx = new Regex("<size=[^>]{0,}>", RegexOptions.Compiled);

	// Token: 0x040009A0 RID: 2464
	internal static readonly Regex CommandRegex = new Regex("^[a-zA-Z0-9\\-_.]{1,40}$", RegexOptions.Compiled);

	// Token: 0x040009A1 RID: 2465
	internal static readonly Regex CommandDescriptionRegex = new Regex("^[\\p{L}\\p{P}\\p{Sm}\\p{Sc}\\p{N} ^]{1,80}(\\.\\.\\.|)$", RegexOptions.Compiled);

	// Token: 0x040009A2 RID: 2466
	internal static readonly Regex RichTextRegex = new Regex("(?!<*?>)<.*?>", RegexOptions.Compiled);

	// Token: 0x040009A3 RID: 2467
	public static readonly Regex PlayerCustomInfoRegex = new Regex("^((?![\\[\\]])[\\p{L}\\p{P}\\p{Sc}\\p{N} ^=+|~`<>\\n]){0,400}$", RegexOptions.Compiled);

	// Token: 0x040009A4 RID: 2468
	private static readonly Regex FormatBracketRegex = new Regex("{\\d*}", RegexOptions.Compiled);

	// Token: 0x040009A5 RID: 2469
	public static readonly string[] AcceptedColours = new string[]
	{
		"FF96DE",
		"C50000",
		"944710",
		"A0A0A0",
		"32CD32",
		"DC143C",
		"00B7EB",
		"00FFFF",
		"FF1493",
		"FF6448",
		"FAFF86",
		"FF0090",
		"4DFFB8",
		"FF9966",
		"BFFF00",
		"228B22",
		"50C878",
		"960018",
		"727472",
		"98FB98",
		"4B5320",
		"EE7600",
		"FFFFFF",
		"000000"
	};

	// Token: 0x040009A6 RID: 2470
	public static readonly Dictionary<Misc.PlayerInfoColorTypes, string> AllowedColors = new Dictionary<Misc.PlayerInfoColorTypes, string>
	{
		{
			Misc.PlayerInfoColorTypes.Pink,
			"#FF96DE"
		},
		{
			Misc.PlayerInfoColorTypes.Red,
			"#C50000"
		},
		{
			Misc.PlayerInfoColorTypes.Brown,
			"#944710"
		},
		{
			Misc.PlayerInfoColorTypes.Silver,
			"#A0A0A0"
		},
		{
			Misc.PlayerInfoColorTypes.LightGreen,
			"#32CD32"
		},
		{
			Misc.PlayerInfoColorTypes.Crimson,
			"#DC143C"
		},
		{
			Misc.PlayerInfoColorTypes.Cyan,
			"#00B7EB"
		},
		{
			Misc.PlayerInfoColorTypes.Aqua,
			"#00FFFF"
		},
		{
			Misc.PlayerInfoColorTypes.DeepPink,
			"#FF1493"
		},
		{
			Misc.PlayerInfoColorTypes.Tomato,
			"#FF6448"
		},
		{
			Misc.PlayerInfoColorTypes.Yellow,
			"#FAFF86"
		},
		{
			Misc.PlayerInfoColorTypes.Magenta,
			"#FF0090"
		},
		{
			Misc.PlayerInfoColorTypes.BlueGreen,
			"#4DFFB8"
		},
		{
			Misc.PlayerInfoColorTypes.Orange,
			"#FF9966"
		},
		{
			Misc.PlayerInfoColorTypes.Lime,
			"#BFFF00"
		},
		{
			Misc.PlayerInfoColorTypes.Green,
			"#228B22"
		},
		{
			Misc.PlayerInfoColorTypes.Emerald,
			"#50C878"
		},
		{
			Misc.PlayerInfoColorTypes.Carmine,
			"#960018"
		},
		{
			Misc.PlayerInfoColorTypes.Nickel,
			"#727472"
		},
		{
			Misc.PlayerInfoColorTypes.Mint,
			"#98FB98"
		},
		{
			Misc.PlayerInfoColorTypes.ArmyGreen,
			"#4B5320"
		},
		{
			Misc.PlayerInfoColorTypes.Pumpkin,
			"#EE7600"
		},
		{
			Misc.PlayerInfoColorTypes.Black,
			"#000000"
		},
		{
			Misc.PlayerInfoColorTypes.White,
			"#FFFFFF"
		}
	};

	// Token: 0x040009A7 RID: 2471
	private static readonly Color32 _defaultColor = Color.white;

	// Token: 0x02000218 RID: 536
	public enum IPAddressType
	{
		// Token: 0x040009A9 RID: 2473
		Unknown,
		// Token: 0x040009AA RID: 2474
		IPV4,
		// Token: 0x040009AB RID: 2475
		IPV6,
		// Token: 0x040009AC RID: 2476
		Localhost,
		// Token: 0x040009AD RID: 2477
		Hostname
	}

	// Token: 0x02000219 RID: 537
	public enum PlayerInfoColorTypes
	{
		// Token: 0x040009AF RID: 2479
		Pink,
		// Token: 0x040009B0 RID: 2480
		Red,
		// Token: 0x040009B1 RID: 2481
		Brown,
		// Token: 0x040009B2 RID: 2482
		Silver,
		// Token: 0x040009B3 RID: 2483
		LightGreen,
		// Token: 0x040009B4 RID: 2484
		Crimson,
		// Token: 0x040009B5 RID: 2485
		Cyan,
		// Token: 0x040009B6 RID: 2486
		Aqua,
		// Token: 0x040009B7 RID: 2487
		DeepPink,
		// Token: 0x040009B8 RID: 2488
		Tomato,
		// Token: 0x040009B9 RID: 2489
		Yellow,
		// Token: 0x040009BA RID: 2490
		Magenta,
		// Token: 0x040009BB RID: 2491
		BlueGreen,
		// Token: 0x040009BC RID: 2492
		Orange,
		// Token: 0x040009BD RID: 2493
		Lime,
		// Token: 0x040009BE RID: 2494
		Green,
		// Token: 0x040009BF RID: 2495
		Emerald,
		// Token: 0x040009C0 RID: 2496
		Carmine,
		// Token: 0x040009C1 RID: 2497
		Nickel,
		// Token: 0x040009C2 RID: 2498
		Mint,
		// Token: 0x040009C3 RID: 2499
		ArmyGreen,
		// Token: 0x040009C4 RID: 2500
		Pumpkin,
		// Token: 0x040009C5 RID: 2501
		Black,
		// Token: 0x040009C6 RID: 2502
		White
	}

	// Token: 0x0200021A RID: 538
	public enum CommandOperationMode
	{
		// Token: 0x040009C8 RID: 2504
		Disable,
		// Token: 0x040009C9 RID: 2505
		Enable,
		// Token: 0x040009CA RID: 2506
		Toggle
	}

	// Token: 0x0200021B RID: 539
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x040009CB RID: 2507
		public static Func<string, int> <0>__Parse;
	}
}
