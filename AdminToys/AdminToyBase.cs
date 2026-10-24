using System;
using System.Runtime.InteropServices;
using Footprinting;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

namespace AdminToys
{
	// Token: 0x02000AD4 RID: 2772
	public abstract class AdminToyBase : NetworkBehaviour
	{
		// Token: 0x140000B5 RID: 181
		// (add) Token: 0x06003E5F RID: 15967 RVA: 0x000CE7B8 File Offset: 0x000CC9B8
		// (remove) Token: 0x06003E60 RID: 15968 RVA: 0x000CE7EC File Offset: 0x000CC9EC
		public static event Action<AdminToyBase> OnAdded;

		// Token: 0x140000B6 RID: 182
		// (add) Token: 0x06003E61 RID: 15969 RVA: 0x000CE820 File Offset: 0x000CCA20
		// (remove) Token: 0x06003E62 RID: 15970 RVA: 0x000CE854 File Offset: 0x000CCA54
		public static event Action<AdminToyBase> OnRemoved;

		// Token: 0x17000E1E RID: 3614
		// (get) Token: 0x06003E63 RID: 15971
		public abstract string CommandName { get; }

		// Token: 0x17000E1F RID: 3615
		// (get) Token: 0x06003E64 RID: 15972 RVA: 0x000CE887 File Offset: 0x000CCA87
		// (set) Token: 0x06003E65 RID: 15973 RVA: 0x000CE88F File Offset: 0x000CCA8F
		public Footprint SpawnerFootprint { get; private set; }

		// Token: 0x17000E20 RID: 3616
		// (get) Token: 0x06003E66 RID: 15974 RVA: 0x000CE898 File Offset: 0x000CCA98
		// (set) Token: 0x06003E67 RID: 15975 RVA: 0x000CE8A5 File Offset: 0x000CCAA5
		public Vector3 RotationEuler
		{
			get
			{
				return this.Rotation.eulerAngles;
			}
			set
			{
				this.NetworkRotation = Quaternion.Euler(value);
			}
		}

		// Token: 0x17000E21 RID: 3617
		// (get) Token: 0x06003E68 RID: 15976 RVA: 0x000CE8B3 File Offset: 0x000CCAB3
		public Transform CachedTransform
		{
			get
			{
				if (this._trCache == null)
				{
					this._trCache = base.transform;
				}
				return this._trCache;
			}
		}

		// Token: 0x06003E69 RID: 15977 RVA: 0x000CE8CF File Offset: 0x000CCACF
		protected virtual void Start()
		{
			Action<AdminToyBase> onAdded = AdminToyBase.OnAdded;
			if (onAdded == null)
			{
				return;
			}
			onAdded(this);
		}

		// Token: 0x06003E6A RID: 15978 RVA: 0x000CE8E1 File Offset: 0x000CCAE1
		protected virtual void LateUpdate()
		{
			if (this.IsStatic)
			{
				return;
			}
			if (NetworkServer.active)
			{
				this.UpdatePositionServer();
				return;
			}
			this.UpdatePositionClient(false);
		}

		// Token: 0x06003E6B RID: 15979 RVA: 0x000CE904 File Offset: 0x000CCB04
		protected void OnTransformParentChanged()
		{
			if (!base.netIdentity.isServer)
			{
				return;
			}
			Transform parent = this.CachedTransform.parent;
			if (this._previousParent != parent)
			{
				this.RpcChangeParent(this.ServerParentId(parent));
				this._previousParent = parent;
			}
		}

		// Token: 0x06003E6C RID: 15980 RVA: 0x000CE94D File Offset: 0x000CCB4D
		protected virtual void OnDestroy()
		{
			Action<AdminToyBase> onRemoved = AdminToyBase.OnRemoved;
			if (onRemoved == null)
			{
				return;
			}
			onRemoved(this);
		}

		// Token: 0x06003E6D RID: 15981 RVA: 0x000CE95F File Offset: 0x000CCB5F
		public override void OnStartServer()
		{
			base.OnStartServer();
			this.UpdatePositionServer();
		}

		// Token: 0x06003E6E RID: 15982 RVA: 0x000CE96D File Offset: 0x000CCB6D
		public override void OnStartClient()
		{
			base.OnStartClient();
			if (NetworkServer.active)
			{
				return;
			}
			this.UpdateParent();
			this.UpdatePositionClient(true);
		}

		// Token: 0x06003E6F RID: 15983 RVA: 0x000CE98A File Offset: 0x000CCB8A
		public override void OnSerialize(NetworkWriter writer, bool initialState)
		{
			base.OnSerialize(writer, initialState);
			if (initialState)
			{
				writer.WriteUInt(this.ServerParentId(this.CachedTransform.parent));
			}
		}

		// Token: 0x06003E70 RID: 15984 RVA: 0x000CE9AE File Offset: 0x000CCBAE
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			base.OnDeserialize(reader, initialState);
			if (initialState)
			{
				this._clientParentId = reader.ReadUInt();
			}
		}

