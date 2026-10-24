using System;
using System.Linq;
using System.Text;
using Interactables.Interobjects.DoorUtils;
using NorthwoodLib.Pools;
using UnityEngine;

namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	// Token: 0x02000A60 RID: 2656
	public abstract class BaseDoorCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D47 RID: 3399
		// (get) Token: 0x06003C08 RID: 15368
		public abstract string Command { get; }

		// Token: 0x17000D48 RID: 3400
		// (get) Token: 0x06003C09 RID: 15369
		public abstract string[] Aliases { get; }

		// Token: 0x17000D49 RID: 3401
		// (get) Token: 0x06003C0A RID: 15370
		public abstract string Description { get; }

		// Token: 0x17000D4A RID: 3402
		// (get) Token: 0x06003C0B RID: 15371 RVA: 0x000C63B4 File Offset: 0x000C45B4
		public virtual string[] Usage { get; } = new string[]
		{
			"%door%"
		};

		// Token: 0x17000D4B RID: 3403
		// (get) Token: 0x06003C0C RID: 15372 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public virtual bool AllowNonDamageableTargets
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003C0D RID: 15373 RVA: 0x000C63BC File Offset: 0x000C45BC
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			if (arguments.Count == 0)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			if (string.IsNullOrEmpty(arguments.At(0)))
			{
				response = "Please specify a door first.";
				return false;
			}
			bool flag = false;
			string text = arguments.At(0).ToUpper();
			string[] source = text.Split('.', StringSplitOptions.None);
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			if (!(text == "*"))
			{
				if (!(text == "!*"))
				{
					if (text == "**")
					{
						stringBuilder.Append(", **");
					}
				}
				else
				{
					stringBuilder.Append(", !*");
				}
			}
			else
			{
				stringBuilder.Append(", *");
			}
			foreach (DoorVariant doorVariant in DoorVariant.AllDoors)
			{
				INonInteractableDoor nonInteractableDoor = doorVariant as INonInteractableDoor;
				if (nonInteractableDoor == null || !nonInteractableDoor.IgnoreRemoteAdmin)
				{
					string text2 = string.Empty;
					DoorNametagExtension nt;
					bool flag2 = doorVariant.TryGetComponent<DoorNametagExtension>(out nt);
					if (!(text == "*"))
					{
						if (!(text == "!*"))
						{
							if (!(text == "**"))
							{
								if (!flag2 || !source.Any((string i) => string.Equals(nt.GetName, i, StringComparison.OrdinalIgnoreCase)))
								{
									continue;
								}
								text2 = nt.GetName;
							}
						}
						else
						{
							if (flag2)
							{
								continue;
							}
							Transform parent = doorVariant.transform.parent;
							DoorNametagExtension doorNametagExtension;
							if (parent != null && parent.TryGetComponent<DoorNametagExtension>(out doorNametagExtension))
							{
								continue;
							}
						}
					}
					else if (!flag2)
					{
						continue;
					}
					if (this.AllowNonDamageableTargets || doorVariant is IDamageableDoor)
					{
						this.OnTargetFound(doorVariant);
						if (!string.IsNullOrEmpty(text2))
						{
							stringBuilder.Append(", " + text2);
						}
						flag = true;
					}
				}
			}
			string text3 = StringBuilderPool.Shared.ToStringReturn(stringBuilder);
			if (flag)
			{
				text3 = text3.Substring(2);
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Concat(new string[]
				{
					sender.LogName,
					" ran ",
					this.Command,
					" on the following door(s): ",
					text3
				}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			}
			response = (flag ? ("Affected the following door(s): " + text3) : ("Can't find any door(s) using \"" + text.Replace(".", ", ") + "\"."));
			return flag;
		}

		// Token: 0x06003C0E RID: 15374
		protected abstract void OnTargetFound(DoorVariant door);
	}
}
