using System;
using Respawning;
using Respawning.Waves;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A0B RID: 2571
	[CommandHandler(typeof(WaveCommand))]
	public class WaveForcePauseCommand : TargetWaveCommandBase
	{
		// Token: 0x17000C46 RID: 3142
		// (get) Token: 0x06003A17 RID: 14871 RVA: 0x000BF741 File Offset: 0x000BD941
		public override string Command { get; } = "forcepause";

		// Token: 0x17000C47 RID: 3143
		// (get) Token: 0x06003A18 RID: 14872 RVA: 0x000BF749 File Offset: 0x000BD949
		public override string[] Aliases { get; } = new string[]
		{
			"pauseforce"
		};

		// Token: 0x17000C48 RID: 3144
		// (get) Token: 0x06003A19 RID: 14873 RVA: 0x000BF751 File Offset: 0x000BD951
		public override string Description { get; } = "Pauses the wave's timer by force. Preventing it from counting down.";

		// Token: 0x06003A1A RID: 14874 RVA: 0x000BF75C File Offset: 0x000BD95C
		public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RespawnEvents, out response))
			{
				return false;
			}
			string text = TargetWaveCommandBase.TranslateWaveName(arguments.At(0));
			bool flag = false;
			SpawnableWaveBase spawnableWaveBase = null;
			foreach (SpawnableWaveBase spawnableWaveBase2 in WaveManager.Waves)
			{
				string name = spawnableWaveBase2.GetType().Name;
				if (name.Equals(text, StringComparison.OrdinalIgnoreCase))
				{
					text = name;
					spawnableWaveBase = spawnableWaveBase2;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				response = "Specified wave not found";
				return false;
			}
			TimeBasedWave timeBasedWave = spawnableWaveBase as TimeBasedWave;
			if (timeBasedWave == null)
			{
				response = "Specified wave is not time based";
				return false;
			}
			bool isForcefullyPaused = !timeBasedWave.Timer.IsForcefullyPaused;
			if (arguments.Count > 1 && !bool.TryParse(arguments.At(1), out isForcefullyPaused))
			{
				response = "Unable to parse boolean value of the pause status";
				return false;
			}
			timeBasedWave.Timer.IsForcefullyPaused = isForcefullyPaused;
			response = string.Concat(new string[]
			{
				"Wave ",
				text,
				" timer is ",
				timeBasedWave.Timer.IsForcefullyPaused ? "now paused" : "no longer paused",
				"."
			});
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Concat(new string[]
			{
				sender.LogName,
				" set ",
				text,
				"'s wave timer to ",
				timeBasedWave.Timer.IsForcefullyPaused ? "now paused" : "no longer paused",
				"."
			}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			return true;
		}
	}
}
