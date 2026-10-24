using System;
using AudioPooling;
using InventorySystem;
using InventorySystem.Items;
using MapGeneration.Holidays;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008C2 RID: 2242
	public class Metal : StatusEffectBase, IMovementSpeedModifier, IUsableItemModifierEffect, ICustomHitSfxEffect, IDamageModifierEffect, IFootstepEffect, IHolidayEffect, ICustomFallSfxEffect, IPulseEffect
	{
		// Token: 0x170009AA RID: 2474
		// (get) Token: 0x060032F1 RID: 13041 RVA: 0x000A960D File Offset: 0x000A780D
		public float MovementSpeedLimit
		{
			get
			{
				this.UpdateGravity();
				return float.MaxValue;
			}
		}

		// Token: 0x170009AB RID: 2475
		// (get) Token: 0x060032F2 RID: 13042 RVA: 0x00034727 File Offset: 0x00032927
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Mixed;
			}
		}

		// Token: 0x170009AC RID: 2476
		// (get) Token: 0x060032F3 RID: 13043 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009AD RID: 2477
		// (get) Token: 0x060032F4 RID: 13044 RVA: 0x000A961A File Offset: 0x000A781A
		public float MovementSpeedMultiplier
		{
			get
			{
				return 0.75f;
			}
		}

		// Token: 0x170009AE RID: 2478
		// (get) Token: 0x060032F5 RID: 13045 RVA: 0x00004070 File Offset: 0x00002270
		public bool DamageModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009AF RID: 2479
		// (get) Token: 0x060032F6 RID: 13046 RVA: 0x000A9621 File Offset: 0x000A7821
		public HolidayType[] TargetHolidays { get; } = new HolidayType[]
		{
			HolidayType.Halloween
		};

		// Token: 0x170009B0 RID: 2480
		// (get) Token: 0x060032F7 RID: 13047 RVA: 0x000A9629 File Offset: 0x000A7829
		public AudioClip HitSfx
		{
			get
			{
				return this._onHitSfx.RandomItem<AudioClip>();
			}
		}

		// Token: 0x170009B1 RID: 2481
		// (get) Token: 0x060032F8 RID: 13048 RVA: 0x000A9636 File Offset: 0x000A7836
		// (set) Token: 0x060032F9 RID: 13049 RVA: 0x000A963E File Offset: 0x000A783E
		public AudioClip FallSfxClip { get; private set; }

		// Token: 0x170009B2 RID: 2482
		// (get) Token: 0x060032FA RID: 13050 RVA: 0x000A9647 File Offset: 0x000A7847
		// (set) Token: 0x060032FB RID: 13051 RVA: 0x000A964F File Offset: 0x000A784F
		public float FallSfxVolume { get; private set; } = 1f;

		// Token: 0x060032FC RID: 13052 RVA: 0x000A9658 File Offset: 0x000A7858
		private void UpdateGravity()
		{
			if (!base.IsLocalPlayer && !NetworkServer.active)
			{
				return;
			}
			if (this._prevController == null)
			{
				return;
			}
			Vector3 vector = (this._prevController.Motor.Velocity.y < 0f) ? this._fallGravity : this._normalGravity;
			if (this._prevController.Gravity == vector)
			{
				return;
			}
			this._prevController.Gravity = vector;
		}

		// Token: 0x060032FD RID: 13053 RVA: 0x000A96CC File Offset: 0x000A78CC
		protected override void Enabled()
		{
			base.Enabled();
			if (NetworkServer.active)
			{
				base.Hub.playerEffectsController.DisableEffect<WhiteCandy>();
				base.Hub.playerEffectsController.DisableEffect<OrangeCandy>();
			}
			this._modelEffect = new ModelVisualEffect(base.Hub.GetModel(), this._metalMaterial);
			this._ambienceSource.clip = this._effectThemes[UnityEngine.Random.Range(0, this._effectThemes.Length)];
			this._ambienceSource.Play();
			if (!base.IsLocalPlayer && !NetworkServer.active)
			{
				return;
			}
			IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole != null)
			{
				this._prevController = fpcRole.FpcModule.Motor.GravityController;
				this._normalGravity = this._prevController.Gravity;
				this._fallGravity = this._normalGravity * 4f;
				return;
			}
			this._prevController = null;
		}

		// Token: 0x060032FE RID: 13054 RVA: 0x000A97C0 File Offset: 0x000A79C0
		protected override void Disabled()
		{
			base.Disabled();
			this._ambienceSource.Stop();
			ModelVisualEffect modelEffect = this._modelEffect;
			if (modelEffect != null)
			{
				modelEffect.Restore();
			}
			if (!base.IsLocalPlayer && !NetworkServer.active)
			{
				return;
			}
			if (this._prevController == null)
			{
				return;
			}
			this._prevController.Gravity = this._normalGravity;
			this._prevController = null;
		}

		// Token: 0x060032FF RID: 13055 RVA: 0x000A9820 File Offset: 0x000A7A20
		protected override void IntensityChanged(byte prevState, byte newState)
		{
			base.IntensityChanged(prevState, newState);
			if (base.IsLocalPlayer || base.IsSpectated)
			{
				this.SetMetalHands(newState > 0);
			}
		}

		// Token: 0x06003300 RID: 13056 RVA: 0x000A9844 File Offset: 0x000A7A44
		public override void OnBeginSpectating()
		{
			base.OnBeginSpectating();
			this.SetMetalHands(base.IsEnabled);
		}

		// Token: 0x06003301 RID: 13057 RVA: 0x000A9858 File Offset: 0x000A7A58
		public override void OnStopSpectating()
		{
			base.OnStopSpectating();
			this.SetMetalHands(false);
		}

		// Token: 0x06003302 RID: 13058 RVA: 0x000A9868 File Offset: 0x000A7A68
		protected override void Awake()
		{
			base.Awake();
			SharedHandsController.OnHandsSet = (Action<SharedHandsController.HandsVisuals>)Delegate.Combine(SharedHandsController.OnHandsSet, new Action<SharedHandsController.HandsVisuals>(this.OnHandsSet));
			SharedHandsController.OnHandsReset = (Action<SharedHandsController.HandsVisuals>)Delegate.Combine(SharedHandsController.OnHandsReset, new Action<SharedHandsController.HandsVisuals>(this.OnHandsReset));
		}

		// Token: 0x06003303 RID: 13059 RVA: 0x000A98BC File Offset: 0x000A7ABC
		private void OnDestroy()
		{
			SharedHandsController.OnHandsSet = (Action<SharedHandsController.HandsVisuals>)Delegate.Remove(SharedHandsController.OnHandsSet, new Action<SharedHandsController.HandsVisuals>(this.OnHandsSet));
			SharedHandsController.OnHandsReset = (Action<SharedHandsController.HandsVisuals>)Delegate.Remove(SharedHandsController.OnHandsReset, new Action<SharedHandsController.HandsVisuals>(this.OnHandsReset));
		}

		// Token: 0x06003304 RID: 13060 RVA: 0x000A990C File Offset: 0x000A7B0C
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
			this.SetMetalHands(false);
		}

		// Token: 0x06003305 RID: 13061 RVA: 0x000A9944 File Offset: 0x000A7B44
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
			this.SetMetalHands(true);
		}

		// Token: 0x06003306 RID: 13062 RVA: 0x000A997C File Offset: 0x000A7B7C
		private void SetMetalHands(bool status)
		{
			if (SharedHandsController.Singleton.LastHandVisuals == null)
			{
				return;
			}
			SharedHandsController.HandsVisuals value = SharedHandsController.Singleton.LastHandVisuals.Value;
			if (status)
			{
				value.SetOverrideTexture(this._metaFirstPersonMaterial);
				return;
			}
			value.SetDefaultTextures();
		}

		// Token: 0x06003307 RID: 13063 RVA: 0x000A99CC File Offset: 0x000A7BCC
		public bool TryGetSpeed(ItemType item, out float speed)
		{
			ItemBase itemBase;
			if (!InventoryItemLoader.AvailableItems.TryGetValue(item, out itemBase))
			{
				speed = 1f;
				return false;
			}
			if (itemBase.Category != ItemCategory.Medical)
			{
				speed = 1f;
				return false;
			}
			speed = 0.5f;
			return true;
		}

		// Token: 0x06003308 RID: 13064 RVA: 0x000A9A0C File Offset: 0x000A7C0C
		public float GetDamageModifier(float baseDamage, DamageHandlerBase handler, HitboxType hitboxType)
		{
			UniversalDamageHandler universalDamageHandler = handler as UniversalDamageHandler;
			StandardDamageHandler standardDamageHandler;
			if (universalDamageHandler == null)
			{
				standardDamageHandler = (handler as StandardDamageHandler);
				if (standardDamageHandler == null)
				{
					goto IL_52;
				}
			}
			else
			{
				if (universalDamageHandler.TranslationId == DeathTranslations.Falldown.Id)
				{
					return this.ProcessFallDamage(baseDamage);
				}
				standardDamageHandler = (StandardDamageHandler)handler;
			}
			if (standardDamageHandler.Damage == -1f)
			{
				return 1f;
			}
			IL_52:
			return 0.2f;
		}

		// Token: 0x06003309 RID: 13065 RVA: 0x000A9A72 File Offset: 0x000A7C72
		public float ProcessFootstepOverrides(AnimatedCharacterModel _, float dis)
		{
			AudioSourcePoolManager.PlayOnTransform(this._footstepSounds.RandomItem<AudioClip>(), base.transform, dis, 1f, FalloffType.Exponential, MixerChannel.DefaultSfx, 1f);
			return this._originalLoudness;
		}

		// Token: 0x0600330A RID: 13066 RVA: 0x000A9AA0 File Offset: 0x000A7CA0
		public void ExecutePulse()
		{
			ReferenceHub referenceHub;
			if (!ReferenceHub.TryGetLocalHub(out referenceHub))
			{
				return;
			}
			IFpcRole fpcRole = referenceHub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole == null)
			{
				return;
			}
			float t = Mathf.Clamp01(fpcRole.SqrDistanceTo(base.Hub) / 16f);
			float explosionForce = Mathf.Lerp(0.35f, 0f, t);
			ExplosionCameraShake.singleton.Shake(explosionForce);
		}

		// Token: 0x0600330B RID: 13067 RVA: 0x000A9B00 File Offset: 0x000A7D00
		private float ProcessFallDamage(float baseDamage)
		{
			if (baseDamage < 35f)
			{
				return 0f;
			}
			this.OnServerProcessFall(baseDamage);
			return 0.05f;
		}

		// Token: 0x0600330C RID: 13068 RVA: 0x000A9B1C File Offset: 0x000A7D1C
		private void OnServerProcessFall(float damage)
		{
			float damage2 = 80f + damage * 0.4f;
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				if (!(referenceHub == base.Hub))
				{
					IFpcRole fpcRole = referenceHub.roleManager.CurrentRole as IFpcRole;
					if (fpcRole != null && fpcRole.SqrDistanceTo(base.Hub) <= 4f && referenceHub.playerStats.DealDamage(new GrayCandyDamageHandler(base.Hub, damage2)))
					{
						referenceHub.playerEffectsController.ServerSendPulse<Metal>();
						Hitmarker.SendHitmarkerDirectly(base.Hub, 1f, true, HitmarkerType.Regular);
					}
				}
			}
			base.Hub.playerEffectsController.ServerSendPulse<Metal>();
		}

		// Token: 0x0400276C RID: 10092
		private const float GravityMultiplier = 4f;

		// Token: 0x0400276D RID: 10093
		private const float AllDamageMultiplier = 0.2f;

		// Token: 0x0400276E RID: 10094
		private const float FallDamageMultiplier = 0.05f;

		// Token: 0x0400276F RID: 10095
		private const float AreaDamageSqrDistance = 4f;

		// Token: 0x04002770 RID: 10096
		private const float AreaDamageMultiplier = 0.4f;

		// Token: 0x04002771 RID: 10097
		private const int BaseFallDamage = 80;

		// Token: 0x04002772 RID: 10098
		private const float MaxShakeAmount = 0.35f;

		// Token: 0x04002773 RID: 10099
		private const float MaxShakeDistanceSqr = 16f;

		// Token: 0x04002774 RID: 10100
		private const float MinFallDamageAllowed = 35f;

		// Token: 0x04002778 RID: 10104
		[SerializeField]
		private Material _metalMaterial;

		// Token: 0x04002779 RID: 10105
		[SerializeField]
		private Material _metaFirstPersonMaterial;

		// Token: 0x0400277A RID: 10106
		[SerializeField]
		private AudioSource _ambienceSource;

		// Token: 0x0400277B RID: 10107
		[SerializeField]
		private AudioClip[] _onHitSfx;

		// Token: 0x0400277C RID: 10108
		[SerializeField]
		private AudioClip[] _effectThemes;

		// Token: 0x0400277D RID: 10109
		[SerializeField]
		private AudioClip[] _footstepSounds;

		// Token: 0x0400277E RID: 10110
		[SerializeField]
		private float _originalLoudness;

		// Token: 0x0400277F RID: 10111
		private ModelVisualEffect _modelEffect;

		// Token: 0x04002780 RID: 10112
		private FpcGravityController _prevController;

		// Token: 0x04002781 RID: 10113
		private Vector3 _normalGravity;

		// Token: 0x04002782 RID: 10114
		private Vector3 _fallGravity;
	}
}
