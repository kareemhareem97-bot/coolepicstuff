using System;
using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using InventorySystem.Items;
using Mirror;
using Mirror.RemoteCalls;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.Spectating;
using UnityEngine;
using UnityEngine.Audio;
using Utils.NonAllocLINQ;

// Token: 0x0200006D RID: 109
public class PlayerEffectsController : NetworkBehaviour
{
	// Token: 0x17000084 RID: 132
	// (get) Token: 0x0600025E RID: 606 RVA: 0x000094F4 File Offset: 0x000076F4
	// (set) Token: 0x0600025F RID: 607 RVA: 0x000094FC File Offset: 0x000076FC
	public StatusEffectBase[] AllEffects { get; private set; }

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x06000260 RID: 608 RVA: 0x00009505 File Offset: 0x00007705
	public IEnumerable<StatusEffectBase> EnabledEffects
	{
		get
		{
			return from effect in this.AllEffects
			where effect.IsEnabled
			select effect;
		}
	}

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x06000261 RID: 609 RVA: 0x00009531 File Offset: 0x00007731
	// (set) Token: 0x06000262 RID: 610 RVA: 0x00009539 File Offset: 0x00007739
	public int EffectsLength { get; private set; }

	// Token: 0x06000263 RID: 611 RVA: 0x00009544 File Offset: 0x00007744
	public bool TryGetEffect(string effectName, out StatusEffectBase playerEffect)
	{
		foreach (StatusEffectBase statusEffectBase in this.AllEffects)
		{
			if (statusEffectBase.name.StartsWith(effectName, StringComparison.InvariantCultureIgnoreCase))
			{
				playerEffect = statusEffectBase;
				return true;
			}
		}
		playerEffect = null;
		return false;
	}

	// Token: 0x06000264 RID: 612 RVA: 0x00009584 File Offset: 0x00007784
	public bool TryGetEffect<T>(out T playerEffect) where T : StatusEffectBase
	{
		StatusEffectBase statusEffectBase;
		if (this._effectsByType.TryGetValue(typeof(T), out statusEffectBase))
		{
			T t = statusEffectBase as T;
			if (t != null)
			{
				playerEffect = t;
				return true;
			}
		}
		playerEffect = default(T);
		return false;
	}

