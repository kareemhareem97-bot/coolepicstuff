using System;
using System.Text;
using Mirror;
using RemoteAdmin;

namespace CommandSystem.Commands.RemoteAdmin.Stripdown
{
	// Token: 0x02000A32 RID: 2610
	[CommandHandler(typeof(StripdownCommand))]
	public abstract class StripdownInstructionBase : ICommand
	{
		// Token: 0x17000CC8 RID: 3272
		// (get) Token: 0x06003B0C RID: 15116
		public abstract string Command { get; }

		// Token: 0x17000CC9 RID: 3273
		// (get) Token: 0x06003B0D RID: 15117 RVA: 0x000042EB File Offset: 0x000024EB
		public string[] Aliases
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000CCA RID: 3274
		// (get) Token: 0x06003B0E RID: 15118
		public abstract string Description { get; }

		// Token: 0x06003B0F RID: 15119 RVA: 0x000C2ACC File Offset: 0x000C0CCC
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			ReferenceHub referenceHub;
			if (!ServerStatic.IsDedicated && ReferenceHub.TryGetLocalHub(out referenceHub) && !referenceHub.serverRoles.RemoteAdmin)
			{
				response = "You need RA access to run this command on client!";
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				return false;
			}
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			NetworkConnection connectionToClient;
			if (playerCommandSender != null)
			{
				connectionToClient = playerCommandSender.ReferenceHub.connectionToClient;
			}
			else
			{
				ReferenceHub referenceHub2;
				if (!(sender is ServerConsoleSender) || !ReferenceHub.TryGetLocalHub(out referenceHub2))
				{
					response = "No valid receiver found.";
					return false;
				}
				connectionToClient = referenceHub2.connectionToClient;
			}
			StripdownInstructionBase.Combiner.Clear();
			bool flag = false;
			for (int i = 0; i < arguments.Count; i++)
			{
				if (flag)
				{
					StripdownInstructionBase.Combiner.Append(' ');
				}
				else
				{
					flag = true;
				}
				StripdownInstructionBase.Combiner.Append(arguments.Array[arguments.Offset + i]);
			}
			response = this.ProcessInstruction(connectionToClient, StripdownInstructionBase.Combiner.ToString());
			return true;
		}

		// Token: 0x06003B10 RID: 15120
		protected abstract string ProcessInstruction(NetworkConnection conn, string instruction);

		// Token: 0x04002B90 RID: 11152
		private static readonly StringBuilder Combiner = new StringBuilder();
	}
}
