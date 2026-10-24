using System;
using Mirror;

namespace Hints
{
	// Token: 0x0200080D RID: 2061
	public abstract class IdHintParameter : HintParameter
	{
		// Token: 0x170008D8 RID: 2264
		// (get) Token: 0x06002FA7 RID: 12199 RVA: 0x000A0558 File Offset: 0x0009E758
		// (set) Token: 0x06002FA8 RID: 12200 RVA: 0x000A0560 File Offset: 0x0009E760
		private protected int Id { protected get; private set; }

		// Token: 0x06002FA9 RID: 12201 RVA: 0x000A0569 File Offset: 0x0009E769
		protected IdHintParameter()
		{
		}

		// Token: 0x06002FAA RID: 12202 RVA: 0x000A0571 File Offset: 0x0009E771
		protected IdHintParameter(byte id)
		{
			this.Id = (int)id;
		}

		// Token: 0x06002FAB RID: 12203 RVA: 0x000A0580 File Offset: 0x0009E780
		public override void Deserialize(NetworkReader reader)
		{
			this.Id = reader.ReadInt();
		}

		// Token: 0x06002FAC RID: 12204 RVA: 0x000A058E File Offset: 0x0009E78E
		public override void Serialize(NetworkWriter writer)
		{
			writer.WriteInt(this.Id);
		}

		// Token: 0x06002FAD RID: 12205 RVA: 0x000A059C File Offset: 0x0009E79C
		protected override string UpdateState(float progress)
		{
			if (this._stopFormatting)
			{
				return null;
			}
			return this.FormatId(progress, out this._stopFormatting);
		}

		// Token: 0x06002FAE RID: 12206
		protected abstract string FormatId(float progress, out bool stopFormatting);

		// Token: 0x0400253B RID: 9531
		private bool _stopFormatting;
	}
}
