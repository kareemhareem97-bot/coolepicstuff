using System;
using System.IO;
using Mirror;
using UnityEngine;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009AB RID: 2475
	[CommandHandler(typeof(ConfigCommand))]
	public class OpenCommand : ICommand
	{
		// Token: 0x17000B15 RID: 2837
		// (get) Token: 0x060037FF RID: 14335 RVA: 0x000B60C1 File Offset: 0x000B42C1
		public string Command { get; } = "open";

		// Token: 0x17000B16 RID: 2838
		// (get) Token: 0x06003800 RID: 14336 RVA: 0x000B60C9 File Offset: 0x000B42C9
		public string[] Aliases { get; } = new string[]
		{
			"o",
			"op"
		};

		// Token: 0x17000B17 RID: 2839
		// (get) Token: 0x06003801 RID: 14337 RVA: 0x000B60D1 File Offset: 0x000B42D1
		public string Description { get; } = "Opens the games config";

		// Token: 0x06003802 RID: 14338 RVA: 0x000B60DC File Offset: 0x000B42DC
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is ServerConsoleSender))
			{
				response = "This command can only be executed from the server or client console.";
				return false;
			}
			if (!NetworkServer.active)
			{
				response = "This command can only be used on a server.";
				return false;
			}
			if (File.Exists(FileManager.GetAppFolder(true, true, "") + "config_gameplay.txt"))
			{
				Application.OpenURL(FileManager.GetAppFolder(true, true, "") + "config_gameplay.txt");
				response = "Config file opened.";
				return true;
			}
			response = "Config file not found!";
			return false;
		}
	}
}
