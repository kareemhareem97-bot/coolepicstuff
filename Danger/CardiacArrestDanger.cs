using System;

namespace CustomPlayerEffects.Danger
{
	// Token: 0x02000914 RID: 2324
	public class CardiacArrestDanger : DangerStackBase
	{
		// Token: 0x17000A73 RID: 2675
		// (get) Token: 0x06003508 RID: 13576 RVA: 0x000ADB4E File Offset: 0x000ABD4E
		// (set) Token: 0x06003509 RID: 13577 RVA: 0x000ADB56 File Offset: 0x000ABD56
		public override float DangerValue { get; set; } = 2f;

		// Token: 0x0600350A RID: 13578 RVA: 0x000ADB60 File Offset: 0x000ABD60
		public override void Initialize(ReferenceHub target)
		{
			base.Initialize(target);
			if (!base.Owner.playerEffectsController.TryGetEffect<CardiacArrest>(out this._cardiacArrest))
			{
				throw new NullReferenceException("Cardiac arrest wasn't found. This DangerOverride will not function as intended.");
			}
			this.IsActive = this._cardiacArrest.IsEnabled;
			StatusEffectBase.OnEnabled += this.UpdateState;
			StatusEffectBase.OnDisabled += this.UpdateState;
		}

		// Token: 0x0600350B RID: 13579 RVA: 0x000ADBCA File Offset: 0x000ABDCA
		public override void Dispose()
		{
			base.Dispose();
			StatusEffectBase.OnEnabled -= this.UpdateState;
			StatusEffectBase.OnDisabled -= this.UpdateState;
		}

		// Token: 0x0600350C RID: 13580 RVA: 0x000ADBF4 File Offset: 0x000ABDF4
		private void UpdateState(StatusEffectBase effect)
		{
			if (effect != this._cardiacArrest)
			{
				return;
			}
			this.IsActive = effect.IsEnabled;
		}

		// Token: 0x0400285F RID: 10335
		private CardiacArrest _cardiacArrest;
	}
}
