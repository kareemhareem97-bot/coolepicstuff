using System;

// Token: 0x02000174 RID: 372
[Serializable]
public class UserGroup
{
	// Token: 0x06000775 RID: 1909 RVA: 0x0001C604 File Offset: 0x0001A804
	public UserGroup Clone()
	{
		return new UserGroup
		{
			Name = this.Name,
			BadgeColor = this.BadgeColor,
			BadgeText = this.BadgeText,
			Permissions = this.Permissions,
			Cover = this.Cover,
			HiddenByDefault = this.HiddenByDefault,
			Shared = this.Shared,
			KickPower = this.KickPower,
			RequiredKickPower = this.RequiredKickPower
		};
	}

	// Token: 0x040006A7 RID: 1703
	public string Name;

	// Token: 0x040006A8 RID: 1704
	public string BadgeColor;

	// Token: 0x040006A9 RID: 1705
	public string BadgeText;

	// Token: 0x040006AA RID: 1706
	public ulong Permissions;

	// Token: 0x040006AB RID: 1707
	public bool Cover;

	// Token: 0x040006AC RID: 1708
	public bool HiddenByDefault;

	// Token: 0x040006AD RID: 1709
	public bool Shared;

	// Token: 0x040006AE RID: 1710
	public byte KickPower;

	// Token: 0x040006AF RID: 1711
	public byte RequiredKickPower;
}
