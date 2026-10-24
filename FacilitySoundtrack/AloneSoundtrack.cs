using System;
using PlayerRoles;
using PlayerRoles.PlayableScps;
using UnityEngine;

namespace FacilitySoundtrack
{
	// Token: 0x02000873 RID: 2163
	public class AloneSoundtrack : SoundtrackLayerBase
	{
		// Token: 0x17000913 RID: 2323
		// (get) Token: 0x0600314E RID: 12622 RVA: 0x00004094 File Offset: 0x00002294
		public override bool Additive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x0600314F RID: 12623 RVA: 0x000A5DA5 File Offset: 0x000A3FA5
		public override float Weight
		{
			get
			{
				return this._weight;
			}
		}

		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x06003150 RID: 12624 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool IsMuteable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003151 RID: 12625 RVA: 0x000A5DB0 File Offset: 0x000A3FB0
		public override void UpdateVolume(float masterScale)
		{
			ReferenceHub referenceHub;
			if (ReferenceHub.TryGetLocalHub(out referenceHub) && (!referenceHub.IsHuman() || referenceHub.GetRoleId() == RoleTypeId.Tutorial))
			{
				this._weight = 0f;
				this.SetVolume(this._weight);
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			foreach (ReferenceHub referenceHub2 in ReferenceHub.AllHubs)
			{
				if (!referenceHub2.isLocalPlayer && referenceHub2.IsAlive() && referenceHub2.GetRoleId() != RoleTypeId.Tutorial)
				{
					flag3 |= HitboxIdentity.IsEnemy(referenceHub, referenceHub2);
					if (VisionInformation.GetVisionInformation(referenceHub, referenceHub.PlayerCameraReference, referenceHub2.PlayerCameraReference.position, 0.25f, 60f, true, true, 0, true).IsLooking)
					{
						if (!referenceHub2.IsSCP(true))
						{
							flag = true;
							break;
						}
						flag2 = true;
					}
				}
			}
			if (!flag && this._hasFoundSCP != flag2)
			{
				this._hasFoundSCP = flag2;
				if (this._oldAloneTime != 0f)
				{
					this._aloneTime = ((90f < this._aloneTime) ? 90f : this._oldAloneTime);
				}
				this._oldAloneTime = (flag2 ? this._aloneTime : 0f);
			}
			this._aloneTime = (flag ? 0f : (this._aloneTime + Time.deltaTime));
			float num = (!base.IsPovMuted && this._oldAloneTime == 0f && this._aloneTime > 120f && flag3) ? this._fadeInSpeed : (-this._fadeOutSpeed);
			this._weight = Mathf.Clamp01(this._weight + num * Time.deltaTime);
			this.SetVolume(this._weight * masterScale * this._maxVolume);
		}

		// Token: 0x06003152 RID: 12626 RVA: 0x000A5F7C File Offset: 0x000A417C
		private void SetVolume(float volume)
		{
			this._aloneSource.volume = volume;
		}

		// Token: 0x06003153 RID: 12627 RVA: 0x000A5F8A File Offset: 0x000A418A
		private void Start()
		{
			PlayerRoleManager.OnRoleChanged += this.OnRoleChanged;
		}

		// Token: 0x06003154 RID: 12628 RVA: 0x000A5F9D File Offset: 0x000A419D
		private void OnDestroy()
		{
			PlayerRoleManager.OnRoleChanged -= this.OnRoleChanged;
		}

		// Token: 0x06003155 RID: 12629 RVA: 0x000A5FB0 File Offset: 0x000A41B0
		private void OnRoleChanged(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
		{
			if (!userHub.isLocalPlayer || newRole.Team == Team.Dead)
			{
				return;
			}
			this._weight = 0f;
			this._aloneTime = 0f;
			this._oldAloneTime = 0f;
			this._hasFoundSCP = false;
		}

		// Token: 0x0400267C RID: 9852
		private const float AloneTime = 120f;

		// Token: 0x0400267D RID: 9853
		private const float SCPBufferTime = 30f;

		// Token: 0x0400267E RID: 9854
		private const float MaxVisionDistance = 60f;

		// Token: 0x0400267F RID: 9855
		private const float CharacterRadius = 0.25f;

		// Token: 0x04002680 RID: 9856
		[SerializeField]
		private float _fadeInSpeed;

		// Token: 0x04002681 RID: 9857
		[SerializeField]
		private float _fadeOutSpeed;

		// Token: 0x04002682 RID: 9858
		[SerializeField]
		private float _maxVolume;

		// Token: 0x04002683 RID: 9859
		[SerializeField]
		private AudioSource _aloneSource;

		// Token: 0x04002684 RID: 9860
		private float _weight;

		// Token: 0x04002685 RID: 9861
		private float _aloneTime;

		// Token: 0x04002686 RID: 9862
		private float _oldAloneTime;

		// Token: 0x04002687 RID: 9863
		private bool _hasFoundSCP;
	}
}
