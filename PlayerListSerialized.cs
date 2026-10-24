using System;
using System.Collections.Generic;
using Utf8Json;

// Token: 0x02000209 RID: 521
public readonly struct PlayerListSerialized : IEquatable<PlayerListSerialized>, IJsonSerializable
{
	// Token: 0x06000AEA RID: 2794 RVA: 0x0002A6FF File Offset: 0x000288FF
	[SerializationConstructor]
	public PlayerListSerialized(List<string> objects)
	{
		this.objects = objects;
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x0002A708 File Offset: 0x00028908
	public bool Equals(PlayerListSerialized other)
	{
		return this.objects == other.objects;
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0002A718 File Offset: 0x00028918
	public override bool Equals(object obj)
	{
		if (obj is PlayerListSerialized)
		{
			PlayerListSerialized other = (PlayerListSerialized)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x0002A73D File Offset: 0x0002893D
	public override int GetHashCode()
	{
		if (this.objects == null)
		{
			return 0;
		}
		return this.objects.GetHashCode();
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0002A754 File Offset: 0x00028954
	public static bool operator ==(PlayerListSerialized left, PlayerListSerialized right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x0002A75E File Offset: 0x0002895E
	public static bool operator !=(PlayerListSerialized left, PlayerListSerialized right)
	{
		return !left.Equals(right);
	}

	// Token: 0x0400095B RID: 2395
	public readonly List<string> objects;
}
