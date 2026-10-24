using System;
using UserSettings.ServerSpecific.Examples;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A20 RID: 2592
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SSSExampleCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C8D RID: 3213
		// (get) Token: 0x06003A98 RID: 15000 RVA: 0x000C181A File Offset: 0x000BFA1A
		public string Command { get; } = "sssimplementationexample";

		// Token: 0x17000C8E RID: 3214
		// (get) Token: 0x06003A99 RID: 15001 RVA: 0x000C1822 File Offset: 0x000BFA22
		public string[] Aliases { get; } = new string[]
		{
			"sssexample"
		};

		// Token: 0x17000C8F RID: 3215
		// (get) Token: 0x06003A9A RID: 15002 RVA: 0x000C182A File Offset: 0x000BFA2A
		public string Description { get; } = "Allows to active server specific settings implementation example.";

		// Token: 0x17000C90 RID: 3216
		// (get) Token: 0x06003A9B RID: 15003 RVA: 0x000C1832 File Offset: 0x000BFA32
		public string[] Usage { get; } = new string[]
		{
			"Example index (Optional, will list all available examples if not provided)"
		};

		// Token: 0x06003A9C RID: 15004 RVA: 0x000C183C File Offset: 0x000BFA3C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement))
			{
				response = "You are not authorized to run this command";
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "Example implementations of server-specific setting:";
				for (int i = 0; i < SSExampleImplementationBase.AllExamples.Length; i++)
				{
					response += string.Format("\n - ('{0}') - {1}", i, SSExampleImplementationBase.AllExamples[i].Name);
				}
				response += "\n\nUse 'stop' as argument to deactivate current implementation example.";
				return true;
			}
			if (string.Equals(arguments.At(0), "stop", StringComparison.InvariantCultureIgnoreCase))
			{
				string str;
				if (SSExampleImplementationBase.TryDeactivateExample(out str))
				{
					response = str + " successfully deactivated.";
					return true;
				}
				response = "No example implementation active.";
				return false;
			}
			else
			{
				int index;
				if (!int.TryParse(arguments.At(0), out index))
				{
					response = "Invalid argument. Provide a number or 'stop' to deactivate current implementation example. Provide no arguments to list available examples.";
					return false;
				}
				return SSExampleImplementationBase.TryActivateExample(index, out response);
			}
		}
	}
}
