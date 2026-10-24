using System;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;

namespace CustomPlayerEffects
{
	// Token: 0x020008B7 RID: 2231
	public class Fade : StatusEffectBase
	{
		// Token: 0x17000992 RID: 2450
		// (get) Token: 0x060032BB RID: 12987 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x060032BC RID: 12988 RVA: 0x000A92B7 File Offset: 0x000A74B7
		protected override void Disabled()
		{
			base.Disabled();
			this.UpdateFade();
		}

		// Token: 0x060032BD RID: 12989 RVA: 0x000A92C5 File Offset: 0x000A74C5
		protected void LateUpdate()
		{
			if (!base.IsEnabled)
			{
				return;
			}
			this.UpdateFade();
		}

		// Token: 0x060032BE RID: 12990 RVA: 0x000A92D8 File Offset: 0x000A74D8
		private void UpdateFade()
		{
			if (base.IsLocalPlayer)
			{
				return;
			}
			IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole == null)
			{
				return;
			}
			CharacterModel characterModelInstance = fpcRole.FpcModule.CharacterModelInstance;
			if (characterModelInstance == null)
			{
				return;
			}
			characterModelInstance.Fade = 1f - (float)base.Intensity / 255f;
		}
	}
}
