using System;
using System.Collections.Generic;
using System.Text;
using Interactables.Interobjects.DoorUtils;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	// Token: 0x02000A66 RID: 2662
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class DoorTPCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D56 RID: 3414
		// (get) Token: 0x06003C27 RID: 15399 RVA: 0x000C6941 File Offset: 0x000C4B41
		public string Command { get; } = "doortp";

		// Token: 0x17000D57 RID: 3415
		// (get) Token: 0x06003C28 RID: 15400 RVA: 0x000C6949 File Offset: 0x000C4B49
		public string[] Aliases { get; } = new string[]
		{
			"dtp",
			"doorteleport"
		};

		// Token: 0x17000D58 RID: 3416
		// (get) Token: 0x06003C29 RID: 15401 RVA: 0x000C6951 File Offset: 0x000C4B51
		public string Description { get; } = "Teleports player(s) to the specified door.";

		// Token: 0x17000D59 RID: 3417
		// (get) Token: 0x06003C2A RID: 15402 RVA: 0x000C6959 File Offset: 0x000C4B59
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"%door%"
		};

		// Token: 0x06003C2B RID: 15403 RVA: 0x000C6964 File Offset: 0x000C4B64
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 2)
			{
				response = string.Concat(new string[]
				{
					"To execute this command provide at least 2 arguments!\nUsage: ",
					arguments.Array[0],
					" ",
					this.DisplayCommandUsage(),
					" [Door name]"
				});
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			if (array == null || string.IsNullOrEmpty(array[0]))
			{
				response = "Invalid door name provided.";
				return false;
			}
			string text = array[0].Split('.', StringSplitOptions.None)[0].ToUpper();
			DoorNametagExtension doorNametagExtension;
			if (!DoorNametagExtension.NamedDoors.TryGetValue(text, out doorNametagExtension))
			{
				response = "Can't find door " + text + ".";
				return false;
			}
			Vector3 position = DoorTPCommand.EnsurePositionSafety(doorNametagExtension.transform);
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			int num = 0;
			foreach (ReferenceHub referenceHub in list)
			{
				if (referenceHub.TryOverridePosition(position))
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
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} teleported player{1}{2} to door {3}.", new object[]
				{
					sender.LogName,
					(num == 1) ? " " : "s ",
					stringBuilder,
					array[0].TrimEnd('.')
				}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			}
			StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}

		// Token: 0x06003C2C RID: 15404 RVA: 0x000C6B28 File Offset: 0x000C4D28
		public static Vector3 EnsurePositionSafety(Transform doorTransform)
		{
			Vector3 vector = doorTransform.position + Vector3.up;
			while (Physics.CheckSphere(vector, 0.35f, FpcStateProcessor.Mask))
			{
				vector += doorTransform.forward * 0.35f;
			}
			return vector;
		}

		// Token: 0x04002C3C RID: 11324
		private const float WallDetectionRadius = 0.35f;
	}
}
