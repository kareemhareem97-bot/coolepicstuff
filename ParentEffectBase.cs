using System;
using PlayerRoles;

namespace CustomPlayerEffects
{
	// Token: 0x02000905 RID: 2309
	public abstract class ParentEffectBase<T> : StatusEffectBase where T : SubEffectBase
	{
		// Token: 0x17000A53 RID: 2643
		// (get) Token: 0x0600349C RID: 13468 RVA: 0x000ACF89 File Offset: 0x000AB189
		// (set) Token: 0x0600349D RID: 13469 RVA: 0x000ACF91 File Offset: 0x000AB191
		public T[] SubEffects { get; private set; }

		// Token: 0x0600349E RID: 13470 RVA: 0x000ACF9C File Offset: 0x000AB19C
		internal override void OnRoleChanged(PlayerRoleBase previousRole, PlayerRoleBase newRole)
		{
			base.OnRoleChanged(previousRole, newRole);
			T[] subEffects = this.SubEffects;
			for (int i = 0; i < subEffects.Length; i++)
			{
				subEffects[i].DisableEffect();
			}
		}

		// Token: 0x0600349F RID: 13471 RVA: 0x000ACFD8 File Offset: 0x000AB1D8
		public override void OnStopSpectating()
		{
			base.OnStopSpectating();
			T[] subEffects = this.SubEffects;
			for (int i = 0; i < subEffects.Length; i++)
			{
				subEffects[i].DisableEffect();
			}
		}

		// Token: 0x060034A0 RID: 13472 RVA: 0x000AD014 File Offset: 0x000AB214
		protected override void Awake()
		{
			base.Awake();
			T[] subEffects = this.SubEffects;
			for (int i = 0; i < subEffects.Length; i++)
			{
				subEffects[i].Init(this);
			}
		}

		// Token: 0x060034A1 RID: 13473 RVA: 0x000AD050 File Offset: 0x000AB250
		protected virtual void UpdateSubEffects()
		{
			T[] subEffects = this.SubEffects;
			for (int i = 0; i < subEffects.Length; i++)
			{
				subEffects[i].UpdateEffect();
			}
		}
	}
}
