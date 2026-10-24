using System;

namespace Cassie.Interpreters
{
	// Token: 0x02000AC8 RID: 2760
	public class ContinuousInterpreter : SuffixInterpreterBase
	{
		// Token: 0x06003E33 RID: 15923 RVA: 0x000CDA0C File Offset: 0x000CBC0C
		protected unsafe override bool TryFindLemma(ReadOnlySpan<char> word, CassieLineDatabase db, out CassieLine originalLine)
		{
			if (word.Length <= 3 || !word.EndsWith("ING".AsSpan()))
			{
				originalLine = null;
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = word;
			ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(0, readOnlySpan.Length - 3);
			if (db.TryGetLine(readOnlySpan2, out originalLine))
			{
				return true;
			}
			if (*readOnlySpan2[readOnlySpan2.Length - 1] == *readOnlySpan2[readOnlySpan2.Length - 2])
			{
				readOnlySpan = readOnlySpan2;
				if (db.TryGetLine(readOnlySpan.Slice(0, readOnlySpan.Length - 1), out originalLine))
				{
					return true;
				}
			}
			ReadOnlySpan<char> word2 = (readOnlySpan2.ToString() + "E").AsSpan();
			if (db.TryGetLine(word2, out originalLine))
			{
				return true;
			}
			originalLine = null;
			return false;
		}

		// Token: 0x06003E34 RID: 15924 RVA: 0x000CDAC9 File Offset: 0x000CBCC9
		protected override string GetSuffix(ReadOnlySpan<char> word, CassieLine originalLine, out float spaceOffset, out float cutoffTime)
		{
			spaceOffset = -0.08f;
			cutoffTime = 0.08f;
			return "_SUFFIX_ING";
		}
	}
}
