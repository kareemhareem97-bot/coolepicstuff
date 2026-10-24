using System;
using AudioPooling;
using Footprinting;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008A3 RID: 2211
	public class CardiacArrest : ParentEffectBase<SubEffectBase>, IHealableEffect, IStaminaModifier
	{
		// Token: 0x17000957 RID: 2391
		// (get) Token: 0x0600323A RID: 12858 RVA: 0x00004070 File Offset: 0x00002270
		public bool StaminaModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000958 RID: 2392
		// (get) Token: 0x0600323B RID: 12859 RVA: 0x000A85C6 File Offset: 0x000A67C6
		public float StaminaUsageMultiplier
		{
			get
			{
				return 3f;
			}
		}

		// Token: 0x17000959 RID: 2393
		// (get) Token: 0x0600323C RID: 12860 RVA: 0x0000408D File Offset: 0x0000228D
		public float StaminaRegenMultiplier
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x1700095A RID: 2394
		// (get) Token: 0x0600323D RID: 12861 RVA: 0x00004094 File Offset: 0x00002294
		public bool SprintingDisabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700095B RID: 2395
		// (get) Token: 0x0600323E RID: 12862 RVA: 0x000A84BF File Offset: 0x000A66BF
		public override bool AllowEnabling
		{
			get
			{
				return !SpawnProtected.CheckPlayer(base.Hub);
			}
		}

		// Token: 0x0600323F RID: 12863 RVA: 0x000A85D0 File Offset: 0x000A67D0
		protected override void Enabled()
		{
			base.Enabled();
			if (base.Hub.isLocalPlayer || base.Hub.IsLocallySpectated())
			{
				this._dyingSoundSession = new AudioPoolSession(AudioSourcePoolManager.Play2D(this._dyingSoundEffect, 1f, MixerChannel.DefaultSfx, 1f));
			}
			if (!NetworkServer.active)
			{
				return;
			}
			this._timeTillTick = 0f;
		}

		// Token: 0x06003240 RID: 12864 RVA: 0x000A8631 File Offset: 0x000A6831
		protected override void Disabled()
		{
			base.Disabled();
			this._attacker = default(Footprint);
		}

		// Token: 0x06003241 RID: 12865 RVA: 0x000A8645 File Offset: 0x000A6845
		public void SetAttacker(ReferenceHub ply)
		{
			this._attacker = new Footprint(ply);
		}

		// Token: 0x06003242 RID: 12866 RVA: 0x000A8653 File Offset: 0x000A6853
		public bool IsHealable(ItemType it)
		{
			return it == ItemType.SCP500 || it == ItemType.Adrenaline;
		}

		// Token: 0x06003243 RID: 12867 RVA: 0x000A8661 File Offset: 0x000A6861
		protected override void OnEffectUpdate()
		{
			if (NetworkServer.active)
			{
				this.ServerUpdate();
			}
			this.UpdateSubEffects();
		}

		// Token: 0x06003244 RID: 12868 RVA: 0x000A8676 File Offset: 0x000A6876
		public override void OnStopSpectating()
		{
			base.OnStopSpectating();
			if (!this._dyingSoundSession.SameSession)
			{
				return;
			}
			this._dyingSoundSession.Source.Stop();
		}

		// Token: 0x06003245 RID: 12869 RVA: 0x000A869C File Offset: 0x000A689C
		private void ServerUpdate()
		{
			this._timeTillTick -= Time.deltaTime;
			if (this._timeTillTick > 0f)
			{
				return;
			}
			this._timeTillTick += this.TimeBetweenTicks;
			base.Hub.playerStats.DealDamage(new Scp049DamageHandler(this._attacker, 8f, Scp049DamageHandler.AttackType.CardiacArrest));
		}

		// Token: 0x0400272C RID: 10028
		private const float SprintStaminaUsage = 3f;

		// Token: 0x0400272D RID: 10029
		private const float DamagePerTick = 8f;

		// Token: 0x0400272E RID: 10030
		private Footprint _attacker;

		// Token: 0x0400272F RID: 10031
		[SerializeField]
		private AudioClip _dyingSoundEffect;

		// Token: 0x04002730 RID: 10032
		[Tooltip("Used to track intervals/timers/etc without every effect needing to redefine a unique float.")]
		public float TimeBetweenTicks;

		// Token: 0x04002731 RID: 10033
		private float _timeTillTick;

		// Token: 0x04002732 RID: 10034
		private AudioPoolSession _dyingSoundSession;
	}
}
