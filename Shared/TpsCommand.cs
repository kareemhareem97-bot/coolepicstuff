using System;
using UnityEngine;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009C8 RID: 2504
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class TpsCommand : ICommand
	{
		// Token: 0x17000B6A RID: 2922
		// (get) Token: 0x06003894 RID: 14484 RVA: 0x000B8059 File Offset: 0x000B6259
		public string Command
		{
			get
			{
				return "tps";
			}
		}

		// Token: 0x17000B6B RID: 2923
		// (get) Token: 0x06003895 RID: 14485 RVA: 0x000B8060 File Offset: 0x000B6260
		public string[] Aliases
		{
			get
			{
				return Array.Empty<string>();
			}
		}

		// Token: 0x17000B6C RID: 2924
		// (get) Token: 0x06003896 RID: 14486 RVA: 0x000B8067 File Offset: 0x000B6267
		public string Description
		{
			get
			{
				return "Returns information about server current tps / max tps";
			}
		}

		// Token: 0x06003897 RID: 14487 RVA: 0x000B806E File Offset: 0x000B626E
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = string.Format("TPS: {0}/{1}", Math.Round((double)(1f / Time.smoothDeltaTime)), ServerStatic.ServerTickrate);
			return true;
		}
	}
}