		// Token: 0x06003E71 RID: 15985 RVA: 0x000CE9C8 File Offset: 0x000CCBC8
		public virtual void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
		{
			this.SpawnerFootprint = new Footprint(admin);
			NetworkServer.Spawn(base.gameObject, null);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} spawned an admin toy: {1} with NetID {2}.", admin.LoggedNameFromRefHub(), this.CommandName, base.netId), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
		}

		// Token: 0x06003E72 RID: 15986 RVA: 0x000CEA18 File Offset: 0x000CCC18
		[ClientRpc]
		public void RpcChangeParent(uint parentId)
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			writer.WriteUInt(parentId);
			this.SendRPCInternal("System.Void AdminToys.AdminToyBase::RpcChangeParent(System.UInt32)", -342419096, writer, 0, true);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x06003E73 RID: 15987 RVA: 0x000CEA54 File Offset: 0x000CCC54
		private uint ServerParentId(Transform parent)
		{
			NetworkIdentity networkIdentity;
			if (!(parent != null) || !parent.TryGetComponent<NetworkIdentity>(out networkIdentity))
			{
				return 0U;
			}
			return networkIdentity.netId;
		}

		// Token: 0x06003E74 RID: 15988 RVA: 0x000CEA7C File Offset: 0x000CCC7C
		private void UpdateParent()
		{
			NetworkIdentity networkIdentity;
			if (this._clientParentId != 0U && NetworkClient.spawned.TryGetValue(this._clientParentId, out networkIdentity))
			{
				this.CachedTransform.SetParent(networkIdentity.transform, false);
				return;
			}
			this.CachedTransform.SetParent(null, false);
		}

		// Token: 0x06003E75 RID: 15989 RVA: 0x000CEAC8 File Offset: 0x000CCCC8
		private void UpdatePositionServer()
		{
			Transform cachedTransform = this.CachedTransform;
			Vector3 networkPosition;
			Quaternion networkRotation;
			cachedTransform.GetLocalPositionAndRotation(out networkPosition, out networkRotation);
			this.NetworkPosition = networkPosition;
			this.NetworkRotation = networkRotation;
			this.NetworkScale = cachedTransform.localScale;
		}

		// Token: 0x06003E76 RID: 15990 RVA: 0x000CEB00 File Offset: 0x000CCD00
		protected virtual void UpdatePositionClient(bool teleport = false)
		{
			Transform cachedTransform = this.CachedTransform;
			Vector3 localPosition;
			Quaternion localRotation;
			Vector3 localScale;
			if (teleport || this.MovementSmoothing == 0)
			{
				localPosition = this.Position;
				localRotation = this.Rotation;
				localScale = this.Scale;
			}
			else
			{
				float t = Time.deltaTime * (float)this.MovementSmoothing * 0.3f;
				Vector3 a;
				Quaternion a2;
				cachedTransform.GetLocalPositionAndRotation(out a, out a2);
				localPosition = Vector3.Lerp(a, this.Position, t);
				localRotation = Quaternion.Lerp(a2, this.Rotation, t);
				localScale = Vector3.Lerp(cachedTransform.localScale, this.Scale, t);
			}
			cachedTransform.SetLocalPositionAndRotation(localPosition, localRotation);
			cachedTransform.localScale = localScale;
		}

		// Token: 0x06003E78 RID: 15992 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x17000E22 RID: 3618
		// (get) Token: 0x06003E79 RID: 15993 RVA: 0x000CEB98 File Offset: 0x000CCD98
		// (set) Token: 0x06003E7A RID: 15994 RVA: 0x000CEBAB File Offset: 0x000CCDAB
		public Vector3 NetworkPosition
		{
			get
			{
				return this.Position;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<Vector3>(value, ref this.Position, 1UL, null);
			}
		}

		// Token: 0x17000E23 RID: 3619
		// (get) Token: 0x06003E7B RID: 15995 RVA: 0x000CEBC8 File Offset: 0x000CCDC8
		// (set) Token: 0x06003E7C RID: 15996 RVA: 0x000CEBDB File Offset: 0x000CCDDB
		public Quaternion NetworkRotation
		{
			get
			{
				return this.Rotation;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<Quaternion>(value, ref this.Rotation, 2UL, null);
			}
		}

		// Token: 0x17000E24 RID: 3620
		// (get) Token: 0x06003E7D RID: 15997 RVA: 0x000CEBF8 File Offset: 0x000CCDF8
		// (set) Token: 0x06003E7E RID: 15998 RVA: 0x000CEC0B File Offset: 0x000CCE0B
		public Vector3 NetworkScale
		{
			get
			{
				return this.Scale;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<Vector3>(value, ref this.Scale, 4UL, null);
			}
		}

		// Token: 0x17000E25 RID: 3621
		// (get) Token: 0x06003E7F RID: 15999 RVA: 0x000CEC28 File Offset: 0x000CCE28
		// (set) Token: 0x06003E80 RID: 16000 RVA: 0x000CEC3B File Offset: 0x000CCE3B
		public byte NetworkMovementSmoothing
		{
			get
			{
				return this.MovementSmoothing;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<byte>(value, ref this.MovementSmoothing, 8UL, null);
			}
		}

		// Token: 0x17000E26 RID: 3622
		// (get) Token: 0x06003E81 RID: 16001 RVA: 0x000CEC58 File Offset: 0x000CCE58
		// (set) Token: 0x06003E82 RID: 16002 RVA: 0x000CEC6B File Offset: 0x000CCE6B
		public bool NetworkIsStatic
		{
			get
			{
				return this.IsStatic;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<bool>(value, ref this.IsStatic, 16UL, null);
			}
		}

		// Token: 0x06003E83 RID: 16003 RVA: 0x000CEC85 File Offset: 0x000CCE85
		protected void UserCode_RpcChangeParent__UInt32(uint parentId)
		{
			if (NetworkServer.active)
			{
				return;
			}
			this._clientParentId = parentId;
			this.UpdateParent();
		}

		// Token: 0x06003E84 RID: 16004 RVA: 0x000CEC9C File Offset: 0x000CCE9C
		protected static void InvokeUserCode_RpcChangeParent__UInt32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcChangeParent called on server.");
				return;
			}
			((AdminToyBase)obj).UserCode_RpcChangeParent__UInt32(reader.ReadUInt());
		}

		// Token: 0x06003E85 RID: 16005 RVA: 0x000CECC5 File Offset: 0x000CCEC5
		static AdminToyBase()
		{
			RemoteProcedureCalls.RegisterRpc(typeof(AdminToyBase), "System.Void AdminToys.AdminToyBase::RpcChangeParent(System.UInt32)", new RemoteCallDelegate(AdminToyBase.InvokeUserCode_RpcChangeParent__UInt32));
		}

		// Token: 0x06003E86 RID: 16006 RVA: 0x000CECE8 File Offset: 0x000CCEE8
		public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
		{
			base.SerializeSyncVars(writer, forceAll);
			if (forceAll)
			{
				writer.WriteVector3(this.Position);
				writer.WriteQuaternion(this.Rotation);
				writer.WriteVector3(this.Scale);
				writer.WriteByte(this.MovementSmoothing);
				writer.WriteBool(this.IsStatic);
				return;
			}
			writer.WriteULong(base.syncVarDirtyBits);
			if ((base.syncVarDirtyBits & 1UL) != 0UL)
			{
				writer.WriteVector3(this.Position);
			}
			if ((base.syncVarDirtyBits & 2UL) != 0UL)
			{
				writer.WriteQuaternion(this.Rotation);
			}
			if ((base.syncVarDirtyBits & 4UL) != 0UL)
			{
				writer.WriteVector3(this.Scale);
			}
			if ((base.syncVarDirtyBits & 8UL) != 0UL)
			{
				writer.WriteByte(this.MovementSmoothing);
			}
			if ((base.syncVarDirtyBits & 16UL) != 0UL)
			{
				writer.WriteBool(this.IsStatic);
			}
		}

		// Token: 0x06003E87 RID: 16007 RVA: 0x000CEDF8 File Offset: 0x000CCFF8
		public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
		{
			base.DeserializeSyncVars(reader, initialState);
			if (initialState)
			{
				base.GeneratedSyncVarDeserialize<Vector3>(ref this.Position, null, reader.ReadVector3());
				base.GeneratedSyncVarDeserialize<Quaternion>(ref this.Rotation, null, reader.ReadQuaternion());
				base.GeneratedSyncVarDeserialize<Vector3>(ref this.Scale, null, reader.ReadVector3());
				base.GeneratedSyncVarDeserialize<byte>(ref this.MovementSmoothing, null, reader.ReadByte());
				base.GeneratedSyncVarDeserialize<bool>(ref this.IsStatic, null, reader.ReadBool());
				return;
			}
			long num = (long)reader.ReadULong();
			if ((num & 1L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<Vector3>(ref this.Position, null, reader.ReadVector3());
			}
			if ((num & 2L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<Quaternion>(ref this.Rotation, null, reader.ReadQuaternion());
			}
			if ((num & 4L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<Vector3>(ref this.Scale, null, reader.ReadVector3());
			}
			if ((num & 8L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<byte>(ref this.MovementSmoothing, null, reader.ReadByte());
			}
			if ((num & 16L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<bool>(ref this.IsStatic, null, reader.ReadBool());
			}
		}

		// Token: 0x04002DBB RID: 11707
		[SyncVar]
		public Vector3 Position;

		// Token: 0x04002DBC RID: 11708
		[SyncVar]
		public Quaternion Rotation;

		// Token: 0x04002DBD RID: 11709
		[SyncVar]
		public Vector3 Scale;

		// Token: 0x04002DBE RID: 11710
		[SyncVar]
		public byte MovementSmoothing;

		// Token: 0x04002DBF RID: 11711
		[SyncVar]
		public bool IsStatic;

		// Token: 0x04002DC0 RID: 11712
		private uint _clientParentId;

		// Token: 0x04002DC1 RID: 11713
		private Transform _previousParent;

		// Token: 0x04002DC2 RID: 11714
		private Transform _trCache;

		// Token: 0x04002DC3 RID: 11715
		private const float SmoothingMultiplier = 0.3f;
	}
}
