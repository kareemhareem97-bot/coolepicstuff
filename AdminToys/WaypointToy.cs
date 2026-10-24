using System;
using System.Runtime.InteropServices;
using Mirror;
using RelativePositioning;
using UnityEngine;

namespace AdminToys
{
	// Token: 0x02000AE5 RID: 2789
	public class WaypointToy : AdminToyBase
	{
		// Token: 0x17000E5B RID: 3675
		// (get) Token: 0x06003F6B RID: 16235 RVA: 0x000D2492 File Offset: 0x000D0692
		public override string CommandName
		{
			get
			{
				return "Waypoint";
			}
		}

		// Token: 0x06003F6C RID: 16236 RVA: 0x000D249C File Offset: 0x000D069C
		public override void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
		{
			string[] array = arguments.Array;
			float num;
			float x = (array.Length > 2 && float.TryParse(array[2], out num)) ? num : 255.9961f;
			float num2;
			float y = (array.Length > 3 && float.TryParse(array[3], out num2)) ? num2 : 255.9961f;
			float num3;
			float z = (array.Length > 4 && float.TryParse(array[4], out num3)) ? num3 : 255.9961f;
			this.NetworkBoundsSize = new Vector3(x, y, z);
			float num4;
			this.NetworkPriority = ((array.Length > 5 && float.TryParse(array[5], out num4)) ? num4 : 0f);
			bool flag;
			this.NetworkVisualizeBounds = (array.Length > 6 && bool.TryParse(array[6], out flag) && flag);
			base.transform.SetPositionAndRotation(admin.PlayerCameraReference.position, admin.PlayerCameraReference.rotation);
			base.OnSpawned(admin, arguments);
		}

		// Token: 0x06003F6D RID: 16237 RVA: 0x000D257C File Offset: 0x000D077C
		public void UpdateWaypointChildren()
		{
			Vector3 vector;
			Quaternion quaternion;
			base.transform.GetPositionAndRotation(out vector, out quaternion);
			this._waypoint.OnMoved(vector - this._prevPos, quaternion * Quaternion.Inverse(this._prevRot));
			this._prevPos = vector;
			this._prevRot = quaternion;
		}

		// Token: 0x06003F6E RID: 16238 RVA: 0x000D25CE File Offset: 0x000D07CE
		protected override void Start()
		{
			base.Start();
			base.transform.GetPositionAndRotation(out this._prevPos, out this._prevRot);
		}

		// Token: 0x06003F6F RID: 16239 RVA: 0x000D25ED File Offset: 0x000D07ED
		protected override void LateUpdate()
		{
			this._waypoint.IsStatic = this.IsStatic;
			this._waypoint.Priority = this.Priority;
			if (this.IsStatic)
			{
				return;
			}
			base.LateUpdate();
			this.UpdateWaypointChildren();
		}

		// Token: 0x06003F70 RID: 16240 RVA: 0x000D2626 File Offset: 0x000D0826
		public override void OnStartServer()
		{
			base.OnStartServer();
			this._waypointId = this._waypoint.ServerAssignId();
		}

		// Token: 0x06003F71 RID: 16241 RVA: 0x000D263F File Offset: 0x000D083F
		public override void OnStartClient()
		{
			base.OnStartClient();
			if (NetworkServer.active)
			{
				return;
			}
			this._waypoint.ClientAssignId(this._waypointId);
		}

		// Token: 0x06003F72 RID: 16242 RVA: 0x000D2660 File Offset: 0x000D0860
		public override void OnSerialize(NetworkWriter writer, bool initialState)
		{
			base.OnSerialize(writer, initialState);
			if (initialState)
			{
				writer.WriteByte(this._waypointId);
			}
		}

		// Token: 0x06003F73 RID: 16243 RVA: 0x000D2679 File Offset: 0x000D0879
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			base.OnDeserialize(reader, initialState);
			if (initialState)
			{
				this._waypointId = reader.ReadByte();
			}
		}

		// Token: 0x06003F74 RID: 16244 RVA: 0x000D2692 File Offset: 0x000D0892
		private void OnVisualizeBoundsSet(bool prevState, bool newState)
		{
			this._visualizer.enabled = newState;
		}

		// Token: 0x06003F75 RID: 16245 RVA: 0x000D26A0 File Offset: 0x000D08A0
		private void OnBoundsSizeSet(Vector3 _, Vector3 newSize)
		{
			this.NetworkBoundsSize = Vector3.Min(newSize, Vector3.one * 255.9961f).Abs();
			this._visualizer.transform.localScale = this.BoundsSize;
		}

