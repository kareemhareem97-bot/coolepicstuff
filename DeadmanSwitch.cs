using System;
using System.Runtime.CompilerServices;
using GameCore;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using Respawning;
using Respawning.Waves;
using Respawning.Waves.Generic;
using UnityEngine;
using Utils.Networking;

// Token: 0x02000047 RID: 71
public static class DeadmanSwitch
{
	// Token: 0x17000071 RID: 113
	// (get) Token: 0x060001CE RID: 462 RVA: 0x00007D72 File Offset: 0x00005F72
	// (set) Token: 0x060001CF RID: 463 RVA: 0x00007D79 File Offset: 0x00005F79
	public static bool ForceCountdownToggle
	{
		get
		{
			return DeadmanSwitch._forceCountdownToggle;
		}
		set
		{
			if (!value)
			{
				DeadmanSwitch.Reset(true);
			}
			DeadmanSwitch._forceCountdownToggle = value;
		}
	}

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x060001D0 RID: 464 RVA: 0x00007D8A File Offset: 0x00005F8A
	// (set) Token: 0x060001D1 RID: 465 RVA: 0x00007D91 File Offset: 0x00005F91
	public static bool IsSequenceActive { get; private set; }

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x060001D2 RID: 466 RVA: 0x00007D99 File Offset: 0x00005F99
	// (set) Token: 0x060001D3 RID: 467 RVA: 0x00007DA0 File Offset: 0x00005FA0
	public static bool IsDeadmanSwitchEnabled { get; private set; }

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x060001D4 RID: 468 RVA: 0x00007DA8 File Offset: 0x00005FA8
	// (set) Token: 0x060001D5 RID: 469 RVA: 0x00007DAF File Offset: 0x00005FAF
	public static float CountdownTimeLeft
	{
		get
		{
			return DeadmanSwitch._countdownTimer;
		}
		set
		{
			DeadmanSwitch._countdownTimer = Mathf.Min(value, DeadmanSwitch.CountdownMaxTime);
		}
	}

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060001D6 RID: 470 RVA: 0x00007DC1 File Offset: 0x00005FC1
	// (set) Token: 0x060001D7 RID: 471 RVA: 0x00007DC8 File Offset: 0x00005FC8
	public static float CountdownMaxTime { get; set; }

