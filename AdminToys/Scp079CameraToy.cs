using System;
using System.Runtime.InteropServices;
using MapGeneration;
using Mirror;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PlayerRoles.PlayableScps.Scp079.GUI;
using PlayerRoles.PlayableScps.Scp079.Overcons;
using UnityEngine;

namespace AdminToys
{
	// Token: 0x02000ADE RID: 2782
	[RequireComponent(typeof(Scp079InteractableBase))]
	public class Scp079CameraToy : AdminToyBase
	{
		// Token: 0x17000E3F RID: 3647
		// (get) Token: 0x06003EF7 RID: 16119 RVA: 0x000D0718 File Offset: 0x000CE918
		public override string CommandName
		{
			get
			{
				return "Camera" + this._commandName;
			}
		}

		// Token: 0x06003EF8 RID: 16120 RVA: 0x000D072C File Offset: 0x000CE92C
		public override void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
		{
			string[] array = arguments.Array;
			float num;
			float d = (array.Length > 2 && float.TryParse(array[2], out num)) ? num : 1f;
			base.transform.SetPositionAndRotation(admin.PlayerCameraReference.position, admin.PlayerCameraReference.rotation);
			base.transform.localScale = Vector3.one * d;
			base.NetworkScale = base.transform.localScale;
			base.OnSpawned(admin, arguments);
		}

		// Token: 0x06003EF9 RID: 16121 RVA: 0x000D07AC File Offset: 0x000CE9AC
		public override void OnStartServer()
		{
			base.OnStartServer();
			Scp079InteractableBase.InstancesCount++;
			this.Camera.SyncId = (ushort)Scp079InteractableBase.InstancesCount;
			Scp079InteractableBase.OrderedInstances.Add(this.Camera);
			this.Camera.Position = base.transform.position;
			this.NetworkLabel = this.Camera.Label;
			this.NetworkVerticalConstraint = this.Camera.VerticalAxis.Constraints;
			this.NetworkHorizontalConstraint = this.Camera.HorizontalAxis.Constraints;
			this.NetworkZoomConstraint = this.Camera.ZoomAxis.Constraints;
			this.SetRoom(null, this.Room);
			this.NetworkRoom = this.Camera.Room;
		}

		// Token: 0x06003EFA RID: 16122 RVA: 0x000D0874 File Offset: 0x000CEA74
		public override void OnStartClient()
		{
			base.OnStartClient();
			if (NetworkServer.active)
			{
				return;
			}
			this.Camera.SyncId = this._clientSyncId;
			int num = Mathf.Max(0, (int)this._clientSyncId - Scp079InteractableBase.OrderedInstances.Count);
			for (int i = 0; i < num; i++)
			{
				Scp079InteractableBase.OrderedInstances.Add(null);
			}
			Scp079InteractableBase.OrderedInstances[(int)(this._clientSyncId - 1)] = this.Camera;
			Scp079InteractableBase.InstancesCount = Scp079InteractableBase.OrderedInstances.Count;
			CameraOverconRenderer cameraOverconRenderer;
			if (this.CurrentOvercon == null && OverconManager.Singleton != null && OverconManager.Singleton.TryGetComponent<CameraOverconRenderer>(out cameraOverconRenderer))
			{
				cameraOverconRenderer.SpawnOvercon(Scp079Hud.Instance.CurrentCamera, this.Camera);
			}
		}

		// Token: 0x06003EFB RID: 16123 RVA: 0x000D0935 File Offset: 0x000CEB35
		public override void OnSerialize(NetworkWriter writer, bool initialState)
		{
			base.OnSerialize(writer, initialState);
			if (initialState)
			{
				writer.WriteUShort(this.Camera.SyncId);
			}
		}

		// Token: 0x06003EFC RID: 16124 RVA: 0x000D0953 File Offset: 0x000CEB53
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			base.OnDeserialize(reader, initialState);
			if (initialState)
			{
				this._clientSyncId = reader.ReadUShort();
			}
		}

		// Token: 0x06003EFD RID: 16125 RVA: 0x000D096C File Offset: 0x000CEB6C
		protected override void UpdatePositionClient(bool teleport = false)
		{
			base.UpdatePositionClient(teleport);
			this.Camera.Position = base.transform.position;
			if (this.CurrentOvercon == null)
			{
				return;
			}
			if (this.CurrentOvercon.Target == this.Camera && this.CurrentOvercon.gameObject.activeSelf)
			{
				this.CurrentOvercon.Position = this.Camera.Position;
				if (Scp079Hud.Instance != null && Scp079Hud.Instance.CurrentCamera != null)
				{
					this.CurrentOvercon.Rescale(Scp079Hud.Instance.CurrentCamera);
					return;
				}
			}
			else
			{
				this.CurrentOvercon = null;
			}
		}

