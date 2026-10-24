using System;
using System.Text;
using NorthwoodLib.Pools;
using Respawning;
using Respawning.Waves;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A0A RID: 2570
	[CommandHandler(typeof(WaveCommand))]
	public class WaveDebugCommand : ICommand
	{
		// Token: 0x17000C43 RID: 3139
		// (get) Token: 0x06003A12 RID: 14866 RVA: 0x000BF5F5 File Offset: 0x000BD7F5
		public string Command { get; } = "debug";

		// Token: 0x17000C44 RID: 3140
		// (get) Token: 0x06003A13 RID: 14867 RVA: 0x000BF5FD File Offset: 0x000BD7FD
		public string[] Aliases { get; } = new string[]
		{
			"get",
			"gt",
			"g"
		};

		// Token: 0x17000C45 RID: 3141
		// (get) Token: 0x06003A14 RID: 14868 RVA: 0x000BF605 File Offset: 0x000BD805
		public string Description { get; } = "Prints all the possible information about a wave.";

		// Token: 0x06003A15 RID: 14869 RVA: 0x000BF610 File Offset: 0x000BD810
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RespawnEvents, out response))
			{
				return false;
			}
			if (arguments.Count == 0)
			{
				response = "You must specify a wave to get the information from.";
				return false;
			}
			string text = arguments.At(0);
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			foreach (SpawnableWaveBase spawnableWaveBase in WaveManager.Waves)
			{
				if (spawnableWaveBase.GetType().Name.Contains(text, StringComparison.OrdinalIgnoreCase))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append("\n\n");
					}
					stringBuilder.Append(spawnableWaveBase.CreateDebugString());
				}
			}
			response = StringBuilderPool.Shared.ToStringReturn(stringBuilder);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " requested information about " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			return true;
		}
	}
}