	// Token: 0x060001D8 RID: 472 RVA: 0x00007DD0 File Offset: 0x00005FD0
	public static void Reset(bool resetSequence = true)
	{
		DeadmanSwitch._countdownTimer = ConfigFile.ServerConfig.GetFloat("dms_activation_time", 240f);
		if (resetSequence)
		{
			DeadmanSwitch.IsSequenceActive = false;
			new DeadmanSwitch.DeadMansSequenceActiveMessage
			{
				IsActive = false
			}.SendToAuthenticated(0);
		}
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x00007E18 File Offset: 0x00006018
	public static void InitiateProtocol()
	{
		DeadmanSequenceActivatingEventArgs deadmanSequenceActivatingEventArgs = new DeadmanSequenceActivatingEventArgs();
		ServerEvents.OnDeadmanSequenceActivating(deadmanSequenceActivatingEventArgs);
		if (!deadmanSequenceActivatingEventArgs.IsAllowed)
		{
			DeadmanSwitch.Reset(true);
			return;
		}
		DeadmanSwitch.IsSequenceActive = true;
		new DeadmanSwitch.DeadMansSequenceActiveMessage
		{
			IsActive = true
		}.SendToAuthenticated(0);
		DeadmanSwitch.StartWarhead();
		ServerEvents.OnDeadmanSequenceActivated();
	}

	// Token: 0x060001DA RID: 474 RVA: 0x00007E68 File Offset: 0x00006068
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		CustomNetworkManager.OnClientReady += delegate()
		{
			DeadmanSwitch.ReloadConfigs();
			DeadmanSwitch._forceCountdownToggle = false;
			DeadmanSwitch.IsSequenceActive = false;
			Action<DeadmanSwitch.DeadMansSequenceActiveMessage> handler;
			if ((handler = DeadmanSwitch.<>O.<0>__ClientMessageReceived) == null)
			{
				handler = (DeadmanSwitch.<>O.<0>__ClientMessageReceived = new Action<DeadmanSwitch.DeadMansSequenceActiveMessage>(DeadmanSwitch.ClientMessageReceived));
			}
			NetworkClient.ReplaceHandler<DeadmanSwitch.DeadMansSequenceActiveMessage>(handler, true);
		};
		Action value;
		if ((value = DeadmanSwitch.<>O.<1>__OnUpdate) == null)
		{
			value = (DeadmanSwitch.<>O.<1>__OnUpdate = new Action(DeadmanSwitch.OnUpdate));
		}
		StaticUnityMethods.OnUpdate += value;
		Action<SpawnableWaveBase> value2;
		if ((value2 = DeadmanSwitch.<>O.<2>__OnWaveSpawned) == null)
		{
			value2 = (DeadmanSwitch.<>O.<2>__OnWaveSpawned = new Action<SpawnableWaveBase>(DeadmanSwitch.OnWaveSpawned));
		}
		WaveManager.OnWaveTrigger += value2;
		Action<ReferenceHub> value3;
		if ((value3 = DeadmanSwitch.<>O.<3>__SyncValues) == null)
		{
			value3 = (DeadmanSwitch.<>O.<3>__SyncValues = new Action<ReferenceHub>(DeadmanSwitch.SyncValues));
		}
		ReferenceHub.OnPlayerAdded += value3;
		Delegate onConfigReloaded = ConfigFile.OnConfigReloaded;
		Action b;
		if ((b = DeadmanSwitch.<>O.<4>__ReloadConfigs) == null)
		{
			b = (DeadmanSwitch.<>O.<4>__ReloadConfigs = new Action(DeadmanSwitch.ReloadConfigs));
		}
		ConfigFile.OnConfigReloaded = (Action)Delegate.Combine(onConfigReloaded, b);
	}

	// Token: 0x060001DB RID: 475 RVA: 0x00007F28 File Offset: 0x00006128
	private static void OnWaveSpawned(SpawnableWaveBase wave)
	{
		if (wave is IMiniWave)
		{
			DeadmanSwitch.CountdownTimeLeft += 60f;
			return;
		}
		if (DeadmanSwitch.IsSequenceActive || DeadmanSwitch.CountdownTimeLeft >= 210f)
		{
			return;
		}
		DeadmanSwitch.CountdownTimeLeft = 210f;
	}

	// Token: 0x060001DC RID: 476 RVA: 0x00007F61 File Offset: 0x00006161
	private static void ReloadConfigs()
	{
		DeadmanSwitch.IsDeadmanSwitchEnabled = ConfigFile.ServerConfig.GetBool("dms_enabled", true);
		DeadmanSwitch.CountdownMaxTime = ConfigFile.ServerConfig.GetFloat("dms_activation_time", 240f);
		DeadmanSwitch.Reset(false);
	}

	// Token: 0x060001DD RID: 477 RVA: 0x00007F98 File Offset: 0x00006198
	private static void OnUpdate()
	{
		if (!NetworkServer.active || !AlphaWarheadController.SingletonSet)
		{
			return;
		}
		if (AlphaWarheadController.Singleton.AlreadyDetonated)
		{
			return;
		}
		if (DeadmanSwitch.IsSequenceActive)
		{
			return;
		}
		if (!DeadmanSwitch.ForceCountdownToggle)
		{
			if (!DeadmanSwitch.IsDeadmanSwitchEnabled)
			{
				return;
			}
			NtfSpawnWave ntfSpawnWave;
			ChaosSpawnWave chaosSpawnWave;
			if (!WaveManager.TryGet<NtfSpawnWave>(out ntfSpawnWave) || !WaveManager.TryGet<ChaosSpawnWave>(out chaosSpawnWave))
			{
				return;
			}
			ILimitedWave limitedWave = ntfSpawnWave;
			if (limitedWave != null)
			{
				ILimitedWave limitedWave2 = chaosSpawnWave;
				if (limitedWave2 != null)
				{
					if (limitedWave.RespawnTokens > 0 || limitedWave2.RespawnTokens > 0)
					{
						return;
					}
					goto IL_64;
				}
			}
			return;
		}
		IL_64:
		DeadmanSwitch._countdownTimer -= Time.deltaTime;
		if (DeadmanSwitch._countdownTimer >= 0f)
		{
			return;
		}
		DeadmanSwitch.InitiateProtocol();
	}

