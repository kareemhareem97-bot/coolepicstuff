using System;
using Mirror;

namespace CustomPlayerEffects
{
	// Token: 0x020008B8 RID: 2232
	public class Flashed : StatusEffectBase
	{
		// Token: 0x17000993 RID: 2451
		// (get) Token: 0x060032C0 RID: 12992 RVA: 0x000A84BF File Offset: 0x000A66BF
		public override bool AllowEnabling
		{
			get
			{
				return !SpawnProtected.CheckPlayer(base.Hub);
			}
		}

		// Token: 0x060032C1 RID: 12993 RVA: 0x000A9338 File Offset: 0x000A7538
		protected override void IntensityChanged(byte prevState, byte newState)
		{
			float timeLeft = (float)newState * 0.1f;
			if (NetworkServer.active)
			{
				base.TimeLeft = timeLeft;
			}
		}

		// Token: 0x060032C2 RID: 12994 RVA: 0x000A935C File Offset: 0x000A755C
		protected override void Update()
		{
			base.Update();
			if (NetworkServer.active && base.Duration == 0f)
			{
				base.TimeLeft = 1f;
			}
		}
	}
}
