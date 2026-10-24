using System;
using System.Collections.Generic;
using MEC;
using RoundRestarting;
using ServerOutput;
using Utils.Networking;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009BD RID: 2493
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class RedirectCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000B4A RID: 2890
		// (get) Token: 0x06003859 RID: 14425 RVA: 0x000B731B File Offset: 0x000B551B
		public string Command { get; } = "redirect";

		// Token: 0x17000B4B RID: 2891
		// (get) Token: 0x0600385A RID: 14426 RVA: 0x000B7323 File Offset: 0x000B5523
		public string[] Aliases { get; } = new string[]
		{
			"rstop",
			"rexit",
			"rquit"
		};

		// Token: 0x17000B4C RID: 2892
		// (get) Token: 0x0600385B RID: 14427 RVA: 0x000B732B File Offset: 0x000B552B
		public string Description { get; } = "Shutdowns the server and redirects all the players to a server on another port.";

		// Token: 0x17000B4D RID: 2893
		// (get) Token: 0x0600385C RID: 14428 RVA: 0x000B7333 File Offset: 0x000B5533
		public string[] Usage { get; } = new string[]
		{
			"port number"
		};

		// Token: 0x0600385D RID: 14429 RVA: 0x000B733C File Offset: 0x000B553C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			ushort num;
			if (arguments.Count != 1 || !ushort.TryParse(arguments.At(0), out num))
			{
				response = "First argument must be a valid port number.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} redirected all players to port {1}.", sender.LogName, num), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			CustomLiteNetLib4MirrorTransport.DelayConnections = true;
			IdleMode.SetIdleMode(false);
			IdleMode.PauseIdleMode = true;
			ServerConsole.AddOutputEntry(default(ExitActionShutdownEntry));
			new RoundRestartMessage(RoundRestartType.RedirectRestart, 0.1f, num, true, false).SendToAuthenticated(0);
			Timing.RunCoroutine(this.ScheduleShutdown(), Segment.FixedUpdate);
			response = string.Format("Players have been redirected to port {0}. Server will shutdown in a couple of seconds.", num);
			return true;
		}

		// Token: 0x0600385E RID: 14430 RVA: 0x000B73F4 File Offset: 0x000B55F4
		private IEnumerator<float> ScheduleShutdown()
		{
			yield return Timing.WaitForSeconds(5f);
			Shutdown.Quit(true, false);
			yield break;
		}
	}
}
