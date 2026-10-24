using System;
using System.Collections.Generic;
using UnityEngine;

namespace Decals
{
	// Token: 0x0200110A RID: 4362
	public class DecalPool
	{
		// Token: 0x1700142A RID: 5162
		// (get) Token: 0x06006112 RID: 24850 RVA: 0x00143993 File Offset: 0x00141B93
		public int Instances
		{
			get
			{
				return this._spawned.Count;
			}
		}

		// Token: 0x06006113 RID: 24851 RVA: 0x001439A0 File Offset: 0x00141BA0
		private Decal PrepareInstance(Decal inst)
		{
			if (inst == null)
			{
				inst = UnityEngine.Object.Instantiate<Decal>(this._template);
			}
			else
			{
				inst.gameObject.SetActive(true);
			}
			return inst;
		}

		// Token: 0x06006114 RID: 24852 RVA: 0x001439C8 File Offset: 0x00141BC8
		public Decal Get()
		{
			Decal decal;
			if (this._disabled.TryDequeue(out decal))
			{
				decal = this.PrepareInstance(decal);
			}
			else
			{
				decal = UnityEngine.Object.Instantiate<Decal>(this._template);
			}
			this._spawned.Enqueue(decal);
			return decal;
		}

		// Token: 0x06006115 RID: 24853 RVA: 0x00143A08 File Offset: 0x00141C08
		public void DisableLast()
		{
			Decal decal;
			if (!this._spawned.TryDequeue(out decal))
			{
				return;
			}
			if (decal == null)
			{
				return;
			}
			decal.gameObject.SetActive(false);
			this._disabled.Enqueue(decal);
		}

		// Token: 0x06006116 RID: 24854 RVA: 0x00143A48 File Offset: 0x00141C48
		public void SetLimit(int limit)
		{
			while (this._spawned.Count > limit)
			{
				Decal decal = this._spawned.Dequeue();
				if (!(decal == null))
				{
					decal.Detach();
					UnityEngine.Object.Destroy(decal.gameObject);
				}
			}
		}

		// Token: 0x06006117 RID: 24855 RVA: 0x00143A8B File Offset: 0x00141C8B
		public DecalPool(Decal template)
		{
			this.Type = template.DecalPoolType;
			this._template = template;
			this._spawned = new Queue<Decal>(5000);
			this._disabled = new Queue<Decal>(5000);
		}

		// Token: 0x04004694 RID: 18068
		private const int StartQueueCapacity = 5000;

		// Token: 0x04004695 RID: 18069
		private readonly Queue<Decal> _spawned;

		// Token: 0x04004696 RID: 18070
		private readonly Queue<Decal> _disabled;

		// Token: 0x04004697 RID: 18071
		private readonly Decal _template;

		// Token: 0x04004698 RID: 18072
		public readonly DecalPoolType Type;
	}
}
