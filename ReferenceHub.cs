using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CentralAuth;
using Hints;
using Interactables;
using InventorySystem;
using InventorySystem.Searching;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using MapGeneration;
using Mirror;
using PlayerRoles;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using RemoteAdmin;
using Security;
using UnityEngine;
using Utils.Networking;

// Token: 0x02000096 RID: 150
public sealed class ReferenceHub : NetworkBehaviour, IEquatable<ReferenceHub>
{
	// Token: 0x14000005 RID: 5
	// (add) Token: 0x0600030B RID: 779 RVA: 0x0000B91C File Offset: 0x00009B1C
	// (remove) Token: 0x0600030C RID: 780 RVA: 0x0000B950 File Offset: 0x00009B50
	public static event Action<ReferenceHub> OnPlayerAdded;

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x0600030D RID: 781 RVA: 0x0000B984 File Offset: 0x00009B84
	// (remove) Token: 0x0600030E RID: 782 RVA: 0x0000B9B8 File Offset: 0x00009BB8
	public static event Action<ReferenceHub> OnPlayerRemoved;

	// Token: 0x14000007 RID: 7
	// (add) Token: 0x0600030F RID: 783 RVA: 0x0000B9EC File Offset: 0x00009BEC
	// (remove) Token: 0x06000310 RID: 784 RVA: 0x0000BA20 File Offset: 0x00009C20
	public static event Action<ReferenceHub> OnBeforePlayerDestroyed;

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000311 RID: 785 RVA: 0x0000BA53 File Offset: 0x00009C53
	// (set) Token: 0x06000312 RID: 786 RVA: 0x0000BA5A File Offset: 0x00009C5A
	public static HashSet<ReferenceHub> AllHubs { get; private set; } = new HashSet<ReferenceHub>();

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x06000313 RID: 787 RVA: 0x0000BA64 File Offset: 0x00009C64
	[Obsolete("HostHub is obsolete, use TryGetHostHub instead.")]
	public static ReferenceHub HostHub
	{
		get
		{
			ReferenceHub result;
			if (!ReferenceHub.TryGetHostHub(out result))
			{
				return null;
			}
			return result;
		}
	}

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x06000314 RID: 788 RVA: 0x0000BA80 File Offset: 0x00009C80
	[Obsolete("LocalHub is obsolete, use TryGetLocalHub instead.")]
	public static ReferenceHub LocalHub
	{
		get
		{
			ReferenceHub result;
			if (!ReferenceHub.TryGetLocalHub(out result))
			{
				return null;
			}
			return result;
		}
	}

