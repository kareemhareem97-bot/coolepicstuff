using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using Respawning;
using Respawning.Waves;
using Respawning.Waves.Generic;
using UnityEngine;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A0D RID: 2573
	[CommandHandler(typeof(WaveCommand))]
	public class WaveSetCommand : TargetWaveCommandBase
	{
		// Token: 0x17000C4C RID: 3148
		// (get) Token: 0x06003A21 RID: 14881 RVA: 0x000BFA19 File Offset: 0x000BDC19
		public override string Command { get; } = "set";

		// Token: 0x17000C4D RID: 3149
		// (get) Token: 0x06003A22 RID: 14882 RVA: 0x000BFA21 File Offset: 0x000BDC21
		public override string[] Aliases { get; } = new string[]
		{
			"st",
			"s"
		};

		// Token: 0x17000C4E RID: 3150
		// (get) Token: 0x06003A23 RID: 14883 RVA: 0x000BFA29 File Offset: 0x000BDC29
		public override string Description { get; } = "Sets the value of the specified flag.";

		// Token: 0x06003A24 RID: 14884 RVA: 0x000BFA34 File Offset: 0x000BDC34
		public override bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RespawnEvents, out response))
			{
				return false;
			}
			if (arguments.Count <= 2)
			{
				response = this.InvalidFlagMessage("Not enough arguments");
				return false;
			}
			string text = TargetWaveCommandBase.TranslateWaveName(arguments.At(0));
			string text2 = arguments.At(1);
			if (string.Compare(text2, "influence", true) == 0)
			{
				text2 = "None";
			}
			UpdateMessageFlags updateMessageFlags;
			if (!Enum.TryParse<UpdateMessageFlags>(text2, true, out updateMessageFlags))
			{
				response = this.InvalidFlagMessage("Specified flag does not exist");
				return false;
			}
			Func<SpawnableWaveBase, string, bool> func;
			if (!WaveSetCommand.MethodPerFlag.TryGetValue(updateMessageFlags, out func))
			{
				response = this.InvalidFlagMessage("Specified flag has no specified action");
				return false;
			}
			string text3 = arguments.At(2);
			bool flag = false;
			foreach (SpawnableWaveBase spawnableWaveBase in WaveManager.Waves)
			{
				string name = spawnableWaveBase.GetType().Name;
				if (name.Equals(text, StringComparison.OrdinalIgnoreCase) && func(spawnableWaveBase, text3))
				{
					text = name;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				response = this.InvalidFlagMessage("No wave was found");
				return false;
			}
			response = string.Format("Set {0}'s {1} to {2}.", text, updateMessageFlags, text3);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} set {1}'s {2} to {3}.", new object[]
			{
				sender.LogName,
				text,
				updateMessageFlags,
				text3
			}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			return true;
		}

		// Token: 0x06003A25 RID: 14885 RVA: 0x000BFBA4 File Offset: 0x000BDDA4
		private string InvalidFlagMessage(string context)
		{
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			stringBuilder.Append(context);
			stringBuilder.Append(". The correct syntax is \"wave ");
			stringBuilder.Append(this.Command);
			stringBuilder.Append(" <wave> <flag> <value>\". Available flags:");
			foreach (UpdateMessageFlags updateMessageFlags in WaveSetCommand.MethodPerFlag.Keys)
			{
				stringBuilder.Append("\n- ");
				stringBuilder.Append(updateMessageFlags);
			}
			return StringBuilderPool.Shared.ToStringReturn(stringBuilder);
		}

		// Token: 0x06003A26 RID: 14886 RVA: 0x000BFC50 File Offset: 0x000BDE50
		private static bool SetTokens(SpawnableWaveBase wave, string value)
		{
			ILimitedWave limitedWave = wave as ILimitedWave;
			if (limitedWave == null)
			{
				return false;
			}
			int num;
			if (!int.TryParse(value, out num))
			{
				return false;
			}
			if (value.StartsWith('+') || value.StartsWith('-'))
			{
				num = limitedWave.RespawnTokens + num;
			}
			num = Math.Clamp(num, 0, int.MaxValue);
			limitedWave.RespawnTokens = num;
			WaveUpdateMessage.ServerSendUpdate(wave, UpdateMessageFlags.Tokens);
			return true;
		}

		// Token: 0x06003A27 RID: 14887 RVA: 0x000BFCB0 File Offset: 0x000BDEB0
		private static bool SetTimer(SpawnableWaveBase wave, string value)
		{
			TimeBasedWave timeBasedWave = wave as TimeBasedWave;
			if (timeBasedWave == null)
			{
				return false;
			}
			float num;
			if (!float.TryParse(value, out num))
			{
				return false;
			}
			if (value.StartsWith('+') || value.StartsWith('-'))
			{
				timeBasedWave.Timer.AddTime(num);
				return true;
			}
			timeBasedWave.Timer.SetTime(num);
			return true;
		}

		// Token: 0x06003A28 RID: 14888 RVA: 0x000BFD04 File Offset: 0x000BDF04
		private static bool SetPause(SpawnableWaveBase wave, string value)
		{
			TimeBasedWave timeBasedWave = wave as TimeBasedWave;
			if (timeBasedWave == null)
			{
				return false;
			}
			float duration;
			if (!float.TryParse(value, out duration))
			{
				return false;
			}
			timeBasedWave.Timer.Pause(duration);
			return true;
		}

		// Token: 0x06003A29 RID: 14889 RVA: 0x000BFD36 File Offset: 0x000BDF36
		private static bool PlayAnimation(SpawnableWaveBase wave, string _)
		{
			if (!(wave is IAnimatedWave))
			{
				return false;
			}
			WaveUpdateMessage.ServerSendUpdate(wave, UpdateMessageFlags.Trigger);
			return true;
		}

		// Token: 0x06003A2A RID: 14890 RVA: 0x000BFD4C File Offset: 0x000BDF4C
		private static bool SetInfluence(SpawnableWaveBase wave, string value)
		{
			float num;
			if (!float.TryParse(value, out num))
			{
				return false;
			}
			if (value.StartsWith('+') || value.StartsWith('-'))
			{
				num = FactionInfluenceManager.Get(wave.TargetFaction) + num;
			}
			num = Mathf.Clamp(num, 0f, float.MaxValue);
			FactionInfluenceManager.Set(wave.TargetFaction, num);
			return true;
		}

		// Token: 0x06003A2C RID: 14892 RVA: 0x000BFDE0 File Offset: 0x000BDFE0
		// Note: this type is marked as 'beforefieldinit'.
		static WaveSetCommand()
		{
			Dictionary<UpdateMessageFlags, Func<SpawnableWaveBase, string, bool>> dictionary = new Dictionary<UpdateMessageFlags, Func<SpawnableWaveBase, string, bool>>();
			dictionary[UpdateMessageFlags.Pause] = ((SpawnableWaveBase w, string input) => WaveSetCommand.SetPause(w, input));
			dictionary[UpdateMessageFlags.Timer] = ((SpawnableWaveBase w, string input) => WaveSetCommand.SetTimer(w, input));
			dictionary[UpdateMessageFlags.Tokens] = ((SpawnableWaveBase w, string input) => WaveSetCommand.SetTokens(w, input));
			dictionary[UpdateMessageFlags.Trigger] = ((SpawnableWaveBase w, string input) => WaveSetCommand.PlayAnimation(w, input));
			dictionary[UpdateMessageFlags.None] = ((SpawnableWaveBase w, string input) => WaveSetCommand.SetInfluence(w, input));
			WaveSetCommand.MethodPerFlag = dictionary;
		}

		// Token: 0x04002B1E RID: 11038
		public static Dictionary<UpdateMessageFlags, Func<SpawnableWaveBase, string, bool>> MethodPerFlag;
	}
}
