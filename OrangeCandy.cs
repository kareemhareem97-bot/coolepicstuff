using System;
using InventorySystem.Items;
using MapGeneration.Holidays;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.PlayableScps;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008C5 RID: 2245
	public class OrangeCandy : StatusEffectBase, ISoundtrackMutingEffect, IHolidayEffect
	{
		// Token: 0x170009B9 RID: 2489
		// (get) Token: 0x06003317 RID: 13079 RVA: 0x00004070 File Offset: 0x00002270
		public bool MuteSoundtrack
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009BA RID: 2490
		// (get) Token: 0x06003318 RID: 13080 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x170009BB RID: 2491
		// (get) Token: 0x06003319 RID: 13081 RVA: 0x000A9C51 File Offset: 0x000A7E51
		public HolidayType[] TargetHolidays { get; } = new HolidayType[]
		{
			HolidayType.Halloween
		};

		// Token: 0x0600331A RID: 13082 RVA: 0x000A9C5C File Offset: 0x000A7E5C
		protected override void Awake()
		{
			base.Awake();
			this._maxElapsed = this._lightCurve.keys[this._lightCurve.length - 1].time;
			SharedHandsController.OnHandsSet = (Action<SharedHandsController.HandsVisuals>)Delegate.Combine(SharedHandsController.OnHandsSet, new Action<SharedHandsController.HandsVisuals>(this.OnHandsSet));
			SharedHandsController.OnHandsReset = (Action<SharedHandsController.HandsVisuals>)Delegate.Combine(SharedHandsController.OnHandsReset, new Action<SharedHandsController.HandsVisuals>(this.OnHandsReset));
		}

		// Token: 0x0600331B RID: 13083 RVA: 0x000A9CD8 File Offset: 0x000A7ED8
		private void OnDestroy()
		{
			SharedHandsController.OnHandsSet = (Action<SharedHandsController.HandsVisuals>)Delegate.Remove(SharedHandsController.OnHandsSet, new Action<SharedHandsController.HandsVisuals>(this.OnHandsSet));
			SharedHandsController.OnHandsReset = (Action<SharedHandsController.HandsVisuals>)Delegate.Remove(SharedHandsController.OnHandsReset, new Action<SharedHandsController.HandsVisuals>(this.OnHandsReset));
		}

		// Token: 0x0600331C RID: 13084 RVA: 0x000A9D28 File Offset: 0x000A7F28
		private void OnHandsSet(SharedHandsController.HandsVisuals visuals)
		{
			ReferenceHub left;
			if (!base.IsEnabled || !ReferenceHub.TryGetPovHub(out left))
			{
				return;
			}
			if (left != base.Hub)
			{
				return;
			}
			this.SetOrangeHands(true);
		}

		// Token: 0x0600331D RID: 13085 RVA: 0x000A9D60 File Offset: 0x000A7F60
		private void OnHandsReset(SharedHandsController.HandsVisuals visuals)
		{
			ReferenceHub left;
			if (!base.IsEnabled || !ReferenceHub.TryGetPovHub(out left))
			{
				return;
			}
			if (left != base.Hub)
			{
				return;
			}
			this.SetOrangeHands(false);
		}

		// Token: 0x0600331E RID: 13086 RVA: 0x000A9D98 File Offset: 0x000A7F98
		protected override void Enabled()
		{
			if (NetworkServer.active)
			{
				base.Hub.playerEffectsController.DisableEffect<WhiteCandy>();
				base.Hub.playerEffectsController.DisableEffect<Metal>();
			}
			this._elapsed = 0f;
			this._effectSource.Play();
			this._effectHandler = new ModelVisualEffect(base.Hub.GetModel(), this._litMaterial);
			if (SharedHandsController.Singleton.LastHandVisuals != null)
			{
				this.OnHandsSet(SharedHandsController.Singleton.LastHandVisuals.Value);
			}
		}

		// Token: 0x0600331F RID: 13087 RVA: 0x000A9E2C File Offset: 0x000A802C
		protected override void IntensityChanged(byte prevState, byte newState)
		{
			base.IntensityChanged(prevState, newState);
			if (base.IsLocalPlayer || base.IsSpectated)
			{
				this.SetOrangeHands(newState > 0);
			}
		}

		// Token: 0x06003320 RID: 13088 RVA: 0x000A9E50 File Offset: 0x000A8050
		public override void OnBeginSpectating()
		{
			base.OnBeginSpectating();
			this.SetOrangeHands(base.IsEnabled);
		}

		// Token: 0x06003321 RID: 13089 RVA: 0x000A9E64 File Offset: 0x000A8064
		public override void OnStopSpectating()
		{
			base.OnStopSpectating();
			this.SetOrangeHands(false);
		}

		// Token: 0x06003322 RID: 13090 RVA: 0x000A9E74 File Offset: 0x000A8074
		private void SetOrangeHands(bool status)
		{
			if (SharedHandsController.Singleton.LastHandVisuals == null)
			{
				return;
			}
			SharedHandsController.HandsVisuals value = SharedHandsController.Singleton.LastHandVisuals.Value;
			if (status)
			{
				value.SetOverrideTexture(this._litMaterial);
				return;
			}
			value.SetDefaultTextures();
		}

		// Token: 0x06003323 RID: 13091 RVA: 0x000A9EC4 File Offset: 0x000A80C4
		protected override void Update()
		{
			base.Update();
			this.UpdateBlindingEffect();
			if (!base.IsEnabled && this._elapsed == 0f)
			{
				return;
			}
			if (base.IsEnabled)
			{
				this._elapsed += Time.deltaTime;
			}
			else
			{
				this._elapsed -= Time.deltaTime;
			}
			this._elapsed = Mathf.Clamp(this._elapsed, 0f, this._maxElapsed);
			this._lightSource.enabled = (this._elapsed > 0f);
			this._lightSource.intensity = this._lightCurve.Evaluate(this._elapsed);
		}

		// Token: 0x06003324 RID: 13092 RVA: 0x000A9F71 File Offset: 0x000A8171
		protected override void Disabled()
		{
			this._effectSource.Stop();
			ModelVisualEffect effectHandler = this._effectHandler;
			if (effectHandler == null)
			{
				return;
			}
			effectHandler.Restore();
		}

		// Token: 0x06003325 RID: 13093 RVA: 0x000A9F90 File Offset: 0x000A8190
		private void UpdateBlindingEffect()
		{
			if (base.IsEnabled)
			{
				IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
				if (fpcRole != null)
				{
					Vector3 position = fpcRole.FpcModule.Position;
					foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
					{
						if (!(base.Hub == referenceHub) && HitboxIdentity.IsEnemy(base.Hub, referenceHub))
						{
							IFpcRole fpcRole2 = referenceHub.roleManager.CurrentRole as IFpcRole;
							if (fpcRole2 != null && VisionInformation.GetVisionInformation(referenceHub, referenceHub.PlayerCameraReference, position, 0.3f, 60f, true, true, 0, false).IsLooking)
							{
								referenceHub.playerEffectsController.EnableEffect<OrangeWitness>(Time.deltaTime * 1.15f, true);
								if (Vector3.Distance(fpcRole2.FpcModule.Position, position) <= 4.4f)
								{
									referenceHub.playerEffectsController.EnableEffect<Flashed>(Time.deltaTime * 1.0333333f, true);
								}
							}
						}
					}
					return;
				}
			}
		}

		// Token: 0x04002783 RID: 10115
		private const float Radius = 0.3f;

		// Token: 0x04002784 RID: 10116
		private const float MaxRange = 60f;

		// Token: 0x04002785 RID: 10117
		private const float MinimumFlashDistance = 4.4f;

		// Token: 0x04002786 RID: 10118
		private const float PersistanceModifier = 1.15f;

		// Token: 0x04002787 RID: 10119
		private const float FlashPersistanceModifier = 1.0333333f;

		// Token: 0x04002789 RID: 10121
		[SerializeField]
		private AudioSource _effectSource;

		// Token: 0x0400278A RID: 10122
		[SerializeField]
		private Light _lightSource;

		// Token: 0x0400278B RID: 10123
		[SerializeField]
		private AnimationCurve _lightCurve;

		// Token: 0x0400278C RID: 10124
		[SerializeField]
		private Material _litMaterial;

		// Token: 0x0400278D RID: 10125
		private ModelVisualEffect _effectHandler;

		// Token: 0x0400278E RID: 10126
		private float _elapsed;

		// Token: 0x0400278F RID: 10127
		private float _maxElapsed;
	}
}
