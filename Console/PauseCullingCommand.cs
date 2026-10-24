using System;
using ProgressiveCulling;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A9E RID: 2718
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class PauseCullingCommand : ICommand
	{
		// Token: 0x17000DE6 RID: 3558
		// (get) Token: 0x06003D43 RID: 15683 RVA: 0x000C961E File Offset: 0x000C781E
		public string Command { get; } = "pauseculling";

		// Token: 0x17000DE7 RID: 3559
		// (get) Token: 0x06003D44 RID: 15684 RVA: 0x000C9626 File Offset: 0x000C7826
		public string[] Aliases { get; } = new string[]
		{
			"cullingpause"
		};

		// Token: 0x17000DE8 RID: 3560
		// (get) Token: 0x06003D45 RID: 15685 RVA: 0x000C962E File Offset: 0x000C782E
		public string Description { get; } = "Toggles the culling system's pause on or off.";

		// Token: 0x06003D46 RID: 15686 RVA: 0x000C9638 File Offset: 0x000C7838
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = (CullingCamera.TogglePause() ? "Culling system paused successfully. It may become unpaused when playing on a server with active enemies." : "Culling system unpaused or failed to pause. This feature is unavailable when playing on a server with active enemies.");
			return true;
		}
	}
}
