using System;
using UnityEngine;

namespace CameraShaking
{
	// Token: 0x0200088E RID: 2190
	public class LookatShake : IShakeEffect
	{
		// Token: 0x060031F0 RID: 12784 RVA: 0x000A7CE1 File Offset: 0x000A5EE1
		public LookatShake(Vector3 point)
		{
			this._point = point;
		}

		// Token: 0x060031F1 RID: 12785 RVA: 0x000A7CF0 File Offset: 0x000A5EF0
		public bool GetEffect(ReferenceHub ply, out ShakeEffectValues shakeValues)
		{
			Transform playerCameraReference = ply.PlayerCameraReference;
			Vector3 eulerAngles = playerCameraReference.eulerAngles;
			Vector3 eulerAngles2 = Quaternion.LookRotation(this._point - playerCameraReference.position).eulerAngles;
			float verticalLook = eulerAngles.x - eulerAngles2.x;
			float horizontalLook = eulerAngles2.y - eulerAngles.y;
			shakeValues = new ShakeEffectValues(null, null, null, 1f, verticalLook, horizontalLook);
			return false;
		}

		// Token: 0x04002707 RID: 9991
		private readonly Vector3 _point;
	}
}
