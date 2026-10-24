using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000802 RID: 2050
	public class TextHint : FormattableHint<TextHint>
	{
		// Token: 0x06002F7A RID: 12154 RVA: 0x000A0203 File Offset: 0x0009E403
		public static TextHint FromNetwork(NetworkReader reader)
		{
			TextHint textHint = new TextHint();
			textHint.Deserialize(reader);
			return textHint;
		}

		// Token: 0x170008D6 RID: 2262
		// (get) Token: 0x06002F7B RID: 12155 RVA: 0x000A0211 File Offset: 0x0009E411
		// (set) Token: 0x06002F7C RID: 12156 RVA: 0x000A0219 File Offset: 0x0009E419
		private protected string Text { protected get; private set; }

		// Token: 0x06002F7D RID: 12157 RVA: 0x000A0222 File Offset: 0x0009E422
		private TextHint() : base(null, null, 0f)
		{
		}

		// Token: 0x06002F7E RID: 12158 RVA: 0x000A0231 File Offset: 0x0009E431
		public TextHint(string text, HintParameter[] parameters = null, HintEffect[] effects = null, float durationScalar = 3f) : base(parameters, effects, durationScalar)
		{
			this.Text = text;
		}

		// Token: 0x06002F7F RID: 12159 RVA: 0x000A0244 File Offset: 0x0009E444
		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			this.Text = reader.ReadString();
		}

		// Token: 0x06002F80 RID: 12160 RVA: 0x000A0259 File Offset: 0x0009E459
		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.WriteString(this.Text);
		}
	}
}
