using System;
using System.Collections.Generic;

// Token: 0x02000094 RID: 148
public struct RecyclablePlayerId : IEquatable<RecyclablePlayerId>
{
	// Token: 0x060002FF RID: 767 RVA: 0x0000B7E7 File Offset: 0x000099E7
	public RecyclablePlayerId(int newId)
	{
		this.Value = newId;
	}

	// Token: 0x06000300 RID: 768 RVA: 0x0000B7F0 File Offset: 0x000099F0
	public RecyclablePlayerId(ReferenceHub hub)
	{
		this.Value = hub.PlayerId;
	}

	// Token: 0x06000301 RID: 769 RVA: 0x0000B800 File Offset: 0x00009A00
	public RecyclablePlayerId(bool useMinQueue)
	{
		int num = useMinQueue ? 16 : 0;
		int value = (RecyclablePlayerId.FreeIds.Count < num) ? (++RecyclablePlayerId._autoIncrement) : RecyclablePlayerId.FreeIds.Dequeue();
		this.Value = value;
	}

	// Token: 0x06000302 RID: 770 RVA: 0x0000B844 File Offset: 0x00009A44
	public void Destroy()
	{
		if (this.Value != 0)
		{
			RecyclablePlayerId.FreeIds.Enqueue(this.Value);
		}
	}

	// Token: 0x06000303 RID: 771 RVA: 0x0000B85E File Offset: 0x00009A5E
	public bool Equals(RecyclablePlayerId other)
	{
		return this.Value == other.Value;
	}

	// Token: 0x06000304 RID: 772 RVA: 0x0000B870 File Offset: 0x00009A70
	public override bool Equals(object obj)
	{
		if (obj is RecyclablePlayerId)
		{
			RecyclablePlayerId other = (RecyclablePlayerId)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000305 RID: 773 RVA: 0x0000B895 File Offset: 0x00009A95
	public override int GetHashCode()
	{
		return this.Value;
	}

	// Token: 0x06000306 RID: 774 RVA: 0x0000B89D File Offset: 0x00009A9D
	public static bool operator ==(RecyclablePlayerId left, RecyclablePlayerId right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000307 RID: 775 RVA: 0x0000B8A7 File Offset: 0x00009AA7
	public static bool operator !=(RecyclablePlayerId left, RecyclablePlayerId right)
	{
		return !left.Equals(right);
	}

	// Token: 0x040002B4 RID: 692
	private const int MinQueue = 16;

	// Token: 0x040002B5 RID: 693
	private static readonly Queue<int> FreeIds = new Queue<int>();

	// Token: 0x040002B6 RID: 694
	private static int _autoIncrement;

	// Token: 0x040002B7 RID: 695
	public readonly int Value;
}
