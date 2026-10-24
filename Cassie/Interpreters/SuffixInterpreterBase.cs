using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Cassie.Interpreters
{
	// Token: 0x02000ACE RID: 2766
	public abstract class SuffixInterpreterBase : CassieInterpreter
	{
		// Token: 0x06003E48 RID: 15944 RVA: 0x000CE2E4 File Offset: 0x000CC4E4
		protected override CassieInterpreter.NextAction ProcessWord(ReadOnlySpan<char> word, CassieLineDatabase database, ref CassiePlaybackModifiers modifiers, StringBuilder subtitleBuilder, List<CassieInterpreter.Result> lines)
		{
			CassieLine cassieLine;
			if (!this.TryFindLemma(word, database, out cassieLine))
			{
				return CassieInterpreter.NextAction.Continue;
			}
			float num;
			float num2;
			string suffix = this.GetSuffix(word, cassieLine, out num, out num2);
			CassieLine line;
			if (!database.TryGetLine(suffix, out line))
			{
				Debug.LogError("Unable to find suffix: " + suffix);
				return CassieInterpreter.NextAction.Continue;
			}
			float? spacingOverride = modifiers.SpacingOverride;
			float? maxDuration = modifiers.MaxDuration;
			double num3 = modifiers.GetTimeUntilNextWord(cassieLine) + (double)num;
			double num4 = num3 + (double)num2;
			modifiers.SpacingOverride = new float?((float)num3);
			modifiers.MaxDuration = new float?((float)num4);
			lines.Add(new CassieInterpreter.Result(cassieLine, modifiers));
			modifiers.SpacingOverride = spacingOverride;
			modifiers.MaxDuration = maxDuration;
			lines.Add(new CassieInterpreter.Result(line, modifiers));
			if (subtitleBuilder != null)
			{
				subtitleBuilder.Append(word);
			}
			if (subtitleBuilder != null)
			{
				subtitleBuilder.Append(' ');
			}
			modifiers.ResetSingleUseModifiers();
			return CassieInterpreter.NextAction.Halt;
		}

		// Token: 0x06003E49 RID: 15945
		protected abstract bool TryFindLemma(ReadOnlySpan<char> word, CassieLineDatabase db, out CassieLine originalLine);

		// Token: 0x06003E4A RID: 15946
		protected abstract string GetSuffix(ReadOnlySpan<char> promptedWord, CassieLine originalLine, out float spaceOffset, out float cutoffOffset);
	}
}
