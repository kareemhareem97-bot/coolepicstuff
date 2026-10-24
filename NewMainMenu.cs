using System;
using UnityEngine;

// Token: 0x020000DF RID: 223
public class NewMainMenu : MonoBehaviour
{
	// Token: 0x06000440 RID: 1088 RVA: 0x000105AB File Offset: 0x0000E7AB
	public void QuitGame()
	{
		Shutdown.Quit(true, false);
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x0000FDAE File Offset: 0x0000DFAE
	public void Refresh()
	{
		SimpleMenu.LoadCorrectScene();
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x00002E9A File Offset: 0x0000109A
	private void Start()
	{
	}
}
