using System;
using MapGeneration.Holidays;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008DB RID: 2267
	public class SlowMetabolism : TickingEffectBase, IHealableEffect, IMovementSpeedModifier, IUsableItemModifierEffect, IHolidayEffect
	{
		// Token: 0x170009FB RID: 2555
		// (get) Token: 0x060033D3 RID: 13267 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009FC RID: 2556
		// (get) Token: 0x060033D4 RID: 13268 RVA: 0x000AB4E0 File Offset: 0x000A96E0
		public float MovementSpeedMultiplier
		{
			get
			{
				return 0.9f;
			}
		}

		// Token: 0x170009FD RID: 2557
		// (get) Token: 0x060033D5 RID: 13269 RVA: 0x0000407F File Offset: 0x0000227F
		public float MovementSpeedLimit
		{
			get
			{
				return float.MaxValue;
			}
		}

		// Token: 0x170009FE RID: 2558
		// (get) Token: 0x060033D6 RID: 13270 RVA: 0x000AB4E7 File Offset: 0x000A96E7
		public HolidayType[] TargetHolidays { get; } = new HolidayType[]
		{
			HolidayType.Halloween
		};

		// Token: 0x170009FF RID: 2559
		// (get) Token: 0x060033D7 RID: 13271 RVA: 0x000AB4EF File Offset: 0x000A96EF
		// (set) Token: 0x060033D8 RID: 13272 RVA: 0x000AB502 File Offset: 0x000A9702
		private float CurStamina
		{
			get
			{
				this.PrepCache();
				return this._staminaCache.CurValue;
			}
			set
			{
				this.PrepCache();
				this._staminaCache.CurValue = value;
			}
		}

		// Token: 0x060033D9 RID: 13273 RVA: 0x00004242 File Offset: 0x00002442
		public bool IsHealable(ItemType it)
		{
			return it == ItemType.SCP500;
		}

		// Token: 0x060033DA RID: 13274 RVA: 0x000AB516 File Offset: 0x000A9716
		public bool TryGetSpeed(ItemType item, out float speed)
		{
			speed = 3f;
			return item == ItemType.SCP330;
		}

		// Token: 0x060033DB RID: 13275 RVA: 0x000AB524 File Offset: 0x000A9724
		protected override void OnTick()
		{
			if (NetworkServer.active)
			{
				base.Intensity = ((this.CurStamina <= 0.3f) ? 2 : 1);
			}
		}

		// Token: 0x060033DC RID: 13276 RVA: 0x000AB556 File Offset: 0x000A9756
		protected override void Enabled()
		{
			base.Enabled();
			this.ChangeBodyState(true);
		}

		// Token: 0x060033DD RID: 13277 RVA: 0x000AB565 File Offset: 0x000A9765
		protected override void Disabled()
		{
			base.Disabled();
			this.ChangeBodyState(false);
		}

		// Token: 0x060033DE RID: 13278 RVA: 0x000AB574 File Offset: 0x000A9774
		private void PrepCache()
		{
			if (this._cacheSet)
			{
				return;
			}
			this._cacheSet = true;
			this._staminaCache = base.Hub.playerStats.GetModule<StaminaStat>();
		}

		// Token: 0x060033DF RID: 13279 RVA: 0x000AB59C File Offset: 0x000A979C
		private void ChangeBodyState(bool inflateBody)
		{
			HumanRole humanRole = base.Hub.roleManager.CurrentRole as HumanRole;
			if (humanRole == null)
			{
				return;
			}
			AnimatedCharacterModel animatedCharacterModel = humanRole.FpcModule.CharacterModelInstance as AnimatedCharacterModel;
			if (animatedCharacterModel == null)
			{
				return;
			}
			if (animatedCharacterModel.HasParameter(SlowMetabolism.HashBodyAnim))
			{
				animatedCharacterModel.Animator.SetBool(SlowMetabolism.HashBodyAnim, inflateBody);
			}
		}

		// Token: 0x040027E6 RID: 10214
		private const float EatingSpeedMultiplier = 3f;

		// Token: 0x040027E7 RID: 10215
		private const float BreathingStaminaThreshold = 0.3f;

		// Token: 0x040027E8 RID: 10216
		private static readonly int HashBodyAnim = Animator.StringToHash("PurpleCandy");

		// Token: 0x040027EA RID: 10218
		private StaminaStat _staminaCache;

		// Token: 0x040027EB RID: 10219
		private bool _cacheSet;
	}
}
