using System;

namespace Cassie.Interpreters
{
	// Token: 0x02000AC9 RID: 2761
	public class DegreeInterpreter : SuffixInterpreterBase
	{
		// Token: 0x06003E36 RID: 15926 RVA: 0x000CDAE8 File Offset: 0x000CBCE8
		protected override bool TryFindLemma(ReadOnlySpan<char> word, CassieLineDatabase db, out CassieLine originalLine)
		{
			if (word.Length <= 3)
			{
				originalLine = null;
				return false;
			}
			if (DegreeInterpreter.TryFindLemmaForDegree(word, db, "ER", "IER", out originalLine))
			{
				return true;
			}
			if (DegreeInterpreter.TryFindLemmaForDegree(word, db, "EST", "IEST", out originalLine))
			{
				return true;
			}
			originalLine = null;
			return false;
		}

		// Token: 0x06003E37 RID: 15927 RVA: 0x000CDB34 File Offset: 0x000CBD34
		private unsafe static bool TryFindLemmaForDegree(ReadOnlySpan<char> word, CassieLineDatabase db, string suffix, string ySuffix, out CassieLine originalLine)
		{
			if (!word.EndsWith(suffix))
			{
				originalLine = null;
				return false;
			}
			ReadOnlySpan<char> readOnlySpan;
			int num;
			if (word.EndsWith(ySuffix) && word.Length > ySuffix.Length)
			{
				readOnlySpan = word;
				num = ySuffix.Length;
				string text = readOnlySpan.Slice(0, readOnlySpan.Length - num).ToString() + "Y";
				if (db.TryGetLine(text.AsSpan(), out originalLine))
				{
					return true;
				}
			}
			readOnlySpan = word;
			num = suffix.Length;
			ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(0, readOnlySpan.Length - num);
			if (readOnlySpan2.Length >= 2 && *readOnlySpan2[readOnlySpan2.Length - 1] == *readOnlySpan2[readOnlySpan2.Length - 2])
			{
				readOnlySpan = readOnlySpan2;
				if (db.TryGetLine(readOnlySpan.Slice(0, readOnlySpan.Length - 1), out originalLine))
				{
					return true;
				}
			}
			int num2 = suffix.Length - 1;
			if (word.Length > num2)
			{
				readOnlySpan = word;
				num = num2;
				if (db.TryGetLine(readOnlySpan.Slice(0, readOnlySpan.Length - num), out originalLine))
				{
					return true;
				}
			}
			if (db.TryGetLine(readOnlySpan2, out originalLine))
			{
				return true;
			}
			originalLine = null;
			return false;
		}

		// Token: 0x06003E38 RID: 15928 RVA: 0x000CDC6C File Offset: 0x000CBE6C
		protected override string GetSuffix(ReadOnlySpan<char> word, CassieLine originalLine, out float spaceOffset, out float cutoffTime)
		{
			spaceOffset = -0.08f;
			cutoffTime = 0.06f;
			if (!word.EndsWith("EST"))
			{
				return "_SUFFIX_ER";
			}
			return "_SUFFIX_EST";
		}

		// Token: 0x04002D94 RID: 11668
		private const string SuffixComparative = "_SUFFIX_ER";

		// Token: 0x04002D95 RID: 11669
		private const string SuffixSuperlative = "_SUFFIX_EST";
	}
}
