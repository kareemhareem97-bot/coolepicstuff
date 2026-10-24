using System;
using AudioPooling;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009A8 RID: 2472
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	[CommandHandler(typeof(ClientCommandHandler))]
	public class AudioPoolDebug : ICommand, IUsageProvider
	{
		// Token: 0x17000B09 RID: 2825
		// (get) Token: 0x060037EB RID: 14315 RVA: 0x000B5D2E File Offset: 0x000B3F2E
		public string Command { get; } = "audiopooldebug";

		// Token: 0x17000B0A RID: 2826
		// (get) Token: 0x060037EC RID: 14316 RVA: 0x000B5D36 File Offset: 0x000B3F36
		public string[] Aliases { get; } = new string[]
		{
			"audiopoolingdebug"
		};

		// Token: 0x17000B0B RID: 2827
		// (get) Token: 0x060037ED RID: 14317 RVA: 0x000B5D3E File Offset: 0x000B3F3E
		public string Description { get; } = "Prints debug information about Audio Pooling system.";

		// Token: 0x17000B0C RID: 2828
		// (get) Token: 0x060037EE RID: 14318 RVA: 0x000B5D46 File Offset: 0x000B3F46
		public string[] Usage { get; } = new string[]
		{
			"Force refresh (bool, Optional)"
		};

		// Token: 0x060037EF RID: 14319 RVA: 0x000B5D50 File Offset: 0x000B3F50
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			bool flag;
			bool forceUpdate = arguments.Count > 0 && bool.TryParse(arguments.At(0), out flag) && flag;
			response = AudioSourcePoolManager.ProcessConsoleCommand(forceUpdate);
			return true;
		}
	}
}
