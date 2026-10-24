using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using PlayerRoles.Voice;
using RemoteAdmin;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009FC RID: 2556
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class IntercomSpeakCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C10 RID: 3088
		// (get) Token: 0x060039BB RID: 14779 RVA: 0x000BDBA6 File Offset: 0x000BBDA6
		public string Command { get; } = "icom";

		// Token: 0x17000C11 RID: 3089
		// (get) Token: 0x060039BC RID: 14780 RVA: 0x000BDBAE File Offset: 0x000BBDAE
		public string[] Aliases { get; } = new string[]
		{
			"speak"
		};

		// Token: 0x17000C12 RID: 3090
		// (get) Token: 0x060039BD RID: 14781 RVA: 0x000BDBB6 File Offset: 0x000BBDB6
		public string Description { get; } = "Toggles global voice over the intercom.";

		// Token: 0x17000C13 RID: 3091
		// (get) Token: 0x060039BE RID: 14782 RVA: 0x000BDBBE File Offset: 0x000BBDBE
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"enable/disable"
		};

		// Token: 0x060039BF RID: 14783 RVA: 0x000BDBC8 File Offset: 0x000BBDC8
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Broadcasting, out response))
			{
				return false;
			}
			if (arguments.Count == 0)
			{
				PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
				if (playerCommandSender == null)
				{
					response = "You must be in-game to use this command!";
					return false;
				}
				bool flag = !Intercom.HasOverride(playerCommandSender.ReferenceHub);
				if (!Intercom.TrySetOverride(playerCommandSender.ReferenceHub, flag))
				{
					response = "Failed to set override flags. User or intercom is null.";
					return false;
				}
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " toggled global intercom transmission.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
				response = "Done! Global voice over the intercom toggled " + (flag ? "on" : "off") + ".";
				return true;
			}
			else
			{
				string[] array;
				List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
				StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
				response = "Invalid argument, state was not defined.";
				if (array.Length == 0)
				{
					return false;
				}
				string text = array[0].ToUpper();
				bool flag2 = false;
				bool flag3 = false;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 890022063U)
				{
					if (num <= 785938062U)
					{
						if (num != 304993157U)
						{
							if (num != 785938062U)
							{
								goto IL_1CD;
							}
							if (!(text == "ENABLE"))
							{
								goto IL_1CD;
							}
						}
						else
						{
							if (!(text == "DISABLED"))
							{
								goto IL_1CD;
							}
							goto IL_1C8;
						}
					}
					else if (num != 873244444U)
					{
						if (num != 890022063U)
						{
							goto IL_1CD;
						}
						if (!(text == "0"))
						{
							goto IL_1CD;
						}
						goto IL_1C8;
					}
					else if (!(text == "1"))
					{
						goto IL_1CD;
					}
				}
				else if (num <= 2294480894U)
				{
					if (num != 1343949093U)
					{
						if (num != 2294480894U)
						{
							goto IL_1CD;
						}
						if (!(text == "ENABLED"))
						{
							goto IL_1CD;
						}
					}
					else if (!(text == "TRUE"))
					{
						goto IL_1CD;
					}
				}
				else if (num != 2616878531U)
				{
					if (num != 3998840952U)
					{
						goto IL_1CD;
					}
					if (!(text == "FALSE"))
					{
						goto IL_1CD;
					}
					goto IL_1C8;
				}
				else
				{
					if (!(text == "DISABLE"))
					{
						goto IL_1CD;
					}
					goto IL_1C8;
				}
				flag3 = true;
				goto IL_1D0;
				IL_1C8:
				flag3 = false;
				goto IL_1D0;
				IL_1CD:
				flag2 = true;
				IL_1D0:
				int num2 = 0;
				foreach (ReferenceHub referenceHub in list)
				{
					if (flag2)
					{
						flag3 = !Intercom.HasOverride(referenceHub);
					}
					if (Intercom.TrySetOverride(referenceHub, flag3))
					{
						if (num2 != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
						num2++;
					}
				}
				if (num2 > 0)
				{
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} {1} global intercom transmission for player{2}{3}.", new object[]
					{
						sender.LogName,
						flag3 ? "enabled" : "disabled",
						(num2 == 1) ? " " : "s ",
						stringBuilder
					}), ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
				}
				StringBuilderPool.Shared.Return(stringBuilder);
				response = string.Format("Done! The request affected {0} player{1}", num2, (num2 == 1) ? "!" : "s!");
				return true;
			}
		}
	}
}
