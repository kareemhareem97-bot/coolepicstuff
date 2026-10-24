using System;
using System.Runtime.CompilerServices;
using Hazards;
using MapGeneration.Holidays;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008CA RID: 2250
	public class Prismatic : TickingEffectBase, IDamageModifierEffect, IMovementSpeedModifier, ISoundtrackMutingEffect, IHolidayEffect
	{
		// Token: 0x170009C8 RID: 2504
		// (get) Token: 0x0600334C RID: 13132 RVA: 0x000AA545 File Offset: 0x000A8745
		// (set) Token: 0x0600334D RID: 13133 RVA: 0x000AA54D File Offset: 0x000A874D
		public PrismaticCloud OriginCloud { get; set; }

		// Token: 0x170009C9 RID: 2505
		// (get) Token: 0x0600334E RID: 13134 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x170009CA RID: 2506
		// (get) Token: 0x0600334F RID: 13135 RVA: 0x00004070 File Offset: 0x00002270
		public bool MuteSoundtrack
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009CB RID: 2507
		// (get) Token: 0x06003350 RID: 13136 RVA: 0x00004078 File Offset: 0x00002278
		public float MovementSpeedMultiplier
		{
			get
			{
				return 1.05f;
			}
		}

		// Token: 0x170009CC RID: 2508
		// (get) Token: 0x06003351 RID: 13137 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009CD RID: 2509
		// (get) Token: 0x06003352 RID: 13138 RVA: 0x0000407F File Offset: 0x0000227F
		public float MovementSpeedLimit
		{
			get
			{
				return float.MaxValue;
			}
		}

		// Token: 0x170009CE RID: 2510
		// (get) Token: 0x06003353 RID: 13139 RVA: 0x000AA556 File Offset: 0x000A8756
		public bool DamageModifierActive
		{
			get
			{
				return this._isDamageModifierEnabled && (base.IsEnabled || this.IsTeslaImmunityActive);
			}
		}

		// Token: 0x170009CF RID: 2511
		// (get) Token: 0x06003354 RID: 13140 RVA: 0x000AA572 File Offset: 0x000A8772
		public HolidayType[] TargetHolidays { get; } = new HolidayType[]
		{
			HolidayType.Halloween
		};

		// Token: 0x170009D0 RID: 2512
		// (get) Token: 0x06003355 RID: 13141 RVA: 0x000AA57A File Offset: 0x000A877A
		private bool IsTeslaImmunityActive
		{
			get
			{
				return this._lastSaveTime + Prismatic.TeslaImmunityTime >= Time.timeSinceLevelLoad;
			}
		}

		// Token: 0x170009D1 RID: 2513
		// (get) Token: 0x06003356 RID: 13142 RVA: 0x000AA592 File Offset: 0x000A8792
		private bool IsImmunityActive
		{
			get
			{
				return this._lastSaveTime + Prismatic.DamageImmunityTime >= Time.timeSinceLevelLoad;
			}
		}

		// Token: 0x06003357 RID: 13143 RVA: 0x000AA5AC File Offset: 0x000A87AC
		public float GetDamageModifier(float baseDamage, DamageHandlerBase handler, HitboxType hitboxType)
		{
			UniversalDamageHandler universalDamageHandler = handler as UniversalDamageHandler;
			if (this.IsImmunityActive)
			{
				return 0f;
			}
			if (this.IsTeslaImmunityActive && universalDamageHandler != null && universalDamageHandler.TranslationId == DeathTranslations.Tesla.Id)
			{
				return 0f;
			}
			if (!base.IsEnabled)
			{
				return 0.85f;
			}
			if (universalDamageHandler != null && (universalDamageHandler.TranslationId == DeathTranslations.Scp207.Id || universalDamageHandler.TranslationId == DeathTranslations.PocketDecay.Id))
			{
				return 0.85f;
			}
			Scp939DamageHandler scp939DamageHandler = handler as Scp939DamageHandler;
			if (scp939DamageHandler != null && scp939DamageHandler.Scp939DamageType == Scp939DamageType.LungeTarget)
			{
				return 0.85f;
			}
			float curValue = base.Hub.playerStats.GetModule<HealthStat>().CurValue;
			float curValue2 = base.Hub.playerStats.GetModule<AhpStat>().CurValue;
			float num = curValue + curValue2;
			if (curValue > baseDamage || num > baseDamage)
			{
				return 0.85f;
			}
			this.DisableEffect();
			if (this.OriginCloud != null)
			{
				this.OriginCloud.IgnoredTargets.Add(base.Hub);
			}
			NetworkServer.SendToReady<Prismatic.BreakMessage>(new Prismatic.BreakMessage
			{
				SoundPos = base.Hub.transform.position
			}, 0);
			this._lastSaveTime = Time.timeSinceLevelLoad;
			return (num - Prismatic.DeathSaveHealth) / baseDamage;
		}

		// Token: 0x06003358 RID: 13144 RVA: 0x000AA6E7 File Offset: 0x000A88E7
		protected override void OnTick()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this._hpStat.ServerHeal(5f);
		}

		// Token: 0x06003359 RID: 13145 RVA: 0x000AA704 File Offset: 0x000A8904
		protected override void Enabled()
		{
			base.Enabled();
			this._isDamageModifierEnabled = true;
			if (!this._ambienceSource.isPlaying)
			{
				this._ambienceSource.Play();
			}
			else if (this._ambienceSource.volume == 0f)
			{
				this._ambienceSource.Stop();
				this._ambienceSource.Play();
			}
			this._ambienceSource.mute = true;
			this._ambienceSource.volume = 1f;
			if (!NetworkServer.active)
			{
				return;
			}
			this._hpStat = base.Hub.playerStats.GetModule<HealthStat>();
		}

		// Token: 0x0600335A RID: 13146 RVA: 0x000AA79C File Offset: 0x000A899C
		protected override void Update()
		{
			base.Update();
			if (this._ambienceSource.volume <= 0f)
			{
				return;
			}
			this._ambienceSource.mute = (!base.IsSpectated && !base.IsLocalPlayer);
			if (!base.IsEnabled)
			{
				this._ambienceSource.volume -= Time.deltaTime;
			}
		}

		// Token: 0x0600335B RID: 13147 RVA: 0x00002E9A File Offset: 0x0000109A
		private static void OnBreak(Prismatic.BreakMessage msg)
		{
		}

		// Token: 0x0600335C RID: 13148 RVA: 0x000AA800 File Offset: 0x000A8A00
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			CustomNetworkManager.OnClientReady += delegate()
			{
				Action<Prismatic.BreakMessage> handler;
				if ((handler = Prismatic.<>O.<0>__OnBreak) == null)
				{
					handler = (Prismatic.<>O.<0>__OnBreak = new Action<Prismatic.BreakMessage>(Prismatic.OnBreak));
				}
				NetworkClient.ReplaceHandler<Prismatic.BreakMessage>(handler, true);
			};
		}

		// Token: 0x040027A0 RID: 10144
		private const float HealingPerTick = 5f;

		// Token: 0x040027A1 RID: 10145
		public static readonly float DeathSaveHealth = 1f;

		// Token: 0x040027A2 RID: 10146
		public static readonly float VitalitySpeedMultipler = 1f;

		// Token: 0x040027A3 RID: 10147
		public static readonly float HumanAHPEfficacy = 1f;

		// Token: 0x040027A4 RID: 10148
		public static readonly float TeslaImmunityTime = 2f;

		// Token: 0x040027A5 RID: 10149
		public static readonly float BreakSoundDistance = 25f;

		// Token: 0x040027A6 RID: 10150
		public static readonly float BreakVolumeModifier = 0.25f;

		// Token: 0x040027A7 RID: 10151
		public static readonly float DamageImmunityTime = 0.25f;

		// Token: 0x040027AA RID: 10154
		[SerializeField]
		private AudioSource _ambienceSource;

		// Token: 0x040027AB RID: 10155
		[SerializeField]
		private AudioClip _effectBreakSound;

		// Token: 0x040027AC RID: 10156
		private float _lastSaveTime;

		// Token: 0x040027AD RID: 10157
		private bool _isDamageModifierEnabled;

		// Token: 0x040027AE RID: 10158
		private HealthStat _hpStat;

		// Token: 0x020008CB RID: 2251
		private struct BreakMessage : NetworkMessage
		{
			// Token: 0x040027AF RID: 10159
			public Vector3 SoundPos;
		}

		// Token: 0x020008CC RID: 2252
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040027B0 RID: 10160
			public static Action<Prismatic.BreakMessage> <0>__OnBreak;
		}
	}
}
