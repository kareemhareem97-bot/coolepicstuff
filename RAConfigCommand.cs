using System;
using System.Globalization;
using CustomPlayerEffects;
using Utils;
using Utils.CommandInterpolation;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A04 RID: 2564
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class RAConfigCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C2D RID: 3117
		// (get) Token: 0x060039EC RID: 14828 RVA: 0x000BEBE9 File Offset: 0x000BCDE9
		public string Command { get; } = "setconfig";

		// Token: 0x17000C2E RID: 3118
		// (get) Token: 0x060039ED RID: 14829 RVA: 0x000BEBF1 File Offset: 0x000BCDF1
		public string[] Aliases { get; } = new string[]
		{
			"sc"
		};

		// Token: 0x17000C2F RID: 3119
		// (get) Token: 0x060039EE RID: 14830 RVA: 0x000BEBF9 File Offset: 0x000BCDF9
		public string Description { get; } = "Sets the server configuration.";

		// Token: 0x17000C30 RID: 3120
		// (get) Token: 0x060039EF RID: 14831 RVA: 0x000BEC01 File Offset: 0x000BCE01
		public string[] Usage { get; } = new string[]
		{
			"Option",
			"Value"
		};

		// Token: 0x060039F0 RID: 14832 RVA: 0x000BEC0C File Offset: 0x000BCE0C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConfigs, out response))
			{
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!";
				return false;
			}
			string text = RAUtils.FormatArguments(arguments, 1, null).Trim();
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Concat(new string[]
			{
				sender.LogName,
				" changed server configuration: ",
				arguments.At(0),
				": ",
				text,
				"."
			}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			string a = arguments.At(0).ToUpper();
			if (!(a == "FRIENDLY_FIRE"))
			{
				if (a == "PLAYER_LIST_TITLE")
				{
					string text2 = text ?? string.Empty;
					PlayerList.Title.Value = text2;
					try
					{
						PlayerList.singleton.RefreshTitle();
					}
					catch (Exception ex)
					{
						if (!(ex is CommandInputException) && !(ex is InvalidOperationException))
						{
							throw;
						}
						response = "Could not set player list title [" + text2 + "]:\n" + ex.Message;
						return false;
					}
					response = string.Concat(new string[]
					{
						"Done! Config [",
						arguments.At(0),
						"] has been set to [",
						ServerConfigSynchronizer.Singleton.ServerName,
						"]!"
					});
					ServerConfigSynchronizer.RefreshAllConfigs();
					return true;
				}
				if (!(a == "PD_REFRESH_EXIT"))
				{
					if (!(a == "SPAWN_PROTECT_ENABLED"))
					{
						if (!(a == "SPAWN_PROTECT_TIME"))
						{
							response = "Invalid config " + arguments.At(0);
							return false;
						}
						int num;
						if (int.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
						{
							SpawnProtected.SpawnDuration = (float)num;
							response = string.Format("Done! Config [{0}] has been set to [{1}]!", arguments.At(0), num);
							ServerConfigSynchronizer.RefreshAllConfigs();
							return true;
						}
						response = arguments.At(0) + " has invalid value, " + text + " is not a valid integer!";
						return false;
					}
					else
					{
						bool flag;
						if (bool.TryParse(text, out flag))
						{
							SpawnProtected.IsProtectionEnabled = flag;
							response = string.Format("Done! Config [{0}] has been set to [{1}]!", arguments.At(0), flag);
							return true;
						}
						if (text.Equals("toggle", StringComparison.OrdinalIgnoreCase))
						{
							SpawnProtected.IsProtectionEnabled = !SpawnProtected.IsProtectionEnabled;
							response = string.Format("Done! Config [{0}] has been set to [{1}]!", arguments.At(0), SpawnProtected.IsProtectionEnabled);
							return true;
						}
						response = arguments.At(0) + " has invalid value, " + text + " is not a valid bool!";
						return false;
					}
				}
				else
				{
					bool flag2;
					if (bool.TryParse(text, out flag2))
					{
						PocketDimensionTeleport.RefreshExit = flag2;
						response = string.Format("Done! Config [{0}] has been set to [{1}]!", arguments.At(0), flag2);
						ServerConfigSynchronizer.RefreshAllConfigs();
						return true;
					}
					response = arguments.At(0) + " has invalid value, " + text + " is not a valid bool!";
					return false;
				}
			}
			else
			{
				bool flag3;
				if (bool.TryParse(text, out flag3))
				{
					ServerConsole.FriendlyFire = flag3;
					response = string.Format("Done! Config [{0}] has been set to [{1}]!", arguments.At(0), flag3);
					ServerConfigSynchronizer.RefreshAllConfigs();
					return true;
				}
				if (text.Equals("toggle", StringComparison.OrdinalIgnoreCase))
				{
					ServerConsole.FriendlyFire = !ServerConsole.FriendlyFire;
					response = string.Format("Done! Config [{0}] has been set to [{1}]!", arguments.At(0), ServerConsole.FriendlyFire);
					ServerConfigSynchronizer.RefreshAllConfigs();
					return true;
				}
				response = arguments.At(0) + " has invalid value, " + text + " is not a valid bool!";
				return false;
			}
		}
	}
}
