using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MapGeneration;
using MapGeneration.StaticHelpers;
using Mirror;
using UnityEngine;

namespace AdminToys
{
	// Token: 0x02000ADB RID: 2779
	public class PrimitiveObjectToy : AdminToyBase, IRoomObject
	{
		// Token: 0x06003EDE RID: 16094 RVA: 0x000D014D File Offset: 0x000CE34D
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			CustomNetworkManager.OnClientReady += delegate()
			{
				PrimitiveObjectToy.CachedMaterials.Clear();
				PrimitiveObjectToy.PrimitiveTypeToMesh.Clear();
				foreach (PrimitiveType primitiveType in EnumUtils<PrimitiveType>.Values)
				{
					GameObject gameObject = GameObject.CreatePrimitive(primitiveType);
					Mesh sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
					PrimitiveObjectToy.PrimitiveTypeToMesh.Add(primitiveType, sharedMesh);
					UnityEngine.Object.Destroy(gameObject);
				}
			};
		}

		// Token: 0x17000E3A RID: 3642
		// (get) Token: 0x06003EDF RID: 16095 RVA: 0x000D0173 File Offset: 0x000CE373
		public override string CommandName
		{
			get
			{
				return "PrimitiveObject";
			}
		}

		// Token: 0x17000E3B RID: 3643
		// (get) Token: 0x06003EE0 RID: 16096 RVA: 0x000D017A File Offset: 0x000CE37A
		// (set) Token: 0x06003EE1 RID: 16097 RVA: 0x000D0182 File Offset: 0x000CE382
		public RoomIdentifier OriginalRoom { get; set; }

		// Token: 0x06003EE2 RID: 16098 RVA: 0x000D018C File Offset: 0x000CE38C
		public override void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
		{
			string[] array = arguments.Array;
			PrimitiveType primitiveType;
			this.NetworkPrimitiveType = ((array.Length > 2 && Enum.TryParse<PrimitiveType>(array[2], true, out primitiveType)) ? primitiveType : PrimitiveType.Sphere);
			Color color;
			this.NetworkMaterialColor = ((array.Length > 3 && ColorUtility.TryParseHtmlString(array[3], out color)) ? color : Color.gray);
			float num;
			float d = (array.Length > 4 && float.TryParse(array[4], out num)) ? num : 1f;
			PrimitiveFlags primitiveFlags;
			this.NetworkPrimitiveFlags = ((array.Length > 5 && Enum.TryParse<PrimitiveFlags>(array[5], true, out primitiveFlags)) ? primitiveFlags : ((PrimitiveFlags)255));
			base.transform.SetPositionAndRotation(admin.PlayerCameraReference.position, admin.PlayerCameraReference.rotation);
			base.transform.localScale = Vector3.one * d;
			base.NetworkScale = base.transform.localScale;
			base.OnSpawned(admin, arguments);
		}

		// Token: 0x06003EE3 RID: 16099 RVA: 0x000D026C File Offset: 0x000CE46C
		public void RefreshRoom()
		{
			RoomIdentifier originalRoom;
			if (base.transform.position.TryGetCurrentRoomByBounds(out originalRoom))
			{
				this.OriginalRoom = originalRoom;
			}
		}

		// Token: 0x06003EE4 RID: 16100 RVA: 0x000D0294 File Offset: 0x000CE494
		protected override void Start()
		{
			base.Start();
			this.RefreshRoom();
			this.SetPrimitive(PrimitiveType.Sphere, this.PrimitiveType);
		}

