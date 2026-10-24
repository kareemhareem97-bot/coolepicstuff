using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000043 RID: 67
public class AlphaWarheadScreen : MonoBehaviour
{
	// Token: 0x060001B9 RID: 441 RVA: 0x00007A82 File Offset: 0x00005C82
	private void Start()
	{
		if (!AlphaWarheadScreen.Instances.Contains(this))
		{
			AlphaWarheadScreen.Instances.Add(this);
		}
		this.UpdateScreen();
	}

	// Token: 0x060001BA RID: 442 RVA: 0x00007AA2 File Offset: 0x00005CA2
	private void OnDestroy()
	{
		AlphaWarheadScreen.Instances.Remove(this);
	}

	// Token: 0x060001BB RID: 443 RVA: 0x00007AB0 File Offset: 0x00005CB0
	private void UpdateScreen()
	{
		this._text.text = AlphaWarheadScreen._lastText;
		GameObject[] inevitable = this._inevitable;
		for (int i = 0; i < inevitable.Length; i++)
		{
			inevitable[i].SetActive(AlphaWarheadScreen._lastInevitable);
		}
	}

	// Token: 0x060001BC RID: 444 RVA: 0x00007AF0 File Offset: 0x00005CF0
	public static void SetScreen(string text, bool showInevitableWarn)
	{
		AlphaWarheadScreen._lastText = text;
		AlphaWarheadScreen._lastInevitable = showInevitableWarn;
		foreach (AlphaWarheadScreen alphaWarheadScreen in AlphaWarheadScreen.Instances)
		{
			alphaWarheadScreen.UpdateScreen();
		}
	}

	// Token: 0x0400016E RID: 366
	private static readonly List<AlphaWarheadScreen> Instances = new List<AlphaWarheadScreen>();

	// Token: 0x0400016F RID: 367
	private static string _lastText;

	// Token: 0x04000170 RID: 368
	private static bool _lastInevitable;

	// Token: 0x04000171 RID: 369
	[SerializeField]
	private TMP_Text _text;

	// Token: 0x04000172 RID: 370
	[SerializeField]
	private GameObject[] _inevitable;
}
