using System;
using GameCore;
using UnityEngine;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A23 RID: 2595
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class VersionCommand : ICommand
	{
		// Token: 0x17000C99 RID: 3225
		// (get) Token: 0x06003AAD RID: 15021 RVA: 0x000C1C81 File Offset: 0x000BFE81
		public string Command { get; } = "version";

		// Token: 0x17000C9A RID: 3226
		// (get) Token: 0x06003AAE RID: 15022 RVA: 0x000C1C89 File Offset: 0x000BFE89
		public string[] Aliases { get; }

		// Token: 0x17000C9B RID: 3227
		// (get) Token: 0x06003AAF RID: 15023 RVA: 0x000C1C91 File Offset: 0x000BFE91
		public string Description { get; } = "Returns the version of the server.";

		// Token: 0x06003AB0 RID: 15024 RVA: 0x000C1C99 File Offset: 0x000BFE99
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Server Version: " + GameCore.Version.VersionString + " " + Application.buildGUID;
			return true;
		}
	}
}
