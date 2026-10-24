using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Cassie.Interpreters;
using UnityEngine;

namespace Cassie
{
	// Token: 0x02000AC4 RID: 2756
	public class NumberInterpreter : CassieInterpreter
	{
		// Token: 0x06003E27 RID: 15911 RVA: 0x000CD5E0 File Offset: 0x000CB7E0
		protected unsafe override CassieInterpreter.NextAction ProcessWord(ReadOnlySpan<char> word, CassieLineDatabase database, ref CassiePlaybackModifiers modifiers, StringBuilder subtitleBuilder, List<CassieInterpreter.Result> lines)
		{
			double num;
			if (!double.TryParse(word, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out num))
			{
				return CassieInterpreter.NextAction.Continue;
			}
			if (subtitleBuilder != null)
			{
				subtitleBuilder.Append(word);
			}
			if (subtitleBuilder != null)
			{
				subtitleBuilder.Append(' ');
			}
			NumberInterpreter._adderDatabase = database;
			NumberInterpreter._adderList = lines;
			NumberInterpreter._adderModifiers = modifiers;
			if (num < 0.0)
			{
				NumberInterpreter.AddLine(NumberInterpreter.Negative);
				num = -num;
			}
			bool flag = num > 999999999999.999;
			if (!flag)
			{
				NumberInterpreter.HandleNaturalNumber((long)num);
			}
			bool flag2 = false;
			for (int i = 0; i < word.Length; i++)
			{
				char c = (char)(*word[i]);
				if (c == '.')
				{
					flag2 = true;
					NumberInterpreter.AddLine(NumberInterpreter.Point);
				}
				else if ((flag2 || flag) && c >= '0' && c <= '9')
				{
					NumberInterpreter.AddLine(word.Slice(i, 1));
				}
			}
			modifiers.ResetSingleUseModifiers();
			return CassieInterpreter.NextAction.Halt;
		}

		// Token: 0x06003E28 RID: 15912 RVA: 0x000CD6C0 File Offset: 0x000CB8C0
		private static void HandleNaturalNumber(long num)
		{
			if (num == 0L)
			{
				NumberInterpreter.AddLine(NumberInterpreter.Digits[0]);
				return;
			}
			if (NumberInterpreter._digitsPerScale == null)
			{
				NumberInterpreter._digitsPerScale = new int[NumberInterpreter.Scales.Length];
			}
			Array.Clear(NumberInterpreter._digitsPerScale, 0, NumberInterpreter._digitsPerScale.Length);
			int num2 = 0;
			while (num > 0L && num2 < NumberInterpreter._digitsPerScale.Length)
			{
				NumberInterpreter._digitsPerScale[num2++] = (int)(num % 1000L);
				num /= 1000L;
			}
			for (int i = num2 - 1; i >= 0; i--)
			{
				int num3 = NumberInterpreter._digitsPerScale[i];
				if (num3 != 0)
				{
					NumberInterpreter.AddUnder1000(num3);
					if (i > 0)
					{
						NumberInterpreter.AddLine(NumberInterpreter.Scales[i]);
					}
				}
			}
		}

		// Token: 0x06003E29 RID: 15913 RVA: 0x000CD768 File Offset: 0x000CB968
		private static void AddUnder1000(int n)
		{
			int num = n / 100;
			int num2 = n % 100;
			if (num > 0)
			{
				NumberInterpreter.AddLine(NumberInterpreter.Digits[num]);
				NumberInterpreter.AddLine(NumberInterpreter.Scales[0]);
			}
			if (num2 == 0)
			{
				return;
			}
			if (num2 >= 11 && num2 <= 19)
			{
				NumberInterpreter.AddLine(NumberInterpreter.Teens[num2 - 11]);
				return;
			}
			if (num2 == 10 || num2 >= 20)
			{
				int num3 = num2 / 10;
				NumberInterpreter.AddLine(NumberInterpreter.Tens[num3]);
				int num4 = num2 % 10;
				if (num4 != 0)
				{
					NumberInterpreter.AddLine(NumberInterpreter.Digits[num4]);
				}
				return;
			}
			NumberInterpreter.AddLine(NumberInterpreter.Digits[num2]);
		}

		// Token: 0x06003E2A RID: 15914 RVA: 0x000CD7F5 File Offset: 0x000CB9F5
		private static void AddLine(string word)
		{
			NumberInterpreter.AddLine(word.AsSpan());
		}

		// Token: 0x06003E2B RID: 15915 RVA: 0x000CD804 File Offset: 0x000CBA04
		private static void AddLine(ReadOnlySpan<char> word)
		{
			CassieLine line;
			if (!NumberInterpreter._adderDatabase.TryGetLine(word, out line))
			{
				Debug.LogError("Unable to parse " + word.ToString() + " as a CASSIE line.");
				return;
			}
			NumberInterpreter._adderList.Add(new CassieInterpreter.Result(line, NumberInterpreter._adderModifiers));
		}

		// Token: 0x04002D83 RID: 11651
		private static readonly string[] Digits = new string[]
		{
			"0",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9"
		};

		// Token: 0x04002D84 RID: 11652
		private static readonly string[] Teens = new string[]
		{
			"11",
			"12",
			"13",
			"14",
			"15",
			"16",
			"17",
			"18",
			"19"
		};

		// Token: 0x04002D85 RID: 11653
		private static readonly string[] Tens = new string[]
		{
			null,
			"10",
			"20",
			"30",
			"40",
			"50",
			"60",
			"70",
			"80",
			"90"
		};

		// Token: 0x04002D86 RID: 11654
		private static readonly string[] Scales = new string[]
		{
			"HUNDRED",
			"THOUSAND",
			"MILLION",
			"BILLION",
			"TRILLION"
		};

		// Token: 0x04002D87 RID: 11655
		private static readonly string Negative = "NEGATIVE";

		// Token: 0x04002D88 RID: 11656
		private static readonly string Point = "POINT";

		// Token: 0x04002D89 RID: 11657
		private const double MaxPronouncableNumber = 999999999999.999;

		// Token: 0x04002D8A RID: 11658
		private static int[] _digitsPerScale;

		// Token: 0x04002D8B RID: 11659
		private static CassieLineDatabase _adderDatabase;

		// Token: 0x04002D8C RID: 11660
		private static CassiePlaybackModifiers _adderModifiers;

		// Token: 0x04002D8D RID: 11661
		private static List<CassieInterpreter.Result> _adderList;
	}
}
