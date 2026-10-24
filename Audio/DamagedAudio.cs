using System;
using AudioPooling;
using Mirror;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using UnityEngine;

namespace Audio
{
	// Token: 0x02000ACF RID: 2767
	public class DamagedAudio : MonoBehaviour
	{
		// Token: 0x06003E4C RID: 15948 RVA: 0x000CE3C8 File Offset: 0x000CC5C8
		private void Start()
		{
			if (!ReferenceHub.TryGetHub(base.gameObject, out this._owner))
			{
				return;
			}
			this._owner.playerStats.GetModule<HealthStat>().OnStatChange += this.OnHealthStatChange;
			this._owner.playerStats.GetModule<AhpStat>().OnStatChange += this.OnHealthStatChange;
			this._owner.playerStats.GetModule<HumeShieldStat>().OnStatChange += this.OnHumeStatChange;
		}

		// Token: 0x06003E4D RID: 15949 RVA: 0x000CE44C File Offset: 0x000CC64C
		private void OnDestroy()
		{
			if (this._owner == null)
			{
				return;
			}
			this._owner.playerStats.GetModule<HealthStat>().OnStatChange -= this.OnHealthStatChange;
			this._owner.playerStats.GetModule<AhpStat>().OnStatChange -= this.OnHealthStatChange;
			this._owner.playerStats.GetModule<HumeShieldStat>().OnStatChange -= this.OnHumeStatChange;
		}

		// Token: 0x06003E4E RID: 15950 RVA: 0x000CE4CB File Offset: 0x000CC6CB
		private void OnHumeStatChange(float oldValue, float newValue)
		{
			this.PlayAudio(oldValue - newValue, true);
		}

		// Token: 0x06003E4F RID: 15951 RVA: 0x000CE4D7 File Offset: 0x000CC6D7
		private void OnHealthStatChange(float oldValue, float newValue)
		{
			this.PlayAudio(oldValue - newValue, false);
		}

		// Token: 0x06003E50 RID: 15952 RVA: 0x000CE4E4 File Offset: 0x000CC6E4
		private void PlayAudio(float damage, bool isHume)
		{
			if (!this._owner.isLocalPlayer && !this._owner.IsLocallySpectated())
			{
				return;
			}
			if (damage <= 2.5f || this._lastTime > NetworkTime.time)
			{
				return;
			}
			this._lastTime = NetworkTime.time + 0.25;
			AudioSourcePoolManager.Play2DWithParent(isHume ? this._scpAudioClips.RandomItem<AudioClip>() : this._humanAudioClips.RandomItem<AudioClip>(), this._owner.transform, 1f, MixerChannel.DefaultSfx, 1f);
		}

		// Token: 0x04002DA7 RID: 11687
		private const double Cooldown = 0.25;

		// Token: 0x04002DA8 RID: 11688
		private const float MinimumDamage = 2.5f;

		// Token: 0x04002DA9 RID: 11689
		[SerializeField]
		private AudioClip[] _scpAudioClips;

		// Token: 0x04002DAA RID: 11690
		[SerializeField]
		private AudioClip[] _humanAudioClips;

		// Token: 0x04002DAB RID: 11691
		private double _lastTime;

		// Token: 0x04002DAC RID: 11692
		private ReferenceHub _owner;
	}
}
