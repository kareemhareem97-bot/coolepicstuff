using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AudioPooling;
using Interactables;
using Interactables.Interobjects.DoorButtons;
using Interactables.Interobjects.DoorUtils;
using Interactables.Verification;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

// Token: 0x0200003A RID: 58
public class AlphaWarheadActivationPanel : NetworkBehaviour, IServerInteractable, IInteractable, IDoorPermissionRequester
{
	// Token: 0x17000053 RID: 83
	// (get) Token: 0x06000146 RID: 326 RVA: 0x00006216 File Offset: 0x00004416
	// (set) Token: 0x06000147 RID: 327 RVA: 0x0000621D File Offset: 0x0000441D
	public static AlphaWarheadActivationPanel Instance { get; private set; }

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x06000148 RID: 328 RVA: 0x00006225 File Offset: 0x00004425
	// (set) Token: 0x06000149 RID: 329 RVA: 0x00006240 File Offset: 0x00004440
	public static bool IsUnlocked
	{
		get
		{
			return AlphaWarheadActivationPanel.Instance != null && AlphaWarheadActivationPanel.Instance._unlocked;
		}
		set
		{
			if (AlphaWarheadActivationPanel.Instance == null)
			{
				return;
			}
			AlphaWarheadActivationPanel.Instance.Network_unlocked = value;
			if (value)
			{
				AlphaWarheadActivationPanel.Instance.RpcGranted();
				return;
			}
			AlphaWarheadActivationPanel.Instance.RpcReset();
		}
	}

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x0600014A RID: 330 RVA: 0x00003897 File Offset: 0x00001A97
	public IVerificationRule VerificationRule
	{
		get
		{
			return StandardDistanceVerification.Default;
		}
	}

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x0600014B RID: 331 RVA: 0x00006273 File Offset: 0x00004473
	public DoorPermissionsPolicy PermissionsPolicy
	{
		get
		{
			return new DoorPermissionsPolicy(DoorPermissionFlags.AlphaWarhead, false, false);
		}
	}

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x0600014C RID: 332 RVA: 0x0000627D File Offset: 0x0000447D
	// (set) Token: 0x0600014D RID: 333 RVA: 0x00006285 File Offset: 0x00004485
	public string RequesterLogSignature { get; private set; }

	// Token: 0x0600014E RID: 334 RVA: 0x0000628E File Offset: 0x0000448E
	public void ServerInteract(ReferenceHub ply, byte colliderId)
	{
		if (colliderId == this._activatorColliderId)
		{
			this.ServerInteractActivator(ply);
			return;
		}
		if (colliderId == this._keycardColliderId)
		{
			this.ServerInteractKeycard(ply);
		}
	}

	// Token: 0x0600014F RID: 335 RVA: 0x000062B4 File Offset: 0x000044B4
	private void ServerInteractKeycard(ReferenceHub ply)
	{
		if (this._unlocked)
		{
			return;
		}
		PermissionUsed permissionUsed;
		bool flag = this.PermissionsPolicy.CheckPermissions(ply, this, out permissionUsed);
		PlayerUnlockingWarheadButtonEventArgs playerUnlockingWarheadButtonEventArgs = new PlayerUnlockingWarheadButtonEventArgs(ply);
		playerUnlockingWarheadButtonEventArgs.IsAllowed = flag;
		PlayerEvents.OnUnlockingWarheadButton(playerUnlockingWarheadButtonEventArgs);
		flag = playerUnlockingWarheadButtonEventArgs.IsAllowed;
		if (flag)
		{
			AlphaWarheadActivationPanel.IsUnlocked = true;
			if (permissionUsed != null)
			{
				permissionUsed(this, this);
			}
			PlayerEvents.OnUnlockedWarheadButton(new PlayerUnlockedWarheadButtonEventArgs(ply));
			return;
		}
		if (this._deniedCooldownSw.Elapsed.TotalSeconds > (double)this._deniedCooldownDuration)
		{
			this._deniedCooldownSw.Restart();
			this.RpcDenied(ply.GetCombinedPermissions(this));
			if (permissionUsed != null)
			{
				permissionUsed(this, false);
			}
		}
	}

