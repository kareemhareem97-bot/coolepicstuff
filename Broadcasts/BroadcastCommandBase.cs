using System;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin.Broadcasts
{
	// Token: 0x02000A82 RID: 2690
	public abstract class BroadcastCommandBase : ICommand, IUsageProvider
	{
		// Token: 0x17000D9F RID: 3487
		// (get) Token: 0x06003CB3 RID: 15539
		public abstract string Command { get; }

		// Token: 0x17000DA0 RID: 3488
		// (get) Token: 0x06003CB4 RID: 15540
		public abstract string[] Aliases { get; }

		// Token: 0x17000DA1 RID: 3489
		// (get) Token: 0x06003CB5 RID: 15541
		public abstract string Description { get; }

		// Token: 0x17000DA2 RID: 3490
		// (get) Token: 0x06003CB6 RID: 15542
		public abstract string[] Usage { get; }

		// Token: 0x17000DA3 RID: 3491
		// (get) Token: 0x06003CB7 RID: 15543 RVA: 0x00034727 File Offset: 0x00032927
		public virtual int MinimumArguments
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06003CB8 RID: 15544 RVA: 0x000C7CCC File Offset: 0x000C5ECC
		public virtual bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Broadcasting, out response))
			{
				return false;
			}
			if (arguments.Count < this.MinimumArguments)
			{
				response = string.Format("To execute this command provide at least {0} arguments!\nUsage: {1} {2}", this.MinimumArguments, arguments.Array[0], this.DisplayCommandUsage());
				return false;
			}
			return this.OnExecute(arguments, sender, out response);
		}

		// Token: 0x06003CB9 RID: 15545
		public abstract bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response);

		// Token: 0x06003CBA RID: 15546 RVA: 0x000C7D2A File Offset: 0x000C5F2A
		protected bool HasInputFlag(string inputFlag, out Broadcast.BroadcastFlags broadcastFlag, int argumentCount = 0)
		{
			bool flag = RAUtils.IsDigit.IsMatch(inputFlag);
			broadcastFlag = Broadcast.BroadcastFlags.Normal;
			return !flag && argumentCount >= this.MinimumArguments + 1 && Enum.TryParse<Broadcast.BroadcastFlags>(inputFlag, true, out broadcastFlag);
		}

		// Token: 0x06003CBB RID: 15547 RVA: 0x000C7D51 File Offset: 0x000C5F51
		protected bool IsValidDuration(string inputDuration, out ushort time)
		{
			return ushort.TryParse(inputDuration, out time) && time > 0;
		}
	}
}
