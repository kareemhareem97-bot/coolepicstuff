using System;

namespace CustomPlayerEffects.Danger
{
	// Token: 0x02000917 RID: 2327
	public class CorrodingDanger : DangerStackBase
	{
		// Token: 0x17000A76 RID: 2678
		// (get) Token: 0x06003515 RID: 13589 RVA: 0x000ADC86 File Offset: 0x000ABE86
		// (set) Token: 0x06003516 RID: 13590 RVA: 0x000ADC8E File Offset: 0x000ABE8E
		public override float DangerValue { get; set; } = 1f;

		// Token: 0x06003517 RID: 13591 RVA: 0x000ADC98 File Offset: 0x000ABE98
		public override void Initialize(ReferenceHub target)
		{
			base.Initialize(target);
			if (!base.Owner.playerEffectsController.TryGetEffect<Corroding>(out this._corroding))
			{
				throw new NullReferenceException("Corroding wasn't found. This DangerOverride will not function as intended.");
			}
			if (!base.Owner.playerEffectsController.TryGetEffect<PocketCorroding>(out this._pocketCorroding))
			{
				throw new NullReferenceException("PocketCorroding wasn't found. This DangerOverride will not function as intended.");
			}
			this.IsActive = (this._corroding.IsEnabled || this._pocketCorroding.IsEnabled);
			StatusEffectBase.OnEnabled += this.UpdateState;
			StatusEffectBase.OnDisabled += this.UpdateState;
		}

		// Token: 0x06003518 RID: 13592 RVA: 0x000ADD35 File Offset: 0x000ABF35
		public override void Dispose()
		{
			base.Dispose();
			StatusEffectBase.OnEnabled -= this.UpdateState;
			StatusEffectBase.OnDisabled -= this.UpdateState;
		}

		// Token: 0x06003519 RID: 13593 RVA: 0x000ADD5F File Offset: 0x000ABF5F
		private void UpdateState(StatusEffectBase effect)
		{
			if (effect != this._corroding && effect != this._pocketCorroding)
			{
				return;
			}
			this.IsActive = (this._corroding.IsEnabled || this._pocketCorroding.IsEnabled);
		}

		// Token: 0x04002863 RID: 10339
		private Corroding _corroding;

		// Token: 0x04002864 RID: 10340
		private PocketCorroding _pocketCorroding;
	}
}