		// Token: 0x06003EE5 RID: 16101 RVA: 0x000D02B0 File Offset: 0x000CE4B0
		private void SetPrimitive(PrimitiveType _, PrimitiveType newPrim)
		{
			this._filter.sharedMesh = PrimitiveObjectToy.PrimitiveTypeToMesh[newPrim];
			if (this._collider != null)
			{
				UnityEngine.Object.Destroy(this._collider);
			}
			if (newPrim == PrimitiveType.Cube)
			{
				this._collider = base.gameObject.AddComponent<BoxCollider>();
			}
			else
			{
				MeshCollider meshCollider = base.gameObject.AddComponent<MeshCollider>();
				bool convex = newPrim != PrimitiveType.Plane && newPrim != PrimitiveType.Quad;
				meshCollider.convex = convex;
				this._collider = meshCollider;
			}
			this.SetColor(Color.clear, this.MaterialColor);
			this.SetFlags(PrimitiveFlags.None, this.PrimitiveFlags);
		}

		// Token: 0x06003EE6 RID: 16102 RVA: 0x000D034A File Offset: 0x000CE54A
		private void SetColor(Color _, Color newColor)
		{
			this._renderer.sharedMaterial = this.GetMaterialFromColor(newColor);
		}

		// Token: 0x06003EE7 RID: 16103 RVA: 0x000D0360 File Offset: 0x000CE560
		private Material GetMaterialFromColor(Color color)
		{
			Material material;
			if (PrimitiveObjectToy.CachedMaterials.TryGetValue(color, out material))
			{
				return material;
			}
			material = ((color.a >= 1f) ? new Material(this._regularMatTemplate) : new Material(this._transparentMatTemplate));
			material.SetColor(PrimitiveObjectToy.BaseColor, color);
			PrimitiveObjectToy.CachedMaterials.Add(color, material);
			return material;
		}

		// Token: 0x06003EE8 RID: 16104 RVA: 0x000D03C0 File Offset: 0x000CE5C0
		private void SetFlags(PrimitiveFlags _, PrimitiveFlags newPrimitiveFlags)
		{
			if (this._collider != null)
			{
				this._collider.enabled = newPrimitiveFlags.HasFlag(PrimitiveFlags.Collidable);
			}
			this._renderer.enabled = newPrimitiveFlags.HasFlag(PrimitiveFlags.Visible);
		}

		// Token: 0x06003EEB RID: 16107 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x17000E3C RID: 3644
		// (get) Token: 0x06003EEC RID: 16108 RVA: 0x000D0448 File Offset: 0x000CE648
		// (set) Token: 0x06003EED RID: 16109 RVA: 0x000D045B File Offset: 0x000CE65B
		public PrimitiveType NetworkPrimitiveType
		{
			get
			{
				return this.PrimitiveType;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<PrimitiveType>(value, ref this.PrimitiveType, 32UL, new Action<PrimitiveType, PrimitiveType>(this.SetPrimitive));
			}
		}

		// Token: 0x17000E3D RID: 3645
		// (get) Token: 0x06003EEE RID: 16110 RVA: 0x000D0480 File Offset: 0x000CE680
		// (set) Token: 0x06003EEF RID: 16111 RVA: 0x000D0493 File Offset: 0x000CE693
		public Color NetworkMaterialColor
		{
			get
			{
				return this.MaterialColor;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<Color>(value, ref this.MaterialColor, 64UL, new Action<Color, Color>(this.SetColor));
			}
		}

		// Token: 0x17000E3E RID: 3646
		// (get) Token: 0x06003EF0 RID: 16112 RVA: 0x000D04B8 File Offset: 0x000CE6B8
		// (set) Token: 0x06003EF1 RID: 16113 RVA: 0x000D04CB File Offset: 0x000CE6CB
		public PrimitiveFlags NetworkPrimitiveFlags
		{
			get
			{
				return this.PrimitiveFlags;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<PrimitiveFlags>(value, ref this.PrimitiveFlags, 128UL, new Action<PrimitiveFlags, PrimitiveFlags>(this.SetFlags));
			}
		}