		// Token: 0x06003EFE RID: 16126 RVA: 0x000D0A21 File Offset: 0x000CEC21
		private void SetLabel(string _, string newLabel)
		{
			this.Camera.Label = newLabel;
		}

		// Token: 0x06003EFF RID: 16127 RVA: 0x000D0A30 File Offset: 0x000CEC30
		private void SetRoom(RoomIdentifier _, RoomIdentifier newRoom)
		{
			if (newRoom != null)
			{
				this.Camera.Room = newRoom;
				return;
			}
			RoomIdentifier room;
			if (base.transform.position.TryGetRoom(out room))
			{
				this.Camera.Room = room;
				return;
			}
			RoomIdentifier roomIdentifier = null;
			float num = 0f;
			foreach (RoomIdentifier roomIdentifier2 in RoomIdentifier.AllRoomIdentifiers)
			{
				float sqrMagnitude = (this.Camera.Position - roomIdentifier2.gameObject.transform.position).sqrMagnitude;
				if (roomIdentifier == null || sqrMagnitude < num)
				{
					roomIdentifier = roomIdentifier2;
					num = sqrMagnitude;
				}
			}
			this.Camera.Room = roomIdentifier;
		}

		// Token: 0x06003F00 RID: 16128 RVA: 0x000D0B08 File Offset: 0x000CED08
		private void SetVerticalConstraint(Vector2 _, Vector2 newConstraint)
		{
			this.Camera.VerticalAxis.Constraints = newConstraint;
		}

		// Token: 0x06003F01 RID: 16129 RVA: 0x000D0B1B File Offset: 0x000CED1B
		private void SetHorizontalConstraint(Vector2 _, Vector2 newConstraint)
		{
			this.Camera.HorizontalAxis.Constraints = newConstraint;
		}

		// Token: 0x06003F02 RID: 16130 RVA: 0x000D0B2E File Offset: 0x000CED2E
		private void SetZoomConstraint(Vector2 _, Vector2 newConstraint)
		{
			this.Camera.ZoomAxis.Constraints = newConstraint;
		}

		// Token: 0x06003F04 RID: 16132 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x17000E40 RID: 3648
		// (get) Token: 0x06003F05 RID: 16133 RVA: 0x000D0B44 File Offset: 0x000CED44
		// (set) Token: 0x06003F06 RID: 16134 RVA: 0x000D0B57 File Offset: 0x000CED57
		public string NetworkLabel
		{
			get
			{
				return this.Label;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<string>(value, ref this.Label, 32UL, new Action<string, string>(this.SetLabel));
			}
		}

		// Token: 0x17000E41 RID: 3649
		// (get) Token: 0x06003F07 RID: 16135 RVA: 0x000D0B7C File Offset: 0x000CED7C
		// (set) Token: 0x06003F08 RID: 16136 RVA: 0x000D0B8F File Offset: 0x000CED8F
		public RoomIdentifier NetworkRoom
		{
			get
			{
				return this.Room;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<RoomIdentifier>(value, ref this.Room, 64UL, new Action<RoomIdentifier, RoomIdentifier>(this.SetRoom));
			}
		}

		// Token: 0x17000E42 RID: 3650
		// (get) Token: 0x06003F09 RID: 16137 RVA: 0x000D0BB4 File Offset: 0x000CEDB4
		// (set) Token: 0x06003F0A RID: 16138 RVA: 0x000D0BC7 File Offset: 0x000CEDC7
		public Vector2 NetworkVerticalConstraint
		{
			get
			{
				return this.VerticalConstraint;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<Vector2>(value, ref this.VerticalConstraint, 128UL, new Action<Vector2, Vector2>(this.SetVerticalConstraint));
			}
		}

		// Token: 0x17000E43 RID: 3651
		// (get) Token: 0x06003F0B RID: 16139 RVA: 0x000D0BEC File Offset: 0x000CEDEC
		// (set) Token: 0x06003F0C RID: 16140 RVA: 0x000D0BFF File Offset: 0x000CEDFF
		public Vector2 NetworkHorizontalConstraint
		{
			get
			{
				return this.HorizontalConstraint;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<Vector2>(value, ref this.HorizontalConstraint, 256UL, new Action<Vector2, Vector2>(this.SetHorizontalConstraint));
			}
		}

		// Token: 0x17000E44 RID: 3652
		// (get) Token: 0x06003F0D RID: 16141 RVA: 0x000D0C24 File Offset: 0x000CEE24
		// (set) Token: 0x06003F0E RID: 16142 RVA: 0x000D0C37 File Offset: 0x000CEE37
		public Vector2 NetworkZoomConstraint
		{
			get
			{
				return this.ZoomConstraint;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<Vector2>(value, ref this.ZoomConstraint, 512UL, new Action<Vector2, Vector2>(this.SetZoomConstraint));
			}
		}

