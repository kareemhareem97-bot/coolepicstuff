using System;
using MapGeneration;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using UnityEngine;

namespace FacilitySoundtrack
{
	// Token: 0x02000877 RID: 2167
	public class SCP079BlackoutSoundtrack : SoundtrackLayerBase
	{
		// Token: 0x17000920 RID: 2336
		// (get) Token: 0x0600316E RID: 12654 RVA: 0x000A647C File Offset: 0x000A467C
		public override float Weight
		{
			get
			{
				return this._weight;
			}
		}

		// Token: 0x17000921 RID: 2337
		// (get) Token: 0x0600316F RID: 12655 RVA: 0x00004094 File Offset: 0x00002294
		public override bool Additive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000922 RID: 2338
		// (get) Token: 0x06003170 RID: 12656 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool IsMuteable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003171 RID: 12657 RVA: 0x000A6484 File Offset: 0x000A4684
		public override void UpdateVolume(float volumeScale)
		{
			ReferenceHub hub;
			if (!ReferenceHub.TryGetPovHub(out hub))
			{
				return;
			}
			this._audioSource.volume = volumeScale * this._weight;
			bool flag = hub.IsSCP(true);
			bool flag2 = this._blackoutTime + 60.0 - 1.0 < NetworkTime.time;
			bool flag3 = this._blackoutTime + 10.0 > NetworkTime.time;
			if (flag || flag2 || flag3)
			{
				this.Fade(false);
				return;
			}
			if (hub.GetCurrentZone() != this._blackoutZone)
			{
				this.Fade(false);
				return;
			}
			this.Fade(true);
		}

		// Token: 0x06003172 RID: 12658 RVA: 0x000A651A File Offset: 0x000A471A
		private void Fade(bool increase)
		{
			this._weight = Mathf.Lerp(this._weight, (float)(increase ? 1 : 0), (increase ? this._fadeInMultipler : this._fadeOutMultipler) * Time.deltaTime);
		}

		// Token: 0x06003173 RID: 12659 RVA: 0x000A654C File Offset: 0x000A474C
		private void OnClientZoneBlackout(ReferenceHub scp079Hub, FacilityZone zone)
		{
			this._audioSource.PlayDelayed(10.25f);
			this._blackoutZone = zone;
			this._blackoutTime = NetworkTime.time;
		}

		// Token: 0x06003174 RID: 12660 RVA: 0x000A6570 File Offset: 0x000A4770
		private void Start()
		{
			Scp079BlackoutZoneAbility.OnClientZoneBlackout = (Action<ReferenceHub, FacilityZone>)Delegate.Combine(Scp079BlackoutZoneAbility.OnClientZoneBlackout, new Action<ReferenceHub, FacilityZone>(this.OnClientZoneBlackout));
		}

		// Token: 0x06003175 RID: 12661 RVA: 0x000A6592 File Offset: 0x000A4792
		private void OnDestroy()
		{
			Scp079BlackoutZoneAbility.OnClientZoneBlackout = (Action<ReferenceHub, FacilityZone>)Delegate.Remove(Scp079BlackoutZoneAbility.OnClientZoneBlackout, new Action<ReferenceHub, FacilityZone>(this.OnClientZoneBlackout));
		}

		// Token: 0x040026A4 RID: 9892
		private const float BlackoutTime = 60f;

		// Token: 0x040026A5 RID: 9893
		private const float FadeInTime = 10f;

		// Token: 0x040026A6 RID: 9894
		private const float PlayDelay = 10.25f;

		// Token: 0x040026A7 RID: 9895
		private const float FadeoutTime = 1f;

		// Token: 0x040026A8 RID: 9896
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x040026A9 RID: 9897
		[SerializeField]
		private float _fadeInMultipler;

		// Token: 0x040026AA RID: 9898
		[SerializeField]
		private float _fadeOutMultipler;

		// Token: 0x040026AB RID: 9899
		private float _weight;

		// Token: 0x040026AC RID: 9900
		private FacilityZone _blackoutZone;

		// Token: 0x040026AD RID: 9901
		private double _blackoutTime;
	}
}
