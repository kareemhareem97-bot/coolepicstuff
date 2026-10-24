using System;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A9F RID: 2719
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class ReloadTranslationsCommand : ICommand
	{
		// Token: 0x17000DE9 RID: 3561
		// (get) Token: 0x06003D48 RID: 15688 RVA: 0x000C968F File Offset: 0x000C788F
		public string Command { get; } = "reloadtranslations";

		// Token: 0x17000DEA RID: 3562
		// (get) Token: 0x06003D49 RID: 15689 RVA: 0x000C9697 File Offset: 0x000C7897
		public string[] Aliases { get; }

		// Token: 0x17000DEB RID: 3563
		// (get) Token: 0x06003D4A RID: 15690 RVA: 0x000C969F File Offset: 0x000C789F
		public string Description { get; } = "Reloads game translations";

		// Token: 0x06003D4B RID: 15691 RVA: 0x000C96A7 File Offset: 0x000C78A7
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			TranslationReader.Refresh();
			response = "Translations have been reloaded! Some elements may require full level reload.";
			return true;
		}
	}
}