		// Token: 0x06003F0F RID: 16143 RVA: 0x000D0C5C File Offset: 0x000CEE5C
		public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
		{
			base.SerializeSyncVars(writer, forceAll);
			if (forceAll)
			{
				writer.WriteString(this.Label);
				writer.WriteRoomIdentifier(this.Room);
				writer.WriteVector2(this.VerticalConstraint);
				writer.WriteVector2(this.HorizontalConstraint);
				writer.WriteVector2(this.ZoomConstraint);
				return;
			}
			writer.WriteULong(base.syncVarDirtyBits);
			if ((base.syncVarDirtyBits & 32UL) != 0UL)
			{
				writer.WriteString(this.Label);
			}
			if ((base.syncVarDirtyBits & 64UL) != 0UL)
			{
				writer.WriteRoomIdentifier(this.Room);
			}
			if ((base.syncVarDirtyBits & 128UL) != 0UL)
			{
				writer.WriteVector2(this.VerticalConstraint);
			}
			if ((base.syncVarDirtyBits & 256UL) != 0UL)
			{
				writer.WriteVector2(this.HorizontalConstraint);
			}
			if ((base.syncVarDirtyBits & 512UL) != 0UL)
			{
				writer.WriteVector2(this.ZoomConstraint);
			}
		}

		// Token: 0x06003F10 RID: 16144 RVA: 0x000D0D6C File Offset: 0x000CEF6C
		public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
		{
			base.DeserializeSyncVars(reader, initialState);
			if (initialState)
			{
				base.GeneratedSyncVarDeserialize<string>(ref this.Label, new Action<string, string>(this.SetLabel), reader.ReadString());
				base.GeneratedSyncVarDeserialize<RoomIdentifier>(ref this.Room, new Action<RoomIdentifier, RoomIdentifier>(this.SetRoom), reader.ReadRoomIdentifier());
				base.GeneratedSyncVarDeserialize<Vector2>(ref this.VerticalConstraint, new Action<Vector2, Vector2>(this.SetVerticalConstraint), reader.ReadVector2());
				base.GeneratedSyncVarDeserialize<Vector2>(ref this.HorizontalConstraint, new Action<Vector2, Vector2>(this.SetHorizontalConstraint), reader.ReadVector2());
				base.GeneratedSyncVarDeserialize<Vector2>(ref this.ZoomConstraint, new Action<Vector2, Vector2>(this.SetZoomConstraint), reader.ReadVector2());
				return;
			}
			long num = (long)reader.ReadULong();
			if ((num & 32L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<string>(ref this.Label, new Action<string, string>(this.SetLabel), reader.ReadString());
			}
			if ((num & 64L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<RoomIdentifier>(ref this.Room, new Action<RoomIdentifier, RoomIdentifier>(this.SetRoom), reader.ReadRoomIdentifier());
			}
			if ((num & 128L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<Vector2>(ref this.VerticalConstraint, new Action<Vector2, Vector2>(this.SetVerticalConstraint), reader.ReadVector2());
			}
			if ((num & 256L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<Vector2>(ref this.HorizontalConstraint, new Action<Vector2, Vector2>(this.SetHorizontalConstraint), reader.ReadVector2());
			}
			if ((num & 512L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<Vector2>(ref this.ZoomConstraint, new Action<Vector2, Vector2>(this.SetZoomConstraint), reader.ReadVector2());
			}
		}

		// Token: 0x04002DF2 RID: 11762
		[SyncVar(hook = "SetLabel")]
		[NonSerialized]
		public string Label;

		// Token: 0x04002DF3 RID: 11763
		[SyncVar(hook = "SetRoom")]
		[NonSerialized]
		public RoomIdentifier Room;

		// Token: 0x04002DF4 RID: 11764
		[SyncVar(hook = "SetVerticalConstraint")]
		[NonSerialized]
		public Vector2 VerticalConstraint;

		// Token: 0x04002DF5 RID: 11765
		[SyncVar(hook = "SetHorizontalConstraint")]
		[NonSerialized]
		public Vector2 HorizontalConstraint;

		// Token: 0x04002DF6 RID: 11766
		[SyncVar(hook = "SetZoomConstraint")]
		[NonSerialized]
		public Vector2 ZoomConstraint;

		// Token: 0x04002DF7 RID: 11767
		[NonSerialized]
		public CameraOvercon CurrentOvercon;

		// Token: 0x04002DF8 RID: 11768
		public FacilityZone ZoneIcon;

		// Token: 0x04002DF9 RID: 11769
		public Scp079Camera Camera;

		// Token: 0x04002DFA RID: 11770
		[SerializeField]
		private string _commandName;

		// Token: 0x04002DFB RID: 11771
		private ushort _clientSyncId;
	}
}
