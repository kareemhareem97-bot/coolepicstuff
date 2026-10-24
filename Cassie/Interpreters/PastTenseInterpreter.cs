using System;

namespace Cassie.Interpreters
{
	// Token: 0x02000ACC RID: 2764
	public class PastTenseInterpreter : SuffixInterpreterBase
	{
		// Token: 0x06003E42 RID: 15938 RVA: 0x000CDFE4 File Offset: 0x000CC1E4
		protected override bool TryFindLemma(ReadOnlySpan<char> word, CassieLineDatabase db, out CassieLine originalLine)
		{
			if (word.EndsWith("ED".AsSpan()))
			{
				ReadOnlySpan<char> readOnlySpan = word;
				if (db.TryGetLine(readOnlySpan.Slice(0, readOnlySpan.Length - 2), out originalLine))
				{
					return true;
				}
				if (word.Length > 3)
				{
					readOnlySpan = word;
					if (db.TryGetLine(readOnlySpan.Slice(0, readOnlySpan.Length - 3), out originalLine))
					{
						return true;
					}
				}
			}
			if (word.EndsWith("D".AsSpan()))
			{
				ReadOnlySpan<char> readOnlySpan = word;
				if (db.TryGetLine(readOnlySpan.Slice(0, readOnlySpan.Length - 1), out originalLine))
				{
					return true;
				}
			}
			if (word.EndsWith("IED".AsSpan()))
			{
				ReadOnlySpan<char> readOnlySpan = word;
				string text = readOnlySpan.Slice(0, readOnlySpan.Length - 3).ToString() + "Y";
				if (db.TryGetLine(text.AsSpan(), out originalLine))
				{
					return true;
				}
			}
			originalLine = null;
			return false;
		}

		// Token: 0x06003E43 RID: 15939 RVA: 0x000CE0CC File Offset: 0x000CC2CC
		protected override string GetSuffix(ReadOnlySpan<char> word, CassieLine originalLine, out float spaceOffset, out float cutoffTime)
		{
			spaceOffset = 0f;
			cutoffTime = 0f;
			string apiName = originalLine.ApiName;
			if (apiName.EndsWith('T') || apiName.EndsWith('D'))
			{
				spaceOffset = -0.065f;
				cutoffTime = 0.075f;
				return "_SUFFIX_PAST_ID";
			}
			if (apiName.EndsWith("SH") || apiName.EndsWith("CH") || apiName.EndsWith("SS"))
			{
				return "_SUFFIX_PAST_T";
			}
			string text = apiName;
			char c = text[text.Length - 1];
			if (c != 'P' && c != 'K' && c != 'F' && c != 'S' && c != 'X')
			{
				return "_SUFFIX_PAST_D";
			}
			return "_SUFFIX_PAST_T";
		}

		// Token: 0x04002DA2 RID: 11682
		private const string SuffixT = "_SUFFIX_PAST_T";

		// Token: 0x04002DA3 RID: 11683
		private const string SuffixD = "_SUFFIX_PAST_D";

		// Token: 0x04002DA4 RID: 11684
		private const string SuffixId = "_SUFFIX_PAST_ID";
	}
}
