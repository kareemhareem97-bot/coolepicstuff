using System;
using InventorySystem.Items.MarshmallowMan;
using MapGeneration.Holidays;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008E4 RID: 2276
	public class SugarCrave : StatusEffectBase, IStaminaModifier, IHolidayEffect
	{
		// Token: 0x17000A27 RID: 2599
		// (get) Token: 0x0600342B RID: 13355 RVA: 0x00004070 File Offset: 0x00002270
		public bool StaminaModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A28 RID: 2600
		// (get) Token: 0x0600342C RID: 13356 RVA: 0x0000548E File Offset: 0x0000368E
		public float StaminaUsageMultiplier
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000A29 RID: 2601
		// (get) Token: 0x0600342D RID: 13357 RVA: 0x0000548E File Offset: 0x0000368E
		public float StaminaRegenMultiplier
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000A2A RID: 2602
		// (get) Token: 0x0600342E RID: 13358 RVA: 0x000AC154 File Offset: 0x000AA354
		public HolidayType[] TargetHolidays { get; } = new HolidayType[]
		{
			HolidayType.Halloween,
			HolidayType.Christmas
		};

		// Token: 0x17000A2B RID: 2603
		// (get) Token: 0x0600342F RID: 13359 RVA: 0x000AC15C File Offset: 0x000AA35C
		private StaminaStat StaminaStat
		{
			get
			{
				return base.Hub.playerStats.GetModule<StaminaStat>();
			}
		}

		// Token: 0x06003430 RID: 13360 RVA: 0x000AC170 File Offset: 0x000AA370
		protected override void Enabled()
		{
			base.Enabled();
			if (!NetworkServer.active)
			{
				return;
			}
			AhpStat module = base.Hub.playerStats.GetModule<AhpStat>();
			module.ServerKillAllProcesses();
			module.ServerAddProcess(450f, 450f, 0f, 1f, 0f, false);
			StaminaStat staminaStat = this.StaminaStat;
			staminaStat.CurValue = staminaStat.MaxValue;
		}

		// Token: 0x06003431 RID: 13361 RVA: 0x000AC1D4 File Offset: 0x000AA3D4
		protected override void Disabled()
		{
			base.Disabled();
			if (!NetworkServer.active)
			{
				return;
			}
			base.Hub.playerStats.GetModule<AhpStat>().ServerKillAllProcesses();
			MarshmallowEffect effect = base.Hub.playerEffectsController.GetEffect<MarshmallowEffect>();
			byte intensity = effect.Intensity;
			if (intensity == 0 || intensity == 255)
			{
				return;
			}
			effect.Intensity = byte.MaxValue;
			effect.ServerChangeDuration(1.1f, false);
		}

		// Token: 0x06003432 RID: 13362 RVA: 0x000AC240 File Offset: 0x000AA440
		protected override void OnEffectUpdate()
		{
			base.OnEffectUpdate();
			if (!NetworkServer.active)
			{
				return;
			}
			StaminaStat staminaStat = this.StaminaStat;
			float num = staminaStat.CurValue - 0.06666667f * Time.deltaTime;
			if (num < 0f)
			{
				this.DisableEffect();
				return;
			}
			staminaStat.CurValue = num;
		}

		// Token: 0x06003433 RID: 13363 RVA: 0x000AC28C File Offset: 0x000AA48C
		public void OnKill()
		{
			if (!base.IsEnabled)
			{
				return;
			}
			StaminaStat staminaStat = this.StaminaStat;
			staminaStat.CurValue = Mathf.Min(staminaStat.MaxValue, staminaStat.CurValue + 0.4666667f);
		}

		// Token: 0x04002811 RID: 10257
		private const int AhpAmount = 450;

		// Token: 0x04002812 RID: 10258
		private const float StaminaUseRate = 0.06666667f;

		// Token: 0x04002813 RID: 10259
		private const float StaminaKillAddition = 0.4666667f;
	}
}
