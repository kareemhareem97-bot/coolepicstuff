using System;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp1507;
using UnityEngine;

namespace FacilitySoundtrack
{
	// Token: 0x02000874 RID: 2164
	public class FlamingoSoundtrack : SoundtrackLayerBase
	{
		// Token: 0x17000916 RID: 2326
		// (get) Token: 0x06003157 RID: 12631 RVA: 0x000A5FF4 File Offset: 0x000A41F4
		public override float Weight
		{
			get
			{
				return this._weight;
			}
		}

		// Token: 0x17000917 RID: 2327
		// (get) Token: 0x06003158 RID: 12632 RVA: 0x00004094 File Offset: 0x00002294
		public override bool Additive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000918 RID: 2328
		// (get) Token: 0x06003159 RID: 12633 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool IsMuteable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600315A RID: 12634 RVA: 0x000A5FFC File Offset: 0x000A41FC
		private void Update()
		{
			bool flag;
			bool flag2;
			this.FindFlamingos(out flag, out flag2);
			if (!flag)
			{
				this._weight = Mathf.MoveTowards(this._weight, 0f, this._fadeOutWeightSpeed * Time.deltaTime);
				return;
			}
			if (flag2 || this._elapsed < this._omnipresentDuration)
			{
				this._separationTimer = 0f;
			}
			else
			{
				this._separationTimer += Time.deltaTime;
			}
			float target = (this._separationTimer < this._minSustainTime) ? 1f : 0.01f;
			this._weight = Mathf.MoveTowards(this._weight, target, this._fadePresenceSpeed * Time.deltaTime);
		}

		// Token: 0x0600315B RID: 12635 RVA: 0x000A60A4 File Offset: 0x000A42A4
		private void FindFlamingos(out bool anyFlamingosExist, out bool anyFlamingosInRange)
		{
			anyFlamingosExist = false;
			anyFlamingosInRange = false;
			ReferenceHub referenceHub;
			Vector3? vector;
			if (ReferenceHub.TryGetLocalHub(out referenceHub))
			{
				IFpcRole fpcRole = referenceHub.roleManager.CurrentRole as IFpcRole;
				if (fpcRole != null)
				{
					vector = new Vector3?(fpcRole.FpcModule.Position);
					goto IL_3E;
				}
			}
			vector = null;
			IL_3E:
			Vector3? vector2 = vector;
			foreach (ReferenceHub referenceHub2 in ReferenceHub.AllHubs)
			{
				Scp1507Role scp1507Role = referenceHub2.roleManager.CurrentRole as Scp1507Role;
				if (scp1507Role != null && scp1507Role.Team == Team.Flamingos)
				{
					anyFlamingosExist = true;
					if (vector2 == null)
					{
						break;
					}
					if ((scp1507Role.FpcModule.Position - vector2.Value).sqrMagnitude <= this._flamingoRangeSqr)
					{
						anyFlamingosInRange = true;
						break;
					}
				}
			}
		}

		// Token: 0x0600315C RID: 12636 RVA: 0x000A618C File Offset: 0x000A438C
		public override void UpdateVolume(float volumeScale)
		{
			bool flag = volumeScale <= 0f;
			if (this._ambient.isPlaying)
			{
				if (flag)
				{
					this._ambient.Stop();
				}
				this._elapsed += Time.deltaTime;
			}
			else
			{
				if (flag)
				{
					return;
				}
				this._ambient.Play();
			}
			this._ambient.volume = volumeScale * this._volumeOverElapsed.Evaluate(this._elapsed);
		}

		// Token: 0x04002688 RID: 9864
		[SerializeField]
		private float _fadeInWeightSpeed;

		// Token: 0x04002689 RID: 9865
		[SerializeField]
		private float _fadeOutWeightSpeed;

		// Token: 0x0400268A RID: 9866
		[SerializeField]
		private float _fadePresenceSpeed;

		// Token: 0x0400268B RID: 9867
		[SerializeField]
		private AudioSource _ambient;

		// Token: 0x0400268C RID: 9868
		[SerializeField]
		private AnimationCurve _volumeOverElapsed;

		// Token: 0x0400268D RID: 9869
		[SerializeField]
		private float _minSustainTime;

		// Token: 0x0400268E RID: 9870
		[SerializeField]
		private float _omnipresentDuration;

		// Token: 0x0400268F RID: 9871
		[SerializeField]
		private float _flamingoRangeSqr;

		// Token: 0x04002690 RID: 9872
		private float _weight;

		// Token: 0x04002691 RID: 9873
		private float _elapsed;

		// Token: 0x04002692 RID: 9874
		private float _separationTimer;

		// Token: 0x04002693 RID: 9875
		private const float LateJoinSkipTime = 60f;

		// Token: 0x04002694 RID: 9876
		private const float AwayVolume = 0.01f;
	}
}
