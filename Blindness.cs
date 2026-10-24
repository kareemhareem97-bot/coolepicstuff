using System;
using RemoteAdmin.Interfaces;

namespace CustomPlayerEffects
{
	// Token: 0x0200089F RID: 2207
	public class Blindness : StatusEffectBase, ICustomDisplayName, ICustomHealableEffect, IHealableEffect, IConflictableEffect
	{
		// Token: 0x1700094E RID: 2382
		// (get) Token: 0x06003225 RID: 12837 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Negative;
			}
		}

		// Token: 0x1700094F RID: 2383
		// (get) Token: 0x06003226 RID: 12838 RVA: 0x000A84BF File Offset: 0x000A66BF
		public override bool AllowEnabling
		{
			get
			{
				return !SpawnProtected.CheckPlayer(base.Hub);
			}
		}

		// Token: 0x17000950 RID: 2384
		// (get) Token: 0x06003227 RID: 12839 RVA: 0x000A84CF File Offset: 0x000A66CF
		public string DisplayName
		{
			get
			{
				return "Blindness";
			}
		}

		// Token: 0x17000951 RID: 2385
		// (get) Token: 0x06003228 RID: 12840 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool CanBeDisplayed
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003229 RID: 12841 RVA: 0x000A84D8 File Offset: 0x000A66D8
		public bool IsHealable(ItemType item)
		{
			if (item == ItemType.SCP500)
			{
				byte intensity = base.Intensity;
				return intensity <= 100 && intensity > 15;
			}
			return false;
		}

		// Token: 0x0600322A RID: 12842 RVA: 0x000A84FF File Offset: 0x000A66FF
		public void OnHeal(ItemType item)
		{
			base.Intensity = 15;
		}

		// Token: 0x0600322B RID: 12843 RVA: 0x000A8509 File Offset: 0x000A6709
		public bool CheckConflicts(StatusEffectBase other)
		{
			if (!(other is Flashed))
			{
				return false;
			}
			other.ServerDisable();
			return true;
		}

		// Token: 0x04002727 RID: 10023
		private const float LerpingSpeed = 10f;

		// Token: 0x04002728 RID: 10024
		private const byte MaxHealableIntensity = 100;

		// Token: 0x04002729 RID: 10025
		private const byte MinIntensity = 15;
	}
}
