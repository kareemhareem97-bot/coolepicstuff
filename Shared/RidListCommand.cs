using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapGeneration;
using NorthwoodLib.Pools;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009C1 RID: 2497
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class RidListCommand : ICommand
	{
		// Token: 0x17000B56 RID: 2902
		// (get) Token: 0x06003870 RID: 14448 RVA: 0x000B7639 File Offset: 0x000B5839
		public string Command { get; } = "ridlist";

		// Token: 0x17000B57 RID: 2903
		// (get) Token: 0x06003871 RID: 14449 RVA: 0x000B7641 File Offset: 0x000B5841
		public string[] Aliases { get; } = new string[]
		{
			"rids"
		};

		// Token: 0x17000B58 RID: 2904
		// (get) Token: 0x06003872 RID: 14450 RVA: 0x000B7649 File Offset: 0x000B5849
		public string Description { get; } = "Displays a list of all room ids.";

		// Token: 0x06003873 RID: 14451 RVA: 0x000B7654 File Offset: 0x000B5854
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			RoomIdentifier[] array = (from x in RoomIdentifier.AllRoomIdentifiers
			where x.Name > RoomName.Unnamed
			select x).ToArray<RoomIdentifier>();
			if (array.Length == 0)
			{
				response = "There are no rooms!";
				return false;
			}
			Dictionary<RoomName, int> dictionary = new Dictionary<RoomName, int>();
			foreach (RoomIdentifier roomIdentifier in array)
			{
				int num;
				if (dictionary.TryGetValue(roomIdentifier.Name, out num))
				{
					dictionary[roomIdentifier.Name] = num + 1;
				}
				else
				{
					dictionary.Add(roomIdentifier.Name, 1);
				}
			}
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			stringBuilder.Append("--- RID List ---\n");
			foreach (KeyValuePair<RoomName, int> keyValuePair in from a in dictionary
			orderby a.Key
			select a)
			{
				stringBuilder.Append("- ");
				stringBuilder.Append(keyValuePair.Key);
				stringBuilder.Append(" (");
				stringBuilder.Append(keyValuePair.Value);
				stringBuilder.Append(")\n");
			}
			response = stringBuilder.ToString().TrimEnd();
			StringBuilderPool.Shared.Return(stringBuilder);
			return true;
		}
	}
}
