using System;
using Mirror;

namespace Hints
{
	// Token: 0x0200080E RID: 2062
	public class ItemCategoryHintParameter : IdHintParameter
	{
		// Token: 0x06002FAF RID: 12207 RVA: 0x000A05B5 File Offset: 0x0009E7B5
		public static ItemCategoryHintParameter FromNetwork(NetworkReader reader)
		{
			ItemCategoryHintParameter itemCategoryHintParameter = new ItemCategoryHintParameter();
			itemCategoryHintParameter.Deserialize(reader);
			return itemCategoryHintParameter;
		}

		// Token: 0x06002FB0 RID: 12208 RVA: 0x000A04C8 File Offset: 0x0009E6C8
		private ItemCategoryHintParameter()
		{
		}

		// Token: 0x06002FB1 RID: 12209 RVA: 0x000A05C3 File Offset: 0x0009E7C3
		public ItemCategoryHintParameter(ItemCategory category)
		{
			if (category == ItemCategory.None)
			{
				throw new ArgumentException("Item category cannot be none (no proper translation).", "category");
			}
			base..ctor((byte)category);
		}

		// Token: 0x06002FB2 RID: 12210 RVA: 0x000A05E0 File Offset: 0x0009E7E0
		protected override string FormatId(float progress, out bool stopFormatting)
		{
			stopFormatting = true;
			return TranslationReader.Get("Categories", base.Id, ((ItemCategory)base.Id).ToString());
		}
	}
}
