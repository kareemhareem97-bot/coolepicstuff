using System;
using PlayerStatsSystem;
using RemoteAdmin.Interfaces;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008D3 RID: 2259
	public class Scp1509Resurrected : StatusEffectBase, ICustomDisplayName, IDamageModifierEffect
	{
		// Token: 0x170009DB RID: 2523
		// (get) Token: 0x0600338D RID: 13197 RVA: 0x000AAD30 File Offset: 0x000A8F30
		public string DisplayName
		{
			get
			{
				return "SCP-1509 Resurrected";
			}
		}

		// Token: 0x170009DC RID: 2524
		// (get) Token: 0x0600338E RID: 13198 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool CanBeDisplayed
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170009DD RID: 2525
		// (get) Token: 0x0600338F RID: 13199 RVA: 0x00004094 File Offset: 0x00002294
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Technical;
			}
		}

		// Token: 0x170009DE RID: 2526
		// (get) Token: 0x06003390 RID: 13200 RVA: 0x000AAD37 File Offset: 0x000A8F37
		private bool IsImmunityActive
		{
			get
			{
				return this._lastSaveTime + 3.0 >= (double)Time.timeSinceLevelLoad;
			}
		}

		// Token: 0x170009DF RID: 2527
		// (get) Token: 0x06003391 RID: 13201 RVA: 0x000AAD54 File Offset: 0x000A8F54
		public bool DamageModifierActive
		{
			get
			{
				return this.IsImmunityActive;
			}
		}

		// Token: 0x06003392 RID: 13202 RVA: 0x000AAD5C File Offset: 0x000A8F5C
		protected override void Enabled()
		{
			base.Hub.playerStats.TryGetModule<HealthStat>(out this._hpStat);
			base.Hub.playerStats.TryGetModule<AhpStat>(out this._ahpStat);
			this._hpStat.MaxValue = 75f;
			this._ahpStat.MaxValue = 25f;
			this._ahpStat.ServerAddProcess(25f, 25f, 0f, 0.7f, 0f, false);
			this._lastSaveTime = (double)Time.timeSinceLevelLoad;
		}

		// Token: 0x06003393 RID: 13203 RVA: 0x000AADEC File Offset: 0x000A8FEC
		protected override void OnEffectUpdate()
		{
			if (this._ahpStat.MaxValue != 25f)
			{
				this._ahpStat.MaxValue = 25f;
			}
			if (this._hpStat.MaxValue != 75f)
			{
				this._hpStat.MaxValue = 75f;
			}
		}

		// Token: 0x06003394 RID: 13204 RVA: 0x000AAE40 File Offset: 0x000A9040
		public float GetDamageModifier(float baseDamage, DamageHandlerBase handler, HitboxType hitboxType)
		{
			StandardDamageHandler standardDamageHandler = handler as StandardDamageHandler;
			if (standardDamageHandler != null && standardDamageHandler.Damage == -1f)
			{
				return 1f;
			}
			if (this.IsImmunityActive)
			{
				return 0f;
			}
			if (base.IsEnabled)
			{
				return 0f;
			}
			return 1f;
		}

		// Token: 0x040027C4 RID: 10180
		private const float DamageImmunityTime = 3f;

		// Token: 0x040027C5 RID: 10181
		private AhpStat _ahpStat;

		// Token: 0x040027C6 RID: 10182
		private HealthStat _hpStat;

		// Token: 0x040027C7 RID: 10183
		private double _lastSaveTime;
	}
}
