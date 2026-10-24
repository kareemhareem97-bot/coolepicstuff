using System;
using System.Text;
using UnityEngine;

namespace GameCore
{
	// Token: 0x0200085A RID: 2138
	public readonly struct ConsoleLog
	{
		// Token: 0x060030F2 RID: 12530 RVA: 0x000A448A File Offset: 0x000A268A
		public ConsoleLog(string text, Color color, bool nospace)
		{
			this.Text = text;
			this.Color = color;
			this.NoSpace = nospace;
		}

		// Token: 0x17000907 RID: 2311
		// (get) Token: 0x060030F3 RID: 12531 RVA: 0x000A44A1 File Offset: 0x000A26A1
		public int FormattedTextLength
		{
			get
			{
				return 25 + this.Text.Length;
			}
		}

		// Token: 0x060030F4 RID: 12532 RVA: 0x000A44B4 File Offset: 0x000A26B4
		public void AppendFormatted(StringBuilder sb)
		{
			sb.Append(this.NoSpace ? "\n" : "\n\n");
			sb.Append("<color=").Append(this.Color.ToHex()).Append('>');
			sb.Append(this.Text);
			sb.Append("</color>");
		}

		// Token: 0x040025D7 RID: 9687
		private const int FormattedTextLengthBase = 25;

		// Token: 0x040025D8 RID: 9688
		public readonly string Text;

		// Token: 0x040025D9 RID: 9689
		public readonly Color Color;

		// Token: 0x040025DA RID: 9690
		public readonly bool NoSpace;
	}
}
