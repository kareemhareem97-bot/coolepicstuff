using System;
using GameCore;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009AE RID: 2478
	[CommandHandler(typeof(ConfigCommand))]
	public class ValueCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000B1E RID: 2846
		// (get) Token: 0x0600380E RID: 14350 RVA: 0x000B6342 File Offset: 0x000B4542
		public string Command { get; } = "value";

		// Token: 0x17000B1F RID: 2847
		// (get) Token: 0x0600380F RID: 14351 RVA: 0x000B634A File Offset: 0x000B454A
		public string[] Aliases { get; } = new string[]
		{
			"val"
		};

		// Token: 0x17000B20 RID: 2848
		// (get) Token: 0x06003810 RID: 14352 RVA: 0x000B6352 File Offset: 0x000B4552
		public string Description { get; } = "Returns the value of specified config key";

		// Token: 0x17000B21 RID: 2849
		// (get) Token: 0x06003811 RID: 14353 RVA: 0x000B635A File Offset: 0x000B455A
		public string[] Usage { get; } = new string[]
		{
			"Key"
		};

		// Token: 0x06003812 RID: 14354 RVA: 0x000B6364 File Offset: 0x000B4564
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConfigs, out response))
			{
				return false;
			}
			if (arguments.Count == 0)
			{
				response = "Please specify the config key!";
				return false;
			}
			if (arguments.At(0).Equals("query_administrator_password", StringComparison.OrdinalIgnoreCase) || arguments.At(0).Equals("administrator_query_password", StringComparison.OrdinalIgnoreCase))
			{
				response = "You can't read value of this config key for security reasons.";
				return false;
			}
			string str;
			if (ConfigFile.ServerConfig.TryGetString(arguments.At(0), out str))
			{
				response = "The value of <i>'" + arguments.At(0) + "'</i> is: " + str;
				return true;
			}
			response = "Key <i>'" + arguments.At(0) + "'</i> is not present in the config!";
			return false;
		}
	}
}
