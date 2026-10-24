using System;
using System.Text;
using NorthwoodLib.Pools;
using Respawning;
using Respawning.Waves;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A0C RID: 2572
	[CommandHandler(typeof(WaveCommand))]
	public class WaveListCommand : ICommand
	{
		// Token: 0x17000C49 RID: 3145
		// (get) Token: 0x06003A1C RID: 14876 RVA: 0x000BF91A File Offset: 0x000BDB1A
		public string Command { get; } = "list";

		// Token: 0x17000C4A RID: 3146
		// (get) Token: 0x06003A1D RID: 14877 RVA: 0x000BF922 File Offset: 0x000BDB22
		public string[] Aliases { get; } = new string[]
		{
			"lst",
			"ls",
			"l"
		};

		// Token: 0x17000C4B RID: 3147
		// (get) Token: 0x06003A1E RID: 14878 RVA: 0x000BF92A File Offset: 0x000BDB2A
		public string Description { get; } = "Prints all the currently existing waves.";

		// Token: 0x06003A1F RID: 14879 RVA: 0x000BF934 File Offset: 0x000BDB34
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RespawnEvents, out response))
			{
				return false;
			}
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent("These are all the existing waves:");
			foreach (SpawnableWaveBase spawnableWaveBase in WaveManager.Waves)
			{
				string name = spawnableWaveBase.GetType().Name;
				stringBuilder.Append("\n- ");
				stringBuilder.Append(name);
			}
			response = StringBuilderPool.Shared.ToStringReturn(stringBuilder);
			return true;
		}
	}
}
