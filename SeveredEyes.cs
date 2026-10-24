using System;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008D7 RID: 2263
	public class SeveredEyes : TickingEffectBase
	{
		// Token: 0x170009EE RID: 2542
		// (get) Token: 0x060033BA RID: 13242 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool AllowEnabling
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060033BB RID: 13243 RVA: 0x000AB359 File Offset: 0x000A9559
		protected override void OnTick()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			base.Hub.playerStats.DealDamage(new UniversalDamageHandler(this._damagePerTick, DeathTranslations.Scp1344, null));
		}

		// Token: 0x040027DE RID: 10206
		[SerializeField]
		private float _damagePerTick;
	}
}
