using System;
using DrawableLine;
using Mirror;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A8F RID: 2703
	[CommandHandler(typeof(LineCommand))]
	public class LineDurationCommand : ICommand
	{
		// Token: 0x17000DC4 RID: 3524
		// (get) Token: 0x06003CFF RID: 15615 RVA: 0x000C8919 File Offset: 0x000C6B19
		public string Command { get; } = "setduration";

		// Token: 0x17000DC5 RID: 3525
		// (get) Token: 0x06003D00 RID: 15616 RVA: 0x000C8921 File Offset: 0x000C6B21
		public string[] Aliases { get; } = new string[]
		{
			"setdur",
			"duration",
			"dur",
			"d"
		};

		// Token: 0x17000DC6 RID: 3526
		// (get) Token: 0x06003D01 RID: 15617 RVA: 0x000C8929 File Offset: 0x000C6B29
		public string Description { get; } = "Overrides the duration of any new lines to the specified value.";

		// Token: 0x06003D02 RID: 15618 RVA: 0x000C8934 File Offset: 0x000C6B34
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (NetworkServer.active && !sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				return false;
			}
			float num;
			if (arguments.Count == 0 || !float.TryParse(arguments.At(0), out num))
			{
				if (DrawableLines.DurationOverride != null)
				{
					LineDurationCommand.ResetOverride(out response);
					return true;
				}
				response = "You must specify a valid duration!";
				return false;
			}
			else
			{
				if (num <= 0f)
				{
					LineDurationCommand.ResetOverride(out response);
					return true;
				}
				DrawableLines.DurationOverride = new float?(num);
				DrawableLinesManager.ApplyMaxDurationRetroactively(num);
				response = string.Format("<color=white>The drawable line system now has a default duration of <b>{0:F2}s</b>.</color>", num);
				return true;
			}
		}

		// Token: 0x06003D03 RID: 15619 RVA: 0x000C89C8 File Offset: 0x000C6BC8
		private static void ResetOverride(out string response)
		{
			DrawableLines.DurationOverride = null;
			DrawableLinesManager.ApplyMaxDurationRetroactively(0f);
			response = "Duration override value has been reset.";
		}
	}
}
