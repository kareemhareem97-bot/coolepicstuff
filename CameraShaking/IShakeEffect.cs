using System;

namespace CameraShaking
{
	// Token: 0x0200088C RID: 2188
	public interface IShakeEffect
	{
		// Token: 0x060031ED RID: 12781
		bool GetEffect(ReferenceHub ply, out ShakeEffectValues shakeValues);
	}
}
