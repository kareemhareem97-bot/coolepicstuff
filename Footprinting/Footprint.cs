using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mirror;
using PlayerRoles;
using Respawning.NamingRules;

namespace Footprinting
{
	// Token: 0x02000715 RID: 1813
	public readonly struct Footprint : IEquatable<Footprint>
	{
		// Token: 0x1700082A RID: 2090
		// (get) Token: 0x06002B4D RID: 11085 RVA: 0x00093204 File Offset: 0x00091404
		public string UnitName
		{
			get
			{
				HumanRole humanRole;
				if (!this.Role.TryGetRoleTemplate(out humanRole))
				{
					return string.Empty;
				}
				List<string> list;
				if (!NamingRulesManager.GeneratedNames.TryGetValue(humanRole.Team, out list))
				{
					return string.Empty;
				}
				int count = list.Count;
				if ((int)this.Unit >= count)
				{
					return string.Empty;
				}
				return list[(int)this.Unit];
			}
		}

		// Token: 0x06002B4E RID: 11086 RVA: 0x00093264 File Offset: 0x00091464
		public Footprint(ReferenceHub hub)
		{
			this.IsSet = true;
			this.Stopwatch = Stopwatch.StartNew();
			bool flag = hub != null;
			this.Hub = (flag ? hub : null);
			this.PlayerId = (flag ? hub.PlayerId : 0);
			this.NetId = (flag ? hub.networkIdentity.netId : 0U);
			this.Role = (flag ? hub.GetRoleId() : RoleTypeId.None);
			byte unit;
			if (flag)
			{
				HumanRole humanRole = hub.roleManager.CurrentRole as HumanRole;
				if (humanRole != null)
				{
					unit = humanRole.UnitNameId;
					goto IL_83;
				}
			}
			unit = 0;
			IL_83:
			this.Unit = unit;
			this.LogUserID = (flag ? hub.authManager.UserId : string.Empty);
			this.Nickname = (flag ? hub.nicknameSync.MyNick : "(null)");
			this.BypassStaff = (flag && hub.authManager.BypassBansFlagSet);
			this.IpAddress = ((!flag || !NetworkServer.active || hub.connectionToClient == null) ? null : hub.connectionToClient.address);
			this.LifeIdentifier = (flag ? hub.roleManager.CurrentRole.UniqueLifeIdentifier : 0);
			if (!flag)
			{
				this._serial = 0;
				return;
			}
			if (++Footprint._serialClock == 0)
			{
				Footprint._serialClock++;
			}
			this._serial = Footprint._serialClock;
		}

		// Token: 0x06002B4F RID: 11087 RVA: 0x000933B9 File Offset: 0x000915B9
		public bool Equals(Footprint other)
		{
			return this.IsSet == other.IsSet && this._serial == other._serial;
		}

		// Token: 0x06002B50 RID: 11088 RVA: 0x000933D9 File Offset: 0x000915D9
		public override int GetHashCode()
		{
			return this._serial;
		}

		// Token: 0x040021EE RID: 8686
		public readonly ReferenceHub Hub;

		// Token: 0x040021EF RID: 8687
		public readonly bool IsSet;

		// Token: 0x040021F0 RID: 8688
		public readonly int PlayerId;

		// Token: 0x040021F1 RID: 8689
		public readonly uint NetId;

		// Token: 0x040021F2 RID: 8690
		public readonly int LifeIdentifier;

		// Token: 0x040021F3 RID: 8691
		public readonly RoleTypeId Role;

		// Token: 0x040021F4 RID: 8692
		public readonly byte Unit;

		// Token: 0x040021F5 RID: 8693
		public readonly string Nickname;

		// Token: 0x040021F6 RID: 8694
		public readonly string LogUserID;

		// Token: 0x040021F7 RID: 8695
		public readonly Stopwatch Stopwatch;

		// Token: 0x040021F8 RID: 8696
		public readonly bool BypassStaff;

		// Token: 0x040021F9 RID: 8697
		public readonly string IpAddress;

		// Token: 0x040021FA RID: 8698
		private readonly int _serial;

		// Token: 0x040021FB RID: 8699
		private static int _serialClock;
	}
}
