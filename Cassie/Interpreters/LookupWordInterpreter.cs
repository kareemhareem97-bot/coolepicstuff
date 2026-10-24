using System;
using System.Collections.Generic;
using System.Text;

namespace Cassie.Interpreters
{
	// Token: 0x02000ACA RID: 2762
	public class LookupWordInterpreter : CassieInterpreter
	{
		// Token: 0x06003E3A RID: 15930 RVA: 0x000CDC9C File Offset: 0x000CBE9C
		protected unsafe override CassieInterpreter.NextAction ProcessWord(ReadOnlySpan<char> word, CassieLineDatabase database, ref CassiePlaybackModifiers modifiers, StringBuilder subtitleBuilder, List<CassieInterpreter.Result> lines)
		{
			bool flag = false;
			CassieLine cassieLine;
			if (!database.TryGetLine(word, out cassieLine))
			{
				if (word.IsEmpty)
				{
					return CassieInterpreter.NextAction.Continue;
				}
				char c = (char)(*word[word.Length - 1]);
				if (c != '.' && c != ',')
				{
					return CassieInterpreter.NextAction.Continue;
				}
				ReadOnlySpan<char> readOnlySpan = word;
				if (!database.TryGetLine(readOnlySpan.Slice(0, readOnlySpan.Length - 1), out cassieLine))
				{
					return CassieInterpreter.NextAction.Continue;
				}
				flag = true;
			}
			lines.Add(new CassieInterpreter.Result(cassieLine, modifiers));
			modifiers.ResetSingleUseModifiers();
			CassieLine line;
			if (flag && database.TryGetLine(".".AsSpan(), out line))
			{
				lines.Add(new CassieInterpreter.Result(line, modifiers));
			}
			if (subtitleBuilder != null && cassieLine.Type == CassieClipCategory.Word)
			{
				subtitleBuilder.Append(word);
				subtitleBuilder.Append(' ');
			}
			return CassieInterpreter.NextAction.Halt;
		}
	}
}
