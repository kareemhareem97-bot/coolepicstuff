using System;
using InventorySystem;
using InventorySystem.Items;
using Mirror;

namespace Hints
{
	// Token: 0x02000810 RID: 2064
	public class ItemHintParameter : IdHintParameter
	{
		// Token: 0x06002FB5 RID: 12213 RVA: 0x000A061D File Offset: 0x0009E81D
		public static ItemHintParameter FromNetwork(NetworkReader reader)
		{
			ItemHintParameter itemHintParameter = new ItemHintParameter();
			itemHintParameter.Deserialize(reader);
			return itemHintParameter;
		}

		// Token: 0x06002FB6 RID: 12214 RVA: 0x000A04C8 File Offset: 0x0009E6C8
		private ItemHintParameter()
		{
		}

		// Token: 0x06002FB7 RID: 12215 RVA: 0x000A062B File Offset: 0x0009E82B
		public ItemHintParameter(ItemType item) : base((byte)item)
		{
			if (item == ItemType.None)
			{
				throw new ArgumentException("Item cannot be none (no proper translation).", "item");
			}
		}

		// Token: 0x06002FB8 RID: 12216 RVA: 0x000A064C File Offset: 0x0009E84C
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
