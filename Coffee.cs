using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Interactables;
using Interactables.Verification;
using InventorySystem.Items.Usables;
using MapGeneration;
using Mirror;
using PlayerRoles;
using UnityEngine;

// Token: 0x02000022 RID: 34
public class Coffee : NetworkBehaviour, IClientInteractable, IInteractable, IServerInteractable
{
	// Token: 0x06000080 RID: 128 RVA: 0x00003627 File Offset: 0x00001827
	private void Awake()
	{
		if (!this.IsConsumed)
		{
			return;
		}
		this._animator.enabled = true;
		this._particles.Stop();
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00003649 File Offset: 0x00001849
	private void Start()
	{
		this._meshRenderer.material.color = this._drinkColor;
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x06000082 RID: 130 RVA: 0x00003661 File Offset: 0x00001861
	public IVerificationRule VerificationRule { get; } = StandardDistanceVerification.Default;

	// Token: 0x06000083 RID: 131 RVA: 0x00003669 File Offset: 0x00001869
	public void ServerInteract(ReferenceHub ply, byte colliderId)
	{
		if (!Coffee.BlacklistedPlayers.Add(ply))
		{
			return;
		}
		UsableItemsController.GetHandler(ply).ActiveRegenerations.Add(new RegenerationProcess(this._healProgress, 0.06666667f, 50f));
		this.NetworkIsConsumed = true;
	}

	// Token: 0x06000084 RID: 132 RVA: 0x00002E9A File Offset: 0x0000109A
	public void ClientInteract(InteractableCollider collider)
	{
	}

	// Token: 0x06000085 RID: 133 RVA: 0x000036A5 File Offset: 0x000018A5
	private void SyncCoffeeState(bool wasConsumed, bool isConsumed)
	{
		this._animator.enabled = isConsumed;
		if (isConsumed)
		{
			this._particles.Stop();
			this._audioSource.Play();
			return;
		}
		this._particles.Play();
	}

	// Token: 0x06000086 RID: 134 RVA: 0x000036D8 File Offset: 0x000018D8
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		PlayerRoleManager.OnRoleChanged += delegate(ReferenceHub hub, PlayerRoleBase _, PlayerRoleBase _)
		{
			Coffee.BlacklistedPlayers.Remove(hub);
		};
		ReferenceHub.OnPlayerRemoved += delegate(ReferenceHub hub)
		{
			Coffee.BlacklistedPlayers.Remove(hub);
		};
		SeedSynchronizer.OnGenerationFinished += delegate()
		{
			Coffee.BlacklistedPlayers.Clear();
		};
	}

	// Token: 0x06000089 RID: 137 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x0600008A RID: 138 RVA: 0x00003770 File Offset: 0x00001970
	// (set) Token: 0x0600008B RID: 139 RVA: 0x00003783 File Offset: 0x00001983
	public bool NetworkIsConsumed
	{
		get
		{
			return this.IsConsumed;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<bool>(value, ref this.IsConsumed, 1UL, new Action<bool, bool>(this.SyncCoffeeState));
		}
	}

	// Token: 0x0600008C RID: 140 RVA: 0x000037A8 File Offset: 0x000019A8
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(this.IsConsumed);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteBool(this.IsConsumed);
		}
	}

	// Token: 0x0600008D RID: 141 RVA: 0x00003800 File Offset: 0x00001A00
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this.IsConsumed, new Action<bool, bool>(this.SyncCoffeeState), reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this.IsConsumed, new Action<bool, bool>(this.SyncCoffeeState), reader.ReadBool());
		}
	}

	// Token: 0x04000056 RID: 86
	private const float DurationPerCharacter = 0.035f;

	// Token: 0x04000057 RID: 87
	private const float MaxDuration = 7f;

	// Token: 0x04000058 RID: 88
	private const float MinDuration = 4f;

	// Token: 0x04000059 RID: 89
	private const float TotalRegenerationTime = 15f;

	// Token: 0x0400005A RID: 90
	private const int TotalHpToRegenerate = 50;

	// Token: 0x0400005B RID: 91
	private static readonly HashSet<ReferenceHub> BlacklistedPlayers = new HashSet<ReferenceHub>();

	// Token: 0x0400005C RID: 92
	[SyncVar(hook = "SyncCoffeeState")]
	public bool IsConsumed;

	// Token: 0x0400005D RID: 93
	[SerializeField]
	private Color _drinkColor;

	// Token: 0x0400005E RID: 94
	[SerializeField]
	private CoffeeTranslation _drinkText;

	// Token: 0x0400005F RID: 95
	[SerializeField]
	private string _author;

	// Token: 0x04000060 RID: 96
	[SerializeField]
	private MeshRenderer _meshRenderer;

	// Token: 0x04000061 RID: 97
	[SerializeField]
	private AnimationCurve _healProgress;

	// Token: 0x04000062 RID: 98
	[SerializeField]
	private Animator _animator;

	// Token: 0x04000063 RID: 99
	[SerializeField]
	private ParticleSystem _particles;

	// Token: 0x04000064 RID: 100
	[SerializeField]
	private AudioSource _audioSource;
}