	// Token: 0x060001DE RID: 478 RVA: 0x0000802C File Offset: 0x0000622C
	private static void StartWarhead()
	{
		AlphaWarheadController singleton = AlphaWarheadController.Singleton;
		AlphaWarheadSyncInfo info = singleton.Info;
		if (info.ScenarioType == WarheadScenarioType.DeadmanSwitch)
		{
			return;
		}
		info.ScenarioType = WarheadScenarioType.DeadmanSwitch;
		singleton.NetworkInfo = info;
		DeadmanSwitch.OnActivationHeal();
		singleton.InstantPrepare();
		singleton.StartDetonation(false, false, null);
		singleton.IsLocked = true;
	}

	// Token: 0x060001DF RID: 479 RVA: 0x0000807C File Offset: 0x0000627C
	private static void OnActivationHeal()
	{
		foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
		{
			PlayerRoleBase currentRole = referenceHub.roleManager.CurrentRole;
			if (currentRole.Team == Team.SCPs && currentRole.RoleTypeId != RoleTypeId.Scp0492 && currentRole is IHealthbarRole)
			{
				HealthStat module = referenceHub.playerStats.GetModule<HealthStat>();
				float num = module.MaxValue * 0.3f;
				float num2 = 250f;
				if (module.CurValue + 250f > module.MaxValue)
				{
					num2 = module.MaxValue - module.CurValue;
				}
				if (module.CurValue >= num)
				{
					module.CurValue += num2;
				}
				else
				{
					module.CurValue = num + 250f;
				}
			}
		}
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x00008164 File Offset: 0x00006364
	private static void ClientMessageReceived(DeadmanSwitch.DeadMansSequenceActiveMessage message)
	{
		if (NetworkServer.active)
		{
			return;
		}
		DeadmanSwitch.IsSequenceActive = message.IsActive;
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x0000817C File Offset: 0x0000637C
	private static void SyncValues(ReferenceHub hub)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		hub.connectionToClient.Send<DeadmanSwitch.DeadMansSequenceActiveMessage>(new DeadmanSwitch.DeadMansSequenceActiveMessage
		{
			IsActive = DeadmanSwitch.IsSequenceActive
		}, 0);
	}

	// Token: 0x0400017D RID: 381
	private const float DefaultActivationTime = 240f;

	// Token: 0x0400017E RID: 382
	private const float WaveSpawnActivationTime = 210f;

	// Token: 0x0400017F RID: 383
	private const float MiniWaveSpawnActivationDelay = 60f;

	// Token: 0x04000180 RID: 384
	private const float ScpHealMaxHpPercentage = 0.3f;

	// Token: 0x04000181 RID: 385
	private const float ScpHealHpFlat = 250f;

	// Token: 0x04000185 RID: 389
	private static bool _forceCountdownToggle;

	// Token: 0x04000186 RID: 390
	private static float _countdownTimer;

	// Token: 0x02000048 RID: 72
	private struct DeadMansSequenceActiveMessage : NetworkMessage
	{
		// Token: 0x04000187 RID: 391
		public bool IsActive;
	}

	// Token: 0x02000049 RID: 73
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x04000188 RID: 392
		public static Action<DeadmanSwitch.DeadMansSequenceActiveMessage> <0>__ClientMessageReceived;

		// Token: 0x04000189 RID: 393
		public static Action <1>__OnUpdate;

		// Token: 0x0400018A RID: 394
		public static Action<SpawnableWaveBase> <2>__OnWaveSpawned;

		// Token: 0x0400018B RID: 395
		public static Action<ReferenceHub> <3>__SyncValues;

		// Token: 0x0400018C RID: 396
		public static Action <4>__ReloadConfigs;
	}
}
