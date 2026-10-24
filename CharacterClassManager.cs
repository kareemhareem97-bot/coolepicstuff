using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CentralAuth;
using Cmdbinding;
using GameCore;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using MEC;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using Mirror.RemoteCalls;
using PlayerStatsSystem;
using Security;
using ServerOutput;
using UnityEngine;

// Token: 0x02000113 RID: 275
public class CharacterClassManager : NetworkBehaviour
{
	// Token: 0x170000CF RID: 207
	// (get) Token: 0x06000520 RID: 1312 RVA: 0x000130D3 File Offset: 0x000112D3
	// (set) Token: 0x06000521 RID: 1313 RVA: 0x000130DB File Offset: 0x000112DB
	public bool GodMode
	{
		get
		{
			return this._godMode;
		}
		set
		{
			this._godMode = value;
			this._hub.playerStats.GetModule<AdminFlagsStat>().SetFlag(AdminFlags.GodMode, value);
		}
	}

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x06000522 RID: 1314 RVA: 0x000130FC File Offset: 0x000112FC
	// (remove) Token: 0x06000523 RID: 1315 RVA: 0x00013130 File Offset: 0x00011330
	public static event Action OnRoundStarted;

	// Token: 0x14000011 RID: 17
	// (add) Token: 0x06000524 RID: 1316 RVA: 0x00013164 File Offset: 0x00011364
	// (remove) Token: 0x06000525 RID: 1317 RVA: 0x00013198 File Offset: 0x00011398
	public static event Action ServerOnRoundStartTriggered;

	// Token: 0x14000012 RID: 18
	// (add) Token: 0x06000526 RID: 1318 RVA: 0x000131CC File Offset: 0x000113CC
	// (remove) Token: 0x06000527 RID: 1319 RVA: 0x00013200 File Offset: 0x00011400
	public static event Action ServerWaitingForPlayers;

	// Token: 0x14000013 RID: 19
	// (add) Token: 0x06000528 RID: 1320 RVA: 0x00013234 File Offset: 0x00011434
	// (remove) Token: 0x06000529 RID: 1321 RVA: 0x00013268 File Offset: 0x00011468
	public static event Action<ushort> OnMaxPlayersChanged;

