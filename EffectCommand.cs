using System;
using System.Collections.Generic;
using CustomPlayerEffects;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009E2 RID: 2530
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class EffectCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BBA RID: 3002
		// (get) Token: 0x0600391D RID: 14621 RVA: 0x000BA516 File Offset: 0x000B8716
		public string Command { get; } = "effect";

		// Token: 0x17000BBB RID: 3003
		// (get) Token: 0x0600391E RID: 14622 RVA: 0x000BA51E File Offset: 0x000B871E
		public string[] Aliases { get; } = new string[]
		{
			"pfx"
		};

		// Token: 0x17000BBC RID: 3004
		// (get) Token: 0x0600391F RID: 14623 RVA: 0x000BA526 File Offset: 0x000B8726
		public string Description { get; } = "Applies a status effect to player(s).";

		// Token: 0x17000BBD RID: 3005
		// (get) Token: 0x06003920 RID: 14624 RVA: 0x000BA52E File Offset: 0x000B872E
		public string[] Usage { get; } = new string[]
		{
			"EffectName",
			"Intensity",
			"Duration",
			"%player%"
		};

		// Token: 0x06003921 RID: 14625 RVA: 0x000BA538 File Offset: 0x000B8738
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Effects, out response))
			{
				return false;
			}
			if (arguments.Count < 4)
			{
				response = "To execute this command provide at least 4 arguments!\nUsage: " + this.Command + " " + this.DisplayCommandUsage();
				return false;
			}
			string text = arguments.At(0);
			byte b;
			if (!byte.TryParse(arguments.At(1), out b))
			{
				response = string.Concat(new string[]
				{
					"Effect intensity must be a byte value between 0-255.\nUsage: ",
					this.Command,
					" ",
					this.DisplayCommandUsage(),
					"'"
				});
				return false;
			}
			float num;
			if (!float.TryParse(arguments.At(2), out num))
			{
				response = string.Concat(new string[]
				{
					"Effect duration must be a valid float value.\nUsage: ",
					this.Command,
					" ",
					this.DisplayCommandUsage(),
					"'"
				});
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 3, out array, false);
			if (list == null || list.Count == 0)
			{
				response = "Couldn't find any player(s) using the specified arguments.";
				return false;
			}
			int num2 = 0;
			foreach (ReferenceHub referenceHub in list)
			{
				StatusEffectBase statusEffectBase;
				if (!(referenceHub == null) && referenceHub.playerEffectsController.TryGetEffect(text, out statusEffectBase))
				{
					statusEffectBase.ServerSetState(b, num, false);
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} applied a status effect {1} for player {2}. Intensity: {3} - Duration: {4}.", new object[]
					{
						sender.LogName,
						text,
						referenceHub.LoggedNameFromRefHub(),
						b,
						num
					}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					num2++;
				}
			}
			response = string.Format("Done! The request affected {0} player{1}", num2, (num2 == 1) ? "!" : "s!");
			return true;
		}
	}
}
