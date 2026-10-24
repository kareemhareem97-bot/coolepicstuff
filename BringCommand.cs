using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using RemoteAdmin;
using UnityEngine;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009D1 RID: 2513
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class BringCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000B83 RID: 2947
		// (get) Token: 0x060038C1 RID: 14529 RVA: 0x000B8CE5 File Offset: 0x000B6EE5
		public string Command { get; } = "bring";

		// Token: 0x17000B84 RID: 2948
		// (get) Token: 0x060038C2 RID: 14530 RVA: 0x000B8CED File Offset: 0x000B6EED
		public string[] Aliases { get; }

		// Token: 0x17000B85 RID: 2949
		// (get) Token: 0x060038C3 RID: 14531 RVA: 0x000B8CF5 File Offset: 0x000B6EF5
		public string Description { get; } = "Brings the specified player(s) to your position.";

		// Token: 0x17000B86 RID: 2950
		// (get) Token: 0x060038C4 RID: 14532 RVA: 0x000B8CFD File Offset: 0x000B6EFD
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x060038C5 RID: 14533 RVA: 0x000B8D08 File Offset: 0x000B6F08
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null)
			{
				response = "You must be in-game to use this command!";
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			if (!playerCommandSender.ReferenceHub.IsAlive())
			{
				response = "Command disabled when you are not alive!";
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			Vector3 position = playerCommandSender.ReferenceHub.transform.position;
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			int num = 0;
			foreach (ReferenceHub referenceHub in list)
			{
				if (referenceHub.IsAlive() && referenceHub.TryOverridePosition(position))
				{
					if (num != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
					num++;
				}
			}
			if (num > 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} brought player{1}{2}.", sender.LogName, (num == 1) ? " " : "s ", stringBuilder), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			}
			StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
