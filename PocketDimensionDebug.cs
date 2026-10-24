using System;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A03 RID: 2563
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class PocketDimensionDebug : ICommand
	{
		// Token: 0x17000C29 RID: 3113
		// (get) Token: 0x060039E6 RID: 14822 RVA: 0x000BEAB0 File Offset: 0x000BCCB0
		public string Command { get; } = "pddebug";

		// Token: 0x17000C2A RID: 3114
		// (get) Token: 0x060039E7 RID: 14823 RVA: 0x000BEAB8 File Offset: 0x000BCCB8
		public string[] Aliases { get; } = new string[]
		{
			"106debug"
		};

		// Token: 0x17000C2B RID: 3115
		// (get) Token: 0x060039E8 RID: 14824 RVA: 0x000BEAC0 File Offset: 0x000BCCC0
		public string Description { get; } = "If true SCP-106s Pocket Dimension has 100% chance for you to escape.";

		// Token: 0x17000C2C RID: 3116
		// (get) Token: 0x060039E9 RID: 14825 RVA: 0x000BEAC8 File Offset: 0x000BCCC8
		public string[] Usage { get; } = new string[]
		{
			"enable/disable (Leave blank for toggle)"
		};

		// Token: 0x060039EA RID: 14826 RVA: 0x000BEAD0 File Offset: 0x000BCCD0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			if (arguments.Count >= 1)
			{
				string[] array = new string[]
				{
					arguments.Array[1]
				};
				Misc.CommandOperationMode commandOperationMode;
				if (Misc.TryCommandModeFromArgs(ref array, out commandOperationMode))
				{
					switch (commandOperationMode)
					{
					case Misc.CommandOperationMode.Disable:
						PocketDimensionTeleport.DebugBool = false;
						break;
					case Misc.CommandOperationMode.Enable:
						PocketDimensionTeleport.DebugBool = true;
						break;
					case Misc.CommandOperationMode.Toggle:
						PocketDimensionTeleport.DebugBool = !PocketDimensionTeleport.DebugBool;
						break;
					}
					response = "Pocket Dimension debug set to " + (PocketDimensionTeleport.DebugBool ? "enabled!" : "disabled!");
					return true;
				}
			}
			PocketDimensionTeleport.DebugBool = !PocketDimensionTeleport.DebugBool;
			response = "Pocket Dimension debug is now " + (PocketDimensionTeleport.DebugBool ? "enabled!" : "disabled!");
			return true;
		}
	}
}
