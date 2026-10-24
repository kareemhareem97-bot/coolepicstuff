using System;
using System.Collections.Generic;
using NetworkManagerUtils.Dummies;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin.Dummies
{
	// Token: 0x02000A5A RID: 2650
	[CommandHandler(typeof(DummiesCommand))]
	public class ActionDummyCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D36 RID: 3382
		// (get) Token: 0x06003BE8 RID: 15336 RVA: 0x000C5C1B File Offset: 0x000C3E1B
		public string Command { get; } = "action";

		// Token: 0x17000D37 RID: 3383
		// (get) Token: 0x06003BE9 RID: 15337 RVA: 0x000C5C23 File Offset: 0x000C3E23
		public string[] Aliases { get; }

		// Token: 0x17000D38 RID: 3384
		// (get) Token: 0x06003BEA RID: 15338 RVA: 0x000C5C2B File Offset: 0x000C3E2B
		public string Description { get; } = "Executes an action on selected dummies.";

		// Token: 0x17000D39 RID: 3385
		// (get) Token: 0x06003BEB RID: 15339 RVA: 0x000C5C33 File Offset: 0x000C3E33
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"Module",
			"Action"
		};

		// Token: 0x06003BEC RID: 15340 RVA: 0x000C5C3C File Offset: 0x000C3E3C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 3)
			{
				response = "You must specify all arguments! (target, module, action)";
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			if (list == null)
			{
				response = "An unexpected problem has occurred during PlayerId or name array processing.";
				return false;
			}
			string b = arguments.At(1);
			string b2 = arguments.At(2);
			int num = 0;
			foreach (ReferenceHub referenceHub in list)
			{
				if (!(referenceHub == null) && referenceHub.IsDummy)
				{
					List<DummyAction> list2 = DummyActionCollector.ServerGetActions(referenceHub);
					bool flag = false;
					foreach (DummyAction dummyAction in list2)
					{
						string a = dummyAction.Name.Replace(' ', '_');
						if (dummyAction.Action == null)
						{
							flag = (a == b);
						}
						else if (flag && a == b2)
						{
							dummyAction.Action();
							num++;
						}
					}
				}
			}
			response = string.Format("Action requested on {0} dumm{1}", num, (num == 1) ? "y!" : "ies!");
			return true;
		}
	}
}
