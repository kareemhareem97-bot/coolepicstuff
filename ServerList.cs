using System;
using Utf8Json;

// Token: 0x0200020D RID: 525
public readonly struct ServerList : IEquatable<ServerList>, IJsonSerializable
{
	// Token: 0x06000B02 RID: 2818 RVA: 0x0002AC12 File Offset: 0x00028E12
	[SerializationConstructor]
	public ServerList(ServerListItem[] servers)
	{
		this.servers = servers;
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x0002AC1B File Offset: 0x00028E1B
	public bool Equals(ServerList other)
	{
		return this.servers == other.servers;
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x0002AC2C File Offset: 0x00028E2C
	public override bool Equals(object obj)
	{
		if (obj is ServerList)
		{
			ServerList other = (ServerList)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x0002AC51 File Offset: 0x00028E51
	public override int GetHashCode()
	{
		if (this.servers == null)
		{
			return 0;
		}
		return this.servers.GetHashCode();
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x0002AC68 File Offset: 0x00028E68
	public static bool operator ==(ServerList left, ServerList right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x0002AC72 File Offset: 0x00028E72
	public static bool operator !=(ServerList left, ServerList right)
	{
		return !left.Equals(right);
	}

	// Token: 0x0400096E RID: 2414
	public readonly ServerListItem[] servers;
}