	// Token: 0x0600052A RID: 1322 RVA: 0x0001329C File Offset: 0x0001149C
	private void Start()
	{
		this._hub = ReferenceHub.GetHub(this);
		if (!NetworkServer.active)
		{
			return;
		}
		this._commandRateLimit = this._hub.playerRateLimitHandler.RateLimits[1];
		if (!base.isLocalPlayer)
		{
			return;
		}
		ServerLogs.StartLogging();
		FriendlyFireConfig.PauseDetector = false;
		CustomLiteNetLib4MirrorTransport.DelayConnections = false;
		IdleMode.PauseIdleMode = false;
		ServerConsole.AddOutputEntry(default(RoundRestartedEntry));
		this.NetworkPastebin = ConfigFile.ServerConfig.GetString("serverinfo_pastebin_id", "");
		if (NetworkServer.active)
		{
			Action serverWaitingForPlayers = CharacterClassManager.ServerWaitingForPlayers;
			if (serverWaitingForPlayers != null)
			{
				serverWaitingForPlayers();
			}
			ServerEvents.OnWaitingForPlayers();
			ServerConsole.AddLog("Waiting for players...", ConsoleColor.Gray, false);
		}
		base.StartCoroutine(this.Init());
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x00013357 File Offset: 0x00011557
	private void Update()
	{
		if (base.isLocalPlayer && NetworkServer.active && this.MaxPlayers != (ushort)CustomNetworkManager.slots)
		{
			this.NetworkMaxPlayers = (ushort)CustomNetworkManager.slots;
		}
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x00013382 File Offset: 0x00011582
	private void MaxPlayersHook(ushort prev, ushort cur)
	{
		if (prev == cur)
		{
			return;
		}
		Action<ushort> onMaxPlayersChanged = CharacterClassManager.OnMaxPlayersChanged;
		if (onMaxPlayersChanged == null)
		{
			return;
		}
		onMaxPlayersChanged(cur);
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x0001339C File Offset: 0x0001159C
	public void SyncServerCmdBinding()
	{
		if (!base.isServer || !CharacterClassManager.EnableSyncServerCmdBinding)
		{
			return;
		}
		foreach (Bind bind in CmdBinding.Bindings)
		{
			if (bind.Command.StartsWith('.') || bind.Command.StartsWith('/'))
			{
				this.TargetChangeCmdBinding(bind.Key, bind.Command);
			}
		}
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x00013428 File Offset: 0x00011628
	[TargetRpc]
	public void TargetChangeCmdBinding(KeyCode code, string cmd)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		Mirror.GeneratedNetworkCode._Write_UnityEngine.KeyCode(writer, code);
		writer.WriteString(cmd);
		this.SendTargetRPCInternal(null, "System.Void CharacterClassManager::TargetChangeCmdBinding(UnityEngine.KeyCode,System.String)", -1033122126, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x0001346C File Offset: 0x0001166C
	private IEnumerator Init()
	{
		if (NonFacilityCompatibility.currentSceneSettings.roundAutostart)
		{
			CharacterClassManager.ForceRoundStart();
		}
		else
		{
			short originalTimeLeft = ConfigFile.ServerConfig.GetShort("lobby_waiting_time", 20);
			short timeLeft = originalTimeLeft;
			int topPlayers = 2;
			while (RoundStart.singleton.Timer != -1)
			{
				if (timeLeft == -2)
				{
					timeLeft = originalTimeLeft;
				}
				int playerCount = ReferenceHub.GetPlayerCount(ClientInstanceMode.ReadyClient, ClientInstanceMode.Host, ClientInstanceMode.Dummy);
				if (!RoundStart.LobbyLock && playerCount > 1)
				{
					if (playerCount > topPlayers)
					{
						topPlayers = playerCount;
						if (timeLeft < originalTimeLeft)
						{
							do
							{
								short num = timeLeft;
								timeLeft = num + 1;
								if (timeLeft % 5 != 0)
								{
									break;
								}
							}
							while (timeLeft < originalTimeLeft);
						}
					}
					else
					{
						short num = timeLeft;
						timeLeft = num - 1;
					}
					if (playerCount >= ((CustomNetworkManager)NetworkManager.singleton).ReservedMaxPlayers)
					{
						timeLeft = -1;
					}
					if (timeLeft == -1)
					{
						CharacterClassManager.ForceRoundStart();
					}
				}
				else
				{
					timeLeft = -2;
				}
				if (RoundStart.singleton.Timer != -1)
				{
					RoundStart.singleton.NetworkTimer = timeLeft;
				}
				yield return new WaitForSeconds(1f);
			}
		}
		this.NetworkRoundStarted = true;
		ServerEvents.OnRoundStarted();
		Action serverOnRoundStartTriggered = CharacterClassManager.ServerOnRoundStartTriggered;
		if (serverOnRoundStartTriggered != null)
		{
			serverOnRoundStartTriggered();
		}
		this.RpcRoundStarted();
		yield break;
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x0001347C File Offset: 0x0001167C
	public static bool ForceRoundStart()
	{
		if (!NetworkServer.active)
		{
			return false;
		}
		RoundStartingEventArgs roundStartingEventArgs = new RoundStartingEventArgs();
		ServerEvents.OnRoundStarting(roundStartingEventArgs);
		if (!roundStartingEventArgs.IsAllowed)
		{
			return false;
		}
		ServerLogs.AddLog(ServerLogs.Modules.GameLogic, "Round has been started.", ServerLogs.ServerLogType.GameEvent, false);
		ServerConsole.AddLog("New round has been started.", ConsoleColor.Gray, false);
		RoundStart.singleton.NetworkTimer = -1;
		RoundStart.RoundStartTimer.Restart();
		return true;
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x000134D8 File Offset: 0x000116D8
	[TargetRpc]
	private void TargetSetDisconnectError(NetworkConnection conn, string message)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(message);
		this.SendTargetRPCInternal(conn, "System.Void CharacterClassManager::TargetSetDisconnectError(Mirror.NetworkConnection,System.String)", -2106075371, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x00013514 File Offset: 0x00011714
	[Command(channel = 4)]
	private void CmdConfirmDisconnect()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		base.SendCommandInternal("System.Void CharacterClassManager::CmdConfirmDisconnect()", 460932189, writer, 4, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x00013544 File Offset: 0x00011744
	public void DisconnectClient(NetworkConnection conn, string message)
	{
		this.TargetSetDisconnectError(conn, message);
		Timing.RunCoroutine(CharacterClassManager._DisconnectAfterTimeout(conn), Segment.FixedUpdate);
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x0001355B File Offset: 0x0001175B
	private static IEnumerator<float> _DisconnectAfterTimeout(NetworkConnection conn)
	{
		int num;
		for (int i = 0; i < 150; i = num + 1)
		{
			yield return 0f;
			num = i;
		}
		if (conn != null)
		{
			conn.Disconnect();
		}
		yield break;
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x0001356C File Offset: 0x0001176C
	[ClientRpc]
	private void RpcRoundStarted()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void CharacterClassManager::RpcRoundStarted()", -2093950497, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x0001359C File Offset: 0x0001179C
	static CharacterClassManager()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(CharacterClassManager), "System.Void CharacterClassManager::CmdConfirmDisconnect()", new RemoteCallDelegate(CharacterClassManager.InvokeUserCode_CmdConfirmDisconnect), true);
		RemoteProcedureCalls.RegisterRpc(typeof(CharacterClassManager), "System.Void CharacterClassManager::RpcRoundStarted()", new RemoteCallDelegate(CharacterClassManager.InvokeUserCode_RpcRoundStarted));
		RemoteProcedureCalls.RegisterRpc(typeof(CharacterClassManager), "System.Void CharacterClassManager::TargetChangeCmdBinding(UnityEngine.KeyCode,System.String)", new RemoteCallDelegate(CharacterClassManager.InvokeUserCode_TargetChangeCmdBinding__KeyCode__String));
		RemoteProcedureCalls.RegisterRpc(typeof(CharacterClassManager), "System.Void CharacterClassManager::TargetSetDisconnectError(Mirror.NetworkConnection,System.String)", new RemoteCallDelegate(CharacterClassManager.InvokeUserCode_TargetSetDisconnectError__NetworkConnection__String));
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x06000539 RID: 1337 RVA: 0x00013630 File Offset: 0x00011830
	// (set) Token: 0x0600053A RID: 1338 RVA: 0x00013643 File Offset: 0x00011843
	public string NetworkPastebin
	{
		get
		{
			return this.Pastebin;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<string>(value, ref this.Pastebin, 1UL, null);
		}
	}

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x0600053B RID: 1339 RVA: 0x00013660 File Offset: 0x00011860
	// (set) Token: 0x0600053C RID: 1340 RVA: 0x00013673 File Offset: 0x00011873
	public ushort NetworkMaxPlayers
	{
		get
		{
			return this.MaxPlayers;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<ushort>(value, ref this.MaxPlayers, 2UL, new Action<ushort, ushort>(this.MaxPlayersHook));
		}
	}

	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x0600053D RID: 1341 RVA: 0x00013698 File Offset: 0x00011898
	// (set) Token: 0x0600053E RID: 1342 RVA: 0x000136AB File Offset: 0x000118AB
	public bool NetworkRoundStarted
	{
		get
		{
			return this.RoundStarted;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<bool>(value, ref this.RoundStarted, 4UL, null);
		}
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x00002E9A File Offset: 0x0000109A
	protected void UserCode_TargetChangeCmdBinding__KeyCode__String(KeyCode code, string cmd)
	{
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x000136C5 File Offset: 0x000118C5
	protected static void InvokeUserCode_TargetChangeCmdBinding__KeyCode__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC TargetChangeCmdBinding called on server.");
			return;
		}
		((CharacterClassManager)obj).UserCode_TargetChangeCmdBinding__KeyCode__String(Mirror.GeneratedNetworkCode._Read_UnityEngine.KeyCode(reader), reader.ReadString());
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x000136F4 File Offset: 0x000118F4
	protected void UserCode_TargetSetDisconnectError__NetworkConnection__String(NetworkConnection conn, string message)
	{
		((CustomNetworkManager)LiteNetLib4MirrorNetworkManager.singleton).disconnectMessage = message;
		this.CmdConfirmDisconnect();
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x0001370C File Offset: 0x0001190C
	protected static void InvokeUserCode_TargetSetDisconnectError__NetworkConnection__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC TargetSetDisconnectError called on server.");
			return;
		}
		((CharacterClassManager)obj).UserCode_TargetSetDisconnectError__NetworkConnection__String(null, reader.ReadString());
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x00013736 File Offset: 0x00011936
	protected void UserCode_CmdConfirmDisconnect()
	{
		NetworkConnectionToClient connectionToClient = base.connectionToClient;
		if (connectionToClient == null)
		{
			return;
		}
		connectionToClient.Disconnect();
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x00013748 File Offset: 0x00011948
	protected static void InvokeUserCode_CmdConfirmDisconnect(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdConfirmDisconnect called on client.");
			return;
		}
		((CharacterClassManager)obj).UserCode_CmdConfirmDisconnect();
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x0001376B File Offset: 0x0001196B
	protected void UserCode_RpcRoundStarted()
	{
		Action onRoundStarted = CharacterClassManager.OnRoundStarted;
		if (onRoundStarted == null)
		{
			return;
		}
		onRoundStarted();
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x0001377C File Offset: 0x0001197C
	protected static void InvokeUserCode_RpcRoundStarted(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcRoundStarted called on server.");
			return;
		}
		((CharacterClassManager)obj).UserCode_RpcRoundStarted();
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x000137A0 File Offset: 0x000119A0
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteString(this.Pastebin);
			writer.WriteUShort(this.MaxPlayers);
			writer.WriteBool(this.RoundStarted);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteString(this.Pastebin);
		}
		if ((base.syncVarDirtyBits & 2UL) != 0UL)
		{
			writer.WriteUShort(this.MaxPlayers);
		}
		if ((base.syncVarDirtyBits & 4UL) != 0UL)
		{
			writer.WriteBool(this.RoundStarted);
		}
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x00013854 File Offset: 0x00011A54
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this.Pastebin, null, reader.ReadString());
			base.GeneratedSyncVarDeserialize<ushort>(ref this.MaxPlayers, new Action<ushort, ushort>(this.MaxPlayersHook), reader.ReadUShort());
			base.GeneratedSyncVarDeserialize<bool>(ref this.RoundStarted, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this.Pastebin, null, reader.ReadString());
		}
		if ((num & 2L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<ushort>(ref this.MaxPlayers, new Action<ushort, ushort>(this.MaxPlayersHook), reader.ReadUShort());
		}
		if ((num & 4L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this.RoundStarted, null, reader.ReadBool());
		}
	}

	// Token: 0x0400045D RID: 1117
	private ReferenceHub _hub;

	// Token: 0x0400045E RID: 1118
	[NonSerialized]
	private bool _godMode;

	// Token: 0x0400045F RID: 1119
	private bool _wasAnytimeAlive;

	// Token: 0x04000460 RID: 1120
	internal static bool EnableSyncServerCmdBinding;

	// Token: 0x04000461 RID: 1121
	[SyncVar]
	public string Pastebin;

	// Token: 0x04000462 RID: 1122
	[SyncVar(hook = "MaxPlayersHook")]
	public ushort MaxPlayers;

	// Token: 0x04000463 RID: 1123
	internal static bool CuffedChangeTeam = true;

	// Token: 0x04000464 RID: 1124
	[SyncVar]
	public bool RoundStarted;

	// Token: 0x04000469 RID: 1129
	private RateLimit _commandRateLimit;
}
