using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using InventorySystem.Items.Scp1509;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using RemoteAdmin.Interfaces;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008A5 RID: 2213
	public class AntiScp207 : CokeBase<AntiScp207Stack>, ISpectatorDataPlayerEffect, ICustomDisplayName, IDamageModifierEffect
	{
		// Token: 0x1700095D RID: 2397
		// (get) Token: 0x0600324C RID: 12876 RVA: 0x00034727 File Offset: 0x00032927
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Mixed;
			}
		}

		// Token: 0x1700095E RID: 2398
		// (get) Token: 0x0600324D RID: 12877 RVA: 0x000A8734 File Offset: 0x000A6934
		public override Dictionary<PlayerMovementState, float> StateMultipliers { get; }

		// Token: 0x1700095F RID: 2399
		// (get) Token: 0x0600324E RID: 12878 RVA: 0x000A873C File Offset: 0x000A693C
		public string DisplayName
		{
			get
			{
				return "SCP-207?";
			}
		}

		// Token: 0x17000960 RID: 2400
		// (get) Token: 0x0600324F RID: 12879 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool CanBeDisplayed
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000961 RID: 2401
		// (get) Token: 0x06003250 RID: 12880 RVA: 0x000A8744 File Offset: 0x000A6944
		public override float MovementSpeedMultiplier
		{
			get
			{
				if (!Vitality.CheckPlayer(base.Hub))
				{
					return base.CurrentStack.SpeedMultiplier;
				}
				return AntiScp207.VitalitySpeedMultipler;
			}
		}

		// Token: 0x17000962 RID: 2402
		// (get) Token: 0x06003251 RID: 12881 RVA: 0x000A8772 File Offset: 0x000A6972
		public bool DamageModifierActive
		{
			get
			{
				return this._isDamageModifierEnabled && (base.IsEnabled || this.IsTeslaImmunityActive);
			}
		}

		// Token: 0x17000963 RID: 2403
		// (get) Token: 0x06003252 RID: 12882 RVA: 0x000A878E File Offset: 0x000A698E
		private bool IsTeslaImmunityActive
		{
			get
			{
				return this._lastSaveTime + AntiScp207.TeslaImmunityTime >= Time.timeSinceLevelLoad;
			}
		}

		// Token: 0x17000964 RID: 2404
		// (get) Token: 0x06003253 RID: 12883 RVA: 0x000A87A6 File Offset: 0x000A69A6
		private bool IsImmunityActive
		{
			get
			{
				return this._lastSaveTime + AntiScp207.DamageImmunityTime >= Time.timeSinceLevelLoad;
			}
		}

		// Token: 0x17000965 RID: 2405
		// (get) Token: 0x06003254 RID: 12884 RVA: 0x000A87BE File Offset: 0x000A69BE
		private float CurrentHealing
		{
			get
			{
				return base.CurrentStack.HealAmount * base.GetMovementStateMultiplier();
			}
		}

		// Token: 0x06003255 RID: 12885 RVA: 0x000A87D4 File Offset: 0x000A69D4
		public override bool CheckConflicts(StatusEffectBase other)
		{
			return this._isDamageModifierEnabled = !base.CheckConflicts(other);
		}

		// Token: 0x06003256 RID: 12886 RVA: 0x000A87F4 File Offset: 0x000A69F4
		public bool GetSpectatorText(out string s)
		{
			s = ((base.Intensity > 1) ? string.Format("SCP-207? (x{0})", base.Intensity) : "SCP-207?");
			return true;
		}

		// Token: 0x06003257 RID: 12887 RVA: 0x000A8820 File Offset: 0x000A6A20
		public float GetDamageModifier(float baseDamage, DamageHandlerBase handler, HitboxType hitboxType)
		{
			if (this._simulating)
			{
				return 1f;
			}
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
				return 1f;
			}
			if (universalDamageHandler != null && (universalDamageHandler.TranslationId == DeathTranslations.Scp207.Id || universalDamageHandler.TranslationId == DeathTranslations.PocketDecay.Id || universalDamageHandler.TranslationId == DeathTranslations.Scp1509.Id))
			{
				return 1f;
			}
			if (handler is Scp1509DamageHandler)
			{
				return 1f;
			}
			HealthStat module = base.Hub.playerStats.GetModule<HealthStat>();
			AhpStat module2 = base.Hub.playerStats.GetModule<AhpStat>();
			HumeShieldStat module3 = base.Hub.playerStats.GetModule<HumeShieldStat>();
			if (this.SimulateHandlerOutput(module, module2, module3, handler) != DamageHandlerBase.HandlerOutput.Death)
			{
				return 1f;
			}
			module.CurValue = 1f;
			module3.CurValue = 0f;
			module2.ServerKillAllProcesses();
			this.DisableEffect();
			NetworkServer.SendToReady<AntiScp207.BreakMessage>(new AntiScp207.BreakMessage
			{
				SoundPos = base.Hub.transform.position
			}, 0);
			this._lastSaveTime = Time.timeSinceLevelLoad;
			return 0f;
		}

		// Token: 0x06003258 RID: 12888 RVA: 0x000A8970 File Offset: 0x000A6B70
		private DamageHandlerBase.HandlerOutput SimulateHandlerOutput(HealthStat healthStat, AhpStat ahpStat, HumeShieldStat humeShieldStat, DamageHandlerBase dhb)
		{
			float curValue = healthStat.CurValue;
			float curValue2 = humeShieldStat.CurValue;
			float curValue3 = ahpStat.CurValue;
			List<AhpProcessSnapshot> list = ListPool<AhpProcessSnapshot>.Shared.Rent(ahpStat.GenerateSnapshots());
			this._simulating = true;
			DamageHandlerBase.HandlerOutput result = dhb.ApplyDamage(base.Hub);
			this._simulating = false;
			healthStat.CurValue = curValue;
			ahpStat.CurValue = curValue3;
			humeShieldStat.CurValue = curValue2;
			foreach (AhpProcessSnapshot ahpProcessSnapshot in list)
			{
				AhpProcess ahpProcess;
				if (ahpStat.ServerTryGetProcess(ahpProcessSnapshot.KillCode, out ahpProcess))
				{
					ahpProcess.OverrideProcess(ahpProcessSnapshot);
				}
				else
				{
					ahpStat.ServerAddProcess(ahpProcessSnapshot);
				}
			}
			if (this._ahpProcess != null && ahpStat.CurValue <= 0f)
			{
				this._ahpProcess.DecayRate = 0f;
			}
			ahpStat.ServerUpdateProcesses(true, true);
			ListPool<AhpProcessSnapshot>.Shared.Return(list);
			return result;
		}

		// Token: 0x06003259 RID: 12889 RVA: 0x000A8A7C File Offset: 0x000A6C7C
		protected override void IntensityChanged(byte prevState, byte newState)
		{
			base.IntensityChanged(prevState, newState);
			if (!base.IsLocalPlayer)
			{
				return;
			}
			this._ambienceSource.Stop();
			if (newState > 0)
			{
				this._ambienceSource.clip = this._ambienceSounds[Mathf.Min((int)newState, this._ambienceSounds.Length) - 1];
				this._ambienceSource.Play();
			}
		}

		// Token: 0x0600325A RID: 12890 RVA: 0x000A8AD8 File Offset: 0x000A6CD8
		protected override void OnTick()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			float currentHealing = this.CurrentHealing;
			if (!this._healthStat.FullyHealed)
			{
				if (this._ahpProcess != null)
				{
					this._ahpProcess.DecayRate = 0f;
				}
				this._healthStat.ServerHeal(currentHealing);
				return;
			}
			if (this._ahpProcess == null)
			{
				this._ahpProcess = this._ahpStat.ServerAddProcess(0f, this._ahpStat.MaxValue, -currentHealing, 0.7f, 0f, true);
			}
			this._ahpProcess.DecayRate = -currentHealing;
		}

		// Token: 0x0600325B RID: 12891 RVA: 0x000A8B6C File Offset: 0x000A6D6C
		protected override void Enabled()
		{
			base.Enabled();
			this._isDamageModifierEnabled = true;
			this._ahpProcess = null;
			if (!base.Hub.playerStats.TryGetModule<HealthStat>(out this._healthStat))
			{
				throw new NullReferenceException();
			}
			if (!base.Hub.playerStats.TryGetModule<AhpStat>(out this._ahpStat))
			{
				throw new NullReferenceException();
			}
		}

		// Token: 0x0600325C RID: 12892 RVA: 0x000A8BC9 File Offset: 0x000A6DC9
		protected override void Disabled()
		{
			base.Disabled();
			if (this._ahpProcess == null)
			{
				return;
			}
			this._ahpProcess.DecayRate = 1f;
		}

		// Token: 0x0600325D RID: 12893 RVA: 0x00002E9A File Offset: 0x0000109A
		private static void OnBreak(AntiScp207.BreakMessage msg)
		{
		}

		// Token: 0x0600325E RID: 12894 RVA: 0x000A8BEA File Offset: 0x000A6DEA
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			CustomNetworkManager.OnClientReady += delegate()
			{
				Action<AntiScp207.BreakMessage> handler;
				if ((handler = AntiScp207.<>O.<0>__OnBreak) == null)
				{
					handler = (AntiScp207.<>O.<0>__OnBreak = new Action<AntiScp207.BreakMessage>(AntiScp207.OnBreak));
				}
				NetworkClient.ReplaceHandler<AntiScp207.BreakMessage>(handler, true);
			};
		}

		// Token: 0x0600325F RID: 12895 RVA: 0x000A8C10 File Offset: 0x000A6E10
		public AntiScp207()
		{
			Dictionary<PlayerMovementState, float> dictionary = new Dictionary<PlayerMovementState, float>();
			dictionary[PlayerMovementState.Crouching] = 1f;
			dictionary[PlayerMovementState.Sneaking] = 0.7f;
			dictionary[PlayerMovementState.Walking] = 0.6f;
			dictionary[PlayerMovementState.Sprinting] = 0.1f;
			this.StateMultipliers = dictionary;
			base..ctor();
		}

		// Token: 0x04002736 RID: 10038
		private const float AhpDecayValue = 1f;

		// Token: 0x04002737 RID: 10039
		public static readonly float DeathSaveHealth = 1f;

		// Token: 0x04002738 RID: 10040
		public static readonly float VitalitySpeedMultipler = 1f;

		// Token: 0x04002739 RID: 10041
		public static readonly float HumanAHPEfficacy = 1f;

		// Token: 0x0400273A RID: 10042
		public static readonly float TeslaImmunityTime = 2f;

		// Token: 0x0400273B RID: 10043
		public static readonly float BreakSoundDistance = 25f;

		// Token: 0x0400273C RID: 10044
		public static readonly float BreakVolumeModifier = 0.45f;

		// Token: 0x0400273D RID: 10045
		public static readonly float DamageImmunityTime = 0.75f;

		// Token: 0x0400273F RID: 10047
		[SerializeField]
		private AudioClip[] _ambienceSounds;

		// Token: 0x04002740 RID: 10048
		[SerializeField]
		private AudioSource _ambienceSource;

		// Token: 0x04002741 RID: 10049
		[SerializeField]
		private AudioClip _effectBreakSound;

		// Token: 0x04002742 RID: 10050
		private float _lastSaveTime;

		// Token: 0x04002743 RID: 10051
		private bool _isDamageModifierEnabled;

		// Token: 0x04002744 RID: 10052
		private bool _simulating;

		// Token: 0x04002745 RID: 10053
		private AhpProcess _ahpProcess;

		// Token: 0x04002746 RID: 10054
		private HealthStat _healthStat;

		// Token: 0x04002747 RID: 10055
		private AhpStat _ahpStat;

		// Token: 0x020008A6 RID: 2214
		private struct BreakMessage : NetworkMessage
		{
			// Token: 0x04002748 RID: 10056
			public Vector3 SoundPos;
		}

		// Token: 0x020008A7 RID: 2215
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002749 RID: 10057
			public static Action<AntiScp207.BreakMessage> <0>__OnBreak;
		}
	}
}
