using System;
using UnityEngine;

namespace AudioPooling
{
	// Token: 0x02000986 RID: 2438
	public class PooledAudioSource : MonoBehaviour
	{
		// Token: 0x17000AEB RID: 2795
		// (get) Token: 0x06003748 RID: 14152 RVA: 0x000B39F2 File Offset: 0x000B1BF2
		// (set) Token: 0x06003749 RID: 14153 RVA: 0x000B39FA File Offset: 0x000B1BFA
		public bool Locked { get; set; }

		// Token: 0x17000AEC RID: 2796
		// (get) Token: 0x0600374A RID: 14154 RVA: 0x000B3A03 File Offset: 0x000B1C03
		// (set) Token: 0x0600374B RID: 14155 RVA: 0x000B3A0B File Offset: 0x000B1C0B
		public ulong TotalRecycles { get; private set; }

		// Token: 0x17000AED RID: 2797
		// (get) Token: 0x0600374C RID: 14156 RVA: 0x000B3A14 File Offset: 0x000B1C14
		// (set) Token: 0x0600374D RID: 14157 RVA: 0x000B3A1C File Offset: 0x000B1C1C
		public bool Pooled { get; private set; }

		// Token: 0x17000AEE RID: 2798
		// (get) Token: 0x0600374E RID: 14158 RVA: 0x000B3A25 File Offset: 0x000B1C25
		public AudioSource Source
		{
			get
			{
				if (!this._sourceCached)
				{
					this._audioSource = base.GetComponent<AudioSource>();
					this._sourceCached = true;
				}
				return this._audioSource;
			}
		}

		// Token: 0x17000AEF RID: 2799
		// (get) Token: 0x0600374F RID: 14159 RVA: 0x000B3A48 File Offset: 0x000B1C48
		public Transform FastTransform
		{
			get
			{
				if (!this._transformCached)
				{
					this._transform = base.transform;
					this._transformCached = true;
				}
				return this._transform;
			}
		}

		// Token: 0x17000AF0 RID: 2800
		// (get) Token: 0x06003750 RID: 14160 RVA: 0x000B3A6B File Offset: 0x000B1C6B
		public bool AllowRecycling
		{
			get
			{
				return !this.Locked && !this.Source.isPlaying;
			}
		}

		// Token: 0x06003751 RID: 14161 RVA: 0x000B3A88 File Offset: 0x000B1C88
		internal void OnRecycled()
		{
			this.Pooled = false;
			ulong totalRecycles = this.TotalRecycles;
			this.TotalRecycles = totalRecycles + 1UL;
		}

		// Token: 0x06003752 RID: 14162 RVA: 0x000B3AAD File Offset: 0x000B1CAD
		internal void OnPooled()
		{
			this.Pooled = true;
		}

		// Token: 0x0400298F RID: 10639
		private AudioSource _audioSource;

		// Token: 0x04002990 RID: 10640
		private bool _sourceCached;

		// Token: 0x04002991 RID: 10641
		private Transform _transform;

		// Token: 0x04002992 RID: 10642
		private bool _transformCached;
	}
}
