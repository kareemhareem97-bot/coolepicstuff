using System;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x02000909 RID: 2313
	public abstract class SubEffectBase : MonoBehaviour
	{
		// Token: 0x17000A63 RID: 2659
		// (get) Token: 0x060034D6 RID: 13526 RVA: 0x000AD6C9 File Offset: 0x000AB8C9
		// (set) Token: 0x060034D7 RID: 13527 RVA: 0x000AD6D1 File Offset: 0x000AB8D1
		private protected StatusEffectBase MainEffect { protected get; private set; }

		// Token: 0x17000A64 RID: 2660
		// (get) Token: 0x060034D8 RID: 13528 RVA: 0x000AD6DA File Offset: 0x000AB8DA
		protected ReferenceHub Hub
		{
			get
			{
				return this.MainEffect.Hub;
			}
		}

		// Token: 0x17000A65 RID: 2661
		// (get) Token: 0x060034D9 RID: 13529 RVA: 0x000AD6E7 File Offset: 0x000AB8E7
		protected bool IsLocalPlayer
		{
			get
			{
				return this.MainEffect.IsLocalPlayer;
			}
		}

		// Token: 0x17000A66 RID: 2662
		// (get) Token: 0x060034DA RID: 13530 RVA: 0x000AD6F4 File Offset: 0x000AB8F4
		public virtual bool IsActive
		{
			get
			{
				return base.gameObject.activeInHierarchy && this.MainEffect.IsEnabled;
			}
		}

		// Token: 0x060034DB RID: 13531 RVA: 0x00002E9A File Offset: 0x0000109A
		public virtual void DisableEffect()
		{
		}

		// Token: 0x060034DC RID: 13532 RVA: 0x000AD710 File Offset: 0x000AB910
		internal virtual void Init(StatusEffectBase mainEffect)
		{
			this.MainEffect = mainEffect;
		}

		// Token: 0x060034DD RID: 13533 RVA: 0x00002E9A File Offset: 0x0000109A
		internal virtual void UpdateEffect()
		{
		}
	}
}
