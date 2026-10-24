using System;
using System.Collections.Generic;
using AdminToys;
using Mirror;
using RemoteAdmin;
using UnityEngine;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A1F RID: 2591
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SpawnToyCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C89 RID: 3209
		// (get) Token: 0x06003A92 RID: 14994 RVA: 0x000C1699 File Offset: 0x000BF899
		public string Command { get; } = "spawntoy";

		// Token: 0x17000C8A RID: 3210
		// (get) Token: 0x06003A93 RID: 14995 RVA: 0x000C16A1 File Offset: 0x000BF8A1
		public string[] Aliases { get; }

		// Token: 0x17000C8B RID: 3211
		// (get) Token: 0x06003A94 RID: 14996 RVA: 0x000C16A9 File Offset: 0x000BF8A9
		public string Description { get; } = "Spawns an admin toy.";

		// Token: 0x17000C8C RID: 3212
		// (get) Token: 0x06003A95 RID: 14997 RVA: 0x000C16B1 File Offset: 0x000BF8B1
		public string[] Usage { get; } = new string[]
		{
			"Target toy (Optional, will list all available toys if not provided)"
		};

		// Token: 0x06003A96 RID: 14998 RVA: 0x000C16BC File Offset: 0x000BF8BC
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null)
			{
				response = "Only players can run this command.";
				return false;
			}
			string text = (arguments.Count < 1) ? "list" : arguments.Array[1];
			bool flag = text == "list";
			if (flag)
			{
				response = "List of toys:";
			}
			using (Dictionary<uint, GameObject>.ValueCollection.Enumerator enumerator = NetworkClient.prefabs.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AdminToyBase adminToyBase;
					if (enumerator.Current.TryGetComponent<AdminToyBase>(out adminToyBase))
					{
						if (flag)
						{
							response = response + "\n- " + adminToyBase.CommandName;
						}
						else if (string.Equals(text, adminToyBase.CommandName, StringComparison.InvariantCultureIgnoreCase))
						{
							AdminToyBase adminToyBase2 = UnityEngine.Object.Instantiate<AdminToyBase>(adminToyBase);
							adminToyBase2.OnSpawned(playerCommandSender.ReferenceHub, arguments);
							response = string.Format("Toy \"{0}\" placed! You can remove it by using \"DESTROYTOY {1}\" command.", adminToyBase2.CommandName, adminToyBase2.netId);
							return true;
						}
					}
				}
			}
			if (!flag)
			{
				response = "Toy \"" + text + "\" not found!";
			}
			return flag;
		}

		// Token: 0x04002B52 RID: 11090
		private const string ListCommand = "list";
	}
}
