using System;
using AudioPooling;
using CustomRendering;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using MapGeneration;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerStatsSystem;
using RelativePositioning;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008C8 RID: 2248
	public class PocketCorroding : TickingEffectBase, IFootstepEffect, IMovementSpeedModifier, IStaminaModifier
	{
		// Token: 0x170009BF RID: 2495
		// (get) Token: 0x06003333 RID: 13107 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool AllowEnabling
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170009C0 RID: 2496
		// (get) Token: 0x06003334 RID: 13108 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009C1 RID: 2497
		// (get) Token: 0x06003335 RID: 13109 RVA: 0x00004070 File Offset: 0x00002270
		public bool StaminaModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009C2 RID: 2498
		// (get) Token: 0x06003336 RID: 13110 RVA: 0x0000408D File Offset: 0x0000228D
		public float StaminaUsageMultiplier
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x170009C3 RID: 2499
		// (get) Token: 0x06003337 RID: 13111 RVA: 0x000A961A File Offset: 0x000A781A
		public float MovementSpeedMultiplier
		{
			get
			{
				return 0.75f;
			}
		}

		// Token: 0x170009C4 RID: 2500
		// (get) Token: 0x06003338 RID: 13112 RVA: 0x0000408D File Offset: 0x0000228D
		public float StaminaRegenMultiplier
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x170009C5 RID: 2501
		// (get) Token: 0x06003339 RID: 13113 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool SprintingDisabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170009C6 RID: 2502
		// (get) Token: 0x0600333A RID: 13114 RVA: 0x0000407F File Offset: 0x0000227F
		public float MovementSpeedLimit
		{
			get
			{
				return float.MaxValue;
			}
		}

		// Token: 0x170009C7 RID: 2503
		// (get) Token: 0x0600333B RID: 13115 RVA: 0x000AA219 File Offset: 0x000A8419
		// (set) Token: 0x0600333C RID: 13116 RVA: 0x000AA221 File Offset: 0x000A8421
		public RelativePosition CapturePosition { get; private set; }

		// Token: 0x0600333D RID: 13117 RVA: 0x000AA22C File Offset: 0x000A842C
		protected override void OnTick()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			RoomIdentifier roomIdentifier;
			if (base.Hub.TryGetLastKnownRoom(out roomIdentifier) && roomIdentifier.Name == RoomName.Pocket)
			{
				base.Hub.playerStats.DealDamage(new UniversalDamageHandler(this._damagePerTick, DeathTranslations.PocketDecay, null));
				this._damagePerTick += 0.15f;
				return;
			}
			base.ServerDisable();
		}

		// Token: 0x0600333E RID: 13118 RVA: 0x000AA298 File Offset: 0x000A8498
		protected override void Enabled()
		{
			if (base.IsPOV)
			{
				this.SetFogEnabled(true);
			}
			if (!NetworkServer.active)
			{
				return;
			}
			this._damagePerTick = this._startingDamage;
			IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole == null)
			{
				return;
			}
			RoomIdentifier roomIdentifier;
			if (!RoomUtils.TryFindRoom(new RoomName?(RoomName.Pocket), null, null, out roomIdentifier))
			{
				return;
			}
			PlayerEnteringPocketDimensionEventArgs playerEnteringPocketDimensionEventArgs = new PlayerEnteringPocketDimensionEventArgs(base.Hub);
			PlayerEvents.OnEnteringPocketDimension(playerEnteringPocketDimensionEventArgs);
			if (!playerEnteringPocketDimensionEventArgs.IsAllowed)
			{
				return;
			}
			this.CapturePosition = new RelativePosition(fpcRole.FpcModule.Position);
			if (base.Intensity < 106)
			{
				Vector3 position = roomIdentifier.transform.position;
				Vector3 b = Vector3.up * 1.5f;
				fpcRole.FpcModule.ServerOverridePosition(position + b);
			}
			PlayerEvents.OnEnteredPocketDimension(new PlayerEnteredPocketDimensionEventArgs(base.Hub));
		}

		// Token: 0x0600333F RID: 13119 RVA: 0x000AA380 File Offset: 0x000A8580
		protected override void Disabled()
		{
			base.Disabled();
			if (base.IsPOV)
			{
				this.SetFogEnabled(false);
			}
		}

		// Token: 0x06003340 RID: 13120 RVA: 0x000AA397 File Offset: 0x000A8597
		public override void OnBeginSpectating()
		{
			base.OnBeginSpectating();
			this.SetFogEnabled(base.IsEnabled);
		}

		// Token: 0x06003341 RID: 13121 RVA: 0x000AA3AB File Offset: 0x000A85AB
		public override void OnStopSpectating()
		{
			base.OnStopSpectating();
			this.SetFogEnabled(false);
		}

		// Token: 0x06003342 RID: 13122 RVA: 0x000AA3BA File Offset: 0x000A85BA
		public float ProcessFootstepOverrides(AnimatedCharacterModel _, float dis)
		{
			AudioSourcePoolManager.PlayOnTransform(this._footstepSounds.RandomItem<AudioClip>(), base.transform, dis, 1f, FalloffType.Exponential, MixerChannel.DefaultSfx, 1f);
			return this._originalLoudness;
		}

		// Token: 0x06003343 RID: 13123 RVA: 0x000AA3E6 File Offset: 0x000A85E6
		private void SetFogEnabled(bool isEnabled)
		{
			if (isEnabled)
			{
				FogController.EnableFogType(FogType.PocketDimension, 0f);
				return;
			}
			FogController.DisableFogType(FogType.PocketDimension, 0f);
		}

		// Token: 0x06003344 RID: 13124 RVA: 0x000AA404 File Offset: 0x000A8604
		public void ServerOverrideCapturePosition(RelativePosition capturePosition)
		{
			this.CapturePosition = capturePosition;
		}

		// Token: 0x06003345 RID: 13125 RVA: 0x000AA410 File Offset: 0x000A8610
		internal override void OnRoleChanged(PlayerRoleBase previousRole, PlayerRoleBase newRole)
		{
			base.OnRoleChanged(previousRole, newRole);
			if (NetworkServer.active)
			{
				IFpcRole fpcRole = newRole as IFpcRole;
				if (fpcRole != null)
				{
					RoomIdentifier roomIdentifier;
					if (!fpcRole.FpcModule.Position.TryGetRoom(out roomIdentifier) || roomIdentifier.Name != RoomName.Pocket)
					{
						return;
					}
					base.ServerSetState(106, 0f, false);
					return;
				}
			}
		}

		// Token: 0x04002794 RID: 10132
		private const float DamageScaling = 0.15f;

		// Token: 0x04002795 RID: 10133
		private const float PDSpawnHeightOffset = 1.5f;

		// Token: 0x04002796 RID: 10134
		private const byte NonTeleportIntensity = 106;

		// Token: 0x04002797 RID: 10135
		[SerializeField]
		private float _startingDamage = 1f;

		// Token: 0x04002798 RID: 10136
		[SerializeField]
		private AudioClip[] _footstepSounds;

		// Token: 0x04002799 RID: 10137
		[SerializeField]
		private float _originalLoudness;

		// Token: 0x0400279A RID: 10138
		private float _damagePerTick = 1f;
	}
}
