using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A98 RID: 2712
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class IpCommand : ICommand
	{
		// Token: 0x17000DDD RID: 3549
		// (get) Token: 0x06003D2D RID: 15661 RVA: 0x000C92A4 File Offset: 0x000C74A4
		public string Command { get; } = "ip";

		// Token: 0x17000DDE RID: 3550
		// (get) Token: 0x06003D2E RID: 15662 RVA: 0x000C92AC File Offset: 0x000C74AC
		public string[] Aliases { get; } = new string[]
		{
			"whatismyip",
			"myip"
		};

		// Token: 0x17000DDF RID: 3551
		// (get) Token: 0x06003D2F RID: 15663 RVA: 0x000C92B4 File Offset: 0x000C74B4
		public string Description { get; } = "Returns the IP used when connecting to central servers";

		// Token: 0x06003D30 RID: 15664 RVA: 0x000C92BC File Offset: 0x000C74BC
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			bool result;
			try
			{
				Task.Run(delegate()
				{
					IpCommand.<>c.<<Execute>b__9_0>d <<Execute>b__9_0>d;
					<<Execute>b__9_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<Execute>b__9_0>d.<>1__state = -1;
					<<Execute>b__9_0>d.<>t__builder.Start<IpCommand.<>c.<<Execute>b__9_0>d>(ref <<Execute>b__9_0>d);
					return <<Execute>b__9_0>d.<>t__builder.Task;
				});
				response = "Requesting IP...";
				result = true;
			}
			catch (Exception ex)
			{
				response = "HTTP request failed: " + ex.Message;
				result = false;
			}
			return result;
		}
	}
}
