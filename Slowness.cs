using System;
using PlayerRoles.FirstPersonControl;

namespace CustomPlayerEffects
{
	// Token: 0x020008DC RID: 2268
	public class Slowness : StatusEffectBase, IMovementSpeedModifier, IStaminaModifier, ISpectatorDataPlayerEffect
	{
		// Token: 0x17000A00 RID: 2560
		// (get) Token: 0x060033E2 RID: 13282 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A01 RID: 2561
		// (get) Token: 0x060033E3 RID: 13283 RVA: 0x000AB61F File Offset: 0x000A981F
		public float MovementSpeedMultiplier
		{
			get
			{
				return 1f - (float)base.Intensity * 0.01f;
			}
		}

		// Token: 0x17000A02 RID: 2562
		// (get) Token: 0x060033E4 RID: 13284 RVA: 0x0000407F File Offset: 0x0000227F
		public float MovementSpeedLimit
		{
			get
			{
				return float.MaxValue;
			}
		}

		// Token: 0x17000A03 RID: 2563
		// (get) Token: 0x060033E5 RID: 13285 RVA: 0x000A84BF File Offset: 0x000A66BF
		public override bool AllowEnabling
		{
			get
			{
				return !SpawnProtected.CheckPlayer(base.Hub);
			}
		}

		// Token: 0x17000A04 RID: 2564
		// (get) Token: 0x060033E6 RID: 13286 RVA: 0x000AB634 File Offset: 0x000A9834
		public bool StaminaModifierActive
		{
			get
			{
				return base.Intensity == 100;
			}
		}

		// Token: 0x17000A05 RID: 2565
		// (get) Token: 0x060033E7 RID: 13287 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool SprintingDisabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060033E8 RID: 13288 RVA: 0x000AB640 File Offset: 0x000A9840
		public bool GetSpectatorText(out string s)
		{
			s = string.Format("{0}% Slowness", base.Intensity);
			return true;
		}
	}
}
