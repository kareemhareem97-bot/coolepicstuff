using System;
using InventorySystem;
using InventorySystem.Items;
using Mirror;

namespace Hints
{
	// Token: 0x02000809 RID: 2057
	public class AmmoHintParameter : IdHintParameter
	{
		// Token: 0x06002F9B RID: 12187 RVA: 0x000A04BA File Offset: 0x0009E6BA
		public static AmmoHintParameter FromNetwork(NetworkReader reader)
		{
			AmmoHintParameter ammoHintParameter = new AmmoHintParameter();
			ammoHintParameter.Deserialize(reader);
			return ammoHintParameter;
		}

		// Token: 0x06002F9C RID: 12188 RVA: 0x000A04C8 File Offset: 0x0009E6C8
		private AmmoHintParameter()
		{
		}

		// Token: 0x06002F9D RID: 12189 RVA: 0x000A04D0 File Offset: 0x0009E6D0
		public AmmoHintParameter(byte id) : base(id)
		{
		}

		// Token: 0x06002F9E RID: 12190 RVA: 0x000A04DC File Offset: 0x0009E6DC
		protected override string FormatId(float progress, out bool stopFormatting)
		{
			stopFormatting = true;
			ItemType id = (ItemType)base.Id;
			ItemBase itemBase;
			if (InventoryItemLoader.AvailableItems.TryGetValue(id, out itemBase))
			{
				IItemNametag itemNametag = itemBase as IItemNametag;
				if (itemNametag != null)
				{
					return itemNametag.Name;
				}
			}
			return id.ToString();
		}
	}
}
