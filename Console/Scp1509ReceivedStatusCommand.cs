using System;
using System.Collections.Generic;
using System.Text;
using InventorySystem.Items.Scp1509;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000AA1 RID: 2721
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class Scp1509ReceivedStatusCommand : ICommand, IHiddenCommand
	{
		// Token: 0x17000DEF RID: 3567
		// (get) Token: 0x06003D52 RID: 15698 RVA: 0x000C97D2 File Offset: 0x000C79D2
		public string Command { get; } = "1509";

		// Token: 0x17000DF0 RID: 3568
		// (get) Token: 0x06003D53 RID: 15699 RVA: 0x000C97DA File Offset: 0x000C79DA
		public string[] Aliases { get; }

		// Token: 0x17000DF1 RID: 3569
		// (get) Token: 0x06003D54 RID: 15700 RVA: 0x000C97E2 File Offset: 0x000C79E2
		public string Description { get; } = "poggers";

		// Token: 0x06003D55 RID: 15701 RVA: 0x000C97EC File Offset: 0x000C79EC
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[1509] Start");
			foreach (KeyValuePair<ushort, bool> keyValuePair in Scp1509Item.ResurrectStatuses)
			{
				stringBuilder.AppendLine(string.Format("{0}:{1}", keyValuePair.Key, keyValuePair.Value));
			}
			stringBuilder.AppendLine("[1509] End");
			response = stringBuilder.ToString();
			return true;
		}
	}
}
