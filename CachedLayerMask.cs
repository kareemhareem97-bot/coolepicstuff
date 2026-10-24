using System;
using UnityEngine;

// Token: 0x02000111 RID: 273
public class CachedLayerMask
{
	// Token: 0x170000CC RID: 204
	// (get) Token: 0x06000517 RID: 1303 RVA: 0x00012FFE File Offset: 0x000111FE
	public LayerMask Mask
	{
		get
		{
			if (this._cachedMask == 0)
			{
				this._cachedMask = LayerMask.GetMask(this._layers);
			}
			return this._cachedMask;
		}
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x00013024 File Offset: 0x00011224
	public CachedLayerMask(params string[] layers)
	{
		this._layers = layers;
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x00013033 File Offset: 0x00011233
	public static implicit operator int(CachedLayerMask mask)
	{
		return mask.Mask;
	}

	// Token: 0x04000456 RID: 1110
	private int _cachedMask;

	// Token: 0x04000457 RID: 1111
	private readonly string[] _layers;
}
