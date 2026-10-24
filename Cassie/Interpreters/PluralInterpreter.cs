using System;

namespace Cassie.Interpreters
{
	// Token: 0x02000ACD RID: 2765
	public class PluralInterpreter : SuffixInterpreterBase
	{
		// Token: 0x06003E45 RID: 15941 RVA: 0x000CE184 File Offset: 0x000CC384
		protected override bool TryFindLemma(ReadOnlySpan<char> word, CassieLineDatabase db, out CassieLine originalLine)
		{
			if (word.EndsWith("ies"))
			{
				ReadOnlySpan<char> readOnlySpan = word;
				string text = readOnlySpan.Slice(0, readOnlySpan.Length - 3).ToString() + "y";
				if (db.TryGetLine(text.AsSpan(), out originalLine))
				{
					return true;
				}
			}
			if (word.EndsWith("es".AsSpan()))
			{
				ReadOnlySpan<char> readOnlySpan = word;
				if (db.TryGetLine(readOnlySpan.Slice(0, readOnlySpan.Length - 2), out originalLine))
				{
					return true;
				}
			}
			if (word.EndsWith("s".AsSpan()))
			{
				ReadOnlySpan<char> readOnlySpan = word;
				if (db.TryGetLine(readOnlySpan.Slice(0, readOnlySpan.Length - 1), out originalLine))
				{
					return true;
				}
			}
			originalLine = null;
			return false;
		}

		// Token: 0x06003E46 RID: 15942 RVA: 0x000CE244 File Offset: 0x000CC444
		protected override string GetSuffix(ReadOnlySpan<char> word, CassieLine originalLine, out float spaceOffset, out float cutoffTime)
		{
			spaceOffset = 0f;
			cutoffTime = 0f;
			string apiName = originalLine.ApiName;
			string text = apiName;
			char c = text[text.Length - 1];
			if (c != 's' && c != 'x' && c != 'z' && !apiName.EndsWith("ss") && !apiName.EndsWith("sh") && !apiName.EndsWith("ch") && !apiName.EndsWith("ge") && !apiName.EndsWith("ce") && !apiName.EndsWith("dge"))
			{
				return "_suffix_plural_regular";
			}
			return "_suffix_plural_syllabic";
		}

		// Token: 0x04002DA5 RID: 11685
		private const string SuffixRegular = "_suffix_plural_regular";

		// Token: 0x04002DA6 RID: 11686
		private const string SuffixSyllabic = "_suffix_plural_syllabic";
	}
}
