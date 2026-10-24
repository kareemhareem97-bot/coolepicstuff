using System;
using Mirror.LiteNetLib4Mirror;
using RemoteAdmin;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A02 RID: 2562
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class PingCommand : ICommand
	{
		// Token: 0x17000C26 RID: 3110
		// (get) Token: 0x060039E1 RID: 14817 RVA: 0x000BE974 File Offset: 0x000BCB74
		public string Command { get; } = "ping";

		// Token: 0x17000C27 RID: 3111
		// (get) Token: 0x060039E2 RID: 14818 RVA: 0x000BE97C File Offset: 0x000BCB7C
		public string[] Aliases { get; }

		// Token: 0x17000C28 RID: 3112
		// (get) Token: 0x060039E3 RID: 14819 RVA: 0x000BE984 File Offset: 0x000BCB84
		public string Description { get; } = "Pong!";

		// Token: 0x060039E4 RID: 14820 RVA: 0x000BE98C File Offset: 0x000BCB8C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			int count = arguments.Count;
			if (count != 0)
			{
				if (count != 1)
				{
					response = "Too many arguments! (expected 0 or 1)";
					return false;
				}
				if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
				{
					return false;
				}
				int playerId;
				if (!int.TryParse(arguments.At(0), out playerId))
				{
					response = "Invalid player id!";
					return false;
				}
				ReferenceHub referenceHub;
				if (!ReferenceHub.TryGetHub(playerId, out referenceHub))
				{
					response = "Invalid player id!";
					return false;
				}
				int connectionId = referenceHub.networkIdentity.connectionToClient.connectionId;
				if (connectionId == 0)
				{
					response = "This command is not available for the host!";
					return false;
				}
				response = string.Format("Ping: {0}ms", LiteNetLib4MirrorServer.Peers[connectionId].Ping * 2);
				return true;
			}
			else
			{
				PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
				if (playerCommandSender == null)
				{
					response = "This command is only available for players!";
					return false;
				}
				int connectionId2 = playerCommandSender.ReferenceHub.networkIdentity.connectionToClient.connectionId;
				if (connectionId2 == 0)
				{
					response = "This command is not available for the host!";
					return false;
				}
				response = string.Format("Your ping: {0}ms", LiteNetLib4MirrorServer.Peers[connectionId2].Ping * 2);
				return true;
			}
		}
	}
}
