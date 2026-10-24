using System;
using PlayerRoles.FirstPersonControl;

namespace CustomPlayerEffects
{
	// Token: 0x020008B4 RID: 2228
	public class Disabled : StatusEffectBase, IMovementSpeedModifier
	{
		// Token: 0x17000984 RID: 2436
		// (get) Token: 0x060032A7 RID: 12967 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000985 RID: 2437
		// (get) Token: 0x060032A8 RID: 12968 RVA: 0x000A91FD File Offset: 0x000A73FD
		public float MovementSpeedMultiplier
		{
			get
			{
				return (this.SpeedMultiplier - 1f) * RainbowTaste.CurrentMultiplier(base.Hub) + 1f;
			}
		}

		// Token: 0x17000986 RID: 2438
		// (get) Token: 0x060032A9 RID: 12969 RVA: 0x0000407F File Offset: 0x0000227F
		public float MovementSpeedLimit
		{
			get
			{
				return float.MaxValue;
			}
		}

		// Token: 0x0400275F RID: 10079
		public float SpeedMultiplier = 0.8f;
	}
}
