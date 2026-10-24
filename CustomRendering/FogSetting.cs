using System;
using UnityEngine;

namespace CustomRendering
{
	// Token: 0x02001101 RID: 4353
	public class FogSetting : MonoBehaviour
	{
		// Token: 0x17001420 RID: 5152
		// (get) Token: 0x060060DD RID: 24797 RVA: 0x00143246 File Offset: 0x00141446
		// (set) Token: 0x060060DE RID: 24798 RVA: 0x0014324E File Offset: 0x0014144E
		public bool IsEnabled
		{
			get
			{
				return this._state;
			}
			internal set
			{
				if (this._state == value)
				{
					return;
				}
				this._state = value;
				this._isDirty = true;
			}
		}

		// Token: 0x17001421 RID: 5153
		// (get) Token: 0x060060DF RID: 24799 RVA: 0x00143268 File Offset: 0x00141468
		// (set) Token: 0x060060E0 RID: 24800 RVA: 0x00143270 File Offset: 0x00141470
		public float BlendTime { get; internal set; }

		// Token: 0x17001422 RID: 5154
		// (get) Token: 0x060060E1 RID: 24801 RVA: 0x0014327C File Offset: 0x0014147C
		// (set) Token: 0x060060E2 RID: 24802 RVA: 0x001432CB File Offset: 0x001414CB
		public float Weight
		{
			get
			{
				if (FogController.Singleton.ForcedFog == null)
				{
					return this._weight;
				}
				if (FogController.Singleton.ForcedFog.Value != this.FogType)
				{
					return 0f;
				}
				return 1f;
			}
			private set
			{
				this._weight = value;
			}
		}

		// Token: 0x060060E3 RID: 24803 RVA: 0x001432D4 File Offset: 0x001414D4
		public void UpdateWeight()
		{
			if (!this._isDirty)
			{
				return;
			}
			if (this.BlendTime <= 0f)
			{
				this.Weight = (float)(this.IsEnabled ? 1 : 0);
				this._isDirty = false;
				return;
			}
			float num = 1f / this.BlendTime;
			this.Weight += Time.deltaTime * (this.IsEnabled ? num : (-num));
			if (this.Weight < 0f || this.Weight > 1f)
			{
				this.Weight = Mathf.Clamp01(this.Weight);
				this._isDirty = false;
			}
		}

		// Token: 0x04004660 RID: 18016
		private float _weight;

		// Token: 0x04004661 RID: 18017
		public FogType FogType;

		// Token: 0x04004662 RID: 18018
		public int Priority;

		// Token: 0x04004663 RID: 18019
		public float StartDistance;

		// Token: 0x04004664 RID: 18020
		public float EndDistance = 200f;

		// Token: 0x04004665 RID: 18021
		[ColorUsage(false, true)]
		public Color Color = Color.black;

		// Token: 0x04004667 RID: 18023
		private bool _isDirty;

		// Token: 0x04004668 RID: 18024
		private bool _state;
	}
}
