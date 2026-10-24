using System;
using PlayerRoles;
using PlayerRoles.PlayableScps.HumanTracker;
using PlayerRoles.Spectating;
using UnityEngine;

namespace FacilitySoundtrack
{
	// Token: 0x02000875 RID: 2165
	public class LastHumanSoundtrack : SoundtrackLayerBase
	{
		// Token: 0x17000919 RID: 2329
		// (get) Token: 0x0600315E RID: 12638 RVA: 0x00004094 File Offset: 0x00002294
		public override bool Additive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700091A RID: 2330
		// (get) Token: 0x0600315F RID: 12639 RVA: 0x000A6201 File Offset: 0x000A4401
		public override float Weight
		{
			get
			{
				return this._weight;
			}
		}

		// Token: 0x1700091B RID: 2331
		// (get) Token: 0x06003160 RID: 12640 RVA: 0x00004094 File Offset: 0x00002294
		public override bool IsMuteable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700091C RID: 2332
		// (get) Token: 0x06003161 RID: 12641 RVA: 0x000A6209 File Offset: 0x000A4409
		private float CurFadeAdjustSpeed
		{
			get
			{
				return (LastHumanTracker.IsLocalLastTarget ? this._fadeInSpeed : this._fadeOutSpeed) * Time.deltaTime;
			}
		}

		// Token: 0x06003162 RID: 12642 RVA: 0x000A6228 File Offset: 0x000A4428
		public override void UpdateVolume(float volumeScale)
		{
			float target = (LastHumanTracker.IsLocalLastTarget ? 0.25f : 0f) * this._weight * volumeScale;
			float curFadeAdjustSpeed = this.CurFadeAdjustSpeed;
			this._audioSource.volume = Mathf.MoveTowards(this._audioSource.volume, target, curFadeAdjustSpeed);
		}

		// Token: 0x06003163 RID: 12643 RVA: 0x000A6276 File Offset: 0x000A4476
		private void Awake()
		{
			PlayerRoleManager.OnRoleChanged += this.OnRoleChanged;
			SpectatorTargetTracker.OnTargetChanged += this.OnTargetChanged;
		}

		// Token: 0x06003164 RID: 12644 RVA: 0x00002E9A File Offset: 0x0000109A
		private void OnTargetChanged()
		{
		}

		// Token: 0x06003165 RID: 12645 RVA: 0x000A629A File Offset: 0x000A449A
		private void OnDestroy()
		{
			PlayerRoleManager.OnRoleChanged -= this.OnRoleChanged;
			SpectatorTargetTracker.OnTargetChanged -= this.OnTargetChanged;
		}

		// Token: 0x06003166 RID: 12646 RVA: 0x00002E9A File Offset: 0x0000109A
		private void Update()
		{
		}

		// Token: 0x06003167 RID: 12647 RVA: 0x000A62BE File Offset: 0x000A44BE
		private void OnRoleChanged(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
		{
			if (!userHub.isLocalPlayer)
			{
				return;
			}
			this._weight = 0f;
			this._audioSource.volume = 0f;
		}

		// Token: 0x04002695 RID: 9877
		private const float MaxVolume = 0.25f;

		// Token: 0x04002696 RID: 9878
		private const float ActiveTrackWeight = 0.125f;

		// Token: 0x04002697 RID: 9879
		[SerializeField]
		private float _fadeInSpeed;

		// Token: 0x04002698 RID: 9880
		[SerializeField]
		private float _fadeOutSpeed;

		// Token: 0x04002699 RID: 9881
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x0400269A RID: 9882
		private float _weight;
	}
}
