using System;
using System.Collections.Generic;
using CustomPlayerEffects;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009DC RID: 2524
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ClearEffectsCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BA6 RID: 2982
		// (get) Token: 0x060038FC RID: 14588 RVA: 0x000B9D25 File Offset: 0x000B7F25
		public string Command { get; } = "cleareffects";

		// Token: 0x17000BA7 RID: 2983
		// (get) Token: 0x060038FD RID: 14589 RVA: 0x000B9D2D File Offset: 0x000B7F2D
		public string[] Aliases { get; } = new string[]
		{
			"cfx",
			"clearfx"
		};

		// Token: 0x17000BA8 RID: 2984
		// (get) Token: 0x060038FE RID: 14590 RVA: 0x000B9D35 File Offset: 0x000B7F35
		public string Description { get; } = "Clears all status effects from the specified player(s).";

		// Token: 0x17000BA9 RID: 2985
		// (get) Token: 0x060038FF RID: 14591 RVA: 0x000B9D3D File Offset: 0x000B7F3D
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x06003900 RID: 14592 RVA: 0x000B9D48 File Offset: 0x000B7F48
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Effects, out response))
			{
				return false;
			}
			if (arguments.Count == 0)
			{
				response = "To execute this command provide at least 1 arguments!\nUsage: " + this.Command + " " + string.Join(" ", this.Usage);
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			int num = 0;
			foreach (ReferenceHub referenceHub in list)
			{
				if (!(referenceHub == null))
				{
					StatusEffectBase[] allEffects = referenceHub.playerEffectsController.AllEffects;
					for (int i = 0; i < allEffects.Length; i++)
					{
						allEffects[i].Intensity = 0;
					}
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " clear all effects for player " + referenceHub.LoggedNameFromRefHub() + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					num++;
				}
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}
	}
}
