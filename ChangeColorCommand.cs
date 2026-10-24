using System;
using MapGeneration;
using PlayerRoles;
using RemoteAdmin;
using UnityEngine;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009D9 RID: 2521
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ChangeColorCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000B9A RID: 2970
		// (get) Token: 0x060038EA RID: 14570 RVA: 0x000B9713 File Offset: 0x000B7913
		public string Command { get; } = "changecolor";

		// Token: 0x17000B9B RID: 2971
		// (get) Token: 0x060038EB RID: 14571 RVA: 0x000B971B File Offset: 0x000B791B
		public string[] Aliases { get; } = new string[]
		{
			"changec",
			"ccolor"
		};

		// Token: 0x17000B9C RID: 2972
		// (get) Token: 0x060038EC RID: 14572 RVA: 0x000B9723 File Offset: 0x000B7923
		public string Description { get; } = "Changes the color of the lights in the room you are currently in.";

		// Token: 0x17000B9D RID: 2973
		// (get) Token: 0x060038ED RID: 14573 RVA: 0x000B972B File Offset: 0x000B792B
		public string[] Usage { get; } = new string[]
		{
			"r",
			"g",
			"b"
		};

		// Token: 0x060038EE RID: 14574 RVA: 0x000B9734 File Offset: 0x000B7934
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null)
			{
				response = "You must be in-game to use this command!";
				return false;
			}
			if (!playerCommandSender.ReferenceHub.IsAlive())
			{
				response = "You need to be alive to run this command!";
				return false;
			}
			RoomIdentifier roomIdentifier;
			if (!playerCommandSender.ReferenceHub.TryGetCurrentRoom(out roomIdentifier))
			{
				response = "You are not in a a room!";
				return false;
			}
			RoomLightController closestLightController = roomIdentifier.GetClosestLightController(playerCommandSender.ReferenceHub);
			if (closestLightController == null)
			{
				response = "You are not in a room that supports changing lights color!";
				return false;
			}
			if (arguments.Count == 0)
			{
				closestLightController.NetworkOverrideColor = Color.clear;
				response = "Done! Reset warhead lights to default color.";
				return true;
			}
			if (arguments.Count < 3)
			{
				response = "Type 3 numbers, eg: 255 255 255";
				return false;
			}
			float num;
			float num2;
			float num3;
			if (!float.TryParse(arguments.At(0), out num) || !float.TryParse(arguments.At(1), out num2) || !float.TryParse(arguments.At(2), out num3))
			{
				response = "Invalid input. Type 3 numbers, eg: 255 255 255";
				return false;
			}
			closestLightController.NetworkOverrideColor = new Color(num / 255f, num2 / 255f, num3 / 255f);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} changed color of lights in a room to {1} {2} {3} .", new object[]
			{
				sender.LogName,
				num,
				num2,
				num3
			}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = "Done!";
			return true;
		}
	}
}