	// Token: 0x06000265 RID: 613 RVA: 0x000095D0 File Offset: 0x000077D0
	[Server]
	public void UseMedicalItem(ItemBase item)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PlayerEffectsController::UseMedicalItem(InventorySystem.Items.ItemBase)' called when server was not active");
			return;
		}
		foreach (StatusEffectBase statusEffectBase in this.AllEffects)
		{
			if (statusEffectBase.IsEnabled)
			{
				IHealableEffect healableEffect = statusEffectBase as IHealableEffect;
				if (healableEffect != null && healableEffect.IsHealable(item.ItemTypeId))
				{
					ICustomHealableEffect customHealableEffect = statusEffectBase as ICustomHealableEffect;
					if (customHealableEffect != null)
					{
						customHealableEffect.OnHeal(item.ItemTypeId);
					}
					else
					{
						statusEffectBase.IsEnabled = false;
					}
				}
			}
		}
	}

	// Token: 0x06000266 RID: 614 RVA: 0x00009650 File Offset: 0x00007850
	[Server]
	public StatusEffectBase ChangeState(string effectName, byte intensity, float duration = 0f, bool addDuration = false)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'CustomPlayerEffects.StatusEffectBase PlayerEffectsController::ChangeState(System.String,System.Byte,System.Single,System.Boolean)' called when server was not active");
			return null;
		}
		StatusEffectBase statusEffectBase;
		if (this.TryGetEffect(effectName, out statusEffectBase))
		{
			statusEffectBase.ServerSetState(intensity, duration, addDuration);
		}
		return statusEffectBase;
	}

	// Token: 0x06000267 RID: 615 RVA: 0x00009694 File Offset: 0x00007894
	[Server]
	public T ChangeState<T>(byte intensity, float duration = 0f, bool addDuration = false) where T : StatusEffectBase
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'T PlayerEffectsController::ChangeState(System.Byte,System.Single,System.Boolean)' called when server was not active");
			return default(T);
		}
		T t;
		if (this.TryGetEffect<T>(out t))
		{
			t.ServerSetState(intensity, duration, addDuration);
		}
		return t;
	}

	// Token: 0x06000268 RID: 616 RVA: 0x000096DC File Offset: 0x000078DC
	[Server]
	public T EnableEffect<T>(float duration = 0f, bool addDuration = false) where T : StatusEffectBase
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'T PlayerEffectsController::EnableEffect(System.Single,System.Boolean)' called when server was not active");
			return default(T);
		}
		return this.ChangeState<T>(1, duration, addDuration);
	}

	// Token: 0x06000269 RID: 617 RVA: 0x00009714 File Offset: 0x00007914
	[Server]
	public T DisableEffect<T>() where T : StatusEffectBase
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'T PlayerEffectsController::DisableEffect()' called when server was not active");
			return default(T);
		}
		return this.ChangeState<T>(0, 0f, false);
	}

	// Token: 0x0600026A RID: 618 RVA: 0x00009750 File Offset: 0x00007950
	public void DisableAllEffects()
	{
		StatusEffectBase[] allEffects = this.AllEffects;
		for (int i = 0; i < allEffects.Length; i++)
		{
			allEffects[i].ServerDisable();
		}
	}

	// Token: 0x0600026B RID: 619 RVA: 0x0000977C File Offset: 0x0000797C
	public T GetEffect<T>() where T : StatusEffectBase
	{
		T result;
		if (!this.TryGetEffect<T>(out result))
		{
			return default(T);
		}
		return result;
	}

	// Token: 0x0600026C RID: 620 RVA: 0x000097A0 File Offset: 0x000079A0
	[Server]
	public void ServerSyncEffect(StatusEffectBase effect)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PlayerEffectsController::ServerSyncEffect(CustomPlayerEffects.StatusEffectBase)' called when server was not active");
			return;
		}
		for (int i = 0; i < this.EffectsLength; i++)
		{
			StatusEffectBase statusEffectBase = this.AllEffects[i];
			if (statusEffectBase == effect)
			{
				this._syncEffectsIntensity[i] = statusEffectBase.Intensity;
				return;
			}
		}
	}

	// Token: 0x0600026D RID: 621 RVA: 0x000097F8 File Offset: 0x000079F8
	public void ServerSendPulse<T>() where T : IPulseEffect
	{
		for (int i = 0; i < this.EffectsLength; i++)
		{
			if (this.AllEffects[i] is T)
			{
				byte index = (byte)Mathf.Min(i, 255);
				this.TargetRpcReceivePulse(this._hub.connectionToClient, index);
				SpectatorNetworking.ForeachSpectatorOf(this._hub, delegate(ReferenceHub x)
				{
					this.TargetRpcReceivePulse(x.connectionToClient, index);
				});
				return;
			}
		}
	}

	// Token: 0x0600026E RID: 622 RVA: 0x00009874 File Offset: 0x00007A74
	[TargetRpc]
	private void TargetRpcReceivePulse(NetworkConnection _, byte effectIndex)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteByte(effectIndex);
		this.SendTargetRPCInternal(_, "System.Void PlayerEffectsController::TargetRpcReceivePulse(Mirror.NetworkConnection,System.Byte)", 483637978, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x0600026F RID: 623 RVA: 0x000098B0 File Offset: 0x00007AB0
	private void Awake()
	{
		this._hub = ReferenceHub.GetHub(base.gameObject);
		List<StatusEffectBase> list = ListPool<StatusEffectBase>.Shared.Rent();
		foreach (StatusEffectBase statusEffectBase in this.effectsGameObject.GetComponentsInChildren<StatusEffectBase>())
		{
			IHolidayEffect holidayEffect = statusEffectBase as IHolidayEffect;
			if (holidayEffect != null && !holidayEffect.IsAvailable)
			{
				statusEffectBase.gameObject.SetActive(false);
			}
			else
			{
				list.Add(statusEffectBase);
			}
		}
		this.AllEffects = list.ToArray();
		this.EffectsLength = this.AllEffects.Length;
		ListPool<StatusEffectBase>.Shared.Return(list);
		foreach (StatusEffectBase statusEffectBase2 in this.AllEffects)
		{
			this._effectsByType.Add(statusEffectBase2.GetType(), statusEffectBase2);
			this._syncEffectsIntensity.Add(0);
		}
	}

	// Token: 0x06000270 RID: 624 RVA: 0x00002E9A File Offset: 0x0000109A
	private void Update()
	{
	}

	// Token: 0x06000271 RID: 625 RVA: 0x00009980 File Offset: 0x00007B80
	private void Start()
	{
		this.effectsGameObject.SetActive(true);
	}

	// Token: 0x06000272 RID: 626 RVA: 0x0000998E File Offset: 0x00007B8E
	private void OnEnable()
	{
		PlayerRoleManager.OnRoleChanged += this.OnRoleChanged;
	}

	// Token: 0x06000273 RID: 627 RVA: 0x000099A1 File Offset: 0x00007BA1
	private void OnDisable()
	{
		PlayerRoleManager.OnRoleChanged -= this.OnRoleChanged;
	}

	// Token: 0x06000274 RID: 628 RVA: 0x000099B4 File Offset: 0x00007BB4
	private void OnRoleChanged(ReferenceHub targetHub, PlayerRoleBase oldRole, PlayerRoleBase newRole)
	{
		if (targetHub != this._hub)
		{
			return;
		}
		bool flag = oldRole.Team != Team.Dead && newRole.Team == Team.Dead;
		foreach (StatusEffectBase statusEffectBase in this.AllEffects)
		{
			if (flag)
			{
				statusEffectBase.OnDeath(oldRole);
			}
			else
			{
				statusEffectBase.OnRoleChanged(oldRole, newRole);
			}
		}
	}

	// Token: 0x06000275 RID: 629 RVA: 0x00009A13 File Offset: 0x00007C13
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		SpectatorTargetTracker.OnTargetChanged += delegate()
		{
			ReferenceHub referenceHub;
			StatusEffectBase[] allEffects;
			if (ReferenceHub.AllHubs.TryGetFirst((ReferenceHub x) => x.playerEffectsController._wasSpectated, out referenceHub))
			{
				allEffects = referenceHub.playerEffectsController.AllEffects;
				for (int i = 0; i < allEffects.Length; i++)
				{
					allEffects[i].OnStopSpectating();
				}
				referenceHub.playerEffectsController._wasSpectated = false;
			}
			ReferenceHub referenceHub2;
			if (!SpectatorTargetTracker.TryGetTrackedPlayer(out referenceHub2))
			{
				return;
			}
			PlayerEffectsController playerEffectsController = referenceHub2.playerEffectsController;
			allEffects = playerEffectsController.AllEffects;
			for (int i = 0; i < allEffects.Length; i++)
			{
				allEffects[i].OnBeginSpectating();
			}
			playerEffectsController._wasSpectated = true;
		};
	}

	// Token: 0x06000276 RID: 630 RVA: 0x00009A39 File Offset: 0x00007C39
	public PlayerEffectsController()
	{
		base.InitSyncObject(this._syncEffectsIntensity);
	}

	// Token: 0x06000277 RID: 631 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x06000278 RID: 632 RVA: 0x00009A64 File Offset: 0x00007C64
	protected void UserCode_TargetRpcReceivePulse__NetworkConnection__Byte(NetworkConnection _, byte effectIndex)
	{
		int num = Mathf.Min((int)effectIndex, this.EffectsLength - 1);
		IPulseEffect pulseEffect = this.AllEffects[num] as IPulseEffect;
		if (pulseEffect != null)
		{
			pulseEffect.ExecutePulse();
		}
	}

	// Token: 0x06000279 RID: 633 RVA: 0x00009A97 File Offset: 0x00007C97
	protected static void InvokeUserCode_TargetRpcReceivePulse__NetworkConnection__Byte(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC TargetRpcReceivePulse called on server.");
			return;
		}
		((PlayerEffectsController)obj).UserCode_TargetRpcReceivePulse__NetworkConnection__Byte(null, reader.ReadByte());
	}

	// Token: 0x0600027A RID: 634 RVA: 0x00009AC1 File Offset: 0x00007CC1
	static PlayerEffectsController()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerEffectsController), "System.Void PlayerEffectsController::TargetRpcReceivePulse(Mirror.NetworkConnection,System.Byte)", new RemoteCallDelegate(PlayerEffectsController.InvokeUserCode_TargetRpcReceivePulse__NetworkConnection__Byte));
	}

	// Token: 0x040001FC RID: 508
	public AudioMixer mixer;

	// Token: 0x040001FD RID: 509
	public GameObject effectsGameObject;

	// Token: 0x040001FE RID: 510
	private readonly Dictionary<Type, StatusEffectBase> _effectsByType = new Dictionary<Type, StatusEffectBase>();

	// Token: 0x040001FF RID: 511
	private readonly SyncList<byte> _syncEffectsIntensity = new SyncList<byte>();

	// Token: 0x04000200 RID: 512
	private bool _wasSpectated;

	// Token: 0x04000201 RID: 513
	private ReferenceHub _hub;
}
