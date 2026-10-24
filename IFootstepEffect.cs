using System;
using PlayerRoles.FirstPersonControl.Thirdperson;

namespace CustomPlayerEffects
{
	// Token: 0x020008F8 RID: 2296
	public interface IFootstepEffect
	{
		// Token: 0x0600348C RID: 13452
		float ProcessFootstepOverrides(AnimatedCharacterModel model, float dis);
	}
}
