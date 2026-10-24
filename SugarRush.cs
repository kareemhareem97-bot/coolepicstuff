using System;
using MapGeneration.Holidays;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008E6 RID: 2278
	public class SugarRush : StatusEffectBase, IStaminaModifier, IMovementSpeedModifier, IMovementInputOverride, ISoundtrackMutingEffect, IHolidayEffect
	{
		// Token: 0x17000A2F RID: 2607
		// (get) Token: 0x06003441 RID: 13377 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x17000A30 RID: 2608
		// (get) Token: 0x06003442 RID: 13378 RVA: 0x00004070 File Offset: 0x00002270
		public bool MuteSoundtrack
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A31 RID: 2609
		// (get) Token: 0x06003443 RID: 13379 RVA: 0x00004070 File Offset: 0x00002270
		public bool StaminaModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A32 RID: 2610
		// (get) Token: 0x06003444 RID: 13380 RVA: 0x0000548E File Offset: 0x0000368E
		public float StaminaUsageMultiplier
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000A33 RID: 2611
		// (get) Token: 0x06003445 RID: 13381 RVA: 0x0000408D File Offset: 0x0000228D
		public float StaminaRegenMultiplier
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000A34 RID: 2612
		// (get) Token: 0x06003446 RID: 13382 RVA: 0x00004094 File Offset: 0x00002294
		public bool SprintingDisabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000A35 RID: 2613
		// (get) Token: 0x06003447 RID: 13383 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool ForceSprinting
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A36 RID: 2614
		// (get) Token: 0x06003448 RID: 13384 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A37 RID: 2615
		// (get) Token: 0x06003449 RID: 13385 RVA: 0x000AC5BA File Offset: 0x000AA7BA
		public float MovementSpeedMultiplier
		{
			get
			{
				return 1.5f;
			}
		}

		// Token: 0x17000A38 RID: 2616
		// (get) Token: 0x0600344A RID: 13386 RVA: 0x0000407F File Offset: 0x0000227F
		public float MovementSpeedLimit
		{
			get
			{
				return float.MaxValue;
			}
		}

		// Token: 0x17000A39 RID: 2617
		// (get) Token: 0x0600344B RID: 13387 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementOverrideActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A3A RID: 2618
		// (get) Token: 0x0600344C RID: 13388 RVA: 0x000AC5C1 File Offset: 0x000AA7C1
		public Vector3 MovementOverrideDirection
		{
			get
			{
				return base.Hub.transform.forward;
			}
		}

		// Token: 0x17000A3B RID: 2619
		// (get) Token: 0x0600344D RID: 13389 RVA: 0x000AC5D3 File Offset: 0x000AA7D3
		public HolidayType[] TargetHolidays { get; } = new HolidayType[]
		{
			HolidayType.Halloween
		};

		// Token: 0x0600344E RID: 13390 RVA: 0x000AC5DC File Offset: 0x000AA7DC
		private void UpdateSource()
		{
			this._backgroundMusic.mute = (!base.IsSpectated && !base.IsLocalPlayer);
			if (base.IsEnabled)
			{
				this.AdjustSource(1f);
				return;
			}
			if (this._backgroundMusic.volume > 0f)
			{
				this.AdjustSource(0f);
				return;
			}
			this._backgroundMusic.pitch = 1f;
			this._continueUpdateSource = false;
		}

		// Token: 0x0600344F RID: 13391 RVA: 0x000AC654 File Offset: 0x000AA854
		private void AdjustSource(float target)
		{
			this._backgroundMusic.volume = Mathf.MoveTowards(this._backgroundMusic.volume, target, Time.deltaTime * this._volumeAdjustRate);
			this._backgroundMusic.pitch = Mathf.MoveTowards(this._backgroundMusic.pitch, target, Time.deltaTime * this._pitchAdjustRate);
		}

		// Token: 0x06003450 RID: 13392 RVA: 0x000AC6B4 File Offset: 0x000AA8B4
		protected override void Enabled()
		{
			base.Enabled();
			this._continueUpdateSource = true;
			this._backgroundMusic.Play();
			this.UpdateSource();
			if (!NetworkServer.active)
			{
				return;
			}
			StaminaStat staminaStat;
			if (base.Hub.playerStats.TryGetModule<StaminaStat>(out staminaStat))
			{
				staminaStat.AddAmount(100f);
			}
		}

		// Token: 0x06003451 RID: 13393 RVA: 0x000AC706 File Offset: 0x000AA906
		protected override void Update()
		{
			base.Update();
			if (this._continueUpdateSource)
			{
				this.UpdateSource();
			}
		}

		// Token: 0x0400281F RID: 10271
		private const float InstantStamina = 100f;

		// Token: 0x04002821 RID: 10273
		[SerializeField]
		private AudioSource _backgroundMusic;

		// Token: 0x04002822 RID: 10274
		[SerializeField]
		private float _volumeAdjustRate;

		// Token: 0x04002823 RID: 10275
		[SerializeField]
		private float _pitchAdjustRate;

		// Token: 0x04002824 RID: 10276
		private bool _continueUpdateSource;
	}
}
