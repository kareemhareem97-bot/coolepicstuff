using System;
using CommandSystem;
using ServerOutput;

namespace _Scripts.CommandSystem.Commands.Console
{
	// Token: 0x020009A2 RID: 2466
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class BufferSizeCommand : ICommand
	{
		// Token: 0x17000B00 RID: 2816
		// (get) Token: 0x060037D3 RID: 14291 RVA: 0x000B51DA File Offset: 0x000B33DA
		public string Command { get; } = "buffer";

		// Token: 0x17000B01 RID: 2817
		// (get) Token: 0x060037D4 RID: 14292 RVA: 0x000B51E2 File Offset: 0x000B33E2
		public string[] Aliases { get; } = new string[]
		{
			"bs"
		};

		// Token: 0x17000B02 RID: 2818
		// (get) Token: 0x060037D5 RID: 14293 RVA: 0x000B51EA File Offset: 0x000B33EA
		public string Description { get; } = "Returns TcpConsole buffers sizes.";

		// Token: 0x060037D6 RID: 14294 RVA: 0x000B51F4 File Offset: 0x000B33F4
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			TcpConsole tcpConsole = ServerStatic.ServerOutput as TcpConsole;
			if (tcpConsole == null)
			{
				response = "Current ServerOutput is not TcpConsole!";
				return false;
			}
			response = string.Format("RX Buffer Size: {0}\nTX Buffer Size: {1}\n\nSpecified RX Buffer Size: {2}\nSpecified TX Buffer Size: {3}\n\nDefault RX Buffer Size: {4}\nDefault TX Buffer Size: {5}", new object[]
			{
				tcpConsole.ReceiveBufferSize,
				tcpConsole.SendBufferSize,
				tcpConsole.SpecifiedReceiveBufferSize,
				tcpConsole.SpecifiedSendBufferSize,
				25000,
				200000
			});
			return true;
		}
	}
}
