using System;
using PlayerRoles.FirstPersonControl;

namespace CustomPlayerEffects
{
	// Token: 0x020008B5 RID: 2229
	public class Ensnared : StatusEffectBase, IMovementSpeedModifier, IStaminaModifier
	{
		// Token: 0x17000987 RID: 2439
		// (get) Token: 0x060032AB RID: 12971 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000988 RID: 2440
		// (get) Token: 0x060032AC RID: 12972 RVA: 0x0000548E File Offset: 0x0000368E
		public float MovementSpeedMultiplier
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000989 RID: 2441
		// (get) Token: 0x060032AD RID: 12973 RVA: 0x0000548E File Offset: 0x0000368E
		public float MovementSpeedLimit
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x1700098A RID: 2442
		// (get) Token: 0x060032AE RID: 12974 RVA: 0x00004070 File Offset: 0x00002270
		public bool StaminaModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x1700098B RID: 2443
		// (get) Token: 0x060032AF RID: 12975 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool SprintingDisabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700098C RID: 2444
		// (get) Token: 0x060032B0 RID: 12976 RVA: 0x000A84BF File Offset: 0x000A66BF
		public override bool AllowEnabling
		{
			get
			{
				return !SpawnProtected.CheckPlayer(base.Hub);
			}
		}
	}
}
