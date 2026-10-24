using System;
using System.Collections.Generic;
using RemoteAdmin;
using UnityEngine;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin.Dummies
{
	// Token: 0x02000A5D RID: 2653
	[CommandHandler(typeof(DummiesCommand))]
	public class FollowDummyCommand : ICommand
	{
		// Token: 0x17000D41 RID: 3393
		// (get) Token: 0x06003BFB RID: 15355 RVA: 0x000C6039 File Offset: 0x000C4239
		public string Command { get; } = "follow";

		// Token: 0x17000D42 RID: 3394
		// (get) Token: 0x06003BFC RID: 15356 RVA: 0x000C6041 File Offset: 0x000C4241
		public string[] Aliases { get; } = new string[]
		{
			"f",
			"trail"
		};

		// Token: 0x17000D43 RID: 3395
		// (get) Token: 0x06003BFD RID: 15357 RVA: 0x000C6049 File Offset: 0x000C4249
		public string Description { get; } = "Makes a dummy un/follow you.";

		// Token: 0x06003BFE RID: 15358 RVA: 0x000C6054 File Offset: 0x000C4254
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null)
			{
				response = "This command can only be executed by a physical player.";
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "You must specify a dummy to follow.";
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			if (list == null)
			{
				response = "An unexpected problem has occurred during PlayerId or name array processing.";
				return false;
			}
			int num = 0;
			foreach (ReferenceHub referenceHub in list)
			{
				if (!(referenceHub == null) && referenceHub.IsDummy)
				{
					PlayerFollower obj;
					if (referenceHub.TryGetComponent<PlayerFollower>(out obj))
					{
						UnityEngine.Object.Destroy(obj);
					}
					else
					{
						referenceHub.gameObject.AddComponent<PlayerFollower>().Init(playerCommandSender.ReferenceHub, 20f, 1.75f, 30f);
					}
					num++;
				}
			}
			response = string.Format("Done! The request affected {0} dumm{1}", num, (num == 1) ? "y!" : "ies!");
			return true;
		}
	}
}
