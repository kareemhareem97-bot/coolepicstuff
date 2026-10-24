using System;
using MapGeneration;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000AA2 RID: 2722
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class SeedCommand : ICommand
	{
		// Token: 0x17000DF2 RID: 3570
		// (get) Token: 0x06003D57 RID: 15703 RVA: 0x000C98A6 File Offset: 0x000C7AA6
		public string Command { get; } = "seed";

		// Token: 0x17000DF3 RID: 3571
		// (get) Token: 0x06003D58 RID: 15704 RVA: 0x000C98AE File Offset: 0x000C7AAE
		public string[] Aliases { get; }

		// Token: 0x17000DF4 RID: 3572
		// (get) Token: 0x06003D59 RID: 15705 RVA: 0x000C98B6 File Offset: 0x000C7AB6
		public string Description { get; } = "Displays the seed for the current round.";

		// Token: 0x06003D5A RID: 15706 RVA: 0x000C98BE File Offset: 0x000C7ABE
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = string.Format("Map seed is: {0}", SeedSynchronizer.Seed);
			return true;
		}
	}
}
