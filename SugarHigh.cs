using System;
using MapGeneration.Holidays;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008E5 RID: 2277
	public class SugarHigh : TickingEffectBase, ISoundtrackMutingEffect, IHolidayEffect
	{
		// Token: 0x17000A2C RID: 2604
		// (get) Token: 0x06003435 RID: 13365 RVA: 0x00004070 File Offset: 0x00002270
		public bool MuteSoundtrack
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A2D RID: 2605
		// (get) Token: 0x06003436 RID: 13366 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x17000A2E RID: 2606
		// (get) Token: 0x06003437 RID: 13367 RVA: 0x000AC2E2 File Offset: 0x000AA4E2
		public HolidayType[] TargetHolidays { get; } = new HolidayType[]
		{
			HolidayType.Halloween
		};

		// Token: 0x06003438 RID: 13368 RVA: 0x000AC2EC File Offset: 0x000AA4EC
		protected override void Enabled()
		{
			base.Enabled();
			if (!(base.Hub.roleManager.CurrentRole is HumanRole))
			{
				return;
			}
			this.UpdateSourceMute();
			this._soundSource.Play();
			if (!NetworkServer.active)
			{
				return;
			}
			this._healthStat = base.Hub.playerStats.GetModule<HealthStat>();
			if (this._beingDisabled)
			{
				this.ServerResetRemaining();
			}
			this._cachedMaxHealth = this._healthStat.MaxValue;
			this._healthStat.MaxValue += 200f;
			this._healthStat.ServerHeal(200f);
			PlayerRoleManager.OnRoleChanged += this.OnRoleChanged;
		}

		// Token: 0x06003439 RID: 13369 RVA: 0x000AC3A0 File Offset: 0x000AA5A0
		protected override void Disabled()
		{
			this._soundSource.Stop();
			if (!NetworkServer.active)
			{
				return;
			}
			PlayerRoleManager.OnRoleChanged -= this.OnRoleChanged;
			if (!(base.Hub.roleManager.CurrentRole is HumanRole))
			{
				return;
			}
			this._beingDisabled = true;
			this._disabledCounter = 0f;
		}

		// Token: 0x0600343A RID: 13370 RVA: 0x000AC3FB File Offset: 0x000AA5FB
		protected override void OnEffectUpdate()
		{
			base.OnEffectUpdate();
			this.UpdateSourceMute();
		}

		// Token: 0x0600343B RID: 13371 RVA: 0x000AC409 File Offset: 0x000AA609
		private void UpdateSourceMute()
		{
			this._soundSource.mute = (!base.IsLocalPlayer && !base.IsSpectated);
		}

		// Token: 0x0600343C RID: 13372 RVA: 0x000AC42A File Offset: 0x000AA62A
		protected override void OnTick()
		{
			if (!NetworkServer.active || this._healthStat == null)
			{
				return;
			}
			if (this._healthStat.CurValue >= 100f)
			{
				this._healthStat.ServerHeal(6f);
			}
		}

		// Token: 0x0600343D RID: 13373 RVA: 0x000AC45E File Offset: 0x000AA65E
		private void OnRoleChanged(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
		{
			if (!NetworkServer.active || hub != base.Hub)
			{
				return;
			}
			this._beingDisabled = false;
		}

		// Token: 0x0600343E RID: 13374 RVA: 0x000AC480 File Offset: 0x000AA680
		private void ServerResetRemaining()
		{
			if (!NetworkServer.active || !this._beingDisabled)
			{
				return;
			}
			float num = 1f - this._disabledCounter / 10f;
			float num2 = 200f * num;
			this._healthStat.MaxValue -= num2;
			this._healthStat.MaxValue = Mathf.Max(this._cachedMaxHealth, this._healthStat.MaxValue);
			this._healthStat.CurValue = Mathf.Min(this._healthStat.CurValue, this._healthStat.MaxValue);
			this._beingDisabled = false;
			this._disabledCounter = 0f;
		}

		// Token: 0x0600343F RID: 13375 RVA: 0x000AC524 File Offset: 0x000AA724
		private void LateUpdate()
		{
			if (!NetworkServer.active || !this._beingDisabled)
			{
				return;
			}
			if (this._healthStat == null)
			{
				return;
			}
			float num = 20f * Time.deltaTime;
			float a = this._healthStat.MaxValue - num;
			this._healthStat.MaxValue = Mathf.Max(a, this._cachedMaxHealth);
			this._disabledCounter += Time.deltaTime;
			if (this._disabledCounter < 10f)
			{
				return;
			}
			this._beingDisabled = false;
		}

		// Token: 0x04002815 RID: 10261
		private const float PassiveRegenHealthThreshold = 100f;

		// Token: 0x04002816 RID: 10262
		private const float MaxHealthAddition = 200f;

		// Token: 0x04002817 RID: 10263
		private const float HealthRegen = 6f;

		// Token: 0x04002818 RID: 10264
		private const float DisableTime = 10f;

		// Token: 0x0400281A RID: 10266
		[SerializeField]
		private AudioSource _soundSource;

		// Token: 0x0400281B RID: 10267
		private HealthStat _healthStat;

		// Token: 0x0400281C RID: 10268
		private float _disabledCounter;

		// Token: 0x0400281D RID: 10269
		private float _cachedMaxHealth;

		// Token: 0x0400281E RID: 10270
		private bool _beingDisabled;
	}
}
