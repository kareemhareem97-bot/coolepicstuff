using System;

namespace CustomPlayerEffects
{
	// Token: 0x020008B2 RID: 2226
	public class Deafened : StatusEffectBase, IHealableEffect
	{
		// Token: 0x0600329E RID: 12958 RVA: 0x00004242 File Offset: 0x00002442
		public bool IsHealable(ItemType it)
		{
			return it == ItemType.SCP500;
		}

		// Token: 0x17000982 RID: 2434
		// (get) Token: 0x0600329F RID: 12959 RVA: 0x000A84BF File Offset: 0x000A66BF
		public override bool AllowEnabling
		{
			get
			{
				return !SpawnProtected.CheckPlayer(base.Hub);
			}
		}

		// Token: 0x060032A0 RID: 12960 RVA: 0x000A9118 File Offset: 0x000A7318
		private void OnDestroy()
		{
			base.Hub.playerEffectsController.mixer.SetFloat("MasterVolumeLowpassFreq", 22000f);
		}
	}
}
