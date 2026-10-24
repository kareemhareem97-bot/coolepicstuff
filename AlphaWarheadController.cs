using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Achievements;
using Footprinting;
using GameCore;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Usables.Scp244;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Events.Handlers;
using LightContainmentZoneDecontamination;
using MapGeneration;
using Mirror;
using Mirror.RemoteCalls;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using Subtitles;
using UnityEngine;
using Utils.Networking;
using Utils.NonAllocLINQ;

// Token: 0x0200003B RID: 59
public class AlphaWarheadController : NetworkBehaviour
{
	// Token: 0x17000059 RID: 89
	// (get) Token: 0x06000164 RID: 356 RVA: 0x00006723 File Offset: 0x00004923
	// (set) Token: 0x06000165 RID: 357 RVA: 0x0000672B File Offset: 0x0000492B
	public bool AlreadyDetonated { get; private set; }

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x06000166 RID: 358 RVA: 0x00006734 File Offset: 0x00004934
	public AlphaWarheadController.DetonationScenario CurScenario
	{
		get
		{
			switch (this.Info.ScenarioType)
			{
			case WarheadScenarioType.Start:
				return this.StartScenarios[(int)this.Info.ScenarioId];
			case WarheadScenarioType.Resume:
				return this.ResumeScenarios[(int)this.Info.ScenarioId];
			case WarheadScenarioType.DeadmanSwitch:
				return this.DeadmanSwitchScenario;
			default:
				return null;
			}
		}
	}

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x06000167 RID: 359 RVA: 0x0000678F File Offset: 0x0000498F
	// (set) Token: 0x06000168 RID: 360 RVA: 0x00006797 File Offset: 0x00004997
	public int WarheadKills { get; private set; }

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x06000169 RID: 361 RVA: 0x000067A0 File Offset: 0x000049A0
	// (set) Token: 0x0600016A RID: 362 RVA: 0x000067A8 File Offset: 0x000049A8
	public bool IsLocked { get; set; }

	// Token: 0x1700005D RID: 93
	// (get) Token: 0x0600016B RID: 363 RVA: 0x000067B1 File Offset: 0x000049B1
	// (set) Token: 0x0600016C RID: 364 RVA: 0x000067B8 File Offset: 0x000049B8
	public static AlphaWarheadController Singleton { get; private set; }

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x0600016D RID: 365 RVA: 0x000067C0 File Offset: 0x000049C0
	// (set) Token: 0x0600016E RID: 366 RVA: 0x000067C7 File Offset: 0x000049C7
	public static bool SingletonSet { get; private set; }

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x0600016F RID: 367 RVA: 0x000067CF File Offset: 0x000049CF
	public static ReferenceHub WarheadTriggeredby
	{
		get
		{
			if (!AlphaWarheadController.SingletonSet)
			{
				return null;
			}
			return AlphaWarheadController.Singleton._triggeringPlayer.Hub;
		}
	}

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x06000170 RID: 368 RVA: 0x000067E9 File Offset: 0x000049E9
	public static bool Detonated
	{
		get
		{
			return AlphaWarheadController.InProgress && AlphaWarheadController.TimeUntilDetonation == 0f;
		}
	}

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x06000171 RID: 369 RVA: 0x00006800 File Offset: 0x00004A00
	public static bool InProgress
	{
		get
		{
			return AlphaWarheadController.SingletonSet && AlphaWarheadController.Singleton.Info.InProgress;
		}
	}

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x06000172 RID: 370 RVA: 0x0000681A File Offset: 0x00004A1A
	public static float TimeUntilDetonation
	{
		get
		{
			return Mathf.Max(0f, (float)(AlphaWarheadController.Singleton.Info.StartTime + (double)AlphaWarheadController.Singleton.CurScenario.TotalTime - NetworkTime.time));
		}
	}

	// Token: 0x14000001 RID: 1
	// (add) Token: 0x06000173 RID: 371 RVA: 0x00006850 File Offset: 0x00004A50
	// (remove) Token: 0x06000174 RID: 372 RVA: 0x00006884 File Offset: 0x00004A84
	public static event Action<bool> OnProgressChanged;

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x06000175 RID: 373 RVA: 0x000068B8 File Offset: 0x00004AB8
	// (remove) Token: 0x06000176 RID: 374 RVA: 0x000068EC File Offset: 0x00004AEC
	public static event Action OnDetonated;

