using System;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class ConnInfoCanceler : ConnInfoButton
{
	// Token: 0x060007AD RID: 1965 RVA: 0x0001D506 File Offset: 0x0001B706
	public override void UseButton()
	{
		base.UseButton();
		UnityEngine.Object.FindObjectOfType<CustomNetworkManager>().StopClient();
	}
}
