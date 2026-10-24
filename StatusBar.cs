using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000078 RID: 120
public class StatusBar : MonoBehaviour
{
	// Token: 0x17000088 RID: 136
	// (get) Token: 0x0600029B RID: 667 RVA: 0x00009FC6 File Offset: 0x000081C6
	private float CurAlpha
	{
		get
		{
			if (this.TargetGraphics.Length != 0)
			{
				return this.TargetGraphics[0].GetAlpha();
			}
			return 1f;
		}
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x0600029C RID: 668 RVA: 0x00009FE4 File Offset: 0x000081E4
	// (set) Token: 0x0600029D RID: 669 RVA: 0x00009FF6 File Offset: 0x000081F6
	private Vector3 LocalPosition
	{
		get
		{
			return (base.transform as RectTransform).localPosition;
		}
		set
		{
			(base.transform as RectTransform).localPosition = value;
		}
	}

	// Token: 0x0600029E RID: 670 RVA: 0x0000A009 File Offset: 0x00008209
	private void OnEnable()
	{
		if (!this.HiddenByDefault)
		{
			return;
		}
		this.TargetGraphics.ForEach(delegate(CanvasRenderer x)
		{
			x.SetAlpha(0f);
		});
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0000A040 File Offset: 0x00008240
	private void Move(float targetY, bool bypassAnimation)
	{
		Vector3 localPosition = this.LocalPosition;
		if (bypassAnimation)
		{
			localPosition.y = targetY;
		}
		else
		{
			float num = Time.deltaTime * this.MoveSpeed;
			float num2 = targetY - localPosition.y;
			if (num > Mathf.Abs(num2))
			{
				localPosition.y = targetY;
			}
			else
			{
				localPosition.y += num * (float)((num2 > 0f) ? 1 : -1);
			}
		}
		this.LocalPosition = localPosition;
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0000A0AB File Offset: 0x000082AB
	private void Update()
	{
		this.UpdateBar(false);
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x0000A0B4 File Offset: 0x000082B4
	public void UpdateBar(bool bypassAnims)
	{
		if (this.MasterBar != null)
		{
			StatusBar masterBar = this.MasterBar;
			while (masterBar.TargetGraphics.Length != 0 && masterBar.CurAlpha <= 0.1f)
			{
				if (masterBar.MasterBar == null)
				{
					return;
				}
				masterBar = masterBar.MasterBar;
			}
			this.Move(masterBar.LocalPosition.y + this.FixedDistance, bypassAnims || this.CurAlpha <= 0f);
		}
		if (this.AutohideOption != StatusBar.AutoHideType.AlwaysVisible)
		{
			float num = this.FadeInSpeed;
			if ((this.AutohideOption == StatusBar.AutoHideType.WhenEmpty && this.TargetSlider.value == this.TargetSlider.minValue) || (this.AutohideOption == StatusBar.AutoHideType.WhenFull && this.TargetSlider.value == this.TargetSlider.maxValue))
			{
				num = -this.FadeOutSpeed;
			}
			if (bypassAnims)
			{
				this.SetAlpha(Mathf.Sign(num));
				return;
			}
			this.SetAlpha(this.TargetGraphics[0].GetAlpha() + num * Time.deltaTime);
		}
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x0000A1B8 File Offset: 0x000083B8
	public void SetAlpha(float a)
	{
		CanvasRenderer[] targetGraphics = this.TargetGraphics;
		for (int i = 0; i < targetGraphics.Length; i++)
		{
			targetGraphics[i].SetAlpha(Mathf.Clamp01(a));
		}
	}

	// Token: 0x04000221 RID: 545
	[Tooltip("Above which object should this be displayed?")]
	public StatusBar MasterBar;

	// Token: 0x04000222 RID: 546
	[Tooltip("Slider which the script will affect")]
	public Slider TargetSlider;

	// Token: 0x04000223 RID: 547
	[Tooltip("All graphics that should be faded in/out")]
	public CanvasRenderer[] TargetGraphics;

	// Token: 0x04000224 RID: 548
	[Tooltip("Y-axis offset of the MasterTransform.position")]
	public float FixedDistance;

	// Token: 0x04000225 RID: 549
	public StatusBar.AutoHideType AutohideOption;

	// Token: 0x04000226 RID: 550
	public bool HiddenByDefault;

	// Token: 0x04000227 RID: 551
	public float FadeInSpeed;

	// Token: 0x04000228 RID: 552
	public float FadeOutSpeed;

	// Token: 0x04000229 RID: 553
	public float MoveSpeed;

	// Token: 0x02000079 RID: 121
	public enum AutoHideType
	{
		// Token: 0x0400022B RID: 555
		WhenFull,
		// Token: 0x0400022C RID: 556
		WhenEmpty,
		// Token: 0x0400022D RID: 557
		AlwaysVisible
	}
}
