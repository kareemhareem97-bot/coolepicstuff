using System;
using Mirror;

namespace CommandSystem.Commands.RemoteAdmin.Stripdown
{
	// Token: 0x02000A3B RID: 2619
	[CommandHandler(typeof(StripdownCommand))]
	public class StripdownValueCommand : StripdownInstructionBase
	{
		// Token: 0x17000CD0 RID: 3280
		// (get) Token: 0x06003B30 RID: 15152 RVA: 0x000C32F7 File Offset: 0x000C14F7
		public override string Command
		{
			get
			{
				return "value";
			}
		}

		// Token: 0x17000CD1 RID: 3281
		// (get) Token: 0x06003B31 RID: 15153 RVA: 0x000C32FE File Offset: 0x000C14FE
		public override string Description
		{
			get
			{
				return "Selects (by name) properties or fields of previously selected objects.";
			}
		}

		// Token: 0x06003B32 RID: 15154 RVA: 0x000C3308 File Offset: 0x000C1508
		protected override string ProcessInstruction(NetworkConnection sender, string instruction)
		{
			string result;
			try
			{
				StripdownProcessor.SelectValues(instruction);
				result = "Value '" + instruction + "' selected on each object.";
			}
			catch (Exception ex)
			{
				result = "Failed to select value. Exception: " + ex.Message + ";" + ex.StackTrace;
			}
			return result;
		}
	}
}
