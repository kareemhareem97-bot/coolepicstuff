using System;
using MapGeneration;
using UnityEngine;

namespace FacilitySoundtrack
{
	// Token: 0x02000876 RID: 2166
	public class PocketDimensionAmbientSoundtrack : SoundtrackLayerBase
	{
		// Token: 0x1700091D RID: 2333
		// (get) Token: 0x06003169 RID: 12649 RVA: 0x00004094 File Offset: 0x00002294
		public override bool Additive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700091E RID: 2334
		// (get) Token: 0x0600316A RID: 12650 RVA: 0x000A62E4 File Offset: 0x000A44E4
		public override float Weight
		{
			get
			{
				return this._weight;
			}
		}

		// Token: 0x1700091F RID: 2335
		// (get) Token: 0x0600316B RID: 12651 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool IsMuteable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600316C RID: 12652 RVA: 0x000A62EC File Offset: 0x000A44EC
		public override void UpdateVolume(float masterScale)
		{
			Vector3 lastPosition = MainCameraController.LastPosition;
			RoomIdentifier roomIdentifier;
			if (!lastPosition.TryGetRoom(out roomIdentifier) || roomIdentifier.Name != RoomName.Pocket)
			{
				this._weight = Mathf.Max(0f, this._weight - Time.deltaTime * this._fadeOutSpeed);
				AudioSource[] bassSources = this._bassSources;
				for (int i = 0; i < bassSources.Length; i++)
				{
					bassSources[i].UpdateFadeOut(this._fadeOutSpeed);
				}
				this._ambienceSource.volume = Mathf.Min(this._ambienceSource.volume, masterScale);
				return;
			}
			if (this._weight == 0f)
			{
				AudioSource[] bassSources = this._bassSources;
				for (int i = 0; i < bassSources.Length; i++)
				{
					bassSources[i].Replay();
				}
				this._ambienceSource.Replay();
			}
			float value = (lastPosition - roomIdentifier.transform.position).MagnitudeIgnoreY();
			float time = Mathf.InverseLerp(this._startBassDistance, this._endBassDistance, value);
			float num = this._bassBlendCurve.Evaluate(time);
			this._weight = Mathf.Min(1f, this._weight + this._fadeInSpeed * Time.deltaTime);
			this._ambienceSource.volume = masterScale * this._ambienceBlendCurve.Evaluate(time);
			for (int j = 0; j < this._bassSources.Length; j++)
			{
				float num2 = Mathf.Abs((float)j - num + 1f);
				float num3 = num2 * num2;
				float num4 = Mathf.Clamp01(1f - num3);
				this._bassSources[j].volume = num4 * masterScale;
			}
		}

		// Token: 0x0400269B RID: 9883
		[SerializeField]
		private float _fadeInSpeed;

		// Token: 0x0400269C RID: 9884
		[SerializeField]
		private float _fadeOutSpeed;

		// Token: 0x0400269D RID: 9885
		[SerializeField]
		private AnimationCurve _bassBlendCurve;

		// Token: 0x0400269E RID: 9886
		[SerializeField]
		private AnimationCurve _ambienceBlendCurve;

		// Token: 0x0400269F RID: 9887
		[SerializeField]
		private float _startBassDistance;

		// Token: 0x040026A0 RID: 9888
		[SerializeField]
		private float _endBassDistance;

		// Token: 0x040026A1 RID: 9889
		[SerializeField]
		private AudioSource[] _bassSources;

		// Token: 0x040026A2 RID: 9890
		[SerializeField]
		private AudioSource _ambienceSource;

		// Token: 0x040026A3 RID: 9891
		private float _weight;
	}
}
