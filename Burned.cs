using System;
using PlayerStatsSystem;

namespace CustomPlayerEffects
{
	// Token: 0x020008A2 RID: 2210
	public class Burned : StatusEffectBase, IHealableEffect, IDamageModifierEffect
	{
		// Token: 0x17000955 RID: 2389
		// (get) Token: 0x06003235 RID: 12853 RVA: 0x000A8597 File Offset: 0x000A6797
		public float DamageMultiplier
		{
			get
			{
				return 0.25f * RainbowTaste.CurrentMultiplier(base.Hub) + 1f;
			}
		}

		// Token: 0x17000956 RID: 2390
		// (get) Token: 0x06003236 RID: 12854 RVA: 0x00004070 File Offset: 0x00002270
		public bool DamageModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x06003237 RID: 12855 RVA: 0x000A85B0 File Offset: 0x000A67B0
		public bool IsHealable(ItemType it)
		{
			return it == ItemType.Medkit || it == ItemType.SCP500;
		}

		// Token: 0x06003238 RID: 12856 RVA: 0x000A85BE File Offset: 0x000A67BE
		public float GetDamageModifier(float baseDamage, DamageHandlerBase handler, HitboxType hitboxType)
		{
			return this.DamageMultiplier;
		}

		// Token: 0x0400272B RID: 10027
		private const float BaseDamageMultiplier = 1.25f;
	}
}
