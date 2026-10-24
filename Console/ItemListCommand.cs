using System;
using System.Collections.Generic;
using System.Text;
using InventorySystem;
using InventorySystem.Items;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A9C RID: 2716
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class ItemListCommand : ICommand
	{
		// Token: 0x17000DE0 RID: 3552
		// (get) Token: 0x06003D39 RID: 15673 RVA: 0x000C9516 File Offset: 0x000C7716
		public string Command { get; } = "itemlist";

		// Token: 0x17000DE1 RID: 3553
		// (get) Token: 0x06003D3A RID: 15674 RVA: 0x000C951E File Offset: 0x000C771E
		public string[] Aliases { get; }

		// Token: 0x17000DE2 RID: 3554
		// (get) Token: 0x06003D3B RID: 15675 RVA: 0x000C9526 File Offset: 0x000C7726
		public string Description { get; } = "Displays a list of items.";

		// Token: 0x06003D3C RID: 15676 RVA: 0x000C9530 File Offset: 0x000C7730
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<size=25>Item List:</size>");
			foreach (KeyValuePair<ItemType, ItemBase> keyValuePair in InventoryItemLoader.AvailableItems)
			{
				stringBuilder.AppendLine(string.Format("ITEM#{0:000} : {1}", (int)keyValuePair.Key, keyValuePair.Key));
			}
			response = stringBuilder.ToString();
			return true;
		}
	}
}
