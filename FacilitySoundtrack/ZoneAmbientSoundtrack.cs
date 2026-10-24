using System;
using MapGeneration;
using UnityEngine;

namespace FacilitySoundtrack
{
	// Token: 0x0200087B RID: 2171
	public class ZoneAmbientSoundtrack : SoundtrackLayerBase
	{
		// Token: 0x1700092B RID: 2347
		// (get) Token: 0x06003186 RID: 12678 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Additive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700092C RID: 2348
		// (get) Token: 0x06003187 RID: 12679 RVA: 0x000A6710 File Offset: 0x000A4910
		public override float Weight
		{
			get
			{
				return this._weight;
			}
		}

		// Token: 0x1700092D RID: 2349
		// (get) Token: 0x06003188 RID: 12680 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool IsMuteable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003189 RID: 12681 RVA: 0x000A6718 File Offset: 0x000A4918
		public override void UpdateVolume(float masterScale)
		{
			RoomIdentifier roomIdentifier;
			if (AlphaWarheadController.Detonated)
			{
				this._lastZone = FacilityZone.None;
			}
			else if (MainCameraController.LastPosition.TryGetRoom(out roomIdentifier))
			{
				this._lastZone = roomIdentifier.Zone;
			}
			float num = base.IsPovMuted ? (-this._fadeOutSpeed) : this._fadeInSpeed;
			this._weight = Mathf.Clamp01(this._weight + num * Time.deltaTime);
			foreach (ZoneAmbientSoundtrack.ZoneSoundtrack zoneSoundtrack in this._zoneSoundtracks)
			{
				float target = (float)((zoneSoundtrack.TargetZone == this._lastZone) ? 1 : 0);
				zoneSoundtrack.CrossfadeVolume = Mathf.MoveTowards(zoneSoundtrack.CrossfadeVolume, target, this._crossfadeSpeed * Time.deltaTime);
				zoneSoundtrack.Source.volume = zoneSoundtrack.CrossfadeVolume * zoneSoundtrack.VolumeScale * masterScale;
			}
		}

		// Token: 0x040026B2 RID: 9906
		[SerializeField]
		private float _fadeInSpeed;

		// Token: 0x040026B3 RID: 9907
		[SerializeField]
		private float _fadeOutSpeed;

		// Token: 0x040026B4 RID: 9908
		[SerializeField]
		private float _crossfadeSpeed;

		// Token: 0x040026B5 RID: 9909
		[SerializeField]
		private ZoneAmbientSoundtrack.ZoneSoundtrack[] _zoneSoundtracks;

		// Token: 0x040026B6 RID: 9910
		private float _weight;

		// Token: 0x040026B7 RID: 9911
		private FacilityZone _lastZone;

		// Token: 0x0200087C RID: 2172
		[Serializable]
		private class ZoneSoundtrack
		{
			// Token: 0x1700092E RID: 2350
			// (get) Token: 0x0600318B RID: 12683 RVA: 0x000A67EE File Offset: 0x000A49EE
			// (set) Token: 0x0600318C RID: 12684 RVA: 0x000A67F6 File Offset: 0x000A49F6
			public float CrossfadeVolume { get; set; }

			// Token: 0x040026B8 RID: 9912
			public FacilityZone TargetZone;

			// Token: 0x040026B9 RID: 9913
			public AudioSource Source;

			// Token: 0x040026BA RID: 9914
			public float VolumeScale;
		}
	}
}
