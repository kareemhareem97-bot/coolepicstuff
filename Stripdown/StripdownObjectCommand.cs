using System;
using Mirror;

namespace CommandSystem.Commands.RemoteAdmin.Stripdown
{
	// Token: 0x02000A37 RID: 2615
	[CommandHandler(typeof(StripdownCommand))]
	public class StripdownObjectCommand : StripdownInstructionBase
	{
		// Token: 0x17000CCC RID: 3276
		// (get) Token: 0x06003B1B RID: 15131 RVA: 0x000C2CB5 File Offset: 0x000C0EB5
		public override string Command
		{
			get
			{
				return "object";
			}
		}

		// Token: 0x17000CCD RID: 3277
		// (get) Token: 0x06003B1C RID: 15132 RVA: 0x000C2CBC File Offset: 0x000C0EBC
		public override string Description
		{
			get
			{
				return "Finds all objects of provided type and selects them.";
			}
		}

		// Token: 0x06003B1D RID: 15133 RVA: 0x000C2CC4 File Offset: 0x000C0EC4
		protected override string ProcessInstruction(NetworkConnection sender, string instruction)
		{
			string result;
			try
			{
				int num = StripdownProcessor.SelectUnityObjects(instruction);
				result = string.Format("Selected {0} objects of type {1}", num, instruction);
			}
			catch (Exception ex)
			{
				result = "Failed to select objects. Exception: " + ex.Message + ";" + ex.StackTrace;
			}
			return result;
		}
	}
}
