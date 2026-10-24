using System;
using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects.Danger
{
	// Token: 0x02000918 RID: 2328
	public class PlayerDamagedDanger : ParentDangerBase
	{
		// Token: 0x0600351B RID: 13595 RVA: 0x000ADDB2 File Offset: 0x000ABFB2
		public override void Initialize(ReferenceHub target)
		{
			base.Initialize(target);
			this._leftoverDamage = 0f;
			PlayerStats.OnAnyPlayerDamaged += this.UpdateState;
		}

		// Token: 0x0600351C RID: 13596 RVA: 0x000ADDD7 File Offset: 0x000ABFD7
		public override void Dispose()
		{
			base.Dispose();
			PlayerStats.OnAnyPlayerDamaged -= this.UpdateState;
		}

		// Token: 0x0600351D RID: 13597 RVA: 0x000ADDF0 File Offset: 0x000ABFF0
		private void UpdateState(ReferenceHub damagedHub, DamageHandlerBase damageHandler)
		{
			if (damagedHub != base.Owner)
			{
				return;
			}
			if (damageHandler is Scp049DamageHandler)
			{
				return;
			}
			AttackerDamageHandler attackerDamageHandler = damageHandler as AttackerDamageHandler;
			if (attackerDamageHandler == null)
			{
				UniversalDamageHandler universalDamageHandler = damageHandler as UniversalDamageHandler;
				if (universalDamageHandler != null)
				{
					if (universalDamageHandler.TranslationId == DeathTranslations.PocketDecay.Id)
					{
						return;
					}
				}
			}
			else if (attackerDamageHandler.Attacker.Hub.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Scp106)
			{
				return;
			}
			StandardDamageHandler standardDamageHandler = damageHandler as StandardDamageHandler;
			if (standardDamageHandler == null)
			{
				return;
			}
			float num = Mathf.Floor((standardDamageHandler.DealtHealthDamage + this._leftoverDamage) / 10f) * 0.25f;
			if (standardDamageHandler.DealtHealthDamage % 10f + this._leftoverDamage >= 10f)
			{
				this._leftoverDamage = 0f;
			}
			this._leftoverDamage += standardDamageHandler.DealtHealthDamage % 10f;
			if (num < 0.25f)
			{
				return;
			}
			base.ChildDangers.Add(new ExpiringDanger(num, base.Owner));
		}

		// Token: 0x04002866 RID: 10342
		private const int DangerStepThreshold = 10;

		// Token: 0x04002867 RID: 10343
		private const float DangerPerThreshold = 0.25f;

		// Token: 0x04002868 RID: 10344
		private float _leftoverDamage;
	}
}
