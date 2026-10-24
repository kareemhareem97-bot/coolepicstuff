using System;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;

namespace Christmas.Scp2536
{
	// Token: 0x020010D7 RID: 4311
	public static class Scp2536Utils
	{
		// Token: 0x06006002 RID: 24578 RVA: 0x00141090 File Offset: 0x0013F290
		public static void GrantAmmoReward(this ItemBase reward)
		{
			if (!(reward.Owner == null))
			{
				Firearm firearm = reward as Firearm;
				if (firearm != null)
				{
					IPrimaryAmmoContainerModule primaryAmmoContainerModule;
					if (!firearm.TryGetModule(out primaryAmmoContainerModule, true))
					{
						return;
					}
					reward.OwnerInventory.ServerAddAmmo(primaryAmmoContainerModule.AmmoType, primaryAmmoContainerModule.AmmoMax);
					return;
				}
			}
		}
	}
}
