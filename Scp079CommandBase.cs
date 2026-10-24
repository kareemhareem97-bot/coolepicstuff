using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using PlayerRoles.PlayableScps.Scp079;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A16 RID: 2582
	public abstract class Scp079CommandBase : ICommand, IUsageProvider
	{
		// Token: 0x17000C65 RID: 3173
		// (get) Token: 0x06003A59 RID: 14937
		public abstract string Command { get; }

		// Token: 0x17000C66 RID: 3174
		// (get) Token: 0x06003A5A RID: 14938
		public abstract string[] Aliases { get; }

		// Token: 0x17000C67 RID: 3175
		// (get) Token: 0x06003A5B RID: 14939
		public abstract string Description { get; }

		// Token: 0x17000C68 RID: 3176
		// (get) Token: 0x06003A5C RID: 14940
		public abstract string[] Usage { get; }

		// Token: 0x06003A5D RID: 14941 RVA: 0x000C07DC File Offset: 0x000BE9DC
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			uint input;
			if (!uint.TryParse(arguments.At(arguments.Count - 1), out input))
			{
				response = "Value argument must be a valid number.\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			return this.OnExecute(arguments, sender, (int)input, out response);
		}

		// Token: 0x06003A5E RID: 14942 RVA: 0x000C086C File Offset: 0x000BEA6C
		public virtual bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, int input, out string response)
		{
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			int num = 0;
			foreach (ReferenceHub referenceHub in list)
			{
				Scp079Role scp079Role = referenceHub.roleManager.CurrentRole as Scp079Role;
				Scp079TierManager manager;
				if (scp079Role != null && scp079Role.SubroutineModule.TryGetSubroutine<Scp079TierManager>(out manager))
				{
					this.ApplyChanges(manager, input);
					num++;
					stringBuilder.Append(", " + referenceHub.LoggedNameFromRefHub());
				}
			}
			if (num > 0)
			{
				string text = StringBuilderPool.Shared.ToStringReturn(stringBuilder).Substring(2);
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} used \"{1} ({2})\" command on player{3}{4}.", new object[]
				{
					sender.LogName,
					this.Command,
					input,
					(num == 1) ? " " : "s ",
					text
				}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			}
			else
			{
				StringBuilderPool.Shared.Return(stringBuilder);
			}
			response = string.Format("Done! The request affected {0} player{1}", num, (num == 1) ? "!" : "s!");
			return true;
		}

		// Token: 0x06003A5F RID: 14943
		public abstract void ApplyChanges(Scp079TierManager manager, int input);
	}
}