	// Token: 0x06000177 RID: 375 RVA: 0x0000691F File Offset: 0x00004B1F
	private void Awake()
	{
		AlphaWarheadController.Singleton = this;
		AlphaWarheadController.SingletonSet = true;
	}

	// Token: 0x06000178 RID: 376 RVA: 0x00006930 File Offset: 0x00004B30
	private void Start()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		this.NetworkCooldownEndTime = 0.0;
		this._autoDetonateTime = ConfigFile.ServerConfig.GetFloat("auto_warhead_start_minutes", 0f) * 60f;
		this._autoDetonate = (this._autoDetonateTime > 0f);
		this._autoDetonateLock = ConfigFile.ServerConfig.GetBool("auto_warhead_lock", false);
		this._openDoors = ConfigFile.ServerConfig.GetBool("open_doors_on_countdown", true);
		this._cooldown = ConfigFile.ServerConfig.GetInt("warhead_cooldown", 40);
		AlphaWarheadSyncInfo networkInfo = default(AlphaWarheadSyncInfo);
		int @int = ConfigFile.ServerConfig.GetInt("warhead_tminus_start_duration", 90);
		networkInfo.ScenarioId = this.DefaultScenarioId;
		byte b = 0;
		while ((int)b < this.StartScenarios.Length)
		{
			if (this.StartScenarios[(int)b].TimeToDetonate == @int)
			{
				networkInfo.ScenarioId = b;
			}
			b += 1;
		}
		this.NetworkInfo = networkInfo;
		ConfigFile.OnConfigReloaded = (Action)Delegate.Combine(ConfigFile.OnConfigReloaded, new Action(this.OnConfigReloaded));
	}

	// Token: 0x06000179 RID: 377 RVA: 0x00006A44 File Offset: 0x00004C44
	private void OnDestroy()
	{
		AlphaWarheadController.SingletonSet = false;
		ConfigFile.OnConfigReloaded = (Action)Delegate.Remove(ConfigFile.OnConfigReloaded, new Action(this.OnConfigReloaded));
	}

	// Token: 0x0600017A RID: 378 RVA: 0x00006A6C File Offset: 0x00004C6C
	private void Update()
	{
		if (this.Info != this._prevInfo)
		{
			this.OnInfoUpdated();
			this._prevInfo = this.Info;
		}
		if (this.Info.InProgress)
		{
			this._lastAnnouncer.UpdateActive();
		}
		this.UpdateFog();
		this.ServerUpdateDetonationTime();
		this.ServerUpdateAutonuke();
	}

	// Token: 0x0600017B RID: 379 RVA: 0x00006AC8 File Offset: 0x00004CC8
	private static bool TryGetBroadcaster(out Broadcast broadcaster)
	{
		broadcaster = null;
		ReferenceHub referenceHub;
		return ReferenceHub.TryGetLocalHub(out referenceHub) && referenceHub.TryGetComponent<Broadcast>(out broadcaster);
	}

	// Token: 0x0600017C RID: 380 RVA: 0x00006AEC File Offset: 0x00004CEC
	private void OnInfoUpdated()
	{
		bool inProgress = this.Info.InProgress;
		if (inProgress == this._prevInfo.InProgress)
		{
			return;
		}
		Action<bool> onProgressChanged = AlphaWarheadController.OnProgressChanged;
		if (onProgressChanged != null)
		{
			onProgressChanged(inProgress);
		}
		if (inProgress)
		{
			if (this._lastAnnouncer != null)
			{
				this._lastAnnouncer.Clear();
			}
			this._lastAnnouncer = this.CurScenario.Announcer;
			this._lastAnnouncer.OnStarted();
			return;
		}
		if (this._lastAnnouncer != null)
		{
			this._lastAnnouncer.OnCancelled();
		}
	}

	// Token: 0x0600017D RID: 381 RVA: 0x00006B78 File Offset: 0x00004D78
	public void ForceTime(float remaining)
	{
		this.InstantPrepare();
		this.StartDetonation(false, true, null);
		AlphaWarheadSyncInfo info = this.Info;
		remaining -= this.CurScenario.TotalTime;
		info.StartTime = NetworkTime.time + (double)remaining;
		this.NetworkInfo = info;
	}

	// Token: 0x0600017E RID: 382 RVA: 0x00006BC0 File Offset: 0x00004DC0
	public void InstantPrepare()
	{
		AlphaWarheadSyncInfo info = this.Info;
		info.StartTime = 0.0;
		this.NetworkInfo = info;
		this.NetworkCooldownEndTime = 0.0;
	}

	// Token: 0x0600017F RID: 383 RVA: 0x00006BFC File Offset: 0x00004DFC
	public void StartDetonation(bool isAutomatic = false, bool suppressSubtitles = false, ReferenceHub trigger = null)
	{
		if (this.Info.InProgress || this.CooldownEndTime > NetworkTime.time || this.IsLocked)
		{
			return;
		}
		AlphaWarheadSyncInfo alphaWarheadSyncInfo = this.Info;
		alphaWarheadSyncInfo.StartTime = NetworkTime.time;
		WarheadStartingEventArgs warheadStartingEventArgs = new WarheadStartingEventArgs((trigger == null) ? ReferenceHub.HostHub : trigger, isAutomatic, suppressSubtitles, alphaWarheadSyncInfo);
		WarheadEvents.OnStarting(warheadStartingEventArgs);
		if (!warheadStartingEventArgs.IsAllowed)
		{
			return;
		}
		isAutomatic = warheadStartingEventArgs.IsAutomatic;
		suppressSubtitles = warheadStartingEventArgs.SuppressSubtitles;
		alphaWarheadSyncInfo = warheadStartingEventArgs.WarheadState;
		trigger = warheadStartingEventArgs.Player.ReferenceHub;
		this._isAutomatic = isAutomatic;
		this.AlreadyDetonated = false;
		if (isAutomatic)
		{
			this.IsLocked |= this._autoDetonateLock;
			Broadcast broadcast;
			if (!this.AlreadyDetonated && !this.Info.InProgress && AlphaWarheadController.AutoWarheadBroadcastEnabled && AlphaWarheadController.TryGetBroadcaster(out broadcast))
			{
				broadcast.RpcAddElement(AlphaWarheadController.WarheadBroadcastMessage, AlphaWarheadController.WarheadBroadcastMessageTime, Broadcast.BroadcastFlags.Normal);
			}
			this._autoDetonate = false;
		}
		this._doorsAlreadyOpen = false;
		ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Countdown started.", ServerLogs.ServerLogType.GameEvent, false);
		this._triggeringPlayer = new Footprint(trigger);
		this.NetworkInfo = alphaWarheadSyncInfo;
		WarheadEvents.OnStarted(new WarheadStartedEventArgs((trigger == null) ? ReferenceHub.HostHub : trigger, isAutomatic, suppressSubtitles, alphaWarheadSyncInfo));
		if (suppressSubtitles)
		{
			return;
		}
		SubtitleType subtitle = this.CurScenario.Announcer.Subtitle;
		string text = this.CurScenario.TimeToDetonate.ToString();
		new SubtitleMessage(new SubtitlePart[]
		{
			new SubtitlePart(subtitle, new string[]
			{
				text
			})
		}).SendToAuthenticated(0);
	}

	// Token: 0x06000180 RID: 384 RVA: 0x00006D81 File Offset: 0x00004F81
	public void CancelDetonation()
	{
		this.CancelDetonation(null);
	}

	// Token: 0x06000181 RID: 385 RVA: 0x00006D8C File Offset: 0x00004F8C
	public void CancelDetonation(ReferenceHub disabler)
	{
		if (!this.Info.InProgress || AlphaWarheadController.TimeUntilDetonation <= 10f || this.IsLocked)
		{
			return;
		}
		AlphaWarheadSyncInfo alphaWarheadSyncInfo = this.Info;
		WarheadStoppingEventArgs warheadStoppingEventArgs = new WarheadStoppingEventArgs((disabler == null) ? ReferenceHub.HostHub : disabler, alphaWarheadSyncInfo);
		WarheadEvents.OnStopping(warheadStoppingEventArgs);
		if (!warheadStoppingEventArgs.IsAllowed)
		{
			return;
		}
		alphaWarheadSyncInfo = warheadStoppingEventArgs.WarheadState;
		disabler = warheadStoppingEventArgs.Player.ReferenceHub;
		ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Detonation cancelled.", ServerLogs.ServerLogType.GameEvent, false);
		if (AlphaWarheadController.TimeUntilDetonation <= 15f && disabler != null)
		{
			AchievementHandlerBase.ServerAchieve(disabler.connectionToClient, AchievementName.ThatWasClose);
		}
		alphaWarheadSyncInfo.StartTime = 0.0;
		int num = (int)Mathf.Min(AlphaWarheadController.TimeUntilDetonation, (float)this.CurScenario.TimeToDetonate);
		int num2 = int.MaxValue;
		alphaWarheadSyncInfo.ScenarioType = WarheadScenarioType.Resume;
		byte b = 0;
		while ((int)b < this.ResumeScenarios.Length)
		{
			int num3 = this.ResumeScenarios[(int)b].TimeToDetonate - num;
			if (num3 >= 0 && num3 <= num2)
			{
				num2 = num3;
				alphaWarheadSyncInfo.ScenarioId = b;
			}
			b += 1;
		}
		this.NetworkInfo = alphaWarheadSyncInfo;
		this.NetworkCooldownEndTime = NetworkTime.time + (double)this._cooldown;
		DoorEventOpenerExtension.TriggerAction(DoorEventOpenerExtension.OpenerEventType.WarheadCancel);
		if (!NetworkServer.active)
		{
			return;
		}
		this._isAutomatic = false;
		new SubtitleMessage(new SubtitlePart[]
		{
			new SubtitlePart(SubtitleType.AlphaWarheadCancelled, null)
		}).SendToAuthenticated(0);
		WarheadEvents.OnStopped(new WarheadStoppedEventArgs((disabler == null) ? ReferenceHub.HostHub : disabler, alphaWarheadSyncInfo));
	}

	// Token: 0x06000182 RID: 386 RVA: 0x00006F10 File Offset: 0x00005110
	private void Detonate()
	{
		ReferenceHub hub = (this._triggeringPlayer.Hub == null) ? ReferenceHub.HostHub : this._triggeringPlayer.Hub;
		WarheadDetonatingEventArgs warheadDetonatingEventArgs = new WarheadDetonatingEventArgs(hub);
		WarheadEvents.OnDetonating(warheadDetonatingEventArgs);
		if (!warheadDetonatingEventArgs.IsAllowed)
		{
			return;
		}
		this._triggeringPlayer = new Footprint(warheadDetonatingEventArgs.Player.ReferenceHub);
		Action onDetonated = AlphaWarheadController.OnDetonated;
		if (onDetonated != null)
		{
			onDetonated();
		}
		Broadcast broadcast;
		if (this._isAutomatic && !this.AlreadyDetonated && !this.Info.InProgress && AlphaWarheadController.AutoWarheadBroadcastEnabled && AlphaWarheadController.TryGetBroadcaster(out broadcast))
		{
			broadcast.RpcAddElement(AlphaWarheadController.WarheadExplodedBroadcastMessage, AlphaWarheadController.WarheadExplodedBroadcastMessageTime, Broadcast.BroadcastFlags.Normal);
		}
		ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Warhead detonated.", ServerLogs.ServerLogType.GameEvent, false);
		if (DecontaminationController.Singleton.DecontaminationOverride != DecontaminationController.DecontaminationStatus.Disabled)
		{
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, "LCZ decontamination has been disabled by detonation of the Alpha Warhead.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			DecontaminationController.Singleton.DecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;
		}
		this.AlreadyDetonated = true;
		HashSet<Team> hashSet = new HashSet<Team>();
		foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
		{
			PlayerRoleBase currentRole = referenceHub.roleManager.CurrentRole;
			if (referenceHub.IsAlive())
			{
				IFpcRole fpcRole = currentRole as IFpcRole;
				if (fpcRole == null || AlphaWarheadController.CanBeDetonated(fpcRole.FpcModule.Position, false))
				{
					hashSet.Add(referenceHub.GetTeam());
					if (referenceHub.playerStats.DealDamage(new WarheadDamageHandler()))
					{
						int warheadKills = this.WarheadKills;
						this.WarheadKills = warheadKills + 1;
					}
				}
			}
		}
		foreach (Scp244DeployablePickup scp244DeployablePickup in Scp244DeployablePickup.Instances)
		{
			if (AlphaWarheadController.CanBeDetonated(scp244DeployablePickup.transform.position, true))
			{
				scp244DeployablePickup.DestroySelf();
			}
		}
		foreach (DoorVariant doorVariant in DoorVariant.AllDoors)
		{
			ElevatorDoor elevatorDoor = doorVariant as ElevatorDoor;
			if (elevatorDoor != null)
			{
				ElevatorDoor elevatorDoor2 = elevatorDoor;
				elevatorDoor2.NetworkActiveLocks = (elevatorDoor2.ActiveLocks | 4);
			}
		}
		this.RpcShake(true);
		WarheadEvents.OnDetonated(new WarheadDetonatedEventArgs(hub));
	}

	// Token: 0x06000183 RID: 387 RVA: 0x00007168 File Offset: 0x00005368
	private static bool CanBeDetonated(Vector3 pos, bool includeOnlyLifts = false)
	{
		if (pos.GetZone() != FacilityZone.Surface && !includeOnlyLifts)
		{
			return true;
		}
		using (List<ElevatorChamber>.Enumerator enumerator = ElevatorChamber.AllChambers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.WorldspaceBounds.Contains(pos))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000184 RID: 388 RVA: 0x000071D8 File Offset: 0x000053D8
	private void OnConfigReloaded()
	{
		if (!AlphaWarheadController.InProgress)
		{
			return;
		}
		foreach (DoorVariant doorVariant in DoorVariant.AllDoors)
		{
			PryableDoor pryableDoor = doorVariant as PryableDoor;
			if (pryableDoor != null)
			{
				if (AlphaWarheadController.LockGatesOnCountdown)
				{
					pryableDoor.NetworkTargetState = true;
					pryableDoor.ServerChangeLock(DoorLockReason.Warhead, true);
				}
				else
				{
					pryableDoor.ServerChangeLock(DoorLockReason.Warhead, false);
				}
			}
		}
	}

	// Token: 0x06000185 RID: 389 RVA: 0x00007254 File Offset: 0x00005454
	[ClientRpc]
	public void RpcShake(bool achieve)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(achieve);
		this.SendRPCInternal("System.Void AlphaWarheadController::RpcShake(System.Boolean)", 1208415683, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000186 RID: 390 RVA: 0x00002E9A File Offset: 0x0000109A
	private void UpdateFog()
	{
	}

	// Token: 0x06000187 RID: 391 RVA: 0x00007290 File Offset: 0x00005490
	[ServerCallback]
	private void ServerUpdateAutonuke()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (!NetworkServer.active || !RoundStart.RoundStarted)
		{
			return;
		}
		if (!this._autoDetonate || this.AlreadyDetonated || this.Info.InProgress)
		{
			return;
		}
		if (RoundStart.RoundLength.TotalSeconds < (double)this._autoDetonateTime)
		{
			return;
		}
		this.StartDetonation(true, false, null);
	}

	// Token: 0x06000188 RID: 392 RVA: 0x000072F8 File Offset: 0x000054F8
	[ServerCallback]
	private void ServerUpdateDetonationTime()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (!NetworkServer.active || !this.Info.InProgress)
		{
			return;
		}
		if (!this._blastDoorsShut && AlphaWarheadController.TimeUntilDetonation < 2f)
		{
			this._blastDoorsShut = true;
			BlastDoor.Instances.ForEach(delegate(BlastDoor x)
			{
				x.ServerSetTargetState(false);
			});
		}
		if (this._openDoors && !this._doorsAlreadyOpen && AlphaWarheadController.TimeUntilDetonation < (float)this.CurScenario.TimeToDetonate)
		{
			this._doorsAlreadyOpen = true;
			DoorEventOpenerExtension.TriggerAction(DoorEventOpenerExtension.OpenerEventType.WarheadStart);
		}
		if (this.AlreadyDetonated || AlphaWarheadController.TimeUntilDetonation > 0f)
		{
			return;
		}
		this.Detonate();
	}

	// Token: 0x0600018A RID: 394 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x0600018B RID: 395 RVA: 0x000073B4 File Offset: 0x000055B4
	// (set) Token: 0x0600018C RID: 396 RVA: 0x000073C7 File Offset: 0x000055C7
	public AlphaWarheadSyncInfo NetworkInfo
	{
		get
		{
			return this.Info;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<AlphaWarheadSyncInfo>(value, ref this.Info, 1UL, null);
		}
	}

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x0600018D RID: 397 RVA: 0x000073E4 File Offset: 0x000055E4
	// (set) Token: 0x0600018E RID: 398 RVA: 0x000073F7 File Offset: 0x000055F7
	public double NetworkCooldownEndTime
	{
		get
		{
			return this.CooldownEndTime;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<double>(value, ref this.CooldownEndTime, 2UL, null);
		}
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00002E9A File Offset: 0x0000109A
	protected void UserCode_RpcShake__Boolean(bool achieve)
	{
	}

	// Token: 0x06000190 RID: 400 RVA: 0x00007411 File Offset: 0x00005611
	protected static void InvokeUserCode_RpcShake__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcShake called on server.");
			return;
		}
		((AlphaWarheadController)obj).UserCode_RpcShake__Boolean(reader.ReadBool());
	}

	// Token: 0x06000191 RID: 401 RVA: 0x0000743A File Offset: 0x0000563A
	static AlphaWarheadController()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(AlphaWarheadController), "System.Void AlphaWarheadController::RpcShake(System.Boolean)", new RemoteCallDelegate(AlphaWarheadController.InvokeUserCode_RpcShake__Boolean));
	}

	// Token: 0x06000192 RID: 402 RVA: 0x0000745C File Offset: 0x0000565C
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteAlphaWarheadSyncInfo(this.Info);
			writer.WriteDouble(this.CooldownEndTime);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteAlphaWarheadSyncInfo(this.Info);
		}
		if ((base.syncVarDirtyBits & 2UL) != 0UL)
		{
			writer.WriteDouble(this.CooldownEndTime);
		}
	}

	// Token: 0x06000193 RID: 403 RVA: 0x000074E4 File Offset: 0x000056E4
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<AlphaWarheadSyncInfo>(ref this.Info, null, reader.ReadAlphaWarheadSyncInfo());
			base.GeneratedSyncVarDeserialize<double>(ref this.CooldownEndTime, null, reader.ReadDouble());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<AlphaWarheadSyncInfo>(ref this.Info, null, reader.ReadAlphaWarheadSyncInfo());
		}
		if ((num & 2L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<double>(ref this.CooldownEndTime, null, reader.ReadDouble());
		}
	}

	// Token: 0x04000131 RID: 305
	public AlphaWarheadController.DetonationScenario[] StartScenarios;

	// Token: 0x04000132 RID: 306
	public AlphaWarheadController.DetonationScenario[] ResumeScenarios;

	// Token: 0x04000133 RID: 307
	public AlphaWarheadController.DetonationScenario DeadmanSwitchScenario;

	// Token: 0x04000134 RID: 308
	public byte DefaultScenarioId;

	// Token: 0x04000137 RID: 311
	[SyncVar]
	public AlphaWarheadSyncInfo Info;

	// Token: 0x04000138 RID: 312
	[SyncVar]
	public double CooldownEndTime;

	// Token: 0x0400013B RID: 315
	internal static bool AutoWarheadBroadcastEnabled;

	// Token: 0x0400013C RID: 316
	internal static string WarheadBroadcastMessage;

	// Token: 0x0400013D RID: 317
	internal static string WarheadExplodedBroadcastMessage;

	// Token: 0x0400013E RID: 318
	internal static ushort WarheadBroadcastMessageTime;

	// Token: 0x0400013F RID: 319
	internal static ushort WarheadExplodedBroadcastMessageTime;

	// Token: 0x04000140 RID: 320
	internal static bool LockGatesOnCountdown;

	// Token: 0x04000143 RID: 323
	public const float InevitableTime = 10f;

	// Token: 0x04000144 RID: 324
	private bool _doorsAlreadyOpen;

	// Token: 0x04000145 RID: 325
	private bool _blastDoorsShut;

	// Token: 0x04000146 RID: 326
	private bool _openDoors;

	// Token: 0x04000147 RID: 327
	private int _cooldown;

	// Token: 0x04000148 RID: 328
	private bool _isAutomatic;

	// Token: 0x04000149 RID: 329
	private bool _fogEnabled;

	// Token: 0x0400014A RID: 330
	private float _autoDetonateTime;

	// Token: 0x0400014B RID: 331
	private bool _autoDetonate;

	// Token: 0x0400014C RID: 332
	private bool _autoDetonateLock;

	// Token: 0x0400014D RID: 333
	private Footprint _triggeringPlayer;

	// Token: 0x0400014E RID: 334
	private AlphaWarheadSyncInfo _prevInfo;

	// Token: 0x0400014F RID: 335
	private AlphaWarheadScenarioAnnouncer _lastAnnouncer;

	// Token: 0x0200003C RID: 60
	[Serializable]
	public class DetonationScenario
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000194 RID: 404 RVA: 0x00007576 File Offset: 0x00005776
		public float TotalTime
		{
			get
			{
				return (float)this.TimeToDetonate + this.AdditionalTime;
			}
		}

		// Token: 0x04000150 RID: 336
		public AlphaWarheadScenarioAnnouncer Announcer;

		// Token: 0x04000151 RID: 337
		public int TimeToDetonate;

		// Token: 0x04000152 RID: 338
		public float AdditionalTime;
	}
}
