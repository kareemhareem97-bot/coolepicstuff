using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CentralAuth;
using GameCore;
using InventorySystem.Disarming;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using MapGeneration.Holidays;
using MEC;
using Mirror;
using Mirror.RemoteCalls;
using PlayerRoles;
using PlayerStatsSystem;
using RoundRestarting;
using UnityEngine;
using Utils.NonAllocLINQ;

// Token: 0x02000155 RID: 341
public class RoundSummary : NetworkBehaviour
{
	// Token: 0x17000104 RID: 260
	// (get) Token: 0x0600066D RID: 1645 RVA: 0x00018384 File Offset: 0x00016584
	public static bool SummaryActive
	{
		get
		{
			return RoundSummary._singletonSet && RoundSummary.singleton._summaryActive;
		}
	}

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x0600066E RID: 1646 RVA: 0x00018399 File Offset: 0x00016599
	// (set) Token: 0x0600066F RID: 1647 RVA: 0x000183A1 File Offset: 0x000165A1
	public int ExtraTargets
	{
		get
		{
			return this._extraTargets;
		}
		set
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this.Network_extraTargets = value;
		}
	}

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000670 RID: 1648 RVA: 0x000183B2 File Offset: 0x000165B2
	// (set) Token: 0x06000671 RID: 1649 RVA: 0x000183BA File Offset: 0x000165BA
	public int TargetCount
	{
		get
		{
			return this._targetCount;
		}
		set
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this.Network_targetCount = value;
		}
	}

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000672 RID: 1650 RVA: 0x000183CB File Offset: 0x000165CB
	// (set) Token: 0x06000673 RID: 1651 RVA: 0x000183D2 File Offset: 0x000165D2
	public static int Kills { get; private set; }

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x06000674 RID: 1652 RVA: 0x000183DA File Offset: 0x000165DA
	// (set) Token: 0x06000675 RID: 1653 RVA: 0x000183E1 File Offset: 0x000165E1
	public static int EscapedClassD { get; private set; }

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x06000676 RID: 1654 RVA: 0x000183E9 File Offset: 0x000165E9
	// (set) Token: 0x06000677 RID: 1655 RVA: 0x000183F0 File Offset: 0x000165F0
	public static int EscapedScientists { get; private set; }

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x06000678 RID: 1656 RVA: 0x000183F8 File Offset: 0x000165F8
	// (set) Token: 0x06000679 RID: 1657 RVA: 0x000183FF File Offset: 0x000165FF
	public static int SurvivingSCPs { get; private set; }

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x0600067A RID: 1658 RVA: 0x00018407 File Offset: 0x00016607
	// (set) Token: 0x0600067B RID: 1659 RVA: 0x0001840E File Offset: 0x0001660E
	public static int KilledBySCPs { get; private set; }

	// Token: 0x1700010C RID: 268
	// (get) Token: 0x0600067C RID: 1660 RVA: 0x00018416 File Offset: 0x00016616
	// (set) Token: 0x0600067D RID: 1661 RVA: 0x0001841D File Offset: 0x0001661D
	public static int ChangedIntoZombies { get; private set; }

	// Token: 0x1400001B RID: 27
	// (add) Token: 0x0600067E RID: 1662 RVA: 0x00018428 File Offset: 0x00016628
	// (remove) Token: 0x0600067F RID: 1663 RVA: 0x0001845C File Offset: 0x0001665C
	public static event RoundSummary.RoundEnded OnRoundEnded;

	// Token: 0x06000680 RID: 1664 RVA: 0x00018490 File Offset: 0x00016690
	private void Start()
	{
		RoundSummary.singleton = this;
		RoundSummary._singletonSet = true;
		this.IsRoundEnded = false;
		if (!NetworkServer.active)
		{
			return;
		}
		RoundSummary.roundTime = 0;
		this.KeepRoundOnOne = !ConfigFile.ServerConfig.GetBool("end_round_on_one_player", false);
		Timing.RunCoroutine(this._ProcessServerSideCode().CancelWith(base.gameObject), Segment.FixedUpdate);
		RoundSummary.KilledBySCPs = 0;
		RoundSummary.EscapedClassD = 0;
		RoundSummary.EscapedScientists = 0;
		RoundSummary.ChangedIntoZombies = 0;
		RoundSummary.Kills = 0;
		PlayerRoleManager.OnServerRoleSet += this.OnServerRoleSet;
		PlayerStats.OnAnyPlayerDied += this.OnAnyPlayerDied;
		this.UpdateTargetCount();
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x00018535 File Offset: 0x00016735
	private void OnDestroy()
	{
		RoundSummary._singletonSet = false;
		PlayerRoleManager.OnServerRoleSet -= this.OnServerRoleSet;
		PlayerStats.OnAnyPlayerDied -= this.OnAnyPlayerDied;
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x00018560 File Offset: 0x00016760
	private void OnAnyPlayerDied(ReferenceHub ply, DamageHandlerBase handler)
	{
		RoundSummary.Kills++;
		UniversalDamageHandler universalDamageHandler = handler as UniversalDamageHandler;
		if (universalDamageHandler != null)
		{
			if (universalDamageHandler.TranslationId != DeathTranslations.PocketDecay.Id)
			{
				return;
			}
		}
		else
		{
			AttackerDamageHandler attackerDamageHandler = handler as AttackerDamageHandler;
			if (attackerDamageHandler == null)
			{
				return;
			}
			PlayerRoleBase playerRoleBase;
			if (!attackerDamageHandler.Attacker.Role.TryGetRoleTemplate(out playerRoleBase))
			{
				return;
			}
			if (playerRoleBase.Team != Team.SCPs)
			{
				return;
			}
		}
		RoundSummary.KilledBySCPs++;
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x000185CC File Offset: 0x000167CC
	private void OnServerRoleSet(ReferenceHub userHub, RoleTypeId newRole, RoleChangeReason reason)
	{
		switch (reason)
		{
		case RoleChangeReason.RoundStart:
		case RoleChangeReason.LateJoin:
			this.ModifySpawnedTeam(userHub.GetTeam(), -1);
			this.ModifySpawnedTeam(newRole.GetTeam(), 1);
			break;
		case RoleChangeReason.Escaped:
			if (!userHub.inventory.IsDisarmed())
			{
				Team team = newRole.GetTeam();
				if (team != Team.FoundationForces)
				{
					if (team == Team.ChaosInsurgency)
					{
						RoundSummary.EscapedClassD++;
					}
				}
				else
				{
					RoundSummary.EscapedScientists++;
				}
			}
			break;
		case RoleChangeReason.Revived:
			if (newRole != RoleTypeId.Flamingo)
			{
				RoundSummary.ChangedIntoZombies++;
				this.classlistStart.zombies = this.classlistStart.zombies + 1;
			}
			break;
		}
		this.UpdateTargetCount();
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x0001867A File Offset: 0x0001687A
	private void UpdateTargetCount()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		this.TargetCount = ReferenceHub.AllHubs.Count(delegate(ReferenceHub hub)
		{
			Faction faction = hub.GetFaction();
			return faction == Faction.FoundationStaff || faction == Faction.FoundationEnemy || faction == Faction.Flamingos;
		});
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x000186B4 File Offset: 0x000168B4
	private void ModifySpawnedTeam(Team t, int modifyAmount)
	{
		switch (t)
		{
		case Team.SCPs:
			this.classlistStart.scps_except_zombies = this.classlistStart.scps_except_zombies + modifyAmount;
			return;
		case Team.FoundationForces:
			this.classlistStart.mtf_and_guards = this.classlistStart.mtf_and_guards + modifyAmount;
			return;
		case Team.ChaosInsurgency:
			this.classlistStart.chaos_insurgents = this.classlistStart.chaos_insurgents + modifyAmount;
			return;
		case Team.Scientists:
			this.classlistStart.scientists = this.classlistStart.scientists + modifyAmount;
			return;
		case Team.ClassD:
			this.classlistStart.class_ds = this.classlistStart.class_ds + modifyAmount;
			return;
		default:
			return;
		}
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x00018730 File Offset: 0x00016930
	public void ForceEnd()
	{
		this.IsRoundEnded = true;
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x00018739 File Offset: 0x00016939
	public void CancelRoundEnding()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (!this.IsRoundEnded)
		{
			return;
		}
		this.IsRoundEnded = false;
		Timing.KillCoroutines(new CoroutineHandle[]
		{
			this._roundEndCoroutine
		});
		this.RpcHideRoundSummary();
		this.RpcUndimScreen();
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x00018778 File Offset: 0x00016978
	public int CountRole(RoleTypeId role)
	{
		return ReferenceHub.AllHubs.Count((ReferenceHub x) => x.GetRoleId() == role);
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x000187A8 File Offset: 0x000169A8
	public int CountTeam(Team team)
	{
		return ReferenceHub.AllHubs.Count((ReferenceHub x) => x.GetTeam() == team);
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x000187D8 File Offset: 0x000169D8
	private IEnumerator<float> _ProcessServerSideCode()
	{
		float time = Time.unscaledTime;
		while (this != null)
		{
			yield return Timing.WaitForSeconds(2.5f);
			if (NetworkServer.active)
			{
				this.UpdateTargetCount();
			}
			if (!this.IsRoundEnded)
			{
				if (RoundSummary.RoundLock)
				{
					continue;
				}
				if (this.KeepRoundOnOne)
				{
					if (ReferenceHub.AllHubs.Count((ReferenceHub x) => x.authManager.InstanceMode != ClientInstanceMode.DedicatedServer) < 2)
					{
						continue;
					}
				}
				if (!RoundSummary.RoundInProgress() || Time.unscaledTime - time < 15f)
				{
					continue;
				}
			}
			RoundSummary.SumInfo_ClassList newList = default(RoundSummary.SumInfo_ClassList);
			foreach (ReferenceHub hub in ReferenceHub.AllHubs)
			{
				switch (hub.GetTeam())
				{
				case Team.SCPs:
					if (hub.GetRoleId() == RoleTypeId.Scp0492)
					{
						newList.zombies++;
					}
					else
					{
						newList.scps_except_zombies++;
					}
					break;
				case Team.FoundationForces:
					newList.mtf_and_guards++;
					break;
				case Team.ChaosInsurgency:
					newList.chaos_insurgents++;
					break;
				case Team.Scientists:
					newList.scientists++;
					break;
				case Team.ClassD:
					newList.class_ds++;
					break;
				case Team.Flamingos:
					newList.flamingos++;
					break;
				}
			}
			yield return float.NegativeInfinity;
			newList.warhead_kills = (AlphaWarheadController.Detonated ? AlphaWarheadController.Singleton.WarheadKills : -1);
			yield return float.NegativeInfinity;
			int num = newList.mtf_and_guards + newList.scientists;
			int num2 = newList.chaos_insurgents + newList.class_ds;
			int num3 = newList.scps_except_zombies + newList.zombies;
			int num4 = newList.class_ds + RoundSummary.EscapedClassD;
			int num5 = newList.scientists + RoundSummary.EscapedScientists;
			int flamingos = newList.flamingos;
			RoundSummary.SurvivingSCPs = newList.scps_except_zombies;
			float num6 = (float)((this.classlistStart.class_ds == 0) ? 0 : (num4 / this.classlistStart.class_ds));
			float num7 = (float)((this.classlistStart.scientists == 0) ? 1 : (num5 / this.classlistStart.scientists));
			int num8 = 0;
			if (num > 0)
			{
				num8++;
			}
			if (num2 > 0)
			{
				num8++;
			}
			if (num3 > 0)
			{
				num8++;
			}
			if (flamingos > 0)
			{
				num8++;
			}
			if (this.ExtraTargets > 0)
			{
				num8++;
			}
			this.IsRoundEnded = (num8 <= 1);
			RoundEndingConditionsCheckEventArgs roundEndingConditionsCheckEventArgs = new RoundEndingConditionsCheckEventArgs(this.IsRoundEnded);
			ServerEvents.OnRoundEndingConditionsCheck(roundEndingConditionsCheckEventArgs);
			this.IsRoundEnded = roundEndingConditionsCheckEventArgs.CanEnd;
			if (this.IsRoundEnded)
			{
				bool flag = num > 0;
				bool flag2 = num2 > 0;
				bool flag3 = num3 > 0;
				bool flag4 = flamingos > 0;
				RoundSummary.LeadingTeam leadingTeam = RoundSummary.LeadingTeam.Draw;
				if (flag)
				{
					leadingTeam = ((RoundSummary.EscapedScientists >= RoundSummary.EscapedClassD) ? RoundSummary.LeadingTeam.FacilityForces : RoundSummary.LeadingTeam.Draw);
				}
				else if (flag3 || (flag3 && flag2))
				{
					leadingTeam = ((RoundSummary.EscapedClassD > RoundSummary.SurvivingSCPs) ? RoundSummary.LeadingTeam.ChaosInsurgency : ((RoundSummary.SurvivingSCPs > RoundSummary.EscapedScientists) ? RoundSummary.LeadingTeam.Anomalies : RoundSummary.LeadingTeam.Draw));
				}
				else if (flag2)
				{
					leadingTeam = ((RoundSummary.EscapedClassD >= RoundSummary.EscapedScientists) ? RoundSummary.LeadingTeam.ChaosInsurgency : RoundSummary.LeadingTeam.Draw);
				}
				else if (flag4)
				{
					leadingTeam = RoundSummary.LeadingTeam.Flamingos;
				}
				RoundEndingEventArgs roundEndingEventArgs = new RoundEndingEventArgs(leadingTeam);
				ServerEvents.OnRoundEnding(roundEndingEventArgs);
				if (roundEndingEventArgs.IsAllowed)
				{
					leadingTeam = roundEndingEventArgs.LeadingTeam;
					RoundSummary.RoundEnded onRoundEnded = RoundSummary.OnRoundEnded;
					if (onRoundEnded != null)
					{
						onRoundEnded(leadingTeam, newList);
					}
					FriendlyFireConfig.PauseDetector = true;
					string text = string.Concat(new string[]
					{
						"Round finished! Anomalies: ",
						num3.ToString(),
						" | Chaos: ",
						num2.ToString(),
						" | Facility Forces: ",
						num.ToString(),
						" | D escaped percentage: ",
						num6.ToString(),
						" | S escaped percentage: ",
						num7.ToString(),
						"."
					});
					GameCore.Console.AddLog(text, Color.gray, false, ConsoleLogType.Log);
					ServerLogs.AddLog(ServerLogs.Modules.GameLogic, text, ServerLogs.ServerLogType.GameEvent, false);
					yield return Timing.WaitForSeconds(1.5f);
					RoundEndedEventArgs roundEndedEventArgs = new RoundEndedEventArgs(leadingTeam);
					ServerEvents.OnRoundEnded(roundEndedEventArgs);
					bool showSummary = roundEndedEventArgs.ShowSummary;
					int num9 = Mathf.Clamp(ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000);
					if (this != null && showSummary)
					{
						this.RpcShowRoundSummary(this.classlistStart, newList, leadingTeam, RoundSummary.EscapedClassD, RoundSummary.EscapedScientists, RoundSummary.KilledBySCPs, num9, (int)RoundStart.RoundLength.TotalSeconds);
					}
					this._roundEndCoroutine = Timing.RunCoroutine(this.InitiateRoundEnd(num9), Segment.FixedUpdate);
					yield return Timing.WaitUntilDone(this._roundEndCoroutine);
				}
			}
		}
		yield break;
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x000187E7 File Offset: 0x000169E7
	private IEnumerator<float> InitiateRoundEnd(int timeToRoundRestart)
	{
		yield return Timing.WaitForSeconds((float)(timeToRoundRestart - 1));
		if (!this.IsRoundEnded)
		{
			yield break;
		}
		this.RpcDimScreen();
		yield return Timing.WaitForSeconds(1f);
		RoundRestart.InitiateRoundRestart();
		yield break;
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x00018800 File Offset: 0x00016A00
	[ClientRpc]
	private void RpcShowRoundSummary(RoundSummary.SumInfo_ClassList listStart, RoundSummary.SumInfo_ClassList listFinish, RoundSummary.LeadingTeam leadingTeam, int eDS, int eSc, int scpKills, int roundCd, int seconds)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		Mirror.GeneratedNetworkCode._Write_RoundSummary/SumInfo_ClassList(writer, listStart);
		Mirror.GeneratedNetworkCode._Write_RoundSummary/SumInfo_ClassList(writer, listFinish);
		Mirror.GeneratedNetworkCode._Write_RoundSummary/LeadingTeam(writer, leadingTeam);
		writer.WriteInt(eDS);
		writer.WriteInt(eSc);
		writer.WriteInt(scpKills);
		writer.WriteInt(roundCd);
		writer.WriteInt(seconds);
		this.SendRPCInternal("System.Void RoundSummary::RpcShowRoundSummary(RoundSummary/SumInfo_ClassList,RoundSummary/SumInfo_ClassList,RoundSummary/LeadingTeam,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", -173396156, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x00018880 File Offset: 0x00016A80
	[ClientRpc]
	private void RpcHideRoundSummary()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void RoundSummary::RpcHideRoundSummary()", 593541252, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x000188B0 File Offset: 0x00016AB0
	[ClientRpc]
	private void RpcDimScreen()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void RoundSummary::RpcDimScreen()", -1745793588, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x000188E0 File Offset: 0x00016AE0
	[ClientRpc]
	private void RpcUndimScreen()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void RoundSummary::RpcUndimScreen()", 44991443, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x00018910 File Offset: 0x00016B10
	public static bool RoundInProgress()
	{
		ReferenceHub referenceHub;
		return RoundSummary._singletonSet && ReferenceHub.TryGetHostHub(out referenceHub) && referenceHub.characterClassManager.RoundStarted;
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000693 RID: 1683 RVA: 0x0001893C File Offset: 0x00016B3C
	// (set) Token: 0x06000694 RID: 1684 RVA: 0x0001894F File Offset: 0x00016B4F
	public int Network_extraTargets
	{
		get
		{
			return this._extraTargets;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<int>(value, ref this._extraTargets, 1UL, null);
		}
	}

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000695 RID: 1685 RVA: 0x0001896C File Offset: 0x00016B6C
	// (set) Token: 0x06000696 RID: 1686 RVA: 0x0001897F File Offset: 0x00016B7F
	public int Network_targetCount
	{
		get
		{
			return this._targetCount;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<int>(value, ref this._targetCount, 2UL, null);
		}
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x00018999 File Offset: 0x00016B99
	protected void UserCode_RpcShowRoundSummary__SumInfo_ClassList__SumInfo_ClassList__LeadingTeam__Int32__Int32__Int32__Int32__Int32(RoundSummary.SumInfo_ClassList listStart, RoundSummary.SumInfo_ClassList listFinish, RoundSummary.LeadingTeam leadingTeam, int eDS, int eSc, int scpKills, int roundCd, int seconds)
	{
		this._summaryActive = true;
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x000189A4 File Offset: 0x00016BA4
	protected static void InvokeUserCode_RpcShowRoundSummary__SumInfo_ClassList__SumInfo_ClassList__LeadingTeam__Int32__Int32__Int32__Int32__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcShowRoundSummary called on server.");
			return;
		}
		((RoundSummary)obj).UserCode_RpcShowRoundSummary__SumInfo_ClassList__SumInfo_ClassList__LeadingTeam__Int32__Int32__Int32__Int32__Int32(Mirror.GeneratedNetworkCode._Read_RoundSummary/SumInfo_ClassList(reader), Mirror.GeneratedNetworkCode._Read_RoundSummary/SumInfo_ClassList(reader), Mirror.GeneratedNetworkCode._Read_RoundSummary/LeadingTeam(reader), reader.ReadInt(), reader.ReadInt(), reader.ReadInt(), reader.ReadInt(), reader.ReadInt());
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x00018A02 File Offset: 0x00016C02
	protected void UserCode_RpcHideRoundSummary()
	{
		this._summaryActive = false;
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x00018A0B File Offset: 0x00016C0B
	protected static void InvokeUserCode_RpcHideRoundSummary(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcHideRoundSummary called on server.");
			return;
		}
		((RoundSummary)obj).UserCode_RpcHideRoundSummary();
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x00002E9A File Offset: 0x0000109A
	protected void UserCode_RpcDimScreen()
	{
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x00018A2E File Offset: 0x00016C2E
	protected static void InvokeUserCode_RpcDimScreen(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcDimScreen called on server.");
			return;
		}
		((RoundSummary)obj).UserCode_RpcDimScreen();
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x00002E9A File Offset: 0x0000109A
	protected void UserCode_RpcUndimScreen()
	{
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x00018A51 File Offset: 0x00016C51
	protected static void InvokeUserCode_RpcUndimScreen(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcUndimScreen called on server.");
			return;
		}
		((RoundSummary)obj).UserCode_RpcUndimScreen();
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x00018A74 File Offset: 0x00016C74
	static RoundSummary()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(RoundSummary), "System.Void RoundSummary::RpcShowRoundSummary(RoundSummary/SumInfo_ClassList,RoundSummary/SumInfo_ClassList,RoundSummary/LeadingTeam,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)", new RemoteCallDelegate(RoundSummary.InvokeUserCode_RpcShowRoundSummary__SumInfo_ClassList__SumInfo_ClassList__LeadingTeam__Int32__Int32__Int32__Int32__Int32));
		RemoteProcedureCalls.RegisterRpc(typeof(RoundSummary), "System.Void RoundSummary::RpcHideRoundSummary()", new RemoteCallDelegate(RoundSummary.InvokeUserCode_RpcHideRoundSummary));
		RemoteProcedureCalls.RegisterRpc(typeof(RoundSummary), "System.Void RoundSummary::RpcDimScreen()", new RemoteCallDelegate(RoundSummary.InvokeUserCode_RpcDimScreen));
		RemoteProcedureCalls.RegisterRpc(typeof(RoundSummary), "System.Void RoundSummary::RpcUndimScreen()", new RemoteCallDelegate(RoundSummary.InvokeUserCode_RpcUndimScreen));
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00018B04 File Offset: 0x00016D04
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(this._extraTargets);
			writer.WriteInt(this._targetCount);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteInt(this._extraTargets);
		}
		if ((base.syncVarDirtyBits & 2UL) != 0UL)
		{
			writer.WriteInt(this._targetCount);
		}
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x00018B8C File Offset: 0x00016D8C
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<int>(ref this._extraTargets, null, reader.ReadInt());
			base.GeneratedSyncVarDeserialize<int>(ref this._targetCount, null, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<int>(ref this._extraTargets, null, reader.ReadInt());
		}
		if ((num & 2L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<int>(ref this._targetCount, null, reader.ReadInt());
		}
	}

	// Token: 0x040005F2 RID: 1522
	[SyncVar]
	private int _extraTargets;

	// Token: 0x040005F3 RID: 1523
	[SyncVar]
	private int _targetCount;

	// Token: 0x040005F4 RID: 1524
	private bool _summaryActive;

	// Token: 0x040005F5 RID: 1525
	private CoroutineHandle _roundEndCoroutine;

	// Token: 0x040005F6 RID: 1526
	public bool KeepRoundOnOne;

	// Token: 0x040005F7 RID: 1527
	public bool IsRoundEnded;

	// Token: 0x040005F8 RID: 1528
	public RoundSummary.SumInfo_ClassList classlistStart;

	// Token: 0x040005F9 RID: 1529
	public GameObject ui_root;

	// Token: 0x040005FA RID: 1530
	public HolidayAudioClip RoundEndSfx;

	// Token: 0x040005FB RID: 1531
	public static bool RoundLock;

	// Token: 0x040005FC RID: 1532
	public static RoundSummary singleton;

	// Token: 0x040005FD RID: 1533
	private static bool _singletonSet;

	// Token: 0x04000605 RID: 1541
	public static int roundTime;

	// Token: 0x02000156 RID: 342
	// (Invoke) Token: 0x060006A3 RID: 1699
	public delegate void RoundEnded(RoundSummary.LeadingTeam leadingTeam, RoundSummary.SumInfo_ClassList sumInfo);

	// Token: 0x02000157 RID: 343
	public enum LeadingTeam : byte
	{
		// Token: 0x04000607 RID: 1543
		FacilityForces,
		// Token: 0x04000608 RID: 1544
		ChaosInsurgency,
		// Token: 0x04000609 RID: 1545
		Anomalies,
		// Token: 0x0400060A RID: 1546
		Flamingos,
		// Token: 0x0400060B RID: 1547
		Draw
	}

	// Token: 0x02000158 RID: 344
	[Serializable]
	public struct SumInfo_ClassList : IEquatable<RoundSummary.SumInfo_ClassList>
	{
		// Token: 0x060006A6 RID: 1702 RVA: 0x00018C20 File Offset: 0x00016E20
		public bool Equals(RoundSummary.SumInfo_ClassList other)
		{
			return this.class_ds == other.class_ds && this.scientists == other.scientists && this.chaos_insurgents == other.chaos_insurgents && this.mtf_and_guards == other.mtf_and_guards && this.scps_except_zombies == other.scps_except_zombies && this.zombies == other.zombies && this.warhead_kills == other.warhead_kills && this.flamingos == other.flamingos;
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x00018CA0 File Offset: 0x00016EA0
		public override bool Equals(object obj)
		{
			if (obj is RoundSummary.SumInfo_ClassList)
			{
				RoundSummary.SumInfo_ClassList other = (RoundSummary.SumInfo_ClassList)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x00018CC8 File Offset: 0x00016EC8
		public override int GetHashCode()
		{
			return ((((((this.class_ds * 397 ^ this.scientists) * 397 ^ this.chaos_insurgents) * 397 ^ this.mtf_and_guards) * 397 ^ this.scps_except_zombies) * 397 ^ this.zombies) * 397 ^ this.warhead_kills) * 397 ^ this.flamingos;
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x00018D36 File Offset: 0x00016F36
		public static bool operator ==(RoundSummary.SumInfo_ClassList left, RoundSummary.SumInfo_ClassList right)
		{
			return left.Equals(right);
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x00018D40 File Offset: 0x00016F40
		public static bool operator !=(RoundSummary.SumInfo_ClassList left, RoundSummary.SumInfo_ClassList right)
		{
			return !left.Equals(right);
		}

		// Token: 0x0400060C RID: 1548
		public int class_ds;

		// Token: 0x0400060D RID: 1549
		public int scientists;

		// Token: 0x0400060E RID: 1550
		public int chaos_insurgents;

		// Token: 0x0400060F RID: 1551
		public int mtf_and_guards;

		// Token: 0x04000610 RID: 1552
		public int scps_except_zombies;

		// Token: 0x04000611 RID: 1553
		public int zombies;

		// Token: 0x04000612 RID: 1554
		public int warhead_kills;

		// Token: 0x04000613 RID: 1555
		public int flamingos;
	}
}
