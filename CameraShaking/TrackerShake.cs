using System;
using UnityEngine;

namespace CameraShaking
{
	// Token: 0x02000891 RID: 2193
	public class TrackerShake : IShakeEffect
	{
		// Token: 0x060031F5 RID: 12789 RVA: 0x000A7ECC File Offset: 0x000A60CC
		public TrackerShake(Transform target, Vector3 offset, float intensity = 1f) : this(target, Quaternion.Euler(offset), intensity)
		{
		}

		// Token: 0x060031F6 RID: 12790 RVA: 0x000A7EDC File Offset: 0x000A60DC
		public TrackerShake(Transform target, Quaternion offset, float intensity = 1f)
		{
			this._tracker = target;
			this._offset = offset;
			this._intensity = intensity;
			this._remainingFade = 0.1f;
			this._isVisible = true;
		}

		// Token: 0x060031F7 RID: 12791 RVA: 0x000A7F0C File Offset: 0x000A610C
		public bool GetEffect(ReferenceHub ply, out ShakeEffectValues shakeValues)
		{
			if (this._tracker == null || !this._tracker.gameObject.activeInHierarchy)
			{
				this._isVisible = false;
			}
			Quaternion quaternion;
			if (this._isVisible)
			{
				quaternion = this._tracker.localRotation * this._offset;
				if (this._intensity != 1f)
				{
					quaternion = Quaternion.LerpUnclamped(Quaternion.identity, quaternion, this._intensity);
				}
				this._lastKnownRotation = quaternion;
			}
			else
			{
				this._remainingFade -= Time.deltaTime;
				quaternion = Quaternion.Lerp(Quaternion.identity, this._lastKnownRotation, this._remainingFade / 0.1f);
			}
			Vector3 eulerAngles = quaternion.eulerAngles;
			quaternion = Quaternion.Euler(-eulerAngles.x, eulerAngles.z, eulerAngles.y);
			shakeValues = new ShakeEffectValues(new Quaternion?(quaternion), new Quaternion?(Quaternion.Inverse(quaternion)), null, 1f, 0f, 0f);
			return this._remainingFade > 0f;
		}

		// Token: 0x04002711 RID: 10001
		private const float FadeoutTime = 0.1f;

		// Token: 0x04002712 RID: 10002
		private readonly float _intensity;

		// Token: 0x04002713 RID: 10003
		private readonly Transform _tracker;

		// Token: 0x04002714 RID: 10004
		private readonly Quaternion _offset;

		// Token: 0x04002715 RID: 10005
		private bool _isVisible;

		// Token: 0x04002716 RID: 10006
		private float _remainingFade;

		// Token: 0x04002717 RID: 10007
		private Quaternion _lastKnownRotation;
	}
}
