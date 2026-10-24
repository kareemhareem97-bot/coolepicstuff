using System;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A12 RID: 2578
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class RoundLockCommand : ICommand
	{
		// Token: 0x17000C56 RID: 3158
		// (get) Token: 0x06003A42 RID: 14914 RVA: 0x000C0355 File Offset: 0x000BE555
		public string Command { get; } = "roundlock";

		// Token: 0x17000C57 RID: 3159
		// (get) Token: 0x06003A43 RID: 14915 RVA: 0x000C035D File Offset: 0x000BE55D
		public string[] Aliases { get; } = new string[]
		{
			"rl",
			"rlock"
		};

		// Token: 0x17000C58 RID: 3160
		// (get) Token: 0x06003A44 RID: 14916 RVA: 0x000C0365 File Offset: 0x000BE565
		public string Description { get; } = "Locks or unlocks the current round (prevents it from ending).";

		// Token: 0x17000C59 RID: 3161
		// (get) Token: 0x06003A45 RID: 14917 RVA: 0x000C036D File Offset: 0x000BE56D
		public string[] Usage { get; } = new string[]
		{
			"enable/disable (Leave blank for toggle)"
		};

		// Token: 0x06003A46 RID: 14918 RVA: 0x000C0378 File Offset: 0x000BE578
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			bool flag = !RoundSummary.RoundLock;
			if (arguments.Count >= 1)
			{
				string[] array = new string[]
				{
					arguments.Array[1]
				};
				Misc.CommandOperationMode commandOperationMode;
				if (Misc.TryCommandModeFromArgs(ref array, out commandOperationMode))
				{
					bool flag2 = commandOperationMode != Misc.CommandOperationMode.Disable && (commandOperationMode == Misc.CommandOperationMode.Enable || flag);
					flag = flag2;
				}
			}
			RoundSummary.RoundLock = flag;
			if (flag)
			{
				RoundSummary.singleton.CancelRoundEnding();
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + (RoundSummary.RoundLock ? " enabled " : " disabled ") + "round lock.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = "Round lock " + (RoundSummary.RoundLock ? "enabled!" : "disabled!");
			return true;
		}
	}
}
