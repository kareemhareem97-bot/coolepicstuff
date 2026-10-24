using System;
using UnityEngine;

// Token: 0x020001A5 RID: 421
[Serializable]
public struct Offset : IEquatable<Offset>
{
	// Token: 0x0600084B RID: 2123 RVA: 0x00021163 File Offset: 0x0001F363
	public bool Equals(Offset other)
	{
		return this.position == other.position && this.rotation == other.rotation && this.scale == other.scale;
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x000211A0 File Offset: 0x0001F3A0
	public override bool Equals(object obj)
	{
		if (obj is Offset)
		{
			Offset other = (Offset)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x000211C8 File Offset: 0x0001F3C8
	public override int GetHashCode()
	{
		return (this.position.GetHashCode() * 397 ^ this.rotation.GetHashCode()) * 397 ^ this.scale.GetHashCode();
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x00021216 File Offset: 0x0001F416
	public Offset(Transform t, bool local)
	{
		this.position = (local ? t.localPosition : t.position);
		this.rotation = (local ? t.localEulerAngles : t.eulerAngles);
		this.scale = t.localScale;
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x00021252 File Offset: 0x0001F452
	public static bool operator ==(Offset left, Offset right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x0002125C File Offset: 0x0001F45C
	public static bool operator !=(Offset left, Offset right)
	{
		return !left.Equals(right);
	}

	// Token: 0x0400078F RID: 1935
	public Vector3 position;

	// Token: 0x04000790 RID: 1936
	public Vector3 rotation;

	// Token: 0x04000791 RID: 1937
	public Vector3 scale;
}
