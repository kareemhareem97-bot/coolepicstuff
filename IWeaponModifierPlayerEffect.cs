using System;
using InventorySystem.Items.Firearms.Attachments;

namespace CustomPlayerEffects
{
	// Token: 0x02000904 RID: 2308
	public interface IWeaponModifierPlayerEffect
	{
		// Token: 0x0600349A RID: 13466
		bool TryGetWeaponParam(AttachmentParam param, out float val);

		// Token: 0x17000A52 RID: 2642
		// (get) Token: 0x0600349B RID: 13467
		bool ParamsActive { get; }
	}
}
