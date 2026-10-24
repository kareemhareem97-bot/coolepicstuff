using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000062 RID: 98
public class FlipbookPlayer : MonoBehaviour
{
	// Token: 0x06000241 RID: 577 RVA: 0x00008FA1 File Offset: 0x000071A1
	private void Awake()
	{
		this._rect = this._image.uvRect;
	}

	// Token: 0x06000242 RID: 578 RVA: 0x00008FB4 File Offset: 0x000071B4
	private void FixedUpdate()
	{
		if (this._remainingCooldown > 0)
		{
			this._remainingCooldown--;
			return;
		}
		this._rect.position = (float)this._elapsedPrimary * this._primaryOffset + (float)this._elapsedSecondary * this._secondaryOffset;
		this._elapsedPrimary++;
		if (this._elapsedPrimary >= this._primaryCycles)
		{
			this._elapsedSecondary++;
			this._elapsedPrimary = 0;
			if (this._elapsedSecondary >= this._secondaryCycles)
			{
				this._elapsedSecondary = 0;
			}
		}
		this._remainingCooldown = this._updateCooldown;
		this._image.uvRect = this._rect;
	}

	// Token: 0x040001D5 RID: 469
	[SerializeField]
	private int _updateCooldown;

	// Token: 0x040001D6 RID: 470
	[SerializeField]
	private RawImage _image;

	// Token: 0x040001D7 RID: 471
	[SerializeField]
	private Vector2 _primaryOffset;

	// Token: 0x040001D8 RID: 472
	[SerializeField]
	private int _primaryCycles;

	// Token: 0x040001D9 RID: 473
	[SerializeField]
	private Vector2 _secondaryOffset;

	// Token: 0x040001DA RID: 474
	[SerializeField]
	private int _secondaryCycles;

	// Token: 0x040001DB RID: 475
	private int _elapsedPrimary;

	// Token: 0x040001DC RID: 476
	private int _elapsedSecondary;

	// Token: 0x040001DD RID: 477
	private int _remainingCooldown;

	// Token: 0x040001DE RID: 478
	private Rect _rect;
}
