using System;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008B6 RID: 2230
	public class Exhausted : StatusEffectBase, IStaminaModifier
	{
		// Token: 0x1700098D RID: 2445
		// (get) Token: 0x060032B2 RID: 12978 RVA: 0x00004070 File Offset: 0x00002270
		public bool StaminaModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x1700098E RID: 2446
		// (get) Token: 0x060032B3 RID: 12979 RVA: 0x0000408D File Offset: 0x0000228D
		public float StaminaUsageMultiplier
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x1700098F RID: 2447
		// (get) Token: 0x060032B4 RID: 12980 RVA: 0x00004094 File Offset: 0x00002294
		public bool SprintingDisabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000990 RID: 2448
		// (get) Token: 0x060032B5 RID: 12981 RVA: 0x000A9230 File Offset: 0x000A7430
		public float StaminaRegenMultiplier
		{
			get
			{
				if (this.CurStamina >= 0.5f)
				{
					return 0f;
				}
				return 0.5f;
			}
		}

		// Token: 0x17000991 RID: 2449
		// (get) Token: 0x060032B6 RID: 12982 RVA: 0x000A924A File Offset: 0x000A744A
		// (set) Token: 0x060032B7 RID: 12983 RVA: 0x000A925D File Offset: 0x000A745D
		private float CurStamina
		{
			get
			{
				this.PrepCache();
				return this._staminaCache.CurValue;
			}
			set
			{
				this.PrepCache();
				this._staminaCache.CurValue = value;
			}
		}

		// Token: 0x060032B8 RID: 12984 RVA: 0x000A9271 File Offset: 0x000A7471
		private void PrepCache()
		{
			if (this._cacheSet)
			{
				return;
			}
			this._cacheSet = true;
			this._staminaCache = base.Hub.playerStats.GetModule<StaminaStat>();
		}

		// Token: 0x060032B9 RID: 12985 RVA: 0x000A9299 File Offset: 0x000A7499
		protected override void Enabled()
		{
			base.Enabled();
			this.CurStamina = Mathf.Min(this.CurStamina, 0.5f);
		}

		// Token: 0x04002760 RID: 10080
		private const float MaxStamina = 0.5f;

		// Token: 0x04002761 RID: 10081
		private const float StaminaRegenSpeed = 0.5f;

		// Token: 0x04002762 RID: 10082
		private StaminaStat _staminaCache;

		// Token: 0x04002763 RID: 10083
		private bool _cacheSet;
	}
}
