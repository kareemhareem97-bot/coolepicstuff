using System;
using System.Collections.Generic;
using System.Text;

namespace Cassie.Interpreters
{
	// Token: 0x02000AC5 RID: 2757
	public abstract class CassieInterpreter
	{
		// Token: 0x06003E2E RID: 15918 RVA: 0x000CD9BC File Offset: 0x000CBBBC
		public List<CassieInterpreter.Result> GetResults(CassieLineDatabase database, ref CassiePlaybackModifiers modifiers, ReadOnlySpan<char> word, StringBuilder subtitleBuilder, out bool halt)
		{
			CassieInterpreter.ResultNonAlloc.Clear();
			CassieInterpreter.NextAction nextAction = this.ProcessWord(word, database, ref modifiers, subtitleBuilder, CassieInterpreter.ResultNonAlloc);
			halt = (nextAction == CassieInterpreter.NextAction.Halt);
			return CassieInterpreter.ResultNonAlloc;
		}

		// Token: 0x06003E2F RID: 15919
		protected abstract CassieInterpreter.NextAction ProcessWord(ReadOnlySpan<char> word, CassieLineDatabase database, ref CassiePlaybackModifiers modifiers, StringBuilder subtitleBuilder, List<CassieInterpreter.Result> lines);

		// Token: 0x04002D8E RID: 11662
		private static readonly List<CassieInterpreter.Result> ResultNonAlloc = new List<CassieInterpreter.Result>();

		// Token: 0x02000AC6 RID: 2758
		protected enum NextAction
		{
			// Token: 0x04002D90 RID: 11664
			Continue,
			// Token: 0x04002D91 RID: 11665
			Halt
		}

		// Token: 0x02000AC7 RID: 2759
		public readonly struct Result
		{
			// Token: 0x06003E32 RID: 15922 RVA: 0x000CD9FC File Offset: 0x000CBBFC
			public Result(CassieLine line, CassiePlaybackModifiers modifiers)
			{
				this.Line = line;
				this.Modifiers = modifiers;
			}

			// Token: 0x04002D92 RID: 11666
			public readonly CassieLine Line;

			// Token: 0x04002D93 RID: 11667
			public readonly CassiePlaybackModifiers Modifiers;
		}
	}
}