		// Token: 0x06003F77 RID: 16247 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x17000E5C RID: 3676
		// (get) Token: 0x06003F78 RID: 16248 RVA: 0x000D26F8 File Offset: 0x000D08F8
		// (set) Token: 0x06003F79 RID: 16249 RVA: 0x000D270B File Offset: 0x000D090B
		public bool NetworkVisualizeBounds
		{
			get
			{
				return this.VisualizeBounds;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<bool>(value, ref this.VisualizeBounds, 32UL, new Action<bool, bool>(this.OnVisualizeBoundsSet));
			}
		}

		// Token: 0x17000E5D RID: 3677
		// (get) Token: 0x06003F7A RID: 16250 RVA: 0x000D2730 File Offset: 0x000D0930
		// (set) Token: 0x06003F7B RID: 16251 RVA: 0x000D2743 File Offset: 0x000D0943
		public float NetworkPriority
		{
			get
			{
				return this.Priority;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<float>(value, ref this.Priority, 64UL, null);
			}
		}

		// Token: 0x17000E5E RID: 3678
		// (get) Token: 0x06003F7C RID: 16252 RVA: 0x000D2760 File Offset: 0x000D0960
		// (set) Token: 0x06003F7D RID: 16253 RVA: 0x000D2773 File Offset: 0x000D0973
		public Vector3 NetworkBoundsSize
		{
			get
			{
				return this.BoundsSize;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<Vector3>(value, ref this.BoundsSize, 128UL, new Action<Vector3, Vector3>(this.OnBoundsSizeSet));
			}
		}

		// Token: 0x06003F7E RID: 16254 RVA: 0x000D2798 File Offset: 0x000D0998
		public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
		{
			base.SerializeSyncVars(writer, forceAll);
			if (forceAll)
			{
				writer.WriteBool(this.VisualizeBounds);
				writer.WriteFloat(this.Priority);
				writer.WriteVector3(this.BoundsSize);
				return;
			}
			writer.WriteULong(base.syncVarDirtyBits);
			if ((base.syncVarDirtyBits & 32UL) != 0UL)
			{
				writer.WriteBool(this.VisualizeBounds);
			}
			if ((base.syncVarDirtyBits & 64UL) != 0UL)
			{
				writer.WriteFloat(this.Priority);
			}
			if ((base.syncVarDirtyBits & 128UL) != 0UL)
			{
				writer.WriteVector3(this.BoundsSize);
			}
		}

		// Token: 0x06003F7F RID: 16255 RVA: 0x000D284C File Offset: 0x000D0A4C
		public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
		{
			base.DeserializeSyncVars(reader, initialState);
			if (initialState)
			{
				base.GeneratedSyncVarDeserialize<bool>(ref this.VisualizeBounds, new Action<bool, bool>(this.OnVisualizeBoundsSet), reader.ReadBool());
				base.GeneratedSyncVarDeserialize<float>(ref this.Priority, null, reader.ReadFloat());
				base.GeneratedSyncVarDeserialize<Vector3>(ref this.BoundsSize, new Action<Vector3, Vector3>(this.OnBoundsSizeSet), reader.ReadVector3());
				return;
			}
			long num = (long)reader.ReadULong();
			if ((num & 32L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<bool>(ref this.VisualizeBounds, new Action<bool, bool>(this.OnVisualizeBoundsSet), reader.ReadBool());
			}
			if ((num & 64L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<float>(ref this.Priority, null, reader.ReadFloat());
			}
			if ((num & 128L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<Vector3>(ref this.BoundsSize, new Action<Vector3, Vector3>(this.OnBoundsSizeSet), reader.ReadVector3());
			}
		}

		// Token: 0x04002E29 RID: 11817
		public const float MaxBounds = 255.9961f;

		// Token: 0x04002E2A RID: 11818
		[SyncVar(hook = "OnVisualizeBoundsSet")]
		public bool VisualizeBounds;

		// Token: 0x04002E2B RID: 11819
		[SyncVar]
		public float Priority;

		// Token: 0x04002E2C RID: 11820
		[SyncVar(hook = "OnBoundsSizeSet")]
		[NonSerialized]
		public Vector3 BoundsSize = Vector3.one * 255.9961f;

		// Token: 0x04002E2D RID: 11821
		[SerializeField]
		private AdminToyWaypoint _waypoint;

		// Token: 0x04002E2E RID: 11822
		[SerializeField]
		private Renderer _visualizer;

		// Token: 0x04002E2F RID: 11823
		private Vector3 _prevPos;

		// Token: 0x04002E30 RID: 11824
		private Quaternion _prevRot;

		// Token: 0x04002E31 RID: 11825
		private byte _waypointId;
	}
}
