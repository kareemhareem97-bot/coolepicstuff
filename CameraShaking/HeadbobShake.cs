using System;
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;
using UserSettings;
using UserSettings.VideoSettings;

namespace CameraShaking
{
	// Token: 0x0200088B RID: 2187
	public class HeadbobShake : IShakeEffect
	{
		// Token: 0x060031EA RID: 12778 RVA: 0x000A7B55 File Offset: 0x000A5D55
		public HeadbobShake(AnimatedCharacterModel model)
		{
			this._model = model;
			HeadbobShake._mostCurrent = this;
		}

		// Token: 0x060031EB RID: 12779 RVA: 0x000A7B6C File Offset: 0x000A5D6C
		public bool GetEffect(ReferenceHub ply, out ShakeEffectValues shakeValues)
		{
			if (this._model.Pooled || !this._model.IsTracked || this != HeadbobShake._mostCurrent)
			{
				shakeValues = ShakeEffectValues.None;
				return false;
			}
			Vector3 value = (!HeadbobShake.EnableBobbing.Value) ? Vector3.zero : ply.transform.TransformDirection(this._model.HeadBobPosition);
			Vector3? rootCameraPositionOffset = new Vector3?(value);
			shakeValues = new ShakeEffectValues(null, null, rootCameraPositionOffset, 1f, 0f, 0f);
			return true;
		}

		// Token: 0x040026FD RID: 9981
		private readonly AnimatedCharacterModel _model;

		// Token: 0x040026FE RID: 9982
		private static HeadbobShake _mostCurrent;

		// Token: 0x040026FF RID: 9983
		private static readonly CachedUserSetting<bool> EnableBobbing = new CachedUserSetting<bool>(MiscVideoSetting.HeadBobbing);
	}
}
