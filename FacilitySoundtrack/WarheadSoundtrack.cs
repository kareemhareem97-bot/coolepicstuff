using System;
using UnityEngine;

namespace FacilitySoundtrack
{
	// Token: 0x0200087A RID: 2170
	public class WarheadSoundtrack : SoundtrackLayerBase
	{
		// Token: 0x17000927 RID: 2343
		// (get) Token: 0x06003180 RID: 12672 RVA: 0x000A668C File Offset: 0x000A488C
		public override float Weight
		{
			get
			{
				return this._curWeight;
			}
		}

		// Token: 0x17000928 RID: 2344
		// (get) Token: 0x06003181 RID: 12673 RVA: 0x00004094 File Offset: 0x00002294
		public override bool Additive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000929 RID: 2345
		// (get) Token: 0x06003182 RID: 12674 RVA: 0x00004094 File Offset: 0x00002294
		public override bool IsMuteable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700092A RID: 2346
		// (get) Token: 0x06003183 RID: 12675 RVA: 0x000A6694 File Offset: 0x000A4894
		private float TargetWeight
		{
			get
			{
				if (!AlphaWarheadController.SingletonSet)
				{
					return 0f;
				}
				AlphaWarheadController singleton = AlphaWarheadController.Singleton;
				if (!singleton.Info.InProgress || AlphaWarheadController.Detonated)
				{
					return 0f;
				}
				float currentMusicOutput = singleton.CurScenario.Announcer.CurrentMusicOutput;
				return this._attenuationOverOutput.Evaluate(currentMusicOutput);
			}
		}

		// Token: 0x06003184 RID: 12676 RVA: 0x000A66EB File Offset: 0x000A48EB
		public override void UpdateVolume(float volumeScale)
		{
			this._curWeight = Mathf.MoveTowards(this._curWeight, this.TargetWeight, Time.deltaTime * this._weightAdjustSpeed);
		}

		// Token: 0x040026AF RID: 9903
		private float _curWeight;

		// Token: 0x040026B0 RID: 9904
		[SerializeField]
		private AnimationCurve _attenuationOverOutput;

		// Token: 0x040026B1 RID: 9905
		[SerializeField]
		private float _weightAdjustSpeed;
	}
}
