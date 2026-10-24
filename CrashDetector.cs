using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000116 RID: 278
public class CrashDetector : MonoBehaviour
{
	// Token: 0x06000555 RID: 1365 RVA: 0x00013B88 File Offset: 0x00011D88
	private void Awake()
	{
		if (this.image == null)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		CrashDetector.singleton = this;
		base.gameObject.SetActive(this.Show());
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x00004094 File Offset: 0x00002294
	public bool Show()
	{
		return false;
	}

	// Token: 0x04000474 RID: 1140
	public static CrashDetector singleton;

	// Token: 0x04000475 RID: 1141
	[SerializeField]
	private GameObject image;

	// Token: 0x04000476 RID: 1142
	[SerializeField]
	private Button button;

	// Token: 0x04000477 RID: 1143
	[SerializeField]
	private Text text;
}
