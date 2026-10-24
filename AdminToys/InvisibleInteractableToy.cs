using System;
using System.Runtime.InteropServices;
using Interactables;
using Interactables.Verification;
using InventorySystem.Searching;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Mirror;
using UnityEngine;

namespace AdminToys
{
	// Token: 0x02000AD6 RID: 2774
	public class InvisibleInteractableToy : AdminToyBase, IServerInteractable, IInteractable, ISearchable
	{
		// Token: 0x17000E29 RID: 3625
		// (get) Token: 0x06003E93 RID: 16019 RVA: 0x000CF1CD File Offset: 0x000CD3CD
		public override string CommandName
		{
			get
			{
				return "Interactable";
			}
		}

		// Token: 0x17000E2A RID: 3626
		// (get) Token: 0x06003E94 RID: 16020 RVA: 0x00003897 File Offset: 0x00001A97
		public IVerificationRule VerificationRule
		{
			get
			{
				return StandardDistanceVerification.Default;
			}
		}

		// Token: 0x17000E2B RID: 3627
		// (get) Token: 0x06003E95 RID: 16021 RVA: 0x000CF1D4 File Offset: 0x000CD3D4
		public bool CanSearch
		{
			get
			{
				return this.InteractionDuration > 0f && !this.IsLocked;
			}
		}

		// Token: 0x140000B7 RID: 183
		// (add) Token: 0x06003E96 RID: 16022 RVA: 0x000CF1F0 File Offset: 0x000CD3F0
		// (remove) Token: 0x06003E97 RID: 16023 RVA: 0x000CF228 File Offset: 0x000CD428
		public event Action<ReferenceHub> OnInteracted;

		// Token: 0x140000B8 RID: 184
		// (add) Token: 0x06003E98 RID: 16024 RVA: 0x000CF260 File Offset: 0x000CD460
		// (remove) Token: 0x06003E99 RID: 16025 RVA: 0x000CF298 File Offset: 0x000CD498
		public event Action<ReferenceHub> OnSearching;

		// Token: 0x140000B9 RID: 185
		// (add) Token: 0x06003E9A RID: 16026 RVA: 0x000CF2D0 File Offset: 0x000CD4D0
		// (remove) Token: 0x06003E9B RID: 16027 RVA: 0x000CF308 File Offset: 0x000CD508
		public event Action<ReferenceHub> OnSearched;

		// Token: 0x140000BA RID: 186
		// (add) Token: 0x06003E9C RID: 16028 RVA: 0x000CF340 File Offset: 0x000CD540
		// (remove) Token: 0x06003E9D RID: 16029 RVA: 0x000CF378 File Offset: 0x000CD578
		public event Action<ReferenceHub> OnSearchAborted;

		// Token: 0x06003E9E RID: 16030 RVA: 0x000CF3B0 File Offset: 0x000CD5B0
		public override void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
		{
			string[] array = arguments.Array;
			InvisibleInteractableToy.ColliderShape colliderShape;
			this.NetworkShape = ((array.Length > 2 && Enum.TryParse<InvisibleInteractableToy.ColliderShape>(array[2], true, out colliderShape)) ? colliderShape : InvisibleInteractableToy.ColliderShape.Box);
			float num;
			this.NetworkInteractionDuration = ((array.Length > 3 && float.TryParse(array[3], out num)) ? num : 0f);
			float num2;
			float d = (array.Length > 4 && float.TryParse(array[4], out num2)) ? num2 : 1f;
			base.transform.SetPositionAndRotation(admin.PlayerCameraReference.position, admin.PlayerCameraReference.rotation);
			base.transform.localScale = base.transform.localScale * d;
			this.OnInteracted += delegate(ReferenceHub x)
			{
				admin.gameConsoleTransmission.SendToClient(string.Format("{0} triggered OnInteraction", x), "green");
			};
			this.OnSearching += delegate(ReferenceHub x)
			{
				admin.gameConsoleTransmission.SendToClient(string.Format("{0} triggered OnSearching", x), "green");
			};
			this.OnSearched += delegate(ReferenceHub x)
			{
				admin.gameConsoleTransmission.SendToClient(string.Format("{0} triggered OnSearched", x), "green");
			};
			this.OnSearchAborted += delegate(ReferenceHub x)
			{
				admin.gameConsoleTransmission.SendToClient(string.Format("{0} triggered OnSearchAborted", x), "green");
			};
			base.OnSpawned(admin, arguments);
		}

		// Token: 0x06003E9F RID: 16031 RVA: 0x000CF4C9 File Offset: 0x000CD6C9
		protected override void Start()
		{
			base.Start();
			this.SetCollider(InvisibleInteractableToy.ColliderShape.Box, this.Shape);
		}

		// Token: 0x06003EA0 RID: 16032 RVA: 0x000CF4DE File Offset: 0x000CD6DE
		public void ServerInteract(ReferenceHub ply, byte colliderId)
		{
			if (this.InteractionDuration > 0f)
			{
				return;
			}
			Action<ReferenceHub> onInteracted = this.OnInteracted;
			if (onInteracted != null)
			{
				onInteracted(ply);
			}
			PlayerEvents.OnInteractedToy(new PlayerInteractedToyEventArgs(ply, this));
		}

