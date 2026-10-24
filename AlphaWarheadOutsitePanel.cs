using System;
using Mirror;
using UnityEngine;

// Token: 0x02000041 RID: 65
public class AlphaWarheadOutsitePanel : NetworkBehaviour
{
	// Token: 0x1700006A RID: 106
	// (get) Token: 0x060001AA RID: 426 RVA: 0x0000798B File Offset: 0x00005B8B
	public static AlphaWarheadNukesitePanel nukeside
	{
		get
		{
			return AlphaWarheadNukesitePanel.Singleton;
		}
	}

	// Token: 0x060001AB RID: 427 RVA: 0x00007994 File Offset: 0x00005B94
	private void Update()
	{
		if (AlphaWarheadOutsitePanel.nukeside == null)
		{
			return;
		}
		base.transform.localPosition = new Vector3(0f, 0f, 9f);
		this.panelButtonCoverAnim.SetBool(AlphaWarheadOutsitePanel.Enabled, AlphaWarheadActivationPanel.IsUnlocked);
	}

	// Token: 0x060001AE RID: 430 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x0400016B RID: 363
	public Animator panelButtonCoverAnim;

	// Token: 0x0400016C RID: 364
	private static readonly int Enabled = Animator.StringToHash("enabled");
}