	// Token: 0x06000150 RID: 336 RVA: 0x0000635C File Offset: 0x0000455C
	private void ServerInteractActivator(ReferenceHub ply)
	{
		if (!this._unlocked)
		{
			return;
		}
		if (AlphaWarheadController.Singleton.IsLocked)
		{
			return;
		}
		if (!AlphaWarheadOutsitePanel.nukeside.enabled)
		{
			return;
		}
		AlphaWarheadController.Singleton.StartDetonation(false, false, ply);
		ServerLogs.AddLog(ServerLogs.Modules.Warhead, ply.LoggedNameFromRefHub() + " started the Alpha Warhead detonation.", ServerLogs.ServerLogType.GameEvent, false);
	}

	// Token: 0x06000151 RID: 337 RVA: 0x000063B4 File Offset: 0x000045B4
	private void Start()
	{
		this._permsIndicator.Register(this);
		if (this._unlocked)
		{
			this._nfcScanner.SetGranted();
			this._permsIndicator.PlayAccepted(null);
			return;
		}
		this._nfcScanner.SetRegular();
	}

	// Token: 0x06000152 RID: 338 RVA: 0x00006400 File Offset: 0x00004600
	private void Awake()
	{
		AlphaWarheadActivationPanel.Instance = this;
	}

	// Token: 0x06000153 RID: 339 RVA: 0x00006408 File Offset: 0x00004608
	[ClientRpc]
	private void RpcGranted()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void AlphaWarheadActivationPanel::RpcGranted()", 1769627351, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000154 RID: 340 RVA: 0x00006438 File Offset: 0x00004638
	[ClientRpc]
	private void RpcDenied(DoorPermissionFlags flags)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		Mirror.GeneratedNetworkCode._Write_Interactables.Interobjects.DoorUtils.DoorPermissionFlags(writer, flags);
		this.SendRPCInternal("System.Void AlphaWarheadActivationPanel::RpcDenied(Interactables.Interobjects.DoorUtils.DoorPermissionFlags)", -1577515699, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000155 RID: 341 RVA: 0x00006474 File Offset: 0x00004674
	[ClientRpc]
	private void RpcReset()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void AlphaWarheadActivationPanel::RpcReset()", 58845995, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000156 RID: 342 RVA: 0x000064A4 File Offset: 0x000046A4
	private void PlayBeep(AudioClip clip)
	{
		AudioSourcePoolManager.PlayOnTransform(clip, base.transform, 10f, 1f, FalloffType.Exponential, MixerChannel.DefaultSfx, 1f);
	}

	// Token: 0x06000158 RID: 344 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x06000159 RID: 345 RVA: 0x000064D8 File Offset: 0x000046D8
	// (set) Token: 0x0600015A RID: 346 RVA: 0x000064EB File Offset: 0x000046EB
	public bool Network_unlocked
	{
		get
		{
			return this._unlocked;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<bool>(value, ref this._unlocked, 1UL, null);
		}
	}

	// Token: 0x0600015B RID: 347 RVA: 0x00006508 File Offset: 0x00004708
	protected void UserCode_RpcGranted()
	{
		this.PlayBeep(this._grantedClip);
		this._nfcScanner.SetGranted();
		this._permsIndicator.PlayAccepted(null);
	}

	// Token: 0x0600015C RID: 348 RVA: 0x00006540 File Offset: 0x00004740
	protected static void InvokeUserCode_RpcGranted(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcGranted called on server.");
			return;
		}
		((AlphaWarheadActivationPanel)obj).UserCode_RpcGranted();
	}