		// Token: 0x06003EA1 RID: 16033 RVA: 0x000CF50C File Offset: 0x000CD70C
		public ISearchCompletor GetSearchCompletor(SearchCoordinator coordinator, float sqrDistance)
		{
			return new InvisibleInteractableToy.InteractableToySearchCompletor(coordinator.Hub, this, (double)sqrDistance);
		}

		// Token: 0x06003EA2 RID: 16034 RVA: 0x000CF51C File Offset: 0x000CD71C
		public float SearchTimeForPlayer(ReferenceHub hub)
		{
			return this.InteractionDuration;
		}

		// Token: 0x06003EA3 RID: 16035 RVA: 0x000CF524 File Offset: 0x000CD724
		public bool ServerValidateRequest(NetworkConnection source, SearchSessionPipe session)
		{
			if (!this.CanSearch)
			{
				session.Invalidate();
				return false;
			}
			return true;
		}

		// Token: 0x06003EA4 RID: 16036 RVA: 0x000CF537 File Offset: 0x000CD737
		public void ServerHandleAbort(ReferenceHub hub)
		{
			Action<ReferenceHub> onSearchAborted = this.OnSearchAborted;
			if (onSearchAborted != null)
			{
				onSearchAborted(hub);
			}
			PlayerEvents.OnSearchToyAborted(new PlayerSearchToyAbortedEventArgs(hub, this));
		}

