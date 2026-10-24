using System;
using System.Diagnostics;
using UnityEngine;

namespace CameraShaking
{
	// Token: 0x0200088F RID: 2191
	public class RecoilShake : IShakeEffect
	{
		// Token: 0x060031F2 RID: 12786 RVA: 0x000A7D7C File Offset: 0x000A5F7C
		public RecoilShake(RecoilSettings settings)
		{
			this._settings = settings;
			this._startQuaternion = Quaternion.Euler(0f, 0f, settings.ZAxis * (UnityEngine.Random.value - 0.5f));
			this._firstFrame = true;
			this._removeStopwatch = new Stopwatch();
			this._removeStopwatch.Start();
		}

		// Token: 0x060031F3 RID: 12787 RVA: 0x000A7DDC File Offset: 0x000A5FDC
		public bool GetEffect(ReferenceHub ply, out ShakeEffectValues shakeValues)
		{
			float num = Mathf.Clamp01((float)this._removeStopwatch.Elapsed.TotalSeconds / this._settings.AnimationTime);
			float num2;
			float num3;
			if (this._firstFrame)
			{
				num2 = this._settings.UpKick;
				num3 = this._settings.SideKick;
				this._firstFrame = false;
			}
			else
			{
				num2 = 0f;
				num3 = 0f;
			}
			float verticalLook = num2;
			float horizontalLook = num3;
			Quaternion? rootCameraRotation = new Quaternion?(Quaternion.Slerp(this._startQuaternion, Quaternion.identity, num));
			float fovPercent = Mathf.SmoothStep(this._settings.FovKick, 1f, num);
			shakeValues = new ShakeEffectValues(rootCameraRotation, null, null, fovPercent, verticalLook, horizontalLook);
			return num < 1f;
		}

		// Token: 0x04002708 RID: 9992
		private readonly Stopwatch _removeStopwatch;

		// Token: 0x04002709 RID: 9993
		private readonly RecoilSettings _settings;

		// Token: 0x0400270A RID: 9994
		private readonly Quaternion _startQuaternion;

		// Token: 0x0400270B RID: 9995
		private bool _firstFrame;
	}
}
