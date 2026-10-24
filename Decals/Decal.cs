using System;
using ProgressiveCulling;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Decals
{
	// Token: 0x02001104 RID: 4356
	[RequireComponent(typeof(DecalProjector))]
	public class Decal : CullableBehaviour
	{
		// Token: 0x17001423 RID: 5155
		// (get) Token: 0x060060E7 RID: 24807 RVA: 0x0014338F File Offset: 0x0014158F
		// (set) Token: 0x060060E8 RID: 24808 RVA: 0x00143398 File Offset: 0x00141598
		public Color InstancedColor
		{
			get
			{
				return this._instancedColor;
			}
			set
			{
				if (this._materialInstance == null)
				{
					this._materialInstance = new Material(this._projector.material);
					this._projector.material = this._materialInstance;
				}
				this._instancedColor = value;
				this._projector.material.SetColor("_BaseColor", this._instancedColor);
			}
		}

		// Token: 0x17001424 RID: 5156
		// (get) Token: 0x060060E9 RID: 24809 RVA: 0x001433FC File Offset: 0x001415FC
		// (set) Token: 0x060060EA RID: 24810 RVA: 0x00143409 File Offset: 0x00141609
		public Material DecalMaterial
		{
			get
			{
				return this._projector.material;
			}
			set
			{
				this._projector.material = value;
			}
		}

		// Token: 0x17001425 RID: 5157
		// (get) Token: 0x060060EB RID: 24811 RVA: 0x00143417 File Offset: 0x00141617
		// (set) Token: 0x060060EC RID: 24812 RVA: 0x00143424 File Offset: 0x00141624
		public Vector3 DecalSize
		{
			get
			{
				return this._projector.size;
			}
			set
			{
				this._projector.size = value;
			}
		}

		// Token: 0x17001426 RID: 5158
		// (get) Token: 0x060060ED RID: 24813 RVA: 0x00143432 File Offset: 0x00141632
		// (set) Token: 0x060060EE RID: 24814 RVA: 0x0014343F File Offset: 0x0014163F
		public Vector2 UVOffset
		{
			get
			{
				return this._projector.uvBias;
			}
			set
			{
				this._projector.uvBias = value;
			}
		}

		// Token: 0x17001427 RID: 5159
		// (get) Token: 0x060060EF RID: 24815 RVA: 0x0014344D File Offset: 0x0014164D
		// (set) Token: 0x060060F0 RID: 24816 RVA: 0x0014345A File Offset: 0x0014165A
		public Vector2 UVTiling
		{
			get
			{
				return this._projector.uvScale;
			}
			set
			{
				this._projector.uvScale = value;
			}
		}

		// Token: 0x17001428 RID: 5160
		// (get) Token: 0x060060F1 RID: 24817 RVA: 0x00143468 File Offset: 0x00141668
		// (set) Token: 0x060060F2 RID: 24818 RVA: 0x00143470 File Offset: 0x00141670
		public Transform CachedTransform { get; private set; }

		// Token: 0x17001429 RID: 5161
		// (get) Token: 0x060060F3 RID: 24819 RVA: 0x00143479 File Offset: 0x00141679
		public override bool ShouldBeVisible
		{
			get
			{
				return CullingCamera.CheckBoundsVisibility(new Bounds(this.CachedTransform.position, this.DecalSize));
			}
		}

		// Token: 0x060060F4 RID: 24820 RVA: 0x00143498 File Offset: 0x00141698
		public virtual void AttachToSurface(RaycastHit hitSurface)
		{
			Transform transform = hitSurface.collider.transform;
			this.CachedTransform.SetParent(transform);
			this.CachedTransform.SetPositionAndRotation(hitSurface.point + hitSurface.normal * 0.02f, Quaternion.LookRotation(-hitSurface.normal));
			if (transform.TryGetComponentInParent(out this._parentCullableRoom))
			{
				this._parentCullableRoom.AddChildCullable(this);
				this._attachedToSurface = true;
			}
			this._gridSettings.ProcessAttachment();
			if (this._sizeMaxRandomMultiplier > 0f)
			{
				this.DecalSize = this._defaultSize * (UnityEngine.Random.value * this._sizeMaxRandomMultiplier + 1f);
			}
		}

		// Token: 0x060060F5 RID: 24821 RVA: 0x00143553 File Offset: 0x00141753
		public virtual void Detach()
		{
			if (!this._attachedToSurface)
			{
				return;
			}
			this._attachedToSurface = false;
			if (this._parentCullableRoom != null)
			{
				this._parentCullableRoom.RemoveChildSullable(this);
			}
		}

		// Token: 0x060060F6 RID: 24822 RVA: 0x0014357F File Offset: 0x0014177F
		public void SetRandomRotation()
		{
			this.CachedTransform.Rotate(0f, 0f, UnityEngine.Random.value * 360f, Space.Self);
		}

		// Token: 0x060060F7 RID: 24823 RVA: 0x001435A2 File Offset: 0x001417A2
		protected virtual void Awake()
		{
			this._projector = base.GetComponent<DecalProjector>();
			this.CachedTransform = base.transform;
			this._gridSettings.SetReference(this);
			this._defaultSize = this.DecalSize;
		}

		// Token: 0x060060F8 RID: 24824 RVA: 0x001435D4 File Offset: 0x001417D4
		protected virtual void OnEnable()
		{
			this._projector.enabled = true;
		}

		// Token: 0x060060F9 RID: 24825 RVA: 0x001435E2 File Offset: 0x001417E2
		protected virtual void OnDisable()
		{
			this._projector.enabled = false;
		}

		// Token: 0x060060FA RID: 24826 RVA: 0x000488B1 File Offset: 0x00046AB1
		protected override void OnVisibilityChanged(bool isVisible)
		{
			base.enabled = isVisible;
		}

		// Token: 0x04004677 RID: 18039
		private DecalProjector _projector;

		// Token: 0x04004678 RID: 18040
		private CullableRoom _parentCullableRoom;

		// Token: 0x04004679 RID: 18041
		private bool _attachedToSurface;

		// Token: 0x0400467A RID: 18042
		private const float SurfaceDistance = 0.02f;

		// Token: 0x0400467B RID: 18043
		private Color _instancedColor;

		// Token: 0x0400467C RID: 18044
		private Material _materialInstance;

		// Token: 0x0400467D RID: 18045
		private Vector3 _defaultSize;

		// Token: 0x0400467E RID: 18046
		[SerializeField]
		private Decal.GridSettings _gridSettings;

		// Token: 0x0400467F RID: 18047
		[SerializeField]
		private float _sizeMaxRandomMultiplier;

		// Token: 0x04004680 RID: 18048
		public DecalPoolType DecalPoolType;

		// Token: 0x02001105 RID: 4357
		[Serializable]
		private class GridSettings
		{
			// Token: 0x060060FC RID: 24828 RVA: 0x001435F0 File Offset: 0x001417F0
			public void SetReference(Decal decal)
			{
				this._decalRef = decal;
			}

			// Token: 0x060060FD RID: 24829 RVA: 0x001435FC File Offset: 0x001417FC
			public void ProcessAttachment()
			{
				if (!this.Enabled)
				{
					return;
				}
				int num = UnityEngine.Random.Range(0, this.DecalsNum);
				int num2 = num % this.GridSize;
				int num3 = num / this.GridSize;
				num3 = this.GridSize - 1 - num3;
				this._decalRef.UVOffset = new Vector2((float)num2, (float)num3) / (float)this.GridSize;
				if (this._tilingSet)
				{
					return;
				}
				this._decalRef.UVTiling = Vector2.one / (float)this.GridSize;
				this._tilingSet = true;
			}

			// Token: 0x04004682 RID: 18050
			public bool Enabled;

			// Token: 0x04004683 RID: 18051
			public int GridSize;

			// Token: 0x04004684 RID: 18052
			public int DecalsNum;

			// Token: 0x04004685 RID: 18053
			private Decal _decalRef;

			// Token: 0x04004686 RID: 18054
			private bool _tilingSet;
		}
	}
}
