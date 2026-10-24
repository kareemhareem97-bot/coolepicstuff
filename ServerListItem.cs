using System;
using Utf8Json;

// Token: 0x0200020E RID: 526
[Serializable]
public struct ServerListItem : IEquatable<ServerListItem>, IJsonSerializable
{
	// Token: 0x06000B08 RID: 2824 RVA: 0x0002AC80 File Offset: 0x00028E80
	[SerializationConstructor]
	public ServerListItem(uint accountId, uint serverId, string ip, ushort port, string players, string info, string pastebin, string version, bool friendlyFire, bool modded, bool whitelist, byte officialCode)
	{
		this.accountId = accountId;
		this.serverId = serverId;
		this.ip = ip;
		this.port = port;
		this.players = players;
		this.info = info;
		this.pastebin = pastebin;
		this.version = version;
		this.friendlyFire = friendlyFire;
		this.modded = modded;
		this.whitelist = whitelist;
		this.officialCode = officialCode;
		this.NameFilterPoints = 0;
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x0002ACF4 File Offset: 0x00028EF4
	public bool Equals(ServerListItem other)
	{
		return this.accountId == other.accountId && this.serverId == other.serverId && this.ip == other.ip && this.port == other.port && this.players == other.players && this.info == other.info && this.pastebin == other.pastebin && this.version == other.version && this.friendlyFire == other.friendlyFire && this.modded == other.modded && this.whitelist == other.whitelist && this.officialCode == other.officialCode;
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x0002ADD0 File Offset: 0x00028FD0
	public override bool Equals(object obj)
	{
		if (obj is ServerListItem)
		{
			ServerListItem other = (ServerListItem)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0002ADF8 File Offset: 0x00028FF8
	public override int GetHashCode()
	{
		return ((((((((((this.accountId.GetHashCode() * 397 ^ this.serverId.GetHashCode()) * 397 ^ ((this.ip != null) ? this.ip.GetHashCode() : 0)) * 397 ^ this.port.GetHashCode()) * 397 ^ ((this.players != null) ? this.players.GetHashCode() : 0)) * 397 ^ ((this.info != null) ? this.info.GetHashCode() : 0)) * 397 ^ ((this.pastebin != null) ? this.pastebin.GetHashCode() : 0)) * 397 ^ ((this.version != null) ? this.version.GetHashCode() : 0)) * 397 ^ this.friendlyFire.GetHashCode()) * 397 ^ this.modded.GetHashCode()) * 397 ^ this.whitelist.GetHashCode()) * 397 ^ this.officialCode.GetHashCode();
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x0002AF22 File Offset: 0x00029122
	public static bool operator ==(ServerListItem left, ServerListItem right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x0002AF2C File Offset: 0x0002912C
	public static bool operator !=(ServerListItem left, ServerListItem right)
	{
		return !left.Equals(right);
	}

	// Token: 0x0400096F RID: 2415
	public readonly uint accountId;

	// Token: 0x04000970 RID: 2416
	public readonly uint serverId;

	// Token: 0x04000971 RID: 2417
	public readonly string ip;

	// Token: 0x04000972 RID: 2418
	public readonly ushort port;

	// Token: 0x04000973 RID: 2419
	public readonly string players;

	// Token: 0x04000974 RID: 2420
	public readonly string info;

	// Token: 0x04000975 RID: 2421
	public readonly string pastebin;

	// Token: 0x04000976 RID: 2422
	public readonly string version;

	// Token: 0x04000977 RID: 2423
	public readonly bool friendlyFire;

	// Token: 0x04000978 RID: 2424
	public readonly bool modded;

	// Token: 0x04000979 RID: 2425
	public readonly bool whitelist;

	// Token: 0x0400097A RID: 2426
	public readonly byte officialCode;

	// Token: 0x0400097B RID: 2427
	public int NameFilterPoints;
}
