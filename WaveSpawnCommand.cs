using System;
using System.Text;
using NorthwoodLib.Pools;
using Respawning;
using Respawning.Waves;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A0F RID: 2575
	[CommandHandler(typeof(WaveCommand))]
	public class WaveSpawnCommand : TargetWaveCommandBase
	{
		// Token: 0x17000C4F RID: 3151
		// (get) Token: 0x06003A34 RID: 14900 RVA: 0x000BFEA3 File Offset: 0x000BE0A3
		public override string Command { get; } = "spawn";

		// Token: 0x17000C50 RID: 3152
		// (get) Token: 0x06003A35 RID: 14901 RVA: 0x000BFEAB File Offset: 0x000BE0AB
		public override string[] Aliases { get; } = new string[]
		{
			"sp",
			"fs",
			"force"
		};

		// Token: 0x17000C51 RID: 3153
		// (get) Token: 0x06003A36 RID: 14902 RVA: 0x000BFEB3 File Offset: 0x000BE0B3
		public override string Description { get; } = "Sets the value of the specified flag.";

		// Token: 0x06003A37 RID: 14903 RVA: 0x000BFEBC File Offset: 0x000BE0BC
		public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RespawnEvents, out response))
			{
				return false;
			}
			bool flag = false;
			if (arguments.Count <= 0)
			{
				response = this.InvalidFlagMessage("Not enough arguments");
				return false;
			}
			if (arguments.Count > 1)
			{
				bool flag2;
				flag = (bool.TryParse(arguments.At(1), out flag2) && flag2);
			}
			string text = TargetWaveCommandBase.TranslateWaveName(arguments.At(0));
			bool flag3 = false;
			foreach (SpawnableWaveBase spawnableWaveBase in WaveManager.Waves)
			{
				string name = spawnableWaveBase.GetType().Name;
				if (name.Equals(text, StringComparison.OrdinalIgnoreCase))
				{
					if (flag)
					{
						WaveManager.Spawn(spawnableWaveBase);
					}
					else
					{
						WaveManager.InitiateRespawn(spawnableWaveBase);
					}
					text = name;
					flag3 = true;
					break;
				}
			}
			if (!flag3)
			{
				response = this.InvalidFlagMessage("No wave was found");
				return false;
			}
			string text2 = flag ? "force spawned" : "spawned";
			response = string.Concat(new string[]
			{
				"Action completed, ",
				text,
				" was ",
				text2,
				"."
			});
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Concat(new string[]
			{
				sender.LogName,
				" ",
				text2,
				" ",
				text,
				"."
			}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			return true;
		}

		// Token: 0x06003A38 RID: 14904 RVA: 0x000C0020 File Offset: 0x000BE220
		private string InvalidFlagMessage(string context)
		{
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			stringBuilder.Append(context);
			stringBuilder.Append(". The correct syntax is \"wave ");
			stringBuilder.Append(this.Command);
			stringBuilder.Append(" <wave> [forceSpawn? (true/false)]");
			return StringBuilderPool.Shared.ToStringReturn(stringBuilder);
		}
	}
}
