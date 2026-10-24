using System;
using System.Runtime.InteropServices;
using DrawableLine;
using Mirror;
using ProgressiveCulling;
using UnityEngine;

namespace AdminToys
{
	// Token: 0x02000AE2 RID: 2786
	public class SpawnableCullingParent : NetworkBehaviour, IRootCullable, ICullable, IBoundsCullable
	{
		// Token: 0x140000BB RID: 187
		// (add) Token: 0x06003F2B RID: 16171 RVA: 0x000D1818 File Offset: 0x000CFA18
		// (remove) Token: 0x06003F2C RID: 16172 RVA: 0x000D184C File Offset: 0x000CFA4C
		public static event Action<SpawnableCullingParent> OnAdded;

		// Token: 0x140000BC RID: 188
		// (add) Token: 0x06003F2D RID: 16173 RVA: 0x000D1880 File Offset: 0x000CFA80
		// (remove) Token: 0x06003F2E RID: 16174 RVA: 0x000D18B4 File Offset: 0x000CFAB4
		public static event Action<SpawnableCullingParent> OnRemoved;

		// Token: 0x17000E4A RID: 3658
		// (get) Token: 0x06003F2F RID: 16175 RVA: 0x00034727 File Offset: 0x00032927
		public RootCullablePriority Priority
		{
			get
			{
				return RootCullablePriority.Dynamic;
			}
		}

		// Token: 0x17000E4B RID: 3659
		// (get) Token: 0x06003F30 RID: 16176 RVA: 0x000D18E7 File Offset: 0x000CFAE7
		public bool ShouldBeVisible
		{
			get
			{
				return CullingCamera.CheckBoundsVisibility(this.WorldspaceBounds);
			}
		}

		// Token: 0x17000E4C RID: 3660
		// (get) Token: 0x06003F31 RID: 16177 RVA: 0x000D18F4 File Offset: 0x000CFAF4
		// (set) Token: 0x06003F32 RID: 16178 RVA: 0x000D18FC File Offset: 0x000CFAFC
		public Bounds WorldspaceBounds { get; private set; }

		// Token: 0x06003F33 RID: 16179 RVA: 0x000D1905 File Offset: 0x000CFB05
		public void SetupCache()
		{
			this.WorldspaceBounds = new Bounds(this.BoundsPosition, this.BoundsSize.Abs());
		}

		// Token: 0x06003F34 RID: 16180 RVA: 0x000D1923 File Offset: 0x000CFB23
		public void SetVisibility(bool isVisible)
		{
			base.gameObject.SetActive(isVisible);
		}

		// Token: 0x06003F35 RID: 16181 RVA: 0x000D1934 File Offset: 0x000CFB34
		public void DrawDebugBounds(Color? color, float duration = 5f)
		{
			Color value = color.GetValueOrDefault();
			if (color == null)
			{
				value = Color.green;
				color = new Color?(value);
			}
			DrawableLines.GenerateBounds(this.WorldspaceBounds, new float?(duration), color);
		}

		// Token: 0x06003F36 RID: 16182 RVA: 0x000D1972 File Offset: 0x000CFB72
		private void Start()
		{
			CullingCamera.RegisterRootCullable(this);
			Action<SpawnableCullingParent> onAdded = SpawnableCullingParent.OnAdded;
			if (onAdded == null)
			{
				return;
			}
			onAdded(this);
		}

		// Token: 0x06003F37 RID: 16183 RVA: 0x000D198A File Offset: 0x000CFB8A
		private void OnDestroy()
		{
			CullingCamera.UnregisterRootCullable(this);
			Action<SpawnableCullingParent> onRemoved = SpawnableCullingParent.OnRemoved;
			if (onRemoved == null)
			{
				return;
			}
			onRemoved(this);
		}

		// Token: 0x06003F38 RID: 16184 RVA: 0x000D19A4 File Offset: 0x000CFBA4
		private void SetBoundsPosition(Vector3 _, Vector3 newPos)
		{
			this.WorldspaceBounds = new Bounds(newPos, this.WorldspaceBounds.size);
			base.transform.position = newPos;
		}

		// Token: 0x06003F39 RID: 16185 RVA: 0x000D19D8 File Offset: 0x000CFBD8
		private void SetBoundsSize(Vector3 _, Vector3 newSize)
		{
			this.WorldspaceBounds = new Bounds(this.WorldspaceBounds.center, newSize.Abs());
		}

		// Token: 0x06003F3B RID: 16187 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x17000E4D RID: 3661
		// (get) Token: 0x06003F3C RID: 16188 RVA: 0x000D1A04 File Offset: 0x000CFC04
		// (set) Token: 0x06003F3D RID: 16189 RVA: 0x000D1A17 File Offset: 0x000CFC17
		public Vector3 NetworkBoundsPosition
		{
			get
			{
				return this.BoundsPosition;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<Vector3>(value, ref this.BoundsPosition, 1UL, new Action<Vector3, Vector3>(this.SetBoundsPosition));
			}
		}

		// Token: 0x17000E4E RID: 3662
		// (get) Token: 0x06003F3E RID: 16190 RVA: 0x000D1A3C File Offset: 0x000CFC3C
		// (set) Token: 0x06003F3F RID: 16191 RVA: 0x000D1A4F File Offset: 0x000CFC4F
		public Vector3 NetworkBoundsSize
		{
			get
			{
				return this.BoundsSize;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<Vector3>(value, ref this.BoundsSize, 2UL, new Action<Vector3, Vector3>(this.SetBoundsSize));
			}
		}

		// Token: 0x06003F40 RID: 16192 RVA: 0x000D1A74 File Offset: 0x000CFC74
		public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
		{
			base.SerializeSyncVars(writer, forceAll);
			if (forceAll)
			{
				writer.WriteVector3(this.BoundsPosition);
				writer.WriteVector3(this.BoundsSize);
				return;
			}
			writer.WriteULong(base.syncVarDirtyBits);
			if ((base.syncVarDirtyBits & 1UL) != 0UL)
			{
				writer.WriteVector3(this.BoundsPosition);
			}
			if ((base.syncVarDirtyBits & 2UL) != 0UL)
			{
				writer.WriteVector3(this.BoundsSize);
			}
		}

		// Token: 0x06003F41 RID: 16193 RVA: 0x000D1AFC File Offset: 0x000CFCFC
		public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
		{
			base.DeserializeSyncVars(reader, initialState);
			if (initialState)
			{
				base.GeneratedSyncVarDeserialize<Vector3>(ref this.BoundsPosition, new Action<Vector3, Vector3>(this.SetBoundsPosition), reader.ReadVector3());
				base.GeneratedSyncVarDeserialize<Vector3>(ref this.BoundsSize, new Action<Vector3, Vector3>(this.SetBoundsSize), reader.ReadVector3());
				return;
			}
			long num = (long)reader.ReadULong();
			if ((num & 1L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<Vector3>(ref this.BoundsPosition, new Action<Vector3, Vector3>(this.SetBoundsPosition), reader.ReadVector3());
			}
			if ((num & 2L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<Vector3>(ref this.BoundsSize, new Action<Vector3, Vector3>(this.SetBoundsSize), reader.ReadVector3());
			}
		}

		// Token: 0x04002E1B RID: 11803
		[SyncVar(hook = "SetBoundsPosition")]
		public Vector3 BoundsPosition;

		// Token: 0x04002E1C RID: 11804
		[SyncVar(hook = "SetBoundsSize")]
		public Vector3 BoundsSize;
	}
}
