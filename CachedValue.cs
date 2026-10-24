using System;

// Token: 0x02000112 RID: 274
public class CachedValue<T>
{
	// Token: 0x0600051A RID: 1306 RVA: 0x00013040 File Offset: 0x00011240
	public CachedValue(Func<T> factory)
	{
		this._factory = factory;
		this._usesChecker = false;
		this._updateChecker = null;
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x0001305D File Offset: 0x0001125D
	public CachedValue(Func<T> factory, Func<bool> checker)
	{
		this._factory = factory;
		this._usesChecker = true;
		this._updateChecker = checker;
	}

	// Token: 0x170000CD RID: 205
	// (get) Token: 0x0600051C RID: 1308 RVA: 0x0001307A File Offset: 0x0001127A
	public T Value
	{
		get
		{
			if (this.IsDirty || (this._usesChecker && this._updateChecker()))
			{
				this.RefreshValue();
			}
			return this._cachedValue;
		}
	}

	// Token: 0x170000CE RID: 206
	// (get) Token: 0x0600051D RID: 1309 RVA: 0x000130A5 File Offset: 0x000112A5
	public bool IsDirty
	{
		get
		{
			return !this._cacheSet;
		}
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x000130B0 File Offset: 0x000112B0
	public void RefreshValue()
	{
		this._cacheSet = true;
		this._cachedValue = this._factory();
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x000130CA File Offset: 0x000112CA
	public void SetDirty()
	{
		this._cacheSet = false;
	}

	// Token: 0x04000458 RID: 1112
	private bool _cacheSet;

	// Token: 0x04000459 RID: 1113
	private T _cachedValue;

	// Token: 0x0400045A RID: 1114
	private readonly Func<T> _factory;

	// Token: 0x0400045B RID: 1115
	private readonly Func<bool> _updateChecker;

	// Token: 0x0400045C RID: 1116
	private readonly bool _usesChecker;
}
