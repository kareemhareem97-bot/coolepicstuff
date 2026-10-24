using System;
using System.Collections.Generic;
using System.Text;
using CentralAuth;
using Mirror;
using NorthwoodLib.Pools;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009BB RID: 2491
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class PlayersCommand : ICommand
	{
		// Token: 0x17000B44 RID: 2884
		// (get) Token: 0x0600384F RID: 14415 RVA: 0x000B7096 File Offset: 0x000B5296
		public string Command { get; } = "players";

		// Token: 0x17000B45 RID: 2885
		// (get) Token: 0x06003850 RID: 14416 RVA: 0x000B709E File Offset: 0x000B529E
		public string[] Aliases { get; } = new string[]
		{
			"pl",
			"list"
		};

		// Token: 0x17000B46 RID: 2886
		// (get) Token: 0x06003851 RID: 14417 RVA: 0x000B70A6 File Offset: 0x000B52A6
		public string Description { get; } = "Displays a list of all players.";

		// Token: 0x06003852 RID: 14418 RVA: 0x000B70B0 File Offset: 0x000B52B0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (NetworkServer.active && !sender.CheckPermission(PlayerPermissions.PlayerSensitiveDataAccess, out response))
			{
				return false;
			}
			if (ReferenceHub.LocalHub == null)
			{
				response = "You must join a server to execute this command.";
				return false;
			}
			HashSet<ReferenceHub> allHubs = ReferenceHub.AllHubs;
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			bool result;
			try
			{
				stringBuilder.AppendFormat("<color=cyan>List of players ({0}):</color>\n", ServerStatic.IsDedicated ? (allHubs.Count - 1) : allHubs.Count);
				foreach (ReferenceHub referenceHub in allHubs)
				{
					ClientInstanceMode mode = referenceHub.Mode;
					if (mode != ClientInstanceMode.DedicatedServer && mode != ClientInstanceMode.Dummy)
					{
						string text = referenceHub.authManager.UserId;
						if (text == null)
						{
							text = "(no UserID)";
						}
						stringBuilder.AppendFormat("- {0}: {1} [{2}]\n", referenceHub.nicknameSync.CombinedName ?? "(no nickname)", text, referenceHub.PlayerId);
					}
				}
				response = stringBuilder.ToString();
				result = true;
			}
			finally
			{
				StringBuilderPool.Shared.Return(stringBuilder);
			}
			return result;
		}
	}
}
