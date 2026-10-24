using System;
using System.Collections.Generic;
using System.Text;
using CustomPlayerEffects.Danger;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Searching;
using Mirror;
using PlayerRoles.Spectating;
using RemoteAdmin.Interfaces;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008D5 RID: 2261
	public class Scp1853 : TickingEffectBase, ISpectatorDataPlayerEffect, IHealableEffect, IConflictableEffect, IWeaponModifierPlayerEffect, ISearchTimeModifier, ICustomDisplayName, IUsableItemModifierEffect, IHeavyItemPenaltyImmunity, ISoundtrackMutingEffect, IDisplayInfoEffect
	{
		// Token: 0x170009E3 RID: 2531
		// (get) Token: 0x0600339B RID: 13211 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x170009E4 RID: 2532
		// (get) Token: 0x0600339C RID: 13212 RVA: 0x0007BB79 File Offset: 0x00079D79
		public override byte MaxIntensity
		{
			get
			{
				return 20;
			}
		}

		// Token: 0x170009E5 RID: 2533
		// (get) Token: 0x0600339D RID: 13213 RVA: 0x00004070 File Offset: 0x00002270
		public bool ParamsActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009E6 RID: 2534
		// (get) Token: 0x0600339E RID: 13214 RVA: 0x000AAEBE File Offset: 0x000A90BE
		// (set) Token: 0x0600339F RID: 13215 RVA: 0x000AAEC6 File Offset: 0x000A90C6
		public float ItemUsageSpeedMultiplier { get; private set; }

		// Token: 0x170009E7 RID: 2535
		// (get) Token: 0x060033A0 RID: 13216 RVA: 0x000AAECF File Offset: 0x000A90CF
		public string DisplayName
		{
			get
			{
				return "SCP-1853";
			}
		}

		// Token: 0x170009E8 RID: 2536
		// (get) Token: 0x060033A1 RID: 13217 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool CanBeDisplayed
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170009E9 RID: 2537
		// (get) Token: 0x060033A2 RID: 13218 RVA: 0x00004094 File Offset: 0x00002294
		public bool SprintingDisabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170009EA RID: 2538
		// (get) Token: 0x060033A3 RID: 13219 RVA: 0x00004070 File Offset: 0x00002270
		public bool MuteSoundtrack
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009EB RID: 2539
		// (get) Token: 0x060033A4 RID: 13220 RVA: 0x000AAED6 File Offset: 0x000A90D6
		public float CurrentDanger
		{
			get
			{
				return (float)base.Intensity * 0.25f;
			}
		}

		// Token: 0x170009EC RID: 2540
		// (get) Token: 0x060033A5 RID: 13221 RVA: 0x000AAEE5 File Offset: 0x000A90E5
		public float StatMultiplier
		{
			get
			{
				return 1f + (float)this.OffsetedDanger * 0.5f;
			}
		}

		// Token: 0x170009ED RID: 2541
		// (get) Token: 0x060033A6 RID: 13222 RVA: 0x000AAEFA File Offset: 0x000A90FA
		private int OffsetedDanger
		{
			get
			{
				return Mathf.Max(0, Mathf.FloorToInt(this.CurrentDanger - 1f));
			}
		}

		// Token: 0x060033A7 RID: 13223 RVA: 0x000AAF14 File Offset: 0x000A9114
		protected override void IntensityChanged(byte prevState, byte newState)
		{
			float statMultiplier = this.StatMultiplier;
			float num = Mathf.Floor((float)prevState * 0.25f);
			float num2 = Mathf.Floor((float)newState * 0.25f);
			foreach (Scp1853.SerializedStat serializedStat in Scp1853.BaseWeaponModifiers)
			{
				float boostAmount = serializedStat.BoostPercentage * statMultiplier;
				this._processedParams[serializedStat.Parameter] = this.CalculateStat(boostAmount, serializedStat.MaxBoost, serializedStat.IsAdditive);
			}
			this._searchSpeedMultiplier = 1.5f;
			this.ItemUsageSpeedMultiplier = this.CalculateStat(0.2f * statMultiplier, 0.4f, true);
			if (!base.IsLocalPlayer && !base.Hub.IsLocallySpectated())
			{
				return;
			}
			this.UpdateHeartbeat();
			if (num == num2 || num == 0f || num2 == 0f)
			{
				return;
			}
			bool flag = num2 > num;
			this._dangerChangedSource.PlayOneShot(flag ? this._dangerIncreasedClip : this._dangerDecreasedClip, 1f);
		}

		// Token: 0x060033A8 RID: 13224 RVA: 0x000AB017 File Offset: 0x000A9217
		public override void OnStopSpectating()
		{
			base.OnStopSpectating();
			this.UpdateHeartbeat();
		}

		// Token: 0x060033A9 RID: 13225 RVA: 0x00002E9A File Offset: 0x0000109A
		protected override void OnTick()
		{
		}

		// Token: 0x060033AA RID: 13226 RVA: 0x000AB028 File Offset: 0x000A9228
		protected override void Enabled()
		{
			base.Enabled();
			this.UpdateHeartbeat();
			if (!NetworkServer.active)
			{
				return;
			}
			DangerStackBase[] dangers = this.Dangers;
			for (int i = 0; i < dangers.Length; i++)
			{
				dangers[i].Initialize(base.Hub);
			}
		}

		// Token: 0x060033AB RID: 13227 RVA: 0x000AB06C File Offset: 0x000A926C
		protected override void Disabled()
		{
			base.Disabled();
			this.UpdateHeartbeat();
			if (!NetworkServer.active)
			{
				return;
			}
			DangerStackBase[] dangers = this.Dangers;
			for (int i = 0; i < dangers.Length; i++)
			{
				dangers[i].Dispose();
			}
		}

		// Token: 0x060033AC RID: 13228 RVA: 0x000AB0AC File Offset: 0x000A92AC
		protected override void Update()
		{
			base.Update();
			if (!base.IsEnabled || !NetworkServer.active)
			{
				return;
			}
			float num = 1f;
			foreach (DangerStackBase dangerStackBase in this.Dangers)
			{
				if (dangerStackBase.IsActive)
				{
					num += dangerStackBase.DangerValue;
				}
			}
			num = Mathf.Clamp(num, 1f, 5f);
			byte b = (byte)(num / 0.25f);
			if (b == base.Intensity)
			{
				return;
			}
			base.Intensity = b;
		}

		// Token: 0x060033AD RID: 13229 RVA: 0x00002E9A File Offset: 0x0000109A
		private void ExecuteVignettePulse()
		{
		}

		// Token: 0x060033AE RID: 13230 RVA: 0x00002E9A File Offset: 0x0000109A
		private void UpdateHeartbeat()
		{
		}

		// Token: 0x060033AF RID: 13231 RVA: 0x000AB12D File Offset: 0x000A932D
		private float CalculateStat(float boostAmount, float max = 0f, bool isAdditive = true)
		{
			if (max > 0f)
			{
				boostAmount = Mathf.Clamp(boostAmount, 0f, max);
			}
			return 1f + (isAdditive ? boostAmount : (-boostAmount));
		}

		// Token: 0x060033B0 RID: 13232 RVA: 0x00004242 File Offset: 0x00002442
		public bool IsHealable(ItemType it)
		{
			return it == ItemType.SCP500;
		}

		// Token: 0x060033B1 RID: 13233 RVA: 0x000AB155 File Offset: 0x000A9355
		public bool GetSpectatorText(out string s)
		{
			s = string.Format("SCP-1853 ({0} danger)", this.CurrentDanger);
			return true;
		}

		// Token: 0x060033B2 RID: 13234 RVA: 0x000AB16F File Offset: 0x000A936F
		public bool TryGetSpeed(ItemType item, out float speed)
		{
			speed = this.ItemUsageSpeedMultiplier;
			return base.IsEnabled && Scp1853.AffectedItems.Contains(item);
		}

		// Token: 0x060033B3 RID: 13235 RVA: 0x000AB18E File Offset: 0x000A938E
		public float ProcessSearchTime(float val)
		{
			return val / Mathf.Max(this._searchSpeedMultiplier, 1f);
		}

		// Token: 0x060033B4 RID: 13236 RVA: 0x000AB1A2 File Offset: 0x000A93A2
		public bool TryGetWeaponParam(AttachmentParam param, out float val)
		{
			return this._processedParams.TryGetValue(param, out val);
		}

		// Token: 0x060033B5 RID: 13237 RVA: 0x000AB1B4 File Offset: 0x000A93B4
		public bool CheckConflicts(StatusEffectBase other)
		{
			if (!(other is CokeBase))
			{
				return false;
			}
			Poisoned poisoned;
			if (!base.Hub.playerEffectsController.TryGetEffect<Poisoned>(out poisoned))
			{
				return false;
			}
			if (!poisoned.IsEnabled)
			{
				poisoned.ForceIntensity(1);
			}
			return true;
		}

		// Token: 0x060033B6 RID: 13238 RVA: 0x000AB1F1 File Offset: 0x000A93F1
		public void AppendDisplayInfo(StringBuilder sb)
		{
			sb.AppendFormat("(Danger: {0})", this.CurrentDanger);
		}

		// Token: 0x040027C8 RID: 10184
		public const float DangerPerIntensity = 0.25f;

		// Token: 0x040027C9 RID: 10185
		private const float MinimumDanger = 1f;

		// Token: 0x040027CA RID: 10186
		private const float MaximumDanger = 5f;

		// Token: 0x040027CB RID: 10187
		private const float SearchSpeed = 1.5f;

		// Token: 0x040027CC RID: 10188
		private const float EquipAndUse = 0.2f;

		// Token: 0x040027CD RID: 10189
		private const float ItemUsageMaxMultiplier = 0.4f;

		// Token: 0x040027CE RID: 10190
		private const float NoMaxBoost = 0f;

		// Token: 0x040027CF RID: 10191
		private const float StatMultiplierPerDanger = 0.5f;

		// Token: 0x040027D0 RID: 10192
		private static readonly Scp1853.SerializedStat[] BaseWeaponModifiers = new Scp1853.SerializedStat[]
		{
			new Scp1853.SerializedStat(AttachmentParam.AdsSpeedMultiplier, 0.2f, 0.4f, true),
			new Scp1853.SerializedStat(AttachmentParam.OverallRecoilMultiplier, 0.2f, 0f, false),
			new Scp1853.SerializedStat(AttachmentParam.AdsInaccuracyMultiplier, 0.2f, 0f, false),
			new Scp1853.SerializedStat(AttachmentParam.HipInaccuracyMultiplier, 0.2f, 0f, false),
			new Scp1853.SerializedStat(AttachmentParam.ReloadSpeedMultiplier, 0.35f, 0f, true),
			new Scp1853.SerializedStat(AttachmentParam.DrawSpeedMultiplier, 0.2f, 0f, true)
		};

		// Token: 0x040027D1 RID: 10193
		private static readonly ItemType[] AffectedItems = new ItemType[]
		{
			ItemType.SCP018,
			ItemType.SCP500,
			ItemType.SCP330,
			ItemType.GrenadeFlash,
			ItemType.GrenadeHE,
			ItemType.Medkit,
			ItemType.Painkillers,
			ItemType.Adrenaline
		};

		// Token: 0x040027D2 RID: 10194
		public DangerStackBase[] Dangers = new DangerStackBase[]
		{
			new WarheadDanger(),
			new CardiacArrestDanger(),
			new RageTargetDanger(),
			new CorrodingDanger(),
			new PlayerDamagedDanger(),
			new ScpEncounterDanger(),
			new ZombieEncounterDanger(),
			new ArmedEnemyDanger()
		};

		// Token: 0x040027D4 RID: 10196
		[SerializeField]
		private AudioSource _dangerChangedSource;

		// Token: 0x040027D5 RID: 10197
		[SerializeField]
		private AudioSource _heartbeatSource;

		// Token: 0x040027D6 RID: 10198
		[SerializeField]
		private AudioClip _dangerIncreasedClip;

		// Token: 0x040027D7 RID: 10199
		[SerializeField]
		private AudioClip _dangerDecreasedClip;

		// Token: 0x040027D8 RID: 10200
		private readonly Dictionary<AttachmentParam, float> _processedParams = new Dictionary<AttachmentParam, float>();

		// Token: 0x040027D9 RID: 10201
		private float _searchSpeedMultiplier;

		// Token: 0x020008D6 RID: 2262
		[Serializable]
		private struct SerializedStat
		{
			// Token: 0x060033B9 RID: 13241 RVA: 0x000AB33A File Offset: 0x000A953A
			public SerializedStat(AttachmentParam param, float boostPercentage, float maxBoost = 0f, bool isAdditive = true)
			{
				this.Parameter = param;
				this.BoostPercentage = boostPercentage;
				this.MaxBoost = maxBoost;
				this.IsAdditive = isAdditive;
			}

			// Token: 0x040027DA RID: 10202
			public AttachmentParam Parameter;

			// Token: 0x040027DB RID: 10203
			public float BoostPercentage;

			// Token: 0x040027DC RID: 10204
			public float MaxBoost;

			// Token: 0x040027DD RID: 10205
			public bool IsAdditive;
		}
	}
}