	// Token: 0x06000315 RID: 789 RVA: 0x0000BA9C File Offset: 0x00009C9C
	public static int GetPlayerCount(ClientInstanceMode allowedState)
	{
		int num = 0;
		foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
		{
			if (allowedState == referenceHub.Mode)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000316 RID: 790 RVA: 0x0000BAF8 File Offset: 0x00009CF8
	public static int GetPlayerCount(ClientInstanceMode allowedState, ClientInstanceMode allowedState2)
	{
		int num = 0;
		foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
		{
			if (allowedState == referenceHub.Mode || allowedState2 == referenceHub.Mode)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000317 RID: 791 RVA: 0x0000BB5C File Offset: 0x00009D5C
	public static int GetPlayerCount(ClientInstanceMode allowedState, ClientInstanceMode allowedState2, ClientInstanceMode allowedState3)
	{
		int num = 0;
		foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
		{
			if (allowedState == referenceHub.Mode || allowedState2 == referenceHub.Mode || allowedState3 == referenceHub.Mode)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x06000318 RID: 792 RVA: 0x0000BBCC File Offset: 0x00009DCC
	public int PlayerId
	{
		get
		{
			return this._playerId.Value;
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x06000319 RID: 793 RVA: 0x0000BBD9 File Offset: 0x00009DD9
	public ClientInstanceMode Mode
	{
		get
		{
			return this.authManager.InstanceMode;
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x0600031A RID: 794 RVA: 0x0000BBE8 File Offset: 0x00009DE8
	public bool IsHost
	{
		get
		{
			ClientInstanceMode mode = this.Mode;
			return mode == ClientInstanceMode.Host || mode == ClientInstanceMode.DedicatedServer;
		}
	}

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x0600031B RID: 795 RVA: 0x0000BC0D File Offset: 0x00009E0D
	public bool IsDummy
	{
		get
		{
			return this.Mode == ClientInstanceMode.Dummy;
		}
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x0600031C RID: 796 RVA: 0x0000BC18 File Offset: 0x00009E18
	public bool IsPOV
	{
		get
		{
			return base.isLocalPlayer || this.IsLocallySpectated();
		}
	}

	// Token: 0x0600031D RID: 797 RVA: 0x0000BC2A File Offset: 0x00009E2A
	private void Awake()
	{
		ReferenceHub.AllHubs.Add(this);
		ReferenceHub.HubsByGameObjects[base.gameObject] = this;
		if (NetworkServer.active)
		{
			this.Network_playerId = new RecyclablePlayerId(true);
			this.FriendlyFireHandler = new FriendlyFireHandler(this);
		}
	}

	// Token: 0x0600031E RID: 798 RVA: 0x0000BC68 File Offset: 0x00009E68
	private void Start()
	{
		Action<ReferenceHub> onPlayerAdded = ReferenceHub.OnPlayerAdded;
		if (onPlayerAdded == null)
		{
			return;
		}
		onPlayerAdded(this);
	}

	// Token: 0x0600031F RID: 799 RVA: 0x0000BC7C File Offset: 0x00009E7C
	private void OnDestroy()
	{
		if (!base.isLocalPlayer)
		{
			PlayerEvents.OnLeft(new PlayerLeftEventArgs(this));
		}
		ReferenceHub.AllHubs.Remove(this);
		ReferenceHub.HubsByGameObjects.Remove(base.gameObject);
		ReferenceHub.HubByPlayerIds.Remove(this.PlayerId);
		this._playerId.Destroy();
		if (ReferenceHub._hostHub == this)
		{
			ReferenceHub._hostHub = null;
			ReferenceHub._hostHubSet = false;
		}
		if (ReferenceHub._localHub == this)
		{
			ReferenceHub._localHub = null;
			ReferenceHub._localHubSet = false;
		}
		Action<ReferenceHub> onPlayerRemoved = ReferenceHub.OnPlayerRemoved;
		if (onPlayerRemoved == null)
		{
			return;
		}
		onPlayerRemoved(this);
	}

	// Token: 0x06000320 RID: 800 RVA: 0x0000BD17 File Offset: 0x00009F17
	public override void OnStopClient()
	{
		base.OnStopClient();
		Action<ReferenceHub> onBeforePlayerDestroyed = ReferenceHub.OnBeforePlayerDestroyed;
		if (onBeforePlayerDestroyed == null)
		{
			return;
		}
		onBeforePlayerDestroyed(this);
	}

	// Token: 0x06000321 RID: 801 RVA: 0x0000BD2F File Offset: 0x00009F2F
	public override string ToString()
	{
		return string.Format("{0} (Name='{1}', NetID='{2}', PlayerID='{3}')", new object[]
		{
			"ReferenceHub",
			base.name,
			base.netId,
			this.PlayerId
		});
	}

	// Token: 0x06000322 RID: 802 RVA: 0x0000BD70 File Offset: 0x00009F70
	public static ReferenceHub GetHub(GameObject player)
	{
		ReferenceHub result;
		if (!ReferenceHub.TryGetHub(player, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x06000323 RID: 803 RVA: 0x0000BD8C File Offset: 0x00009F8C
	public static ReferenceHub GetHub(MonoBehaviour player)
	{
		ReferenceHub result;
		if (!ReferenceHub.TryGetHub(player.gameObject, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x06000324 RID: 804 RVA: 0x0000BDAB File Offset: 0x00009FAB
	public static bool TryGetHub(GameObject player, out ReferenceHub hub)
	{
		if (player == null)
		{
			hub = null;
			return false;
		}
		return ReferenceHub.HubsByGameObjects.TryGetValue(player, out hub) || player.TryGetComponent<ReferenceHub>(out hub);
	}

	// Token: 0x06000325 RID: 805 RVA: 0x0000BDD4 File Offset: 0x00009FD4
	public static bool TryGetHub(NetworkConnection connection, out ReferenceHub hub)
	{
		NetworkIdentity identity = connection.identity;
		if (!connection.isReady || identity == null)
		{
			hub = null;
			return false;
		}
		return ReferenceHub.HubsByGameObjects.TryGetValue(identity.gameObject, out hub) || identity.TryGetComponent<ReferenceHub>(out hub);
	}

	// Token: 0x06000326 RID: 806 RVA: 0x0000BE1C File Offset: 0x0000A01C
	public static bool TryGetHubNetID(uint netId, out ReferenceHub hub)
	{
		NetworkIdentity networkIdentity;
		if (NetworkUtils.SpawnedNetIds.TryGetValue(netId, out networkIdentity) && ReferenceHub.TryGetHub(networkIdentity.gameObject, out hub))
		{
			return true;
		}
		hub = null;
		return false;
	}

	// Token: 0x06000327 RID: 807 RVA: 0x0000BE4C File Offset: 0x0000A04C
	public static bool TryGetLocalHub(out ReferenceHub hub)
	{
		if (ReferenceHub._localHubSet)
		{
			hub = ReferenceHub._localHub;
			return true;
		}
		foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
		{
			if (referenceHub.isLocalPlayer)
			{
				hub = referenceHub;
				ReferenceHub._localHub = referenceHub;
				ReferenceHub._localHubSet = true;
				return true;
			}
		}
		hub = null;
		return false;
	}

	// Token: 0x06000328 RID: 808 RVA: 0x0000BEC8 File Offset: 0x0000A0C8
	public static bool TryGetPovHub(out ReferenceHub hub)
	{
		ReferenceHub referenceHub;
		if (SpectatorTargetTracker.TryGetTrackedPlayer(out referenceHub))
		{
			hub = referenceHub;
			return true;
		}
		return ReferenceHub.TryGetLocalHub(out hub);
	}

	// Token: 0x06000329 RID: 809 RVA: 0x0000BEEC File Offset: 0x0000A0EC
	public static bool TryGetHostHub(out ReferenceHub hub)
	{
		if (ReferenceHub._hostHubSet)
		{
			hub = ReferenceHub._hostHub;
			return true;
		}
		foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
		{
			if (referenceHub.IsHost)
			{
				hub = referenceHub;
				ReferenceHub._hostHub = referenceHub;
				ReferenceHub._hostHubSet = true;
				return true;
			}
		}
		hub = null;
		return false;
	}

	// Token: 0x0600032A RID: 810 RVA: 0x0000BF68 File Offset: 0x0000A168
	public static ReferenceHub GetHub(int playerId)
	{
		ReferenceHub result;
		if (!ReferenceHub.TryGetHub(playerId, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x0600032B RID: 811 RVA: 0x0000BF84 File Offset: 0x0000A184
	public static bool TryGetHub(int playerId, out ReferenceHub hub)
	{
		if (playerId > 0)
		{
			if (ReferenceHub.HubByPlayerIds.TryGetValue(playerId, out hub))
			{
				return true;
			}
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				if (referenceHub.PlayerId == playerId)
				{
					ReferenceHub.HubByPlayerIds[playerId] = referenceHub;
					hub = referenceHub;
					return true;
				}
			}
		}
		hub = null;
		return false;
	}

	// Token: 0x0600032C RID: 812 RVA: 0x0000C008 File Offset: 0x0000A208
	public bool Equals(ReferenceHub other)
	{
		return this == other;
	}

	// Token: 0x0600032D RID: 813 RVA: 0x0000C014 File Offset: 0x0000A214
	public override bool Equals(object obj)
	{
		ReferenceHub referenceHub = obj as ReferenceHub;
		return referenceHub != null && this == referenceHub;
	}

	// Token: 0x0600032E RID: 814 RVA: 0x0000C034 File Offset: 0x0000A234
	public override int GetHashCode()
	{
		return base.gameObject.GetHashCode();
	}

	// Token: 0x0600032F RID: 815 RVA: 0x0000C041 File Offset: 0x0000A241
	public static bool operator ==(ReferenceHub left, ReferenceHub right)
	{
		return left == right;
	}

	// Token: 0x06000330 RID: 816 RVA: 0x0000C04A File Offset: 0x0000A24A
	public static bool operator !=(ReferenceHub left, ReferenceHub right)
	{
		return left != right;
	}

	// Token: 0x06000333 RID: 819 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x06000334 RID: 820 RVA: 0x0000C07C File Offset: 0x0000A27C
	// (set) Token: 0x06000335 RID: 821 RVA: 0x0000C08F File Offset: 0x0000A28F
	public RecyclablePlayerId Network_playerId
	{
		get
		{
			return this._playerId;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<RecyclablePlayerId>(value, ref this._playerId, 1UL, null);
		}
	}

	// Token: 0x06000336 RID: 822 RVA: 0x0000C0AC File Offset: 0x0000A2AC
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteRecyclablePlayerId(this._playerId);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteRecyclablePlayerId(this._playerId);
		}
	}

	// Token: 0x06000337 RID: 823 RVA: 0x0000C104 File Offset: 0x0000A304
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<RecyclablePlayerId>(ref this._playerId, null, reader.ReadRecyclablePlayerId());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<RecyclablePlayerId>(ref this._playerId, null, reader.ReadRecyclablePlayerId());
		}
	}

	// Token: 0x040002BC RID: 700
	private static readonly Dictionary<GameObject, ReferenceHub> HubsByGameObjects = new Dictionary<GameObject, ReferenceHub>(20, new ReferenceHub.GameObjectComparer());

	// Token: 0x040002BD RID: 701
	private static readonly Dictionary<int, ReferenceHub> HubByPlayerIds = new Dictionary<int, ReferenceHub>(20);

	// Token: 0x040002BE RID: 702
	private static bool _localHubSet;

	// Token: 0x040002BF RID: 703
	private static bool _hostHubSet;

	// Token: 0x040002C0 RID: 704
	private static ReferenceHub _localHub;

	// Token: 0x040002C1 RID: 705
	private static ReferenceHub _hostHub;

	// Token: 0x040002C2 RID: 706
	[SyncVar]
	private RecyclablePlayerId _playerId;

	// Token: 0x040002C3 RID: 707
	public Transform PlayerCameraReference;

	// Token: 0x040002C4 RID: 708
	public NetworkIdentity networkIdentity;

	// Token: 0x040002C5 RID: 709
	public CharacterClassManager characterClassManager;

	// Token: 0x040002C6 RID: 710
	public PlayerRoleManager roleManager;

	// Token: 0x040002C7 RID: 711
	public PlayerStats playerStats;

	// Token: 0x040002C8 RID: 712
	public Inventory inventory;

	// Token: 0x040002C9 RID: 713
	public SearchCoordinator searchCoordinator;

	// Token: 0x040002CA RID: 714
	public ServerRoles serverRoles;

	// Token: 0x040002CB RID: 715
	public QueryProcessor queryProcessor;

	// Token: 0x040002CC RID: 716
	public NicknameSync nicknameSync;

	// Token: 0x040002CD RID: 717
	public InteractionCoordinator interCoordinator;

	// Token: 0x040002CE RID: 718
	public PlayerEffectsController playerEffectsController;

	// Token: 0x040002CF RID: 719
	public HintDisplay hints;

	// Token: 0x040002D0 RID: 720
	public AspectRatioSync aspectRatioSync;

	// Token: 0x040002D1 RID: 721
	public PlayerRateLimitHandler playerRateLimitHandler;

	// Token: 0x040002D2 RID: 722
	public GameConsoleTransmission gameConsoleTransmission;

	// Token: 0x040002D3 RID: 723
	public PlayerAuthenticationManager authManager;

	// Token: 0x040002D4 RID: 724
	public EncryptedChannelManager encryptedChannelManager;

	// Token: 0x040002D5 RID: 725
	public CurrentRoomPlayerCache CurrentRoomPlayerCache;

	// Token: 0x040002D6 RID: 726
	internal FriendlyFireHandler FriendlyFireHandler;

	// Token: 0x02000097 RID: 151
	private class GameObjectComparer : EqualityComparer<GameObject>
	{
		// Token: 0x06000338 RID: 824 RVA: 0x0000C15F File Offset: 0x0000A35F
		public override bool Equals(GameObject x, GameObject y)
		{
			return x == y;
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0000C168 File Offset: 0x0000A368
		public override int GetHashCode(GameObject obj)
		{
			if (!(obj == null))
			{
				return obj.GetHashCode();
			}
			return 0;
		}
	}
}
