using System;
using AudioPooling;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008E2 RID: 2274
	public class Stained : StatusEffectBase, IStaminaModifier, IMovementSpeedModifier, IFootstepEffect
	{
		// Token: 0x17000A10 RID: 2576
		// (get) Token: 0x06003407 RID: 13319 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A11 RID: 2577
		// (get) Token: 0x06003408 RID: 13320 RVA: 0x000ABC63 File Offset: 0x000A9E63
		public float MovementSpeedMultiplier
		{
			get
			{
				return 0.8f;
			}
		}

		// Token: 0x17000A12 RID: 2578
		// (get) Token: 0x06003409 RID: 13321 RVA: 0x0000407F File Offset: 0x0000227F
		public float MovementSpeedLimit
		{
			get
			{
				return float.MaxValue;
			}
		}

		// Token: 0x17000A13 RID: 2579
		// (get) Token: 0x0600340A RID: 13322 RVA: 0x00004070 File Offset: 0x00002270
		public bool StaminaModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A14 RID: 2580
		// (get) Token: 0x0600340B RID: 13323 RVA: 0x0000408D File Offset: 0x0000228D
		public float StaminaUsageMultiplier
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000A15 RID: 2581
		// (get) Token: 0x0600340C RID: 13324 RVA: 0x0000408D File Offset: 0x0000228D
		public float StaminaRegenMultiplier
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000A16 RID: 2582
		// (get) Token: 0x0600340D RID: 13325 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool SprintingDisabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600340E RID: 13326 RVA: 0x000ABC6C File Offset: 0x000A9E6C
		public float ProcessFootstepOverrides(AnimatedCharacterModel _, float dis)
		{
			SilentWalk effect = base.Hub.playerEffectsController.GetEffect<SilentWalk>();
			if (effect.IsEnabled)
			{
				return effect.ProcessFootstepOverrides(_, dis);
			}
			AudioSourcePoolManager.PlayOnTransform(this._stainedFootsteps.RandomItem<AudioClip>(), base.transform, dis, 1f, FalloffType.Exponential, MixerChannel.DefaultSfx, 1f);
			return this._originalLoudness;
		}

		// Token: 0x04002801 RID: 10241
		[SerializeField]
		private AudioClip[] _stainedFootsteps;

		// Token: 0x04002802 RID: 10242
		[SerializeField]
		private float _originalLoudness;

		// Token: 0x04002803 RID: 10243
		private const float SpeedMultiplier = 0.8f;
	}
}
