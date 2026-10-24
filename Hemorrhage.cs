using System;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;

namespace CustomPlayerEffects
{
	// Token: 0x020008BD RID: 2237
	public class Hemorrhage : TickingEffectBase
	{
		// Token: 0x060032D3 RID: 13011 RVA: 0x000A947C File Offset: 0x000A767C
		protected override void OnTick()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (this._isSprinting)
			{
				float damage = this.damagePerTick * RainbowTaste.CurrentMultiplier(base.Hub);
				base.Hub.playerStats.DealDamage(new UniversalDamageHandler(damage, DeathTranslations.Bleeding, null));
			}
		}

		// Token: 0x060032D4 RID: 13012 RVA: 0x000A94CC File Offset: 0x000A76CC
		protected override void OnEffectUpdate()
		{
			base.OnEffectUpdate();
			IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole == null)
			{
				return;
			}
			this._isSprinting = (fpcRole.FpcModule.CurrentMovementState == PlayerMovementState.Sprinting);
		}

		// Token: 0x04002765 RID: 10085
		public float damagePerTick = 1f;

		// Token: 0x04002766 RID: 10086
		private bool _isSprinting;
	}
}
