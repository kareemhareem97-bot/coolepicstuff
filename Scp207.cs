using System;
using System.Collections.Generic;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using RemoteAdmin.Interfaces;

namespace CustomPlayerEffects
{
	// Token: 0x020008AD RID: 2221
	public class Scp207 : CokeBase<Scp207Stack>, ISpectatorDataPlayerEffect, IStaminaModifier, ICustomDisplayName
	{
		// Token: 0x17000971 RID: 2417
		// (get) Token: 0x06003280 RID: 12928 RVA: 0x000A8ED2 File Offset: 0x000A70D2
		public override Dictionary<PlayerMovementState, float> StateMultipliers { get; }

		// Token: 0x17000972 RID: 2418
		// (get) Token: 0x06003281 RID: 12929 RVA: 0x00034727 File Offset: 0x00032927
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Mixed;
			}
		}

		// Token: 0x17000973 RID: 2419
		// (get) Token: 0x06003282 RID: 12930 RVA: 0x000A8EDC File Offset: 0x000A70DC
		public override float MovementSpeedMultiplier
		{
			get
			{
				return base.CurrentStack.SpeedMultiplier;
			}
		}

		// Token: 0x17000974 RID: 2420
		// (get) Token: 0x06003283 RID: 12931 RVA: 0x000A8EF7 File Offset: 0x000A70F7
		public string DisplayName
		{
			get
			{
				return "SCP-207";
			}
		}

		// Token: 0x17000975 RID: 2421
		// (get) Token: 0x06003284 RID: 12932 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool CanBeDisplayed
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000976 RID: 2422
		// (get) Token: 0x06003285 RID: 12933 RVA: 0x00004070 File Offset: 0x00002270
		public bool StaminaModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000977 RID: 2423
		// (get) Token: 0x06003286 RID: 12934 RVA: 0x0000548E File Offset: 0x0000368E
		public float StaminaUsageMultiplier
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000978 RID: 2424
		// (get) Token: 0x06003287 RID: 12935 RVA: 0x0000408D File Offset: 0x0000228D
		public float StaminaRegenMultiplier
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000979 RID: 2425
		// (get) Token: 0x06003288 RID: 12936 RVA: 0x00004094 File Offset: 0x00002294
		public bool SprintingDisabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003289 RID: 12937 RVA: 0x000A8EFE File Offset: 0x000A70FE
		public bool GetSpectatorText(out string s)
		{
			s = ((base.Intensity > 1) ? string.Format("SCP-207 (x{0})", base.Intensity) : "SCP-207");
			return true;
		}

		// Token: 0x0600328A RID: 12938 RVA: 0x000A8F28 File Offset: 0x000A7128
		protected override void Enabled()
		{
			base.Enabled();
			if (!NetworkServer.active)
			{
				return;
			}
			StaminaStat staminaStat;
			if (base.Hub.playerStats.TryGetModule<StaminaStat>(out staminaStat))
			{
				staminaStat.CurValue = staminaStat.MaxValue;
			}
		}

		// Token: 0x0600328B RID: 12939 RVA: 0x000A8F64 File Offset: 0x000A7164
		protected override void OnTick()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (Vitality.CheckPlayer(base.Hub))
			{
				return;
			}
			float damage = base.CurrentStack.DamageAmount * base.GetMovementStateMultiplier();
			base.Hub.playerStats.DealDamage(new UniversalDamageHandler(damage, DeathTranslations.Scp207, null));
		}

		// Token: 0x0600328C RID: 12940 RVA: 0x000A8FB8 File Offset: 0x000A71B8
		public Scp207()
		{
			Dictionary<PlayerMovementState, float> dictionary = new Dictionary<PlayerMovementState, float>();
			dictionary[PlayerMovementState.Sprinting] = 1f;
			dictionary[PlayerMovementState.Walking] = 0.6f;
			dictionary[PlayerMovementState.Sneaking] = 0.15f;
			dictionary[PlayerMovementState.Crouching] = 0.1f;
			this.StateMultipliers = dictionary;
			base..ctor();
		}
	}
}
