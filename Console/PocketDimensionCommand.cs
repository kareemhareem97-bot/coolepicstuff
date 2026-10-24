using System;
using CustomPlayerEffects;
using Mirror;
using RemoteAdmin;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000AA4 RID: 2724
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class PocketDimensionCommand : ICommand
	{
		// Token: 0x17000DF8 RID: 3576
		// (get) Token: 0x06003D61 RID: 15713 RVA: 0x000C9942 File Offset: 0x000C7B42
		public string Command { get; } = "pocketdimension";

		// Token: 0x17000DF9 RID: 3577
		// (get) Token: 0x06003D62 RID: 15714 RVA: 0x000C994A File Offset: 0x000C7B4A
		public string[] Aliases { get; } = new string[]
		{
			"pocketdim",
			"shadowrealm",
			"pd"
		};

		// Token: 0x17000DFA RID: 3578
		// (get) Token: 0x06003D63 RID: 15715 RVA: 0x000C9952 File Offset: 0x000C7B52
		public string Description { get; } = "Banishes yourself to the pocket dimension.";

		// Token: 0x06003D64 RID: 15716 RVA: 0x000C995C File Offset: 0x000C7B5C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			ReferenceHub referenceHub;
			if (!NetworkServer.active || !ReferenceHub.TryGetHostHub(out referenceHub))
			{
				response = "You are not connected to a local server.";
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			CharacterClassManager characterClassManager = referenceHub.characterClassManager;
			if (characterClassManager == null || !characterClassManager.isLocalPlayer || !characterClassManager.isServer || !characterClassManager.RoundStarted)
			{
				response = "Please start round before using this command.";
				return false;
			}
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender != null)
			{
				ReferenceHub referenceHub2 = playerCommandSender.ReferenceHub;
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " teleported to the Pocket Dimension.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				referenceHub2.playerEffectsController.EnableEffect<PocketCorroding>(0f, false);
				response = "You banished yourself to the Pocket Dimension!";
				return true;
			}
			response = "Only players can run this command.";
			return false;
		}
	}
}
