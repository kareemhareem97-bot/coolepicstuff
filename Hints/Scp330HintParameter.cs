using System;
using InventorySystem.Items.Usables.Scp330;
using Mirror;

namespace Hints
{
	// Token: 0x0200080B RID: 2059
	public class Scp330HintParameter : IdHintParameter
	{
		// Token: 0x06002FA1 RID: 12193 RVA: 0x000A0528 File Offset: 0x0009E728
		public static Scp330HintParameter FromNetwork(NetworkReader reader)
		{
			Scp330HintParameter scp330HintParameter = new Scp330HintParameter();
			scp330HintParameter.Deserialize(reader);
			return scp330HintParameter;
		}

		// Token: 0x06002FA2 RID: 12194 RVA: 0x000A04C8 File Offset: 0x0009E6C8
		private Scp330HintParameter()
		{
		}

		// Token: 0x06002FA3 RID: 12195 RVA: 0x000A0536 File Offset: 0x0009E736
		public Scp330HintParameter(Scp330Translations.Entry index) : base((byte)index)
		{
		}

		// Token: 0x06002FA4 RID: 12196 RVA: 0x000A0540 File Offset: 0x0009E740
		protected override string FormatId(float progress, out bool stopFormatting)
		{
			stopFormatting = true;
			return Scp330Translations.GetEntryTranslation((Scp330Translations.Entry)base.Id);
		}
	}
}
