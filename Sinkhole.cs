using System;
using AudioPooling;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008DA RID: 2266
	public class Sinkhole : StatusEffectBase, IStaminaModifier, IMovementSpeedModifier, IFootstepEffect
	{
		// Token: 0x170009F3 RID: 2547
		// (get) Token: 0x060033C9 RID: 13257 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009F4 RID: 2548
		// (get) Token: 0x060033CA RID: 13258 RVA: 0x000AB4A0 File Offset: 0x000A96A0
		public float MovementSpeedMultiplier
		{
			get
			{
				return 1f - this._slowAmount * 0.01f;
			}
		}

		// Token: 0x170009F5 RID: 2549
		// (get) Token: 0x060033CB RID: 13259 RVA: 0x0000407F File Offset: 0x0000227F
		public float MovementSpeedLimit
		{
			get
			{
				return float.MaxValue;
			}
		}

		// Token: 0x170009F6 RID: 2550
		// (get) Token: 0x060033CC RID: 13260 RVA: 0x00004070 File Offset: 0x00002270
		public bool StaminaModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009F7 RID: 2551
		// (get) Token: 0x060033CD RID: 13261 RVA: 0x0000408D File Offset: 0x0000228D
		public float StaminaUsageMultiplier
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x170009F8 RID: 2552
		// (get) Token: 0x060033CE RID: 13262 RVA: 0x0000408D File Offset: 0x0000228D
		public float StaminaRegenMultiplier
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x170009F9 RID: 2553
		// (get) Token: 0x060033CF RID: 13263 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool SprintingDisabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170009FA RID: 2554
		// (get) Token: 0x060033D0 RID: 13264 RVA: 0x000A84BF File Offset: 0x000A66BF
		public override bool AllowEnabling
		{
			get
			{
				return !SpawnProtected.CheckPlayer(base.Hub);
			}
		}

		// Token: 0x060033D1 RID: 13265 RVA: 0x000AB4B4 File Offset: 0x000A96B4
		public float ProcessFootstepOverrides(AnimatedCharacterModel _, float dis)
		{
			AudioSourcePoolManager.PlayOnTransform(this._footstepSounds.RandomItem<AudioClip>(), base.transform, dis, 1f, FalloffType.Exponential, MixerChannel.DefaultSfx, 1f);
			return this._originalLoudness;
		}

		// Token: 0x040027E3 RID: 10211
		[SerializeField]
		[Range(0f, 100f)]
		private float _slowAmount;

		// Token: 0x040027E4 RID: 10212
		[SerializeField]
		private AudioClip[] _footstepSounds;

		// Token: 0x040027E5 RID: 10213
		[SerializeField]
		private float _originalLoudness;
	}
}
