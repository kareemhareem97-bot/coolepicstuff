using System;
using Mirror;

namespace CommandSystem.Commands.RemoteAdmin.Stripdown
{
	// Token: 0x02000A38 RID: 2616
	[CommandHandler(typeof(StripdownCommand))]
	public class StripdownPrintCommand : StripdownInstructionBase
	{
		// Token: 0x17000CCE RID: 3278
		// (get) Token: 0x06003B1F RID: 15135 RVA: 0x000C2D1C File Offset: 0x000C0F1C
		public override string Command
		{
			get
			{
				return "print";
			}
		}

		// Token: 0x17000CCF RID: 3279
		// (get) Token: 0x06003B20 RID: 15136 RVA: 0x000C2D23 File Offset: 0x000C0F23
		public override string Description
		{
			get
			{
				return "Calls the ToString method on fields or properties of provided names. You can print out multiple values of the same object.";
			}
		}

		// Token: 0x06003B21 RID: 15137 RVA: 0x000C2D2C File Offset: 0x000C0F2C
		protected override string ProcessInstruction(NetworkConnection sender, string instruction)
		{
			string result;
			try
			{
				string[] lines = StripdownProcessor.Print(instruction.Split(' ', StringSplitOptions.None));
				StripdownNetworking.StripdownResponse stripdownResponse = new StripdownNetworking.StripdownResponse
				{
					Lines = lines
				};
				if (sender == null)
				{
					StripdownNetworking.ProcessMessage(stripdownResponse);
				}
				else
				{
					sender.Send<StripdownNetworking.StripdownResponse>(stripdownResponse, 0);
				}
				result = "Values printed into the debug console (or saved into a game data file).";
			}
			catch (Exception ex)
			{
				result = "Failed to print value. Exception: " + ex.Message + ";" + ex.StackTrace;
			}
			return result;
		}
	}
}