	// Token: 0x0600015D RID: 349 RVA: 0x00006563 File Offset: 0x00004763
	protected void UserCode_RpcDenied__DoorPermissionFlags(DoorPermissionFlags flags)
	{
		this.PlayBeep(this._deniedClip);
		this._nfcScanner.SetTemporaryDenied(this._deniedCooldownDuration);
		this._permsIndicator.PlayDenied(flags, new float?(this._deniedCooldownDuration));
	}

	// Token: 0x0600015E RID: 350 RVA: 0x00006599 File Offset: 0x00004799
	protected static void InvokeUserCode_RpcDenied__DoorPermissionFlags(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcDenied called on server.");
			return;
		}
		((AlphaWarheadActivationPanel)obj).UserCode_RpcDenied__DoorPermissionFlags(Mirror.GeneratedNetworkCode._Read_Interactables.Interobjects.DoorUtils.DoorPermissionFlags(reader));
	}

	// Token: 0x0600015F RID: 351 RVA: 0x000065C2 File Offset: 0x000047C2
	protected void UserCode_RpcReset()
	{
		this._nfcScanner.SetRegular();
		this._permsIndicator.ShowIdle();
	}

	// Token: 0x06000160 RID: 352 RVA: 0x000065DA File Offset: 0x000047DA
	protected static void InvokeUserCode_RpcReset(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			UnityEngine.Debug.LogError("RPC RpcReset called on server.");
			return;
		}
		((AlphaWarheadActivationPanel)obj).UserCode_RpcReset();
	}

	// Token: 0x06000161 RID: 353 RVA: 0x00006600 File Offset: 0x00004800
	static AlphaWarheadActivationPanel()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(AlphaWarheadActivationPanel), "System.Void AlphaWarheadActivationPanel::RpcGranted()", new RemoteCallDelegate(AlphaWarheadActivationPanel.InvokeUserCode_RpcGranted));
		RemoteProcedureCalls.RegisterRpc(typeof(AlphaWarheadActivationPanel), "System.Void AlphaWarheadActivationPanel::RpcDenied(Interactables.Interobjects.DoorUtils.DoorPermissionFlags)", new RemoteCallDelegate(AlphaWarheadActivationPanel.InvokeUserCode_RpcDenied__DoorPermissionFlags));
		RemoteProcedureCalls.RegisterRpc(typeof(AlphaWarheadActivationPanel), "System.Void AlphaWarheadActivationPanel::RpcReset()", new RemoteCallDelegate(AlphaWarheadActivationPanel.InvokeUserCode_RpcReset));
	}

	// Token: 0x06000162 RID: 354 RVA: 0x00006670 File Offset: 0x00004870
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(this._unlocked);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteBool(this._unlocked);
		}
	}

	// Token: 0x06000163 RID: 355 RVA: 0x000066C8 File Offset: 0x000048C8
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this._unlocked, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this._unlocked, null, reader.ReadBool());
		}
	}

	// Token: 0x04000126 RID: 294
	private readonly Stopwatch _deniedCooldownSw = Stopwatch.StartNew();

	// Token: 0x04000127 RID: 295
	[SerializeField]
	private KeycardScannerNfcIcon _nfcScanner;

	// Token: 0x04000128 RID: 296
	[SerializeField]
	private KeycardScannerPermsIndicator _permsIndicator;

	// Token: 0x04000129 RID: 297
	[SerializeField]
	private byte _keycardColliderId;

	// Token: 0x0400012A RID: 298
	[SerializeField]
	private byte _activatorColliderId;

	// Token: 0x0400012B RID: 299
	[SerializeField]
	private float _deniedCooldownDuration;

	// Token: 0x0400012C RID: 300
	[SerializeField]
	private AudioClip _grantedClip;

	// Token: 0x0400012D RID: 301
	[SerializeField]
	private AudioClip _deniedClip;

	// Token: 0x0400012E RID: 302
	[SyncVar]
	private bool _unlocked;
}
