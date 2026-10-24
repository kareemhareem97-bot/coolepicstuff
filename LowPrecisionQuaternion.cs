using System;
using UnityEngine;

// Token: 0x02000215 RID: 533
public readonly struct LowPrecisionQuaternion : IEquatable<LowPrecisionQuaternion>
{
	// Token: 0x06000B26 RID: 2854 RVA: 0x0002B500 File Offset: 0x00029700
	public LowPrecisionQuaternion(Quaternion value)
	{
		this._x = (sbyte)(value.x * 127f);
		this._y = (sbyte)(value.y * 127f);
		this._z = (sbyte)(value.z * 127f);
		this._w = (sbyte)(value.w * 127f);
	}

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06000B27 RID: 2855 RVA: 0x0002B55C File Offset: 0x0002975C
	public Quaternion Value
	{
		get
		{
			return new Quaternion((float)this._x / 127f, (float)this._y / 127f, (float)this._z / 127f, (float)this._w / 127f).normalized;
		}
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x0002B5AA File Offset: 0x000297AA
	public override int GetHashCode()
	{
		return (int)this._x | (int)this._y << 8 | (int)this._z << 16 | (int)this._w << 24;
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x0002B5CF File Offset: 0x000297CF
	public static bool operator ==(LowPrecisionQuaternion left, LowPrecisionQuaternion right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x0002B5D9 File Offset: 0x000297D9
	public static bool operator !=(LowPrecisionQuaternion left, LowPrecisionQuaternion right)
	{
		return !left.Equals(right);
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x0002B5E6 File Offset: 0x000297E6
	public bool Equals(LowPrecisionQuaternion other)
	{
		return this._x == other._x && this._y == other._y && this._z == other._z && this._w == other._w;
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x0002B624 File Offset: 0x00029824
	public override bool Equals(object obj)
	{
		if (obj is LowPrecisionQuaternion)
		{
			LowPrecisionQuaternion other = (LowPrecisionQuaternion)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x04000991 RID: 2449
	private const sbyte Range = 127;

	// Token: 0x04000992 RID: 2450
	private readonly sbyte _x;

	// Token: 0x04000993 RID: 2451
	private readonly sbyte _y;

	// Token: 0x04000994 RID: 2452
	private readonly sbyte _z;

	// Token: 0x04000995 RID: 2453
	private readonly sbyte _w;
}
