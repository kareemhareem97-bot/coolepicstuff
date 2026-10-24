using System;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class DetectorBlink : MonoBehaviour
{
	// Token: 0x06000238 RID: 568 RVA: 0x00008EA9 File Offset: 0x000070A9
	private void Start()
	{
		this.Blink();
	}

	// Token: 0x06000239 RID: 569 RVA: 0x00008EB4 File Offset: 0x000070B4
	private void Blink()
	{
		this.state = !this.state;
		int num = this.state ? 2 : 0;
		this.mat.SetColor("_EmissionColor", new Color((float)num, (float)num, (float)num));
		base.Invoke("Blink", this.state ? 0.2f : 1.3f);
	}

	// Token: 0x040001D0 RID: 464
	public Material mat;

	// Token: 0x040001D1 RID: 465
	private bool state;
}
