using System;
using Mirror;

namespace CommandSystem.Commands.RemoteAdmin.Stripdown
{
	// Token: 0x02000A31 RID: 2609
	[CommandHandler(typeof(StripdownCommand))]
	public class StripdownComponentCommand : StripdownInstructionBase
	{
		// Token: 0x17000CC6 RID: 3270
		// (get) Token: 0x06003B08 RID: 15112 RVA: 0x000C2A5C File Offset: 0x000C0C5C
		public override string Command
		{
			get
			{
				return "component";
			}
		}

		// Token: 0x17000CC7 RID: 3271
		// (get) Token: 0x06003B09 RID: 15113 RVA: 0x000C2A63 File Offset: 0x000C0C63
		public override string Description
		{
			get
			{
				return "Performs a GetComponent on all previously selected objects and applies them as new selections.";
			}
		}

		// Token: 0x06003B0A RID: 15114 RVA: 0x000C2A6C File Offset: 0x000C0C6C
		protected override string ProcessInstruction(NetworkConnection sender, string instruction)
		{
			string result;
			try
			{
				StripdownProcessor.SelectComponent(instruction);
				result = "Component '" + instruction + "' selected on each object.";
			}
			catch (Exception ex)
			{
				result = "Failed to get components. Exception: " + ex.Message + ";" + ex.StackTrace;
			}
			return result;
		}
	}
}
