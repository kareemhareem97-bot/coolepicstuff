using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000C6 RID: 198
public class HoldableButton : Button
{
	// Token: 0x170000AC RID: 172
	// (get) Token: 0x060003DC RID: 988 RVA: 0x0000F69D File Offset: 0x0000D89D
	public bool IsHeld
	{
		get
		{
			return base.IsPressed();
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x060003DD RID: 989 RVA: 0x0000F6A5 File Offset: 0x0000D8A5
	public bool IsHovering
	{
		get
		{
			return base.IsHighlighted() || this.IsHeld;
		}
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x060003DE RID: 990 RVA: 0x0000F6B8 File Offset: 0x0000D8B8
	public float HeldPercent
	{
		get
		{
			if (this.HoldTime <= 0f || !this.IsHeld)
			{
				return 0f;
			}
			return Mathf.Clamp01((float)this._holdSw.Elapsed.TotalSeconds / this.HoldTime);
		}
	}

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x060003DF RID: 991 RVA: 0x0000F700 File Offset: 0x0000D900
	// (set) Token: 0x060003E0 RID: 992 RVA: 0x0000F708 File Offset: 0x0000D908
	public virtual float HoldTime { get; private set; }

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x060003E1 RID: 993 RVA: 0x0000F711 File Offset: 0x0000D911
	// (set) Token: 0x060003E2 RID: 994 RVA: 0x0000F719 File Offset: 0x0000D919
	public Button.ButtonClickedEvent OnHeld { get; private set; }

	// Token: 0x060003E3 RID: 995 RVA: 0x0000F722 File Offset: 0x0000D922
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		this._holdSw.Restart();
		this._eventCalled = false;
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x0000F73D File Offset: 0x0000D93D
	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		this._holdSw.Reset();
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x0000F754 File Offset: 0x0000D954
	private void Update()
	{
		if (this._loadingCircle != null)
		{
			this._loadingCircle.fillAmount = this.HeldPercent;
		}
		if (this._eventCalled || !this.IsHeld || this.HeldPercent < 1f)
		{
			return;
		}
		this._eventCalled = true;
		if (this.OnHeld == null)
		{
			return;
		}
		this.OnHeld.Invoke();
	}

	// Token: 0x04000370 RID: 880
	private readonly Stopwatch _holdSw = new Stopwatch();

	// Token: 0x04000371 RID: 881
	private bool _eventCalled;

	// Token: 0x04000372 RID: 882
	[SerializeField]
	private Image _loadingCircle;

	// Token: 0x04000373 RID: 883
	[SerializeField]
	private bool _deselectOnComplete;
}
