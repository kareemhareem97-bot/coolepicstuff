using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Cassie.Interpreters
{
	// Token: 0x02000ACB RID: 2763
	public class ModifierInterpreter : CassieInterpreter
	{
		// Token: 0x06003E3C RID: 15932 RVA: 0x000CDD63 File Offset: 0x000CBF63
		protected override CassieInterpreter.NextAction ProcessWord(ReadOnlySpan<char> word, CassieLineDatabase database, ref CassiePlaybackModifiers modifiers, StringBuilder subtitleBuilder, List<CassieInterpreter.Result> lines)
		{
			if (!ModifierInterpreter.TryParse(word, ref modifiers))
			{
				return CassieInterpreter.NextAction.Continue;
			}
			return CassieInterpreter.NextAction.Halt;
		}

		// Token: 0x06003E3D RID: 15933 RVA: 0x000CDD74 File Offset: 0x000CBF74
		private unsafe static bool TryParse(ReadOnlySpan<char> word, ref CassiePlaybackModifiers mod)
		{
			if (word.IsEmpty || *word[0] != 36)
			{
				return false;
			}
			if (ModifierInterpreter.TryMatchPrefix("PITCH", 1, word))
			{
				mod.Pitch = new float?(Mathf.Clamp(ModifierInterpreter.PartsNonAlloc[0], 0.01f, 15f));
				return true;
			}
			if (ModifierInterpreter.TryMatchPrefix("VOL", 1, word))
			{
				mod.Volume = new float?(Mathf.Clamp01(ModifierInterpreter.PartsNonAlloc[0]));
				return true;
			}
			if (ModifierInterpreter.TryMatchPrefix("STARTT", 1, word))
			{
				mod.StartTime = new float?(Mathf.Max(0f, ModifierInterpreter.PartsNonAlloc[0]));
				return true;
			}
			if (ModifierInterpreter.TryMatchPrefix("MAXDUR", 1, word))
			{
				mod.MaxDuration = new float?(Mathf.Max(0f, ModifierInterpreter.PartsNonAlloc[0]));
				return true;
			}
			if (ModifierInterpreter.TryMatchPrefix("SLEEP", 1, word))
			{
				mod.Sleep = new float?(Mathf.Max(0f, ModifierInterpreter.PartsNonAlloc[0]));
				return true;
			}
			if (ModifierInterpreter.TryMatchPrefix("SPAC", 1, word))
			{
				mod.SpacingOverride = new float?(Mathf.Max(0f, ModifierInterpreter.PartsNonAlloc[0]));
				return true;
			}
			if (ModifierInterpreter.TryMatchPrefix("STUTT", 3, word))
			{
				mod.Stutter = new CassieStutterSettings?(new CassieStutterSettings(ModifierInterpreter.PartsNonAlloc[0], ModifierInterpreter.PartsNonAlloc[1], Mathf.RoundToInt(ModifierInterpreter.PartsNonAlloc[2])));
				return true;
			}
			return false;
		}

		// Token: 0x06003E3E RID: 15934 RVA: 0x000CDF00 File Offset: 0x000CC100
		private unsafe static bool TryMatchPrefix(string prefix, int minParts, ReadOnlySpan<char> word)
		{
			ModifierInterpreter.PartsNonAlloc.Clear();
			int? prevSeparator = null;
			for (int i = 1; i < word.Length; i++)
			{
				if (i <= prefix.Length && prefix[i - 1] != (char)(*word[i]))
				{
					return false;
				}
				if (*word[i] == 95)
				{
					ModifierInterpreter.<TryMatchPrefix>g__ProcessSeparator|14_0(prevSeparator, i, word);
					prevSeparator = new int?(i);
				}
			}
			ModifierInterpreter.<TryMatchPrefix>g__ProcessSeparator|14_0(prevSeparator, word.Length, word);
			return ModifierInterpreter.PartsNonAlloc.Count >= minParts;
		}

		// Token: 0x06003E41 RID: 15937 RVA: 0x000CDF98 File Offset: 0x000CC198
		[CompilerGenerated]
		internal static void <TryMatchPrefix>g__ProcessSeparator|14_0(int? prevSeparator, int curSeparator, ReadOnlySpan<char> word)
		{
			if (prevSeparator == null)
			{
				return;
			}
			int num = prevSeparator.Value + 1;
			int length = curSeparator - num;
			float item;
			if (float.TryParse(word.Slice(num, length), NumberStyles.Float, CultureInfo.InvariantCulture, out item))
			{
				ModifierInterpreter.PartsNonAlloc.Add(item);
			}
		}

		// Token: 0x04002D96 RID: 11670
		public const char ModifierHeaderSymbol = '$';

		// Token: 0x04002D97 RID: 11671
		public const char ModifierPartsSeparator = '_';

		// Token: 0x04002D98 RID: 11672
		private const string PitchPrefix = "PITCH";

		// Token: 0x04002D99 RID: 11673
		private const string VolumePrefix = "VOL";

		// Token: 0x04002D9A RID: 11674
		private const string StartTimePrefix = "STARTT";

		// Token: 0x04002D9B RID: 11675
		private const string MaxDurationPrefix = "MAXDUR";

		// Token: 0x04002D9C RID: 11676
		private const string SleepPrefix = "SLEEP";

		// Token: 0x04002D9D RID: 11677
		private const string SpacingOverridePrefix = "SPAC";

		// Token: 0x04002D9E RID: 11678
		private const string StutterPrefix = "STUTT";

		// Token: 0x04002D9F RID: 11679
		private const float MinPitch = 0.01f;

		// Token: 0x04002DA0 RID: 11680
		private const float MaxPitch = 15f;

		// Token: 0x04002DA1 RID: 11681
		private static readonly List<float> PartsNonAlloc = new List<float>();
	}
}
