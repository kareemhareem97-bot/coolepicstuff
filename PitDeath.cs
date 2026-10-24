using System;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008C7 RID: 2247
	public class PitDeath : StatusEffectBase, IDamageModifierEffect
	{
		// Token: 0x170009BD RID: 2493
		// (get) Token: 0x06003329 RID: 13097 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool AllowEnabling
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170009BE RID: 2494
		// (get) Token: 0x0600332A RID: 13098 RVA: 0x00004070 File Offset: 0x00002270
		public bool DamageModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x0600332B RID: 13099 RVA: 0x000AA0F0 File Offset: 0x000A82F0
		public static bool ValidatePlayer(ReferenceHub hub)
		{
			PlayerRoleBase currentRole = hub.roleManager.CurrentRole;
			IFpcRole fpcRole = currentRole as IFpcRole;
			return fpcRole != null && !fpcRole.FpcModule.Noclip.IsActive && !hub.characterClassManager.GodMode && currentRole.ActiveTime >= 1f;
		}

		// Token: 0x0600332C RID: 13100 RVA: 0x000AA148 File Offset: 0x000A8348
		public float GetDamageModifier(float baseDamage, DamageHandlerBase handler, HitboxType hitboxType)
		{
			return (float)(this.IsFallDamageHandler(handler) ? 0 : 1);
		}

		// Token: 0x0600332D RID: 13101 RVA: 0x000AA158 File Offset: 0x000A8358
		protected override void Enabled()
		{
			base.Enabled();
			this._activeElapsed = 0f;
		}

		// Token: 0x0600332E RID: 13102 RVA: 0x000AA16B File Offset: 0x000A836B
		protected override void OnEffectUpdate()
		{
			base.OnEffectUpdate();
			if (!NetworkServer.active)
			{
				return;
			}
			this._activeElapsed += Time.deltaTime;
			this.CheckKillConditions();
		}

		// Token: 0x0600332F RID: 13103 RVA: 0x000AA194 File Offset: 0x000A8394
		private bool IsFallDamageHandler(DamageHandlerBase handler)
		{
			UniversalDamageHandler universalDamageHandler = handler as UniversalDamageHandler;
			return universalDamageHandler != null && universalDamageHandler.TranslationId == DeathTranslations.Falldown.Id;
		}

		// Token: 0x06003330 RID: 13104 RVA: 0x000AA1BF File Offset: 0x000A83BF
		private void CheckKillConditions()
		{
			if (!PitDeath.ValidatePlayer(base.Hub))
			{
				this.DisableEffect();
				return;
			}
			if (this._activeElapsed <= 1.2f && !base.Hub.IsGrounded())
			{
				return;
			}
			this.KillPlayer();
		}

		// Token: 0x06003331 RID: 13105 RVA: 0x000AA1F6 File Offset: 0x000A83F6
		private void KillPlayer()
		{
			base.Hub.playerStats.DealDamage(new UniversalDamageHandler(-1f, DeathTranslations.Crushed, null));
		}

		// Token: 0x04002791 RID: 10129
		private const float MinAliveDuration = 1f;

		// Token: 0x04002792 RID: 10130
		private const float FallbackMaxDelay = 1.2f;

		// Token: 0x04002793 RID: 10131
		private float _activeElapsed;
	}
}