		// Token: 0x06003EA5 RID: 16037 RVA: 0x000CF558 File Offset: 0x000CD758
		private void SetCollider(InvisibleInteractableToy.ColliderShape _, InvisibleInteractableToy.ColliderShape newShape)
		{
			if (this._collider != null)
			{
				UnityEngine.Object.Destroy(this._collider);
			}
			switch (newShape)
			{
			case InvisibleInteractableToy.ColliderShape.Box:
				this._collider = base.gameObject.AddComponent<BoxCollider>();
				return;
			case InvisibleInteractableToy.ColliderShape.Sphere:
				this._collider = base.gameObject.AddComponent<SphereCollider>();
				return;
			case InvisibleInteractableToy.ColliderShape.Capsule:
			{
				CapsuleCollider capsuleCollider = base.gameObject.AddComponent<CapsuleCollider>();
				capsuleCollider.height = 2f;
				capsuleCollider.radius = 0.5f;
				this._collider = capsuleCollider;
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06003EA7 RID: 16039 RVA: 0x000CF5E6 File Offset: 0x000CD7E6
		NetworkIdentity ISearchable.get_netIdentity()
		{
			return base.netIdentity;
		}

		// Token: 0x06003EA8 RID: 16040 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x17000E2C RID: 3628
		// (get) Token: 0x06003EA9 RID: 16041 RVA: 0x000CF5F0 File Offset: 0x000CD7F0
		// (set) Token: 0x06003EAA RID: 16042 RVA: 0x000CF603 File Offset: 0x000CD803
		public InvisibleInteractableToy.ColliderShape NetworkShape
		{
			get
			{
				return this.Shape;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<InvisibleInteractableToy.ColliderShape>(value, ref this.Shape, 32UL, new Action<InvisibleInteractableToy.ColliderShape, InvisibleInteractableToy.ColliderShape>(this.SetCollider));
			}
		}

		// Token: 0x17000E2D RID: 3629
		// (get) Token: 0x06003EAB RID: 16043 RVA: 0x000CF628 File Offset: 0x000CD828
		// (set) Token: 0x06003EAC RID: 16044 RVA: 0x000CF63B File Offset: 0x000CD83B
		public float NetworkInteractionDuration
		{
			get
			{
				return this.InteractionDuration;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<float>(value, ref this.InteractionDuration, 64UL, null);
			}
		}

		// Token: 0x17000E2E RID: 3630
		// (get) Token: 0x06003EAD RID: 16045 RVA: 0x000CF658 File Offset: 0x000CD858
		// (set) Token: 0x06003EAE RID: 16046 RVA: 0x000CF66B File Offset: 0x000CD86B
		public bool NetworkIsLocked
		{
			get
			{
				return this.IsLocked;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<bool>(value, ref this.IsLocked, 128UL, null);
			}
		}

		// Token: 0x06003EAF RID: 16047 RVA: 0x000CF688 File Offset: 0x000CD888
		public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
		{
			base.SerializeSyncVars(writer, forceAll);
			if (forceAll)
			{
				Mirror.GeneratedNetworkCode._Write_AdminToys.InvisibleInteractableToy/ColliderShape(writer, this.Shape);
				writer.WriteFloat(this.InteractionDuration);
				writer.WriteBool(this.IsLocked);
				return;
			}
			writer.WriteULong(base.syncVarDirtyBits);
			if ((base.syncVarDirtyBits & 32UL) != 0UL)
			{
				Mirror.GeneratedNetworkCode._Write_AdminToys.InvisibleInteractableToy/ColliderShape(writer, this.Shape);
			}
			if ((base.syncVarDirtyBits & 64UL) != 0UL)
			{
				writer.WriteFloat(this.InteractionDuration);
			}
			if ((base.syncVarDirtyBits & 128UL) != 0UL)
			{
				writer.WriteBool(this.IsLocked);
			}
		}

		// Token: 0x06003EB0 RID: 16048 RVA: 0x000CF73C File Offset: 0x000CD93C
		public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
		{
			base.DeserializeSyncVars(reader, initialState);
			if (initialState)
			{
				base.GeneratedSyncVarDeserialize<InvisibleInteractableToy.ColliderShape>(ref this.Shape, new Action<InvisibleInteractableToy.ColliderShape, InvisibleInteractableToy.ColliderShape>(this.SetCollider), Mirror.GeneratedNetworkCode._Read_AdminToys.InvisibleInteractableToy/ColliderShape(reader));
				base.GeneratedSyncVarDeserialize<float>(ref this.InteractionDuration, null, reader.ReadFloat());
				base.GeneratedSyncVarDeserialize<bool>(ref this.IsLocked, null, reader.ReadBool());
				return;
			}
			long num = (long)reader.ReadULong();
			if ((num & 32L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<InvisibleInteractableToy.ColliderShape>(ref this.Shape, new Action<InvisibleInteractableToy.ColliderShape, InvisibleInteractableToy.ColliderShape>(this.SetCollider), Mirror.GeneratedNetworkCode._Read_AdminToys.InvisibleInteractableToy/ColliderShape(reader));
			}
			if ((num & 64L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<float>(ref this.InteractionDuration, null, reader.ReadFloat());
			}
			if ((num & 128L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<bool>(ref this.IsLocked, null, reader.ReadBool());
			}
		}

		// Token: 0x04002DCA RID: 11722
		[SyncVar(hook = "SetCollider")]
		public InvisibleInteractableToy.ColliderShape Shape;

		// Token: 0x04002DCB RID: 11723
		[SyncVar]
		public float InteractionDuration;

		// Token: 0x04002DCC RID: 11724
		[SyncVar]
		public bool IsLocked;

		// Token: 0x04002DCD RID: 11725
		private Collider _collider;

		// Token: 0x02000AD7 RID: 2775
		public class InteractableToySearchCompletor : ISearchCompletor
		{
			// Token: 0x17000E2F RID: 3631
			// (get) Token: 0x06003EB1 RID: 16049 RVA: 0x000CF81B File Offset: 0x000CDA1B
			public ReferenceHub Hub { get; }

			// Token: 0x06003EB2 RID: 16050 RVA: 0x000CF823 File Offset: 0x000CDA23
			public InteractableToySearchCompletor(ReferenceHub hub, InvisibleInteractableToy target, double sqrDistance)
			{
				this.Hub = hub;
				this._target = target;
				this._maxDistanceSqr = sqrDistance;
			}

			// Token: 0x06003EB3 RID: 16051 RVA: 0x000CF840 File Offset: 0x000CDA40
			public bool ValidateStart()
			{
				PlayerSearchingToyEventArgs playerSearchingToyEventArgs = new PlayerSearchingToyEventArgs(this.Hub, this._target);
				PlayerEvents.OnSearchingToy(playerSearchingToyEventArgs);
				if (!playerSearchingToyEventArgs.IsAllowed)
				{
					return false;
				}
				Action<ReferenceHub> onSearching = this._target.OnSearching;
				if (onSearching != null)
				{
					onSearching(this.Hub);
				}
				return !this._target.IsLocked && this.ValidateDistance();
			}

			// Token: 0x06003EB4 RID: 16052 RVA: 0x000CF89E File Offset: 0x000CDA9E
			public bool ValidateUpdate()
			{
				return this.ValidateDistance();
			}

			// Token: 0x06003EB5 RID: 16053 RVA: 0x000CF8A6 File Offset: 0x000CDAA6
			public void Complete()
			{
				Action<ReferenceHub> onSearched = this._target.OnSearched;
				if (onSearched != null)
				{
					onSearched(this.Hub);
				}
				PlayerEvents.OnSearchedToy(new PlayerSearchedToyEventArgs(this.Hub, this._target));
			}

			// Token: 0x06003EB6 RID: 16054 RVA: 0x000CF8DC File Offset: 0x000CDADC
			private bool ValidateDistance()
			{
				return (double)(this._target.transform.position - this.Hub.transform.position).sqrMagnitude <= this._maxDistanceSqr;
			}

			// Token: 0x04002DCF RID: 11727
			private readonly InvisibleInteractableToy _target;

			// Token: 0x04002DD0 RID: 11728
			private readonly double _maxDistanceSqr;
		}

		// Token: 0x02000AD8 RID: 2776
		public enum ColliderShape
		{
			// Token: 0x04002DD2 RID: 11730
			Box,
			// Token: 0x04002DD3 RID: 11731
			Sphere,
			// Token: 0x04002DD4 RID: 11732
			Capsule
		}
	}
}