		// Token: 0x06003EF2 RID: 16114 RVA: 0x000D04F0 File Offset: 0x000CE6F0
		public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
		{
			base.SerializeSyncVars(writer, forceAll);
			if (forceAll)
			{
				Mirror.GeneratedNetworkCode._Write_UnityEngine.PrimitiveType(writer, this.PrimitiveType);
				writer.WriteColor(this.MaterialColor);
				Mirror.GeneratedNetworkCode._Write_AdminToys.PrimitiveFlags(writer, this.PrimitiveFlags);
				return;
			}
			writer.WriteULong(base.syncVarDirtyBits);
			if ((base.syncVarDirtyBits & 32UL) != 0UL)
			{
				Mirror.GeneratedNetworkCode._Write_UnityEngine.PrimitiveType(writer, this.PrimitiveType);
			}
			if ((base.syncVarDirtyBits & 64UL) != 0UL)
			{
				writer.WriteColor(this.MaterialColor);
			}
			if ((base.syncVarDirtyBits & 128UL) != 0UL)
			{
				Mirror.GeneratedNetworkCode._Write_AdminToys.PrimitiveFlags(writer, this.PrimitiveFlags);
			}
		}

		// Token: 0x06003EF3 RID: 16115 RVA: 0x000D05A4 File Offset: 0x000CE7A4
		public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
		{
			base.DeserializeSyncVars(reader, initialState);
			if (initialState)
			{
				base.GeneratedSyncVarDeserialize<PrimitiveType>(ref this.PrimitiveType, new Action<PrimitiveType, PrimitiveType>(this.SetPrimitive), Mirror.GeneratedNetworkCode._Read_UnityEngine.PrimitiveType(reader));
				base.GeneratedSyncVarDeserialize<Color>(ref this.MaterialColor, new Action<Color, Color>(this.SetColor), reader.ReadColor());
				base.GeneratedSyncVarDeserialize<PrimitiveFlags>(ref this.PrimitiveFlags, new Action<PrimitiveFlags, PrimitiveFlags>(this.SetFlags), Mirror.GeneratedNetworkCode._Read_AdminToys.PrimitiveFlags(reader));
				return;
			}
			long num = (long)reader.ReadULong();
			if ((num & 32L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<PrimitiveType>(ref this.PrimitiveType, new Action<PrimitiveType, PrimitiveType>(this.SetPrimitive), Mirror.GeneratedNetworkCode._Read_UnityEngine.PrimitiveType(reader));
			}
			if ((num & 64L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<Color>(ref this.MaterialColor, new Action<Color, Color>(this.SetColor), reader.ReadColor());
			}
			if ((num & 128L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<PrimitiveFlags>(ref this.PrimitiveFlags, new Action<PrimitiveFlags, PrimitiveFlags>(this.SetFlags), Mirror.GeneratedNetworkCode._Read_AdminToys.PrimitiveFlags(reader));
			}
		}

		// Token: 0x04002DE0 RID: 11744
		private static readonly Dictionary<Color, Material> CachedMaterials = new Dictionary<Color, Material>();

		// Token: 0x04002DE1 RID: 11745
		private static readonly Dictionary<PrimitiveType, Mesh> PrimitiveTypeToMesh = new Dictionary<PrimitiveType, Mesh>(6);

		// Token: 0x04002DE2 RID: 11746
		private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

		// Token: 0x04002DE3 RID: 11747
		[SerializeField]
		private Material _regularMatTemplate;

		// Token: 0x04002DE4 RID: 11748
		[SerializeField]
		private Material _transparentMatTemplate;

		// Token: 0x04002DE5 RID: 11749
		[SerializeField]
		private MeshFilter _filter;

		// Token: 0x04002DE6 RID: 11750
		[SerializeField]
		private MeshRenderer _renderer;

		// Token: 0x04002DE7 RID: 11751
		private Collider _collider;

		// Token: 0x04002DE9 RID: 11753
		[SyncVar(hook = "SetPrimitive")]
		public PrimitiveType PrimitiveType;

		// Token: 0x04002DEA RID: 11754
		[SyncVar(hook = "SetColor")]
		public Color MaterialColor;

		// Token: 0x04002DEB RID: 11755
		[SyncVar(hook = "SetFlags")]
		public PrimitiveFlags PrimitiveFlags = PrimitiveFlags.Collidable | PrimitiveFlags.Visible;
	}
}
